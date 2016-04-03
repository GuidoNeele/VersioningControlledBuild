/*
 * Filename:    ProjectInfoList.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: List of ProjectInfo objects.
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
	/// <summary>
	///   A list of all projects satisfying the filter provided.
	/// </summary>
    public class ProjectInfoList : IList, ICollection, IEnumerable {

        #region Constructors

        /// <summary>
        ///   Hidden default constructor.
        /// </summary>
        private ProjectInfoList() {
            m_projectInfos = new ArrayList();
        }

        /// <summary>
        ///   Creates a <c>ProjectInfoList</c> containing <c>ProjectInfo</c>
        ///   objects that have passed the filter provided.
        /// </summary>
        /// <param name="projectInfos">
        ///   An array of all <c>ProjectInfo</c> objects.
        /// </param>
        /// <param name="filter">
        ///   A filter used to select <c>ProjectInfo</c> objects.
        /// </param>
        /// <param name="newVersionProvider">
        ///   <c>NewVersionProvider</c> object responsible to propose a new 
        ///   version for each <c>ProjectInfo</c> object.
        /// </param>
        /// <param name="assemblyVersionsUpdateMask">
        ///   Mask defining which versions (AssemblyInfo, Informational or 
        ///   Product) may be updated.
        /// </param>
        public ProjectInfoList(ProjectInfo[] projectInfos, IProjectFilter filter, NewVersionProvider newVersionProvider, AssemblyVersionType assemblyVersionsUpdateMask) {
            m_projectInfos = new ArrayList(projectInfos.Length);
            m_highestToBeAssemblyVersions    = AssemblyVersions.MinValue;
            m_highestProposedAssemblyVersion = AssemblyVersions.MinValue;
            foreach (ProjectInfo projectInfo in projectInfos) {
                if (filter.Pass(projectInfo)) {
                    ProjectInfo pi = projectInfo;
                    pi.SetToBecomeVersion(newVersionProvider);
                    if (pi.Modified) 
                        pi.MarkAssemblyVersionsForUpdate(assemblyVersionsUpdateMask);
                    m_projectInfos.Add(pi);
                    m_highestToBeAssemblyVersions    = AssemblyVersions.Max(m_highestToBeAssemblyVersions, pi);
                    m_highestProposedAssemblyVersion = AssemblyVersions.MaxProposed(m_highestProposedAssemblyVersion, pi);
                }
            }
        }

        #endregion // Constructors

        #region Public properties

        /// <summary>
        ///   Gets a list of filtered <c>ProjectInfo</c> objects.
        /// </summary>
        public ProjectInfo[] ProjectInfos {
            get {
                return (ProjectInfo[])m_projectInfos.ToArray(typeof(ProjectInfo));
            }
        }

        /// <summary>
        ///   Gets the highest of all "to be" project versions in the current solution.
        /// </summary>
        public AssemblyVersions HighestToBeAssemblyVersions {
            get { 
                return m_highestToBeAssemblyVersions; 
            }
        }

        /// <summary>
        ///   Gets the highest version among those marked for update.
        /// </summary>
        public AssemblyVersions HighestProposedAssemblyVersions {
            get { 
                return m_highestProposedAssemblyVersion; 
            }
        }

        #endregion //Public properties

        #region Public methods

        /// <summary>
        ///   Checks if project with given name exists in the list of 
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public bool Contains(string projectName) {
            foreach (ProjectInfo projectInfo in m_projectInfos) {
                if (string.Compare(projectInfo.ProjectName, projectName, true) == 0)
                    return true;
            }
            return false;
        }

        #endregion // Public methods

        #region IList interface implementation

        bool IList.IsFixedSize {
            get { return true; }
        }

        bool IList.IsReadOnly {
            get { return true; }
        }

        object IList.this[int index] {
            get { return this[index]; }
            set { this[index] = (ProjectInfo)value; }
        }

        public ProjectInfo this[int index] {
            get { return (ProjectInfo)m_projectInfos[index]; }
            set { m_projectInfos[index] = value; }
        }

        int IList.Add(object obj) {
            throw new NotImplementedException();
        }

        void IList.Clear() {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value) {
            throw new NotImplementedException();
        }

        int IList.IndexOf(object value) {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object value) {
            throw new NotImplementedException();
        }

        void IList.Remove(object value) {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index) {
            throw new NotImplementedException();
        }

        #endregion // IList interface implementation

        #region ICollection interface implementation

        int ICollection.Count {
            get { return m_projectInfos.Count; }
        }

        bool ICollection.IsSynchronized {
            get { return m_projectInfos.IsSynchronized; }
        }

        object ICollection.SyncRoot {
            get { return m_projectInfos.SyncRoot; }
        }

        void ICollection.CopyTo(Array array, int index) {
            m_projectInfos.CopyTo(array, index);
        }


        #endregion // ICollection interface implementation

        #region IEnumerable interface implementation

        IEnumerator IEnumerable.GetEnumerator() {
            return m_projectInfos.GetEnumerator();
        }

        #endregion // IEnumerable interface implementation

        #region Private fields

        private ArrayList           m_projectInfos;

        private AssemblyVersions    m_highestToBeAssemblyVersions;

        private AssemblyVersions    m_highestProposedAssemblyVersion;

        #endregion // Private fields
    }
}