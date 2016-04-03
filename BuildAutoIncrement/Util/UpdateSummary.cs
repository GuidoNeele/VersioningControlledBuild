/*
 * Filename:    UpdateSummary.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Stores information about version change of projects.
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
using System.Text;

namespace BuildAutoIncrement {
	/// <summary>
	///   Collection containing summary of updates.
	/// </summary>
	public class UpdateSummary {

        public enum UpdateState {
            Updated,
            NotUpdated,
            Failed
        }

        #region UpdateSummaryItem

        public class UpdateSummaryItem {

            public class AssemblyVersionItem {

                public AssemblyVersionItem(string version, UpdateState updateState) {
                    Version      = version;
                    UpdateState = updateState;
                }

                public string       Version      = "";
                public UpdateState  UpdateState  = UpdateState.NotUpdated; 
            }
            
            public UpdateSummaryItem(ProjectInfo projectInfo, UpdateState updateState) {
                ProjectName = projectInfo.ProjectName;
                ProjectFullName = projectInfo.FullName;
                UpdateState = updateState;
                foreach (AssemblyVersionType versionType in AssemblyVersions.AssemblyVersionTypes) {
                    string version;
                    AssemblyVersionItem avi;
                    if (updateState == UpdateState.Failed) {
                        version = projectInfo.CurrentAssemblyVersions[versionType].ToString();
                        avi = new AssemblyVersionItem(version, UpdateState.Failed);
                    }
                    if (projectInfo.IsMarkedForUpdate(versionType) && updateState == UpdateState.Updated) {
                        version = projectInfo.ToBecomeAssemblyVersions[versionType].ToString();
                        avi = new AssemblyVersionItem(version, UpdateState.Updated);
                    }
                    else {
                        version = projectInfo.CurrentAssemblyVersions[versionType].ToString();
                        avi = new AssemblyVersionItem(version, UpdateState.NotUpdated);
                    }
                    m_assemblyVersions.Add(versionType, avi);
                }
            }

            public AssemblyVersionItem this[AssemblyVersionType assemblyVersionType] {
                get {
                    return (AssemblyVersionItem)m_assemblyVersions[assemblyVersionType];
                }
            }

            public readonly string ProjectName;

            public readonly string ProjectFullName;

            private Hashtable m_assemblyVersions = new Hashtable(AssemblyVersions.AssemblyVersionTypes.Length);

            public  UpdateState UpdateState;

        }

        #endregion // UpdateSummaryItem

        public UpdateSummary() {
		}

        public void AddRange(ProjectInfo[] projectInfos) {
            foreach (ProjectInfo projectInfo in projectInfos) {
                if (projectInfo.IsVersionable) {
                    UpdateSummaryItem usi = new UpdateSummaryItem(projectInfo, UpdateState.NotUpdated);
                    m_projects.Add(usi);
                }
            }
        }

        public void Clear() {
            m_projects.Clear();
        }

        public void SetFailed(ProjectInfo projectInfo) {
            Debug.Assert(Contains(projectInfo));
            this[projectInfo].UpdateState = UpdateState.Failed;
        }

        public void SetUpdated(ProjectInfo projectInfo, AssemblyVersionType versionType, string newVersion) {
            Debug.Assert(Contains(projectInfo));
            this[projectInfo].UpdateState = UpdateState.Updated;
            this[projectInfo][versionType].UpdateState = UpdateState.Updated;
            this[projectInfo][versionType].Version = newVersion;
        }

        private bool Contains(ProjectInfo projectInfo) {
            foreach (UpdateSummaryItem usi in m_projects) {
                if (usi.ProjectFullName == projectInfo.FullName)
                    return true;
            }
            return false;
        }

        private UpdateSummaryItem this[ProjectInfo projectInfo] {
            get {
                foreach (UpdateSummaryItem usi in m_projects) {
                    if (usi.ProjectFullName == projectInfo.FullName)
                        return usi;
                }
                throw new IndexOutOfRangeException(string.Format("Invalid index: {0} : {1}", projectInfo.ProjectName, projectInfo.FullName));
            }
        }

        public int UpdatedItemsCount {
            get {
                int updatedCount = 0;
                foreach (UpdateSummaryItem usi in m_projects) {
                    if (usi.UpdateState == UpdateState.Updated)
                        updatedCount++;
                }
                return updatedCount; 
            }
        }

        public UpdateSummary.UpdateSummaryItem[] SummaryItems {
            get {
                UpdateSummary.UpdateSummaryItem[] items = new UpdateSummary.UpdateSummaryItem[m_projects.Count];
                m_projects.CopyTo(items, 0);
                return items;
            }
        }

        public override string ToString() {
            using (StringWriter sw = new StringWriter()) {
                if (UpdatedItemsCount == 0) {
                    sw.WriteLine(NothingToUpdate);
                }
                else {
                    sw.WriteLine(UpdateCaption);
                    foreach (UpdateSummaryItem item in m_projects) {
                        if (item.UpdateState == UpdateState.Updated) {
                            sw.WriteLine(ProjectNameCaption + sw.NewLine + FullPathCaption, item.ProjectName, item.ProjectFullName);
                            if (item[AssemblyVersionType.AssemblyVersion].UpdateState == UpdateSummary.UpdateState.Updated)
                                sw.WriteLine(AssemblyVersionCaption, item[AssemblyVersionType.AssemblyVersion].Version);
                            if (item[AssemblyVersionType.AssemblyInformationalVersion].UpdateState == UpdateSummary.UpdateState.Updated)
                                sw.WriteLine(ProductVersionCaption, item[AssemblyVersionType.AssemblyInformationalVersion].Version);
                            if (item[AssemblyVersionType.AssemblyFileVersion].UpdateState == UpdateSummary.UpdateState.Updated)
                                sw.WriteLine(FileVersionCaption, item[AssemblyVersionType.AssemblyFileVersion].Version);
                        }
                    }
                }
                sw.Flush();
                return sw.ToString();
            }
        }

        private ArrayList m_projects = new ArrayList();

        private const string NothingToUpdate        = "No project found for version update. Please run the GUI tool.";
        private const string UpdateCaption          = "VERSION UPDATE SUMMARY:";
        private const string ProjectNameCaption     = "Project Name: {0}";
        private const string FullPathCaption        = "Full Path:    {1}";
        private const string AssemblyVersionCaption = "  Assembly version: {0}";
        private const string ProductVersionCaption  = "  Product version:  {0}";
        private const string FileVersionCaption     = "  File version:     {0}";

        public static string CreateSummaryFilename(string solutionFilename) {
            StringBuilder sb = new StringBuilder(Path.GetFileNameWithoutExtension(solutionFilename));
            sb.Append(".VersionUpdateSummary");
            return sb.ToString();
        }
	}
}