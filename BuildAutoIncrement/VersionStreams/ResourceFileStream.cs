/*
 * Filename:    ResourceFileStream.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Reads and writes versions from/to VC++ resource files.
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
	///   Class responsible for getting and saving version from VC++ resource 
	///   (.RC) files.
	/// </summary>
	public class ResourceFileStream : VersionStream {

        /// <summary>
        ///   Creates <c>ResourceFileStream</c> object for resource file provided.
        /// </summary>
        /// <param name="filename">
        ///   Name of the resource file.
        /// </param>
        public ResourceFileStream(string filename) : base(filename) {
        }

        /// <summary>
        ///   Reads version from the file.
        /// </summary>
        /// <returns>
        ///   <c>AssemblyVersions</c> collection.
        /// </returns>
        public override AssemblyVersions GetVersions() {
            Hashtable projectVersions = new Hashtable(AssemblyVersions.AssemblyVersionTypes.Length);
            foreach (AssemblyVersionType avt in AssemblyVersions.AssemblyVersionTypes) {
                // resource file does not contain assembly version
                if (avt == AssemblyVersionType.AssemblyVersion) {
                    projectVersions[avt] = ProjectVersion.Empty;
                }
                else {
                    projectVersions[avt] = GetVersion(avt);
                }
            }
            return new AssemblyVersions((ProjectVersion)projectVersions[AssemblyVersionType.AssemblyVersion], (ProjectVersion)projectVersions[AssemblyVersionType.AssemblyFileVersion], (ProjectVersion)projectVersions[AssemblyVersionType.AssemblyInformationalVersion]);
        }

        /// <summary>
        ///   Saves version to resource file.
        /// </summary>
        /// <param name="typeToSave">
        ///   Flags specifying which versions to save.
        /// </param>
        /// <param name="newVersion">
        ///   New version to save.
        /// </param>
        public override void SaveVersion(AssemblyVersionType typeToSave, string newVersion) {
            Debug.Assert(typeToSave != AssemblyVersionType.All);
            if (typeToSave == AssemblyVersionType.AssemblyVersion)
                return;
            if (typeToSave == AssemblyVersionType.AssemblyFileVersion) {
                SetHeaderVersionString(FileVersionCaps, newVersion);
                SetBlockVersionString(FileVersionQuoted, newVersion);
            }
            if (typeToSave == AssemblyVersionType.AssemblyInformationalVersion) {
                SetHeaderVersionString(ProductVersionCaps, newVersion);
                SetBlockVersionString(ProductVersionQuoted, newVersion);
            }
            FileUtil.SaveTextFile(Filename, m_fileContent, m_encoding);
        }

        /// <summary>
        ///   Extracts version string from resource file content.
        /// </summary>
        /// <param name="versionType">
        ///   Version type to search. Can be AssemblyFileVersion or 
        ///   AssemblyInformationalVersion, corresponding to FILEVERSION and
        ///   PRODUCTVERSION respectively.
        /// </param>
        /// <returns>
        ///   Version string found.
        /// </returns>
        protected override string GetVersionString(AssemblyVersionType versionType) {
            Debug.Assert(versionType != AssemblyVersionType.AssemblyVersion && versionType != AssemblyVersionType.All);
            // if VS_VERSION_INFO header not found, there is no valid version
            Regex regex = new Regex(VersionInfoHeaderLine, RegexOptions.Multiline);
            Match match = regex.Match(m_fileContent);
            if (!match.Success)
                return "";
            int offset = match.Index + match.Length;
            string pattern = "";
            switch (versionType) {
            case AssemblyVersionType.AssemblyFileVersion:
                pattern = StartOfLine + FileVersionCaps + OneOrMoreWhitespacePattern + VersionPattern;
                break;
            case AssemblyVersionType.AssemblyInformationalVersion:
                pattern = StartOfLine + ProductVersionCaps + OneOrMoreWhitespacePattern + VersionPattern;
                break;
            default:
                Debug.Assert(false, string.Format("Illegal versionName: {0}", versionType.ToString()));
                break;
            }
            regex = new Regex(pattern, RegexOptions.Multiline);
            match = regex.Match(m_fileContent, offset);
            Debug.Assert(match.Value.Length > 0);
            return match.Value;
        }

        protected override string VersionPattern {
            get { return ResourceVersionPattern; }
        }

        /// <summary>
        ///   Sets the the version in the VERSIONINFO header (i.e. FILEVERSION 
        ///   and PRODUCTVERSION values);
        /// </summary>
        /// <param name="fileContent">
        ///   Content of the reource file.
        /// </param>
        /// <param name="versionName">
        ///   Name of the version to modify (FILEVERSION or PRODUCTVERSION).
        /// </param>
        /// <param name="version">
        ///   New version string to apply.
        /// </param>
        private void SetHeaderVersionString(string versionName, string version) {
            // first find start of the header
            Regex regex = new Regex(VersionInfoHeaderLine, RegexOptions.Multiline);
            Match match = regex.Match(m_fileContent);
            regex = new Regex(StartOfLine + versionName + OneOrMoreWhitespacePattern + ResourceVersionPattern, RegexOptions.Multiline);
            FindAndReplaceAllVersionStrings(regex, version, match.Index + match.Length);
        }

        /// <summary>
        ///   Sets the the version in the "StringFileInfo" block (i.e. FileVersion 
        ///   and ProductVersion values);
        /// </summary>
        /// <param name="fileContent">
        ///   Content of the resource file.
        /// </param>
        /// <param name="atributeName">
        ///   Name of the version attribute.
        /// </param>
        /// <param name="version">
        ///   Version string.
        /// </param>
        private void SetBlockVersionString(string atributeName, string version) {
            // first find start of the header
            Regex regex = new Regex(VersionInfoHeaderLine, RegexOptions.Multiline);
            Match match = regex.Match(m_fileContent);
            regex = new Regex(StartOfLine + Value + OneOrMoreWhitespacePattern + atributeName + "\\," + OptionalWhitespacePattern + "\"" + VersionPattern + "\"", RegexOptions.Multiline);
            // in block, versions may be shorter 
            version = ReduceBlockVersion(regex, version, match.Index + match.Length);
            FindAndReplaceAllVersionStrings(regex, version, match.Index + match.Length);
        }

        /// <summary>
        ///   Block FileVersion and ProductVersion may consist of less than 4 
        ///   numbers, and in such case version to be applied should be trimmed.
        /// </summary>
        /// <param name="regularExpression">
        ///   Regular expression object for the block version line.
        /// </param>
        /// <param name="version">
        ///   Version to be applied.
        /// </param>
        /// <param name="offset">
        ///   Offset from file start.
        /// </param>
        /// <returns>
        ///   Trimmed version that will be applied.
        /// </returns>
        private string ReduceBlockVersion(Regex regularExpression, string version, int offset) {
            string line = regularExpression.Match(m_fileContent, offset).Value;
            Regex regex = new Regex(VersionPattern);
            string currVersion = regex.Match(line).Value;
            int versionLength = currVersion.Split('.', ',').Length;
            if (versionLength == 4)
                return version;
            offset = 0;
            while (versionLength > 0) {
                offset = version.IndexOf(".", offset + 1);
                versionLength--;
            }
            return version.Substring(0, offset);
        }

        /// <summary>
        ///   Searches and replaces file version strings.
        /// </summary>
        /// <param name="fileContent">
        ///   Content of the resource file.
        /// </param>
        /// <param name="regularExpression">
        ///   Regular expression used to search the content.
        /// </param>
        /// <param name="version">
        ///   Version string.
        /// </param>
        private void FindAndReplaceAllVersionStrings(Regex regularExpression, string version, int offset) {
            string commaSeparatedVersion = version.Replace('.', ',');
            Regex regexVersion = new Regex(VersionPattern);
            Match lineMatch = regularExpression.Match(m_fileContent, offset);
            while (lineMatch.Index > 0) {
                // check the separator used for version components
                string newVersion = regexVersion.Match(lineMatch.Value).Value;
                if (newVersion.IndexOf(',') != -1)
                    newVersion = regexVersion.Replace(lineMatch.Value, commaSeparatedVersion);
                else
                    newVersion = regexVersion.Replace(lineMatch.Value, version);
                m_fileContent = regularExpression.Replace(m_fileContent, newVersion, 1, lineMatch.Index);
                lineMatch = regularExpression.Match(m_fileContent, lineMatch.Index + lineMatch.Length);
            }
        }

        private const string VsVersionInfoHeader        = "VS_VERSION_INFO";
        private const string VersionInfoHeader          = "VERSIONINFO";
        private const string VersionInfoHeaderLine      = StartOfLine + VsVersionInfoHeader + OneOrMoreWhitespacePattern + VersionInfoHeader;
        private const string FileVersionCaps            = "FILEVERSION";
        private const string ProductVersionCaps         = "PRODUCTVERSION";
        private const string Value                      = "VALUE";
        private const string FileVersionQuoted          = "\"FileVersion\"";
        private const string ProductVersionQuoted       = "\"ProductVersion\"";

        private const string ResourceVersionPattern       = @"([0-9]+)([\,\.]\s*[0-9]+){1,3}";
        private const string VersionPatternQuoted         = "\"" + OptionalWhitespacePattern + ResourceVersionPattern + OptionalWhitespacePattern + "\"";

    }
}
