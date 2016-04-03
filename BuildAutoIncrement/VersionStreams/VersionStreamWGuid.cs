/*
 * Filename:    VersionStreamWGuid.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Base class for Setup projects that need GUID modification.
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
using System.Text.RegularExpressions;

namespace BuildAutoIncrement {
	/// <summary>
	///   Base class for Setup (.vd or .cab) and InstallShield projects.
	/// </summary>
	public abstract class VersionStreamWGuid : VersionStream
    {

        #region Constructors

        public VersionStreamWGuid(string filename)
            : base(filename)
        {
        }

        #endregion // Constructors

        #region Public methods

        public override AssemblyVersions GetVersions()
        {
            Hashtable projectVersions = new Hashtable(AssemblyVersions.AssemblyVersionTypes.Length);
            foreach (AssemblyVersionType avt in AssemblyVersions.AssemblyVersionTypes)
            {
                // setup projects have product version only
                if (avt != AssemblyVersionType.AssemblyInformationalVersion)
                    projectVersions[avt] = ProjectVersion.Empty;
                else
                    projectVersions[avt] = GetVersion(avt);
            }
            return new AssemblyVersions((ProjectVersion)projectVersions[AssemblyVersionType.AssemblyVersion], (ProjectVersion)projectVersions[AssemblyVersionType.AssemblyFileVersion], (ProjectVersion)projectVersions[AssemblyVersionType.AssemblyInformationalVersion]);
        }

        public override void SaveVersion(AssemblyVersionType typeToSave, string newVersion)
        {
            Debug.Assert(typeToSave == AssemblyVersionType.AssemblyInformationalVersion);
            int pos = GetMatchPosition(ProductVersionLinePattern, 0);
            Regex regex = new Regex(VersionPattern);
            m_fileContent = regex.Replace(m_fileContent, newVersion, 1, pos + 1);
            if (ConfigurationPersister.Instance.Configuration.NumberingOptions.GeneratePackageAndProductCodes)
                ModifyPackageAndProductCodes();
            FileUtil.SaveTextFile(Filename, m_fileContent, m_encoding);
        }

        #endregion // Public methods

        protected override string GetVersionString(AssemblyVersionType versionType) {
            Debug.Assert(versionType != AssemblyVersionType.AssemblyVersion && versionType != AssemblyVersionType.All);
            int pos = GetMatchPosition(ProductVersionLinePattern, 0);
            Regex regex = new Regex(VersionPattern);
            return regex.Match(m_fileContent, pos + 1).Value;
        }

        protected abstract string ProductVersionLinePattern { get; }

        protected int GetMatchPosition(string pattern, int startAt)
        {
            Regex regex = new Regex(pattern, RegexOptions.Multiline);
            Match match = regex.Match(m_fileContent, startAt);
            Debug.Assert(match.Success);
            return match.Index;
        }

        protected abstract void ModifyPackageAndProductCodes();

        protected void ModifyGuid(string codeLinePattern)
        {
            int pos = GetMatchPosition(codeLinePattern, 0);
            Regex regex = new Regex(GuidPattern);
            string newGuid = string.Format("{{{0}}}", Guid.NewGuid().ToString().ToUpper());
            m_fileContent = regex.Replace(m_fileContent, newGuid, 1, pos + 1);
        }

        protected const string GuidPattern = "{[0-9,A-F]{8}-[0-9,A-F]{4}-[0-9,A-F]{4}-[0-9,A-F]{4}-[0-9,A-F]{12}}";
	}
}
