/*
 * Filename:    AssemblyVersions.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Wrapper arround the set of versions (AssemblyVersion, 
 *              AssemblyFileVersion and AssemblyInformationalVersion).
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
	///   Class encapsulating AssemblyVersion, AssemblyFileVersion and 
	///   AssemblyInformationalVersion.
	/// </summary>
	public class AssemblyVersions : ICloneable {

        #region Constructors

		private AssemblyVersions() : this(ProjectVersion.Empty, ProjectVersion.Empty, ProjectVersion.Empty) {
		}

        public AssemblyVersions(AssemblyVersions assemblyVersions) 
            : this(assemblyVersions[AssemblyVersionType.AssemblyVersion].Clone(), 
                   assemblyVersions[AssemblyVersionType.AssemblyFileVersion].Clone(),
                   assemblyVersions[AssemblyVersionType.AssemblyInformationalVersion].Clone()) {
        }

        public AssemblyVersions(ProjectVersion assemblyVersion, ProjectVersion assemblyFileVersion, ProjectVersion assemblyInformationalVersion) {
            m_versions = new Hashtable();
            m_versions[AssemblyVersionType.AssemblyVersion]              = assemblyVersion;
            m_versions[AssemblyVersionType.AssemblyFileVersion]          = assemblyFileVersion;
            m_versions[AssemblyVersionType.AssemblyInformationalVersion] = assemblyInformationalVersion;
        }

        #endregion // Constructors

        #region Public properties

        public ProjectVersion HighestProjectVersion {
            get {
                ProjectVersion highest = ProjectVersion.MinValue;
                foreach (ProjectVersion pv in m_versions.Values) {
                    if (highest < pv)
                        highest = pv;
                }
                return highest.Clone();
            }
        }

        public bool AreVersionsSynchronized {
            get {
                AssemblyVersionType[] validVersionTypes = GetValidVersionTypes();
                if (validVersionTypes.Length < 1)
                    return true;
                ProjectVersion reference = (ProjectVersion)m_versions[validVersionTypes[0]];
                for (int i = 1; i < validVersionTypes.Length; i++) {
                    if (reference != (ProjectVersion)m_versions[validVersionTypes[i]])
                        return false;
                }
                return true;
            }
        }

        public AssemblyVersionType[] GetValidVersionTypes() {
            ArrayList types = new ArrayList();
            foreach (AssemblyVersionType avt in AssemblyVersionTypes) {
                if (this[avt] != ProjectVersion.Empty)
                    types.Add(avt);
            }
            return (AssemblyVersionType[])types.ToArray(typeof(AssemblyVersionType));
        }

        public ProjectVersion this[AssemblyVersionType type] {
            get {
                Debug.Assert(type != AssemblyVersionType.All);
                return (ProjectVersion)m_versions[type];
            }
            set {
                Debug.Assert(type != AssemblyVersionType.All);
                m_versions[type] = value;
            }
        }

        #endregion //Public properties

        #region Public methods

        public void Increment(AssemblyVersionType typeToIncrement, NumberingOptions numberingOptions) {
            if ((typeToIncrement & AssemblyVersionType.AssemblyVersion) != 0) {
                this[AssemblyVersionType.AssemblyVersion].Increment(numberingOptions);
            }
            if ((typeToIncrement & AssemblyVersionType.AssemblyFileVersion) != 0) {
                this[AssemblyVersionType.AssemblyFileVersion].Increment(numberingOptions);
            }
            if ((typeToIncrement & AssemblyVersionType.AssemblyInformationalVersion) != 0) {
                this[AssemblyVersionType.AssemblyInformationalVersion].Increment(numberingOptions);
            }
        }

        public void SynchronizeVersionsToHighest() {
            ProjectVersion highestProjectVersion = HighestProjectVersion;
            if (highestProjectVersion != ProjectVersion.MinValue) {
                if ((ProjectVersion)m_versions[AssemblyVersionType.AssemblyVersion] != ProjectVersion.Empty)
                    m_versions[AssemblyVersionType.AssemblyVersion] = highestProjectVersion;
                if ((ProjectVersion)m_versions[AssemblyVersionType.AssemblyFileVersion] != ProjectVersion.Empty)
                    m_versions[AssemblyVersionType.AssemblyFileVersion] = highestProjectVersion;
                if ((ProjectVersion)m_versions[AssemblyVersionType.AssemblyInformationalVersion] != ProjectVersion.Empty)
                    m_versions[AssemblyVersionType.AssemblyInformationalVersion] = highestProjectVersion;
            }
        }

        public bool ContainsVersion(AssemblyVersionType assemblyVersionType) {
            Debug.Assert(assemblyVersionType != AssemblyVersionType.All && assemblyVersionType != AssemblyVersionType.None);
            return Array.IndexOf(GetValidVersionTypes(), assemblyVersionType) > -1;
        }

        #endregion // Public methods

        #region ICloneable implementation

        object ICloneable.Clone() {
            return Clone();
        }

        public AssemblyVersions Clone() {
            return new AssemblyVersions(this);
        }

        #endregion // ICloneable implementation

        private Hashtable m_versions;

        #region Static methods

        public static AssemblyVersions Max(AssemblyVersions assemblyVersions1, AssemblyVersions assemblyVersions2) {
            ProjectVersion assemblyVersion              = ProjectVersion.Max(assemblyVersions1[AssemblyVersionType.AssemblyVersion], assemblyVersions2[AssemblyVersionType.AssemblyVersion]).Clone();
            ProjectVersion assemblyFileVersion          = ProjectVersion.Max(assemblyVersions1[AssemblyVersionType.AssemblyFileVersion], assemblyVersions2[AssemblyVersionType.AssemblyFileVersion]).Clone();
            ProjectVersion assemblyInformationalVersion = ProjectVersion.Max(assemblyVersions1[AssemblyVersionType.AssemblyInformationalVersion], assemblyVersions2[AssemblyVersionType.AssemblyInformationalVersion]).Clone();
            return new AssemblyVersions(assemblyVersion, assemblyFileVersion, assemblyInformationalVersion);
        }

        public static AssemblyVersions Max(AssemblyVersions assemblyVersions, ProjectInfo projectInfo) {
            return Max(assemblyVersions, projectInfo.ToBecomeAssemblyVersions);
        }

        public static AssemblyVersions MaxProposed(AssemblyVersions assemblyVersions, ProjectInfo projectInfo) {
            if (projectInfo.Modified)
                return Max(assemblyVersions, projectInfo.ToBecomeAssemblyVersions);
            return Max(assemblyVersions, projectInfo.CurrentAssemblyVersions);
        }

        #endregion // Static methods

        #region Static fields

        public static readonly AssemblyVersions Empty    = new AssemblyVersions();

        public static readonly AssemblyVersions MinValue = new AssemblyVersions(ProjectVersion.MinValue, ProjectVersion.MinValue, ProjectVersion.MinValue);

        public static readonly AssemblyVersionType[] AssemblyVersionTypes = { AssemblyVersionType.AssemblyVersion, AssemblyVersionType.AssemblyFileVersion, AssemblyVersionType.AssemblyInformationalVersion };

        #endregion // Static fields

    }
}