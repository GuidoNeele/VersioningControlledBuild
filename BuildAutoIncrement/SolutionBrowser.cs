/*
 * Filename:    SolutionBrowser.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Interface and base class for solution browsers 
 *              (VSSolutionBrowser and SolutionFileReader).
 * Copyright:   Julijan Šribar, 2004-2013
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the author(s) be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */
using System;
using System.Collections;
using System.Diagnostics;

namespace BuildAutoIncrement {
	/// <summary>
	///   Interface implemented by solution browsers.
	/// </summary>
	public interface ISolutionBrowser : IDisposable {

        /// <summary>
        ///   Gets active solution name.
        /// </summary>
        string SolutionName { get; }
        
        /// <summary>
        ///   Gets active solution file name.
        /// </summary>
        string SolutionFilename { get; }
        
        /// <summary>
        ///   Gets an array of projects for which versions should be updated.
        /// </summary>
        ProjectInfo[] ProjectsToUpdate { get; }

        /// <summary>
        ///   Gets update summary.
        /// </summary>
        UpdateSummary UpdateSummary { get; }

        /// <summary>
        ///   Applies configuration settings to projects found.
        /// </summary>
        /// <param name="configuration">
        ///   Configuration applied to filter out projects.
        /// </param>
        void ApplyConfiguration(VcbConfiguration configuration);

        void ApplyFilter(IProjectFilter filter);

        /// <summary>
        ///   Gets <c>ProjectInfoList</c> of <c>ProjectInfo</c> objects 
        ///   filtered according to configuration provided to 
        ///   <c>ApplyConfiguration</c> method.
        /// </summary>
        ProjectInfoList ProjectInfoList { get; }

        /// <summary>
        ///   Checks out supplied version files.
        /// </summary>
        /// <param name="projectsToCheckOut">
        ///   An array of <c>ProjectInfo</c> objects to check out.
        /// </param>
        void CheckOutProjectVersionFiles(ProjectInfo[] projectsToCheckOut);

        /// <summary>
        ///   Saves versions.
        /// </summary>
        void SaveVersions();

	}

    /// <summary>
    ///   Base class for all solution browsers. Implements <c>ISolutionBrowser</c>
    ///   interface.
    /// </summary>
    public abstract class SolutionBrowser : ISolutionBrowser, IDisposable {

        #region Constructor

        /// <summary>
        ///   Creates an empty instance of <c>SolutionBrowser</c> object.
        /// </summary>
        private SolutionBrowser() {
            m_allProjects           = new ProjectsSortedArrayList();
            m_updateSummary         = new UpdateSummary();
            m_projectFilter         = ProjectFilter.PassAll;
            Debug.Assert(m_allProjects != null);
            Debug.Assert(m_updateSummary != null);
        }

        /// <summary>
        ///   Creates <c>SolutionBrowser</c> object with configuration provided.
        /// </summary>
        /// <param name="configuration">
        ///   Configuration to be used.
        /// </param>
        protected SolutionBrowser(VcbConfiguration configuration) : this() {
            Debug.Assert(configuration != null && configuration.NumberingOptions != null);
            SetNumberingOptions(configuration.NumberingOptions);
        }

        ~SolutionBrowser() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
        }

        #endregion // Constructor

        #region Public properties

        /// <summary>
        ///   Gets a list of <c>ProjectInfo</c> objects currently displayed.
        /// </summary>
        public ProjectInfoList ProjectInfoList {
            get {
                if (m_filteredProjects == null)
                    UpdateFilteredProjectsList();
                return m_filteredProjects;
            }
        }

        /// <summary>
        ///   Gets an array of <c>ProjectInfo</c> objects for which version 
        ///   should be updated.
        /// </summary>
        public ProjectInfo[] ProjectsToUpdate {
            get {
                ArrayList projectsToUpdate = new ArrayList();
                foreach (ProjectInfo projectInfo in ProjectInfoList) {
                    if (m_newVersionProvider.ShouldUpdate(projectInfo, AssemblyVersionsUpdateMask, ProjectInfoList.HighestToBeAssemblyVersions.HighestProjectVersion)) {
                        projectsToUpdate.Add(projectInfo);
                    }
                }
                return (ProjectInfo[])projectsToUpdate.ToArray(typeof(ProjectInfo));
            }
        }

        /// <summary>
        ///   Gets <c>UpdateSummary</c> with update info.
        /// </summary>
        public UpdateSummary UpdateSummary { 
            get {
                Debug.Assert(m_updateSummary != null);
                return m_updateSummary;
            }
        }

        #endregion // Public properties

        #region Implemented public methods

        /// <summary>
        ///   Applies configuration settings to projects found.
        /// </summary>
        /// <param name="configuration">
        ///   Configuration to apply.
        /// </param>
        public void ApplyConfiguration(VcbConfiguration configuration) {
            Debug.Assert(m_allProjects != null);
            Debug.Assert(configuration != null && configuration.NumberingOptions != null);
            SetNumberingOptions(configuration.NumberingOptions);
            m_filteredProjects = null;
        }

        /// <summary>
        ///   Applies a filter on project infos.
        /// </summary>
        /// <param name="filter">
        ///   Filter to apply.
        /// </param>
        public void ApplyFilter(IProjectFilter filter) {
            Debug.Assert(filter != null);
            m_projectFilter = filter;
            m_filteredProjects = null;
        }

        /// <summary>
        ///   Save versions of project marked for update. New version is 
        ///   generated by <c>NewVersionProvider</c>.
        /// </summary>
        public void SaveVersions() {
            ProjectVersion highestVersion = HighestVersion;
            foreach (ProjectInfo projectInfo in ProjectsToUpdate) {
                foreach (AssemblyVersionType at in projectInfo.CurrentAssemblyVersions.GetValidVersionTypes()) {
                    if ((AssemblyVersionsUpdateMask & at) == at) {
                        string newVersion = m_newVersionProvider.ProvideNewVersion(projectInfo, at, highestVersion);
                        if (newVersion != projectInfo.CurrentAssemblyVersions[at].ToString()) {
                            if (projectInfo.Save(at, newVersion)) {
                                m_updateSummary.SetUpdated(projectInfo, at, newVersion);
                            }
                        }
                    }
                }
            }
        }

        #endregion // Implemented public methods

        #region Abstract members

        public abstract string SolutionName { get; }

        public abstract string SolutionFilename { get; }

        public abstract void CheckOutProjectVersionFiles(ProjectInfo[] projectsToCheckOut);

        protected abstract void PreProcess();

        protected abstract void PostProcess();
        
        #endregion // Abstract members

        #region Private properties

        /// <summary>
        ///   Gets the mask for <c>AssemblyVersionType</c> object.
        /// </summary>
        private AssemblyVersionType AssemblyVersionsUpdateMask {
            get {
                Debug.Assert(m_numberingOptions != null);
                return m_numberingOptions.ApplyToAllTypes ? AssemblyVersionType.All : m_numberingOptions.DefaultVersionType;
            }
        }

        /// <summary>
        ///   Gets highest version according to configuration settings.
        /// </summary>
        private ProjectVersion HighestVersion {
            get {
                Debug.Assert(m_numberingOptions != null);
                if (m_numberingOptions.ApplyToAllTypes) {
                    if (m_numberingOptions.BatchCommandIncrementScheme == BatchCommandIncrementScheme.IncrementModifiedOnlyAndSynchronize)
                        return ProjectInfoList.HighestProposedAssemblyVersions.HighestProjectVersion;
                    return ProjectInfoList.HighestToBeAssemblyVersions.HighestProjectVersion;
                }
                if (m_numberingOptions.BatchCommandIncrementScheme == BatchCommandIncrementScheme.IncrementModifiedOnlyAndSynchronize)
                    return ProjectInfoList.HighestProposedAssemblyVersions[m_numberingOptions.DefaultVersionType];
                return ProjectInfoList.HighestToBeAssemblyVersions[m_numberingOptions.DefaultVersionType];
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        ///   Recurses projects tree built to linearize it.
        /// </summary>
        /// <param name="projects">
        /// </param>
        /// <returns>
        /// </returns>
        private ICollection RecurseSubProjects(IList projects) {
            ArrayList subProjects = new ArrayList(projects.Count);
            foreach (ProjectInfo pi in projects) {
                ICollection rs = RecurseSubProjects(pi.SubProjects);
                if (ShouldDisplay(pi.ProjectTypeInfo.ProjectType, rs.Count)) {
                    subProjects.Add(pi);
                    subProjects.AddRange(rs);
                }
            }
            return subProjects;
        }

        /// <summary>
        ///   Checks if some item should be displayed.
        /// </summary>
        /// <param name="projectType"></param>
        /// <param name="subProjectsCount"></param>
        /// <returns></returns>
        private bool ShouldDisplay(ProjectType projectType, int subProjectsCount) {
            if ((projectType != ProjectType.VirtualFolder) && (projectType != ProjectType.SolutionFolder))
                return true;
            if (subProjectsCount > 0 || ConfigurationPersister.Instance.Configuration.DisplayOptions.ShowEmptyFolders) {
                return true;
            }
            return false;
        }

        /// <summary>
        ///   Updates the list of filtered projects, optionally marking "forced"
        ///   projects for update.
        /// </summary>
        private void UpdateFilteredProjectsList() {
            Debug.Assert(m_newVersionProvider != null);
            ArrayList projects = new ArrayList(RecurseSubProjects(m_allProjects));
            ProjectInfo[] projectInfos = (ProjectInfo[])projects.ToArray(typeof(ProjectInfo));
            // check which projects are in the list of "forced" projects
            foreach (ProjectInfo projectInfo in projectInfos) {
                if (Array.IndexOf(m_projectFilter.ProjectsToForce, projectInfo.ProjectName) != -1) {
                    projectInfo.MarkAssemblyVersionsForUpdate(AssemblyVersionType.All);
                }
            }
            m_filteredProjects = new ProjectInfoList(projectInfos, m_projectFilter, m_newVersionProvider, AssemblyVersionsUpdateMask);
            m_updateSummary.Clear();
            m_updateSummary.AddRange(m_filteredProjects.ProjectInfos);
        }

        private void SetNumberingOptions(NumberingOptions options) {
            Debug.Assert(options != null);
            m_numberingOptions = options;
            m_newVersionProvider = new NewVersionProvider(m_numberingOptions);
        }

        #endregion // Private methods

        #region Protected fields

        /// <summary>
        ///   Collection of <c>ProjectInfo</c> objects for the current solution.
        /// </summary>
        protected ProjectsSortedArrayList   m_allProjects;
        /// <summary>
        ///   Collection of <c>ProjectInfo</c> objects currently displayed.
        /// </summary>
        protected ProjectInfoList           m_filteredProjects;
        /// <summary>
        ///   Object responsible to checkout items under source control.
        /// </summary>
        protected ISourceSafeCheckout       m_sourceSafeCheckOut;
        /// <summary>
        ///   Object responsible to provide a new version.
        /// </summary>
        protected NewVersionProvider        m_newVersionProvider;
        /// <summary>
        ///   Configuration used.
        /// </summary>
        protected NumberingOptions          m_numberingOptions;

        private IProjectFilter              m_projectFilter;
        /// <summary>
        ///   Collection with update information.
        /// </summary>
        private UpdateSummary               m_updateSummary;

        #endregion // Protected fields

    }
}