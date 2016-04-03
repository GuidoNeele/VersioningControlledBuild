/*
 * Filename:    ProjectInfo.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Class with project project name, current and next version.
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
using System.IO;
using System.Resources;
using System.Windows.Forms;

namespace BuildAutoIncrement {

	/// <summary>
	///  ProjectInfo class contains project name, current version and next 
	///  version.
	/// </summary>
	public class ProjectInfo {

        /// <summary>
        ///   Hides default constructor.
        /// </summary>
        private ProjectInfo() { 
            m_assemblyVersionsUpdateInfo = new Hashtable(AssemblyVersions.AssemblyVersionTypes.Length);
            m_assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyVersion] = false;
            m_assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyFileVersion] = false;
            m_assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyInformationalVersion] = false;
            m_versionStreams        = new ArrayList();
            SubProjects             = new ProjectsSortedArrayList();
            Modified                = false;
            m_level                 = 0;
            CurrentAssemblyVersions = AssemblyVersions.Empty;
            m_toBecomeAssemblyVersions = AssemblyVersions.Empty;
        }

        /// <summary>
        ///   Initializes <c>ProjectInfo</c> object.
        /// </summary>
        /// <param name="projectName">
        ///   Name of the project.
        /// </param>
        /// <param name="projectFullName">
        ///   Project full name.
        /// </param>
        /// <param name="projectTypeInfo">
        ///   Type of the project.
        /// </param>
        /// <param name="level">
        ///   Indentation level.
        /// </param>
        public ProjectInfo(string projectName, string projectFullName, string uiPath, ProjectTypeInfo projectTypeInfo, int level) : this() {
            ProjectName             = projectName;
            ProjectRoot             = Path.GetDirectoryName(projectFullName);
            FullName                = projectFullName;
            UIPath                  = uiPath;
            ProjectTypeInfo         = projectTypeInfo;
            m_level                 = level;
        }

        /// <summary>
        ///   Initializes <c>ProjectInfo</c> object.
        /// </summary>
        /// <param name="project">
        ///   Project for which object is created.
        /// </param>
        /// <param name="projectTypeInfo">
        ///   ProjectTypeInfo to initialize with.
        /// </param>
        /// <param name="version">
        ///   <c>ProjectVersion</c>.
        /// </param>
        /// <param name="assemblyInfoItem">
        ///   <c>AssemblyInfo</c>
        /// </param>
        /// <param name="numberingOptions">
        ///   Numbering options.
        /// </param>
        public ProjectInfo(string projectName, string projectFullName, string uiPath, ProjectTypeInfo projectTypeInfo, bool modified, int level, AssemblyVersions assemblyVersions, VersionStream[] versionStreams) : this(projectName, projectFullName, uiPath, projectTypeInfo, level) {
			CurrentAssemblyVersions = assemblyVersions;
            m_versionStreams        = new ArrayList(versionStreams);
            Modified                = modified;
		}

        public ProjectInfo(string projectName, string projectFullName, ProjectTypeInfo projectTypeInfo, int level) : this(projectName, projectFullName, "", projectTypeInfo, level) {
        }

        public ProjectInfo(string projectName, string projectFullName, ProjectTypeInfo projectTypeInfo, bool modified, int level, AssemblyVersions assemblyVersions, VersionStream[] versionStreams) : this(projectName, projectFullName, "", projectTypeInfo, modified, level, assemblyVersions, versionStreams) {
        }

        /// <summary>
        ///   Project name.
        /// </summary>
        public readonly string              ProjectName;
        /// <summary>
        ///   Folder containing project root.
        /// </summary>
        public readonly string              ProjectRoot;
        /// <summary>
        ///   Full name of the project file.
        /// </summary>
        public readonly string              FullName;
        /// <summary>
        ///   Path to the project root in Solution Browser window.
        /// </summary>
        public readonly string              UIPath;
        /// <summary>
        ///   Information related to type of the project.
        /// </summary>
        public readonly ProjectTypeInfo     ProjectTypeInfo;
        /// <summary>
        ///   Level in the solution hierarchy. Used for indentation.
        /// </summary>
        private         int                 m_level;
        /// <summary>
        ///   Current AssemblyVersion of the project.
        /// </summary>
        public readonly AssemblyVersions    CurrentAssemblyVersions;
        /// <summary>
        ///   Flag indicating that project has been modifed.
        /// </summary>
        public readonly bool                Modified;
        /// <summary>
        ///   Sorted list of subprojects.
        /// </summary>
        public readonly ProjectsSortedArrayList SubProjects;

        /// <summary>
        ///   Gets the indentation level.
        /// </summary>
        public int Level {
            get {
                return m_level;
            }
        }

        /// <summary>
        ///   Gets the flag indicating if version update will be done.
        /// </summary>
        public bool ToUpdate {
            get {
                return (bool)m_assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyVersion] | (bool)m_assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyFileVersion] | (bool)m_assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyInformationalVersion];
            }
        }

        /// <summary>
        ///   Gets date &amp; time when file containing version has been 
        ///   written last time.
        /// </summary>
        public DateTime VersionFileWrite {
            get {
                DateTime lastWrite = DateTime.MinValue;
                foreach (string filename in VersionFilenames) {
                    DateTime fileWrite = FileUtil.GetLastWriteTime(filename);
                    if (fileWrite > lastWrite)
                        lastWrite = fileWrite;
                }
                return lastWrite;
            }
        }

        /// <summary>
        ///   Gets a flag indicating if version file exists.
        /// </summary>
        public bool AssemblyFileExists {
            get {
                return m_versionStreams.Count > 0;
            }
        }

        /// <summary>
        ///   Gets an array of <c>VersionStream</c> objects attached to the 
        ///   project.
        /// </summary>
        public string[] VersionFilenames {
            get {
                string[] versionFilenames = new string[m_versionStreams.Count];
                for (int i = 0; i < m_versionStreams.Count; i++)
                    versionFilenames[i] = ((VersionStream)m_versionStreams[i]).Filename;
                return versionFilenames;
            }
        }

        /// <summary>
        ///   Gets proposed versions.
        /// </summary>
        public AssemblyVersions ToBecomeAssemblyVersions {
            get { return m_toBecomeAssemblyVersions; }
        }

        /// <summary>
        ///   Flag indicationg if project is versionable. For example, 
        ///   enterprise template projects do not contain any version
        ///   information.
        /// </summary>
        public bool IsVersionable {
            get { return ProjectTypeInfo.IsVersionable; }
        }

        /// <summary>
        ///   Increments project level (i.e. indentation).
        /// </summary>
        public void IncrementLevel() {
            m_level++;
            foreach (ProjectInfo pi in SubProjects) {
                pi.IncrementLevel();
                Debug.Assert(pi.Level == Level + 1);
            }
        }

        /// <summary>
        ///   Sets "to be version" according to configuration settings.
        /// </summary>
        /// <param name="versionProvider">
        ///   Version provider.
        /// </param>
        public void SetToBecomeVersion(NewVersionProvider versionProvider) {
            if (CurrentAssemblyVersions != AssemblyVersions.Empty)
                m_toBecomeAssemblyVersions = versionProvider.ProposeNewVersions(CurrentAssemblyVersions);
        }

        /// <summary>
        ///   Mark assembly version(s) for update.
        /// </summary>
        /// <param name="versionType">
        ///   Version type to mark. May be any combination of flags, including 
        ///   <c>AssemblyVersionType.All</c>.
        /// </param>
        public void MarkAssemblyVersionsForUpdate(AssemblyVersionType versionType) {
            if ((versionType & AssemblyVersionType.AssemblyVersion) == AssemblyVersionType.AssemblyVersion) {
                if (CurrentAssemblyVersions[AssemblyVersionType.AssemblyVersion] != ProjectVersion.Empty)
                    m_assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyVersion] = true;
            }
            if ((versionType & AssemblyVersionType.AssemblyFileVersion) == AssemblyVersionType.AssemblyFileVersion) {
                if (CurrentAssemblyVersions[AssemblyVersionType.AssemblyFileVersion] != ProjectVersion.Empty)
                    m_assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyFileVersion] = true;
            }
            if ((versionType & AssemblyVersionType.AssemblyInformationalVersion) == AssemblyVersionType.AssemblyInformationalVersion) {
                if (CurrentAssemblyVersions[AssemblyVersionType.AssemblyInformationalVersion] != ProjectVersion.Empty)
                    m_assemblyVersionsUpdateInfo[AssemblyVersionType.AssemblyInformationalVersion] = true;
            }
        }

        /// <summary>
        ///   Gets a flag indicating if an assembly version is marked for 
        ///   update.
        /// </summary>
        /// <param name="versionType">
        ///   Version type for which flag should be provided.
        /// </param>
        /// <returns>
        ///   <c>true</c> if version type is marked for update.
        /// </returns>
        public bool IsMarkedForUpdate(AssemblyVersionType versionType) {
            Debug.Assert(versionType != AssemblyVersionType.All);
            return (bool)m_assemblyVersionsUpdateInfo[versionType];
        }

        /// <summary>
        ///   Saves version to corresponding files.
        /// </summary>
        /// <param name="newVersion">
        ///   New version string to be saved.
        /// </param>
        /// <param name="numberingOptions">
        ///   Numbering options.
        /// </param>
        public bool Save(AssemblyVersionType versionTypeToSave, string newVersion) {
            foreach (VersionStream vs in m_versionStreams) {
                try {
                    vs.SaveVersion(versionTypeToSave, newVersion);
                }
                catch (Exception) {
                    MessageBox.Show(s_txtCannotSaveFile + Environment.NewLine +  vs.Filename, s_txtSaveErrorTitle);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Compares <c>ProjectInfo</c> name to the name provided ignoring case.
        /// </summary>
        /// <param name="projectName">
        ///   Name to compare to.
        /// </param>
        /// <returns>
        ///   A 32-bit signed integer indicating the lexical relationship between the two comparands.
        /// </returns>
        public int CompareTo(string projectName) {
            return string.Compare(ProjectName, projectName, true);
        }

        /// <summary>
        ///   Type constructor. Initializes localized strings.
        /// </summary>
        static ProjectInfo() {
            ResourceManager resources = new System.Resources.ResourceManager("BuildAutoIncrement.Resources.Shared", typeof(ResourceAccessor).Assembly);
            Debug.Assert(resources != null);
            
            s_txtSaveErrorTitle          = resources.GetString("Save Error");
            s_txtCannotSaveFile          = resources.GetString("Cannot save file");
            
            Debug.Assert(s_txtSaveErrorTitle != null);
            Debug.Assert(s_txtCannotSaveFile != null);
        }

		/// <summary>
		///   Next version AssemblyVersion.
		/// </summary>
        private AssemblyVersions             m_toBecomeAssemblyVersions;
        /// <summary>
        ///   List of all version streams attached to this project.
        /// </summary>
        private ArrayList                   m_versionStreams;
        /// <summary>
        ///   Flags indicating which assembly versions require version increment.
        /// </summary>
        private IDictionary                 m_assemblyVersionsUpdateInfo;

        private static readonly string s_txtSaveErrorTitle;
        private static readonly string s_txtCannotSaveFile;

    }
}