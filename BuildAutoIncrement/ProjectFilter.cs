/*
 * Filename:    ProjectFilter.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Interface and some implementations of class used to filter 
 *              ProjectInfo objects.
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

namespace BuildAutoIncrement {

    public interface IProjectFilter {
        string[] ProjectsToForce { get; }
        bool Pass(ProjectInfo projectInfo);
    }

    /// <summary>
    ///   Project filter passing all <c>ProjectInfo</c> objects.
    /// </summary>
    public class ProjectFilter : IProjectFilter {

        protected ProjectFilter() {
        }

        #region IProjectFilter Members

        public virtual bool Pass(ProjectInfo projectInfo) {
            return true;
        }

        public virtual string[] ProjectsToForce { 
            get {
                return new string[0]; 
            }
        }

        #endregion

        public static ProjectFilter PassAll = new ProjectFilter();
    }

	/// <summary>
	/// <c>IProjectFilter</c> implementation that passes <c>ProjectInfo</c> 
	/// objects using configuration settings.
	/// </summary>
	public class ProjectFilterByType : ProjectFilter {

		public ProjectFilterByType(bool passSetupProjects, bool passNonVersionableProjects, bool passProjectFolders, bool passEnterpriseProjectRoots) {
            m_passSetupProjects = passSetupProjects;
            m_passNonVersionableProjects = passNonVersionableProjects;
            m_passProjectFolders = passProjectFolders;
            m_passEnterpriseProjectRoots = passEnterpriseProjectRoots;
		}

        /// <summary>
        ///   Filter method.
        /// </summary>
        /// <param name="projectInfo">
        ///   <c>ProjectInfo</c> object on which filter is applied.
        /// </param>
        /// <returns>
        ///   Return <c>true</c> if project passed filter criteria.
        /// </returns>
        public override bool Pass(ProjectInfo projectInfo) {
            ProjectType pt = projectInfo.ProjectTypeInfo.ProjectType;
            switch (pt) {
            case ProjectType.SetupProject:
                return m_passSetupProjects;
            case ProjectType.FileBasedWebProject:
            case ProjectType.DatabaseProject:
            case ProjectType.FSharpProject:
                return m_passNonVersionableProjects;
            case ProjectType.SolutionFolder:
            case ProjectType.VirtualFolder:
                return m_passProjectFolders;
            case ProjectType.EnterpriseProject:
                return m_passEnterpriseProjectRoots;
            }
            return true;
        }

        private bool m_passSetupProjects = false;

        private bool m_passNonVersionableProjects = false;

        private bool m_passProjectFolders = false;

        private bool m_passEnterpriseProjectRoots = false;
    }

    /// <summary>
    ///   Object used by command-line utility to filter projects by their
    ///   name.
    /// </summary>
    public class ProjectFilterByName : ProjectFilterByType {

        public ProjectFilterByName(bool passSetupProjects, bool passNonVersionableProjects, bool passProjectFolders, bool passEnterpriseProjectRoots, string[] projectsToInclude, string[] projectsToExclude, string[] projectsToForce)  : base(passSetupProjects, passNonVersionableProjects, passProjectFolders, passEnterpriseProjectRoots) { 
            m_projectsToInclude = (projectsToInclude == null) ? new string[0] : projectsToInclude;
            m_projectsToExclude = (projectsToExclude == null) ? new string[0] : projectsToExclude;
            m_projectsToForce = (projectsToForce == null) ? new string[0] : projectsToForce;
        }

        public override string[] ProjectsToForce {
            get {
                return m_projectsToForce;
            }
        }

        public override bool Pass(ProjectInfo projectInfo) {
            IEnumerator enumerator = m_projectsToForce.GetEnumerator();
            while (enumerator.MoveNext()) {
                string entry = (string)enumerator.Current;
                if (ProjectNamesAreEqual(entry, projectInfo))
                    return true;
            }
            // if any of lists is not empty
            if (m_projectsToExclude.Length > 0 || m_projectsToInclude.Length > 0) {
                // first the exclusion list
                enumerator = m_projectsToExclude.GetEnumerator();
                while (enumerator.MoveNext()) {
                    string entry = (string)enumerator.Current;
                    if (ProjectNamesAreEqual(entry, projectInfo))
                        return false;
                }
                // if there is an inclusion list
                if (m_projectsToInclude.Length > 0) {
                    enumerator = m_projectsToInclude.GetEnumerator();
                    while (enumerator.MoveNext()) {
                        string entry = (string)enumerator.Current;
                        if (ProjectNamesAreEqual(entry, projectInfo))
                            return true;
                    }
                    return false;
                }
            }
            return base.Pass(projectInfo);
        }

        
        protected bool ProjectNamesAreEqual(string name, ProjectInfo projectInfo) {
            return string.Compare(name, projectInfo.ProjectName, true) == 0;
        }

        private string[] m_projectsToInclude = null;
        private string[] m_projectsToExclude = null;
        private string[] m_projectsToForce = null;
    }
}