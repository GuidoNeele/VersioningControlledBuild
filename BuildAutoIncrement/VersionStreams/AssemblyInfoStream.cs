/*
 * Filename:    AssemblyInfoStream.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Reads and writes assembly versions from/to assembly info files.
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
using System.Text.RegularExpressions;
using System.Globalization;

namespace BuildAutoIncrement {
	/// <summary>
	///   Class for reading and writing assembly versions from assembly info files.
	/// </summary>
	public class AssemblyInfoStream : VersionStream {

        #region VersionPatternProvider
        /// <summary>
        ///   Class responsible for building assembly version search patterns
        ///   for different project types.
        /// </summary>
        protected class VersionPatternProvider {
            public VersionPatternProvider(string assemblyInfoFileExtension) {
                Debug.Assert(m_brackets.ContainsKey(assemblyInfoFileExtension));
                Debug.Assert(m_assemblyPrefixes.ContainsKey(assemblyInfoFileExtension));
                m_extension = assemblyInfoFileExtension;
            }

            /// <summary>
            ///   Gets string used as opening bracket for attribute.
            /// </summary>
            public string LeftBracket {
                get { 
                    Debug.Assert((m_brackets != null) && (m_brackets[m_extension] is string[]) && ((string[])m_brackets[m_extension]).Length == 2);
                    return (string)((string[])m_brackets[m_extension])[0]; 
                }
            }

            /// <summary>
            ///   Gets string used as a closing bracket for attribute.
            /// </summary>
            public string RightBracket {
                get { 
                    Debug.Assert((m_brackets != null) && (m_brackets[m_extension] is string[]) && ((string[])m_brackets[m_extension]).Length == 2);
                    return (string)((string[])m_brackets[m_extension])[1]; 
                }
            }

            /// <summary>
            ///   Gets string which prefixes attribute name
            /// </summary>
            public string AssemblyPrefix {
                get {
                    Debug.Assert((m_brackets != null) && m_assemblyPrefixes.ContainsKey(m_extension));
                    return (string)m_assemblyPrefixes[m_extension];
                }
            }

            /// <summary>
            ///   File extension for handled assembly info file.
            /// </summary>
            private string m_extension;

            /// <summary>
            ///   Table with opening and closing brackets for different file
            ///   types.
            /// </summary>
            private static readonly Hashtable m_brackets = new Hashtable();

            /// <summary>
            ///   Table with assembly prefixes for different file types.
            /// </summary>
            private static readonly Hashtable m_assemblyPrefixes = new Hashtable();

            static VersionPatternProvider() {
                m_brackets.Add(".cs",  new string[] { "\\[",        "\\]" });
                m_brackets.Add(".vb",  new string[] { "\\<",        "\\>" });
                m_brackets.Add(".cpp", new string[] { "\\[",        "\\]" });
                m_brackets.Add(".jsl", new string[] { "/\\*\\*",    "\\*/" });

                m_assemblyPrefixes.Add(".cs",  "\\s*assembly\\s*:\\s*");
                m_assemblyPrefixes.Add(".vb",  "Assembly\\s*:\\s*");
                m_assemblyPrefixes.Add(".cpp", "\\s*assembly\\s*:\\s?");
                m_assemblyPrefixes.Add(".jsl", "\\s*@assembly\\s*" );
            }
        }
        #endregion // VersionPatternProvider

        public AssemblyInfoStream(string fileName) : base(fileName) {
            m_versionPatternProvider = new VersionPatternProvider(Path.GetExtension(fileName));
        }

        public override AssemblyVersions GetVersions() {
            Hashtable projectVersions = new Hashtable(AssemblyVersions.AssemblyVersionTypes.Length);
            foreach (AssemblyVersionType avt in AssemblyVersions.AssemblyVersionTypes) {
                projectVersions[avt] = GetVersion(avt);
            }
            return new AssemblyVersions((ProjectVersion)projectVersions[AssemblyVersionType.AssemblyVersion], (ProjectVersion)projectVersions[AssemblyVersionType.AssemblyFileVersion], (ProjectVersion)projectVersions[AssemblyVersionType.AssemblyInformationalVersion]);
        }

        public override void SaveVersion(AssemblyVersionType typeToSave, string newVersion) {
            SetVersionString(typeToSave, newVersion);
            FileUtil.SaveTextFile(Filename, m_fileContent, m_encoding);
        }

        /// <summary>
        ///   Searches the file content for the version provided.
        /// </summary>
        /// <param name="versionName">
        ///   Short attribute name to search for.
        /// </param>
        /// <returns></returns>
        protected override string GetVersionString(AssemblyVersionType versionType) {
            Debug.Assert(versionType != AssemblyVersionType.All);
            string shortAttributeName = versionType.ToString();
            Debug.Assert(shortAttributeName != null && !shortAttributeName.EndsWith(Attribute));
            // first try with a short name
            string pattern = BuildAttributeLinePattern(shortAttributeName);
            Regex regex = new Regex(pattern, RegexOptions.Multiline);
            Match match = regex.Match(m_fileContent);
            if (match.Length > 0)
                return match.ToString();
            // now try with a long one
            pattern = BuildAttributeLinePattern(shortAttributeName + Attribute);
            regex = new Regex(pattern, RegexOptions.Multiline);
            match = regex.Match(m_fileContent);
            return match.ToString();
        }

        /// <summary>
        ///   Builds a pattern to search the entire attribute line.
        /// </summary>
        /// <param name="attributeName">
        ///   Attribute name to search for
        /// </param>
        /// <returns>
        ///   Pattern for search.
        /// </returns>
        private string BuildAttributeLinePattern(string attributeName) {
            string leftBracket      = m_versionPatternProvider.LeftBracket;
            string rightBracket     = m_versionPatternProvider.RightBracket;
            string assemblyPrefix   = m_versionPatternProvider.AssemblyPrefix;
            StringBuilder pattern   = new StringBuilder(StartOfLine);
            pattern.Append(leftBracket);
            pattern.Append(assemblyPrefix);
            pattern.Append(attributeName);
            pattern.Append(ParenthesesEnclosedString);
            pattern.Append(rightBracket);
            return pattern.ToString();
        }

        protected void SetVersionString(AssemblyVersionType versionType, string newVersion) {
            string entireAttributeString = GetVersionString(versionType);
            Debug.Assert(entireAttributeString.Length > 0);
            Regex regex = new Regex(QuotesEnclosedPattern);
            string newVersionString = regex.Replace(entireAttributeString, string.Format(CultureInfo.CurrentCulture, "\"{0}\"", newVersion), 1);
            Debug.Assert(newVersionString.Length > 0);
            m_fileContent = m_fileContent.Replace(entireAttributeString, newVersionString);
            Debug.Assert(m_fileContent.IndexOf(newVersionString) != -1);
        }


        protected override string VersionPattern {
            get { return AssemblyInfoVersionPattern; }
        }

        protected void SetVersionPatternProvider(VersionPatternProvider versionPatternProvider) {
            m_versionPatternProvider = versionPatternProvider;
        }

        private VersionPatternProvider m_versionPatternProvider;

        private const string ParenthesesEnclosedString              = OptionalWhitespacePattern + @"\((?>[^()]+)*\)" + OptionalWhitespacePattern;

        public const string Attribute                               = "Attribute";

        public const string AssemblyVersion                         = "AssemblyVersion";
        public const string AssemblyVersionAttribute                = "AssemblyVersionAttribute";

        public const string AssemblyFileVersion                     = "AssemblyFileVersion";
        public const string AssemblyFileVersionAttribute            = "AssemblyFileVersionAttribute";

        public const string AssemblyInformationalVersion            = "AssemblyInformationalVersion";
        public const string AssemblyInforationalVersionAttribute    = "AssemblyInformationalVersionAttribute";

        private const string AssemblyInfoVersionPattern             = @"([0-9]+)((\.[0-9]+)*)(((\.\*)|((\.[0-9]+)*)((\.[0-9]+)*)))";

    }
}