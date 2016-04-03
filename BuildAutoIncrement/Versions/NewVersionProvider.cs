/*
 * Filename:    NewVersionProvider.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Provides a new version for project.
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
using System.Diagnostics;
using System.Globalization;

namespace BuildAutoIncrement {
	/// <summary>
	///   Class responsible to provide a new version for project.
	/// </summary>
	public class NewVersionProvider {

        #region Constructors

        /// <summary>
        ///   Hidden default constructor.
        /// </summary>
		protected NewVersionProvider() {
            UpdateAutomaticBuildAndRevision();
		}

        /// <summary>
        ///   Creates <c>NewVersionProvider</c> object with given numbering 
        ///   options.
        /// </summary>
        /// <param name="numberingOptions">
        ///   Numbering options used for version increment.
        /// </param>
        public NewVersionProvider(NumberingOptions numberingOptions) : this() {
            m_numberingOptions = numberingOptions;
        }

        #endregion // Constructors

        #region Public methods

        /// <summary>
        ///   Proposes "to become" <c>AssemblyVersions</c> for a project according 
        ///   to numbering scheme.
        /// </summary>
        /// <param name="currentAssemblyVersions">
        ///   <c>AssemblyVersions</c> set for which new versions have to be 
        ///   evaluated.
        /// </param>
        /// <returns>
        ///   <c>AssemblyVersions</c> set with proposed versions.
        /// </returns>
        public AssemblyVersions ProposeNewVersions(AssemblyVersions currentAssemblyVersions) {
            ProjectVersion toBecomeAssemblyVersion              = GetToBecomeProjectVersion(currentAssemblyVersions[AssemblyVersionType.AssemblyVersion]);
            ProjectVersion toBecomeAssemblyFileVersion          = GetToBecomeProjectVersion(currentAssemblyVersions[AssemblyVersionType.AssemblyFileVersion]);
            ProjectVersion toBecomeAssemblyInformationalVersion = GetToBecomeProjectVersion(currentAssemblyVersions[AssemblyVersionType.AssemblyInformationalVersion]);
            AssemblyVersions toBecomeAssemblyVersions = new AssemblyVersions(toBecomeAssemblyVersion, toBecomeAssemblyFileVersion, toBecomeAssemblyInformationalVersion);
            if (m_numberingOptions.SynchronizeAllVersionTypes)
                toBecomeAssemblyVersions.SynchronizeVersionsToHighest();
            return toBecomeAssemblyVersions;
        }

        /// <summary>
        ///   Determines if an <c>AssemblyVersion</c> for the project should be 
        ///   updated according to configured settings, <c>AssemblyVersionType</c>
        ///   and highest <c>ProjectVersion</c>provided.
        /// </summary>
        /// <param name="projectInfo">
        ///   <c>ProjectInfo</c> for the project that is considered.
        /// </param>
        /// <param name="assemblyVersionType">
        ///   <c>AssemblyVersionType</c> for which version update is determined.
        /// </param>
        /// <param name="highestProjectVersion">
        ///   Highest <c>ProjectVersion</c> used as a reference for update.
        /// </param>
        /// <returns>
        ///   <c>true</c> if version should be incremented.
        /// </returns>
        public bool ShouldUpdate(ProjectInfo projectInfo, AssemblyVersionType assemblyVersionType, ProjectVersion highestProjectVersion) {
            if (m_numberingOptions.SynchronizeAllVersionTypes) {
                if (!projectInfo.CurrentAssemblyVersions.AreVersionsSynchronized) {
                    return true;
                }
            }
            foreach (AssemblyVersionType at in AssemblyVersions.AssemblyVersionTypes) {
                if ((assemblyVersionType & at) == at) {
                    if (ShouldUpdateOneOfAssemblyVersionTypes(projectInfo, at, highestProjectVersion))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        ///   Provides the updated version as a string.
        /// </summary>
        /// <param name="projectInfo">
        ///   <c>ProjectInfo</c> for the project that is updated.
        /// </param>
        /// <param name="assemblyVersionType">
        ///   <c>AssemblyVersionType</c> for which version update is done.
        /// </param>
        /// <param name="highestProjectVersion">
        ///   Highest <c>ProjectVersion</c> used as a reference for update.
        /// </param>
        /// <returns>
        ///   New version as a string.
        /// </returns>
        public string ProvideNewVersion(ProjectInfo projectInfo, AssemblyVersionType assemblyVersionType, ProjectVersion highestProjectVersion) {
            Debug.Assert(assemblyVersionType != AssemblyVersionType.All);
            switch (m_numberingOptions.BatchCommandIncrementScheme) {
            case BatchCommandIncrementScheme.IncrementModifiedIndependently:
            case BatchCommandIncrementScheme.IncrementAllIndependently:
                if (m_numberingOptions.SynchronizeAllVersionTypes && !projectInfo.ToUpdate)
                    return projectInfo.CurrentAssemblyVersions.HighestProjectVersion.ToString();
                return projectInfo.ToBecomeAssemblyVersions[assemblyVersionType].ToString();
            case (BatchCommandIncrementScheme.IncrementAllAndSynchronize):
            case (BatchCommandIncrementScheme.IncrementModifiedOnlyAndSynchronize):
                int resetBuildAndRevisionValues = (int)m_numberingOptions.ResetBuildAndRevisionTo;
                return ProjectVersion.ApplyVersionPattern(highestProjectVersion.ToString(), projectInfo.CurrentAssemblyVersions[assemblyVersionType].ToString(), resetBuildAndRevisionValues);// highestProjectVersion.ToString();
            }
            Debug.Assert(false, "Not supported option");
            return null;
        }

        #endregion // Public methods

        #region Protected methods

        /// <summary>
        ///   Evaluates "to be" <c>ProjectVersion</c> according to numbering 
        ///   scheme configured.
        /// </summary>
        /// <param name="currentProjectVersion">
        ///   Current <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///   Next version.
        /// </returns>
        private ProjectVersion GetToBecomeProjectVersion(ProjectVersion currentProjectVersion) {
            ProjectVersion toBecomeProjectVersion = ProjectVersion.Empty;
            if (currentProjectVersion != ProjectVersion.Empty) {
                if (m_numberingOptions.UseDateTimeBasedBuildAndRevisionNumbering) {
                    toBecomeProjectVersion = currentProjectVersion.Clone(m_autoBuildVersion, m_autoRevisionVersion, m_numberingOptions.ReplaceAsteriskWithVersionComponents);
                }
                else {
                    toBecomeProjectVersion = currentProjectVersion.Clone();
                    toBecomeProjectVersion.Increment(m_numberingOptions);
                }
            }
            return toBecomeProjectVersion;
        }

        #endregion // Protected methods

        #region Private methods

        /// <summary>
        ///   Determines if one of <c>AssemblyVersionType</c> assembly infos 
        ///   should be updated.
        /// </summary>
        /// <param name="projectInfo">
        ///   <c>ProjectInfo</c> for the project that is considered.
        /// </param>
        /// <param name="assemblyVersionType">
        ///   <c>AssemblyVersionType</c> for which version update is determined.
        /// </param>
        /// <param name="highestProjectVersion">
        ///   Highest <c>ProjectVersion</c> used as a reference for update.
        /// </param>
        /// <returns>
        ///   <c>true</c> if version should be incremented.
        /// </returns>
        private bool ShouldUpdateOneOfAssemblyVersionTypes(ProjectInfo projectInfo, AssemblyVersionType assemblyVersionType, ProjectVersion highestProjectVersion) {
            Debug.Assert(assemblyVersionType == AssemblyVersionType.AssemblyVersion || assemblyVersionType == AssemblyVersionType.AssemblyFileVersion || assemblyVersionType == AssemblyVersionType.AssemblyInformationalVersion);
            // first check if corresponding assembly version type exists at all
            if (projectInfo.CurrentAssemblyVersions[assemblyVersionType] == ProjectVersion.Empty)
                return false;
            // else, depending on settings
            switch (m_numberingOptions.BatchCommandIncrementScheme) {
            case (BatchCommandIncrementScheme.IncrementModifiedIndependently):
                return projectInfo.IsMarkedForUpdate(assemblyVersionType);
            case (BatchCommandIncrementScheme.IncrementAllIndependently):
                return true;
            case (BatchCommandIncrementScheme.IncrementModifiedOnlyAndSynchronize):
                int resetBuildAndRevisionValues = (int)m_numberingOptions.ResetBuildAndRevisionTo;
                return projectInfo.Modified || ProjectVersion.ApplyVersionPattern(highestProjectVersion.ToString(), projectInfo.CurrentAssemblyVersions[assemblyVersionType].ToString(), resetBuildAndRevisionValues) != projectInfo.CurrentAssemblyVersions[assemblyVersionType].ToString();//projectInfo.CurrentAssemblyVersions[assemblyVersionType] < highestProjectVersion;
            case (BatchCommandIncrementScheme.IncrementAllAndSynchronize):
                int buildAndRevisionResetValue = (int)m_numberingOptions.ResetBuildAndRevisionTo;
                return ProjectVersion.ApplyVersionPattern(highestProjectVersion.ToString(), projectInfo.CurrentAssemblyVersions[assemblyVersionType].ToString(), buildAndRevisionResetValue) != projectInfo.CurrentAssemblyVersions[assemblyVersionType].ToString(); // projectInfo.CurrentAssemblyVersions[assemblyVersionType] < highestProjectVersion;
            }
            Debug.Assert(false, "Not supported option");
            return false;
        }

        /// <summary>
        ///   Initializes Build and Revision numbers to be used if automatic
        ///   (Microsoft's) scheme is selected.
        /// </summary>
        private void UpdateAutomaticBuildAndRevision() {
            DateTime currentDateTime = DateTime.UtcNow;
            if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(currentDateTime)) {
                DaylightTime dt = TimeZone.CurrentTimeZone.GetDaylightChanges(currentDateTime.Year);
                currentDateTime += dt.Delta;
            }
            DateTime referenceDateTime = new DateTime(2000, 1, 1, 0, 0, 0);
            TimeSpan timeSpan = currentDateTime - referenceDateTime;
            m_autoBuildVersion = timeSpan.Days;
            m_autoRevisionVersion = (int)(currentDateTime.TimeOfDay.TotalSeconds / 2);
        }

        #endregion // Private methods
        
        /// <summary>
        ///   Configured numbering options.
        /// </summary>
        protected readonly NumberingOptions m_numberingOptions;

        /// <summary>
        ///   Values of actual Build and Revision when using Date&amp;Time 
        ///   based versioning.
        /// </summary>
        private int m_autoBuildVersion;
        private int m_autoRevisionVersion;
    }
}