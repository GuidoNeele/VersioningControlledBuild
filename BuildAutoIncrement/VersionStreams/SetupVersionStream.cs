/*
 * Filename:    SetupVersionStream.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Reads and writes versions from/to setup project files.
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
	///   Responsible for reading and writing version of Setup projects.
	/// </summary>
	public class SetupVersionStream : VersionStreamWGuid
    {

        #region Internal IVdProjPattern interface and implementations

        private interface IVdProjPattern
        {
            string ProductVersion { get; }
            string ProductVersionPattern { get; }
            bool HasPackageProductCode { get; }
        }

        private class CabProjectPattern : IVdProjPattern
        {

            public string ProductVersion
            {
                get { return "\"Version\""; }
            }

            public string ProductVersionPattern
            {
                get { return @"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+"; }
            }

            public bool HasPackageProductCode
            {
                get { return false; }
            }
        }

        private class VdProjPattern : IVdProjPattern
        {

            public string ProductVersion
            {
                get { return "\"ProductVersion\""; }
            }

            public string ProductVersionPattern
            {
                get { return @"[0-9]+\.[0-9]+\.[0-9]+"; }
            }

            public bool HasPackageProductCode
            {
                get { return true; }
            }
        }

        #endregion //Internal IVdProjPattern interface and implementations

        #region Constructors

        public SetupVersionStream(string filename)
            : base(filename)
        {
            string projectType = GetProjectTypeGuid().ToUpper();
            switch (projectType)
            {
                case "{3EA9E505-35AC-4774-B492-AD1749C4943A}": // CAB project 
                case "{06A35CCD-C46D-44D5-987B-CF40FF872267}": // Merge Module project
                    m_vdProjectPattern = new CabProjectPattern();
                    break;
                default:
                    m_vdProjectPattern = new VdProjPattern();
                    break;
            }
        }

        #endregion // Constructors

        #region Protected methods

        protected override void ModifyPackageAndProductCodes()
        {
            if (m_vdProjectPattern.HasPackageProductCode)
            {
                ModifyGuid(GetGuidLinePattern(PackageCode));
                ModifyGuid(GetGuidLinePattern(ProductCode));
            }
        }

        #endregion // Protected methods

        #region Protected properties

        protected override string ProductVersionLinePattern
        {
            get { return OptionalWhitespacePattern + m_vdProjectPattern.ProductVersion + OptionalWhitespacePattern + "=" + OptionalWhitespacePattern + VersionPatternQuoted; }
        }

        protected override string VersionPattern
        {
            get
            {
                Debug.Assert(m_vdProjectPattern != null);
                return m_vdProjectPattern.ProductVersionPattern;
            }
        }

        #endregion Protected properties

        #region Private methods

        private string GetProjectTypeGuid()
        {
            int pos = GetMatchPosition(ProjectTypePattern, 0);
            Regex regex = new Regex(GuidPattern);
            return regex.Match(m_fileContent, pos + 1).Value;
        }

        private string GetGuidLinePattern(string codeName)
        {
            return string.Format("{0}{1}{2}", OptionalWhitespacePattern, codeName, OptionalWhitespacePattern + "=" + OptionalWhitespacePattern + GuidPatternQuoted);
        }

        #endregion // Private methods

        #region Private properties

        private string VersionPatternQuoted
        {
            get
            {
                Debug.Assert(m_vdProjectPattern != null);
                return "\"" + "8:" + VersionPattern + "\"";
            }
        }

        #endregion // Private properties

        #region Private fields

        private IVdProjPattern m_vdProjectPattern;

        #endregion // Private fields

        private const string PackageCode = "\"PackageCode\"";
        private const string ProductCode = "\"ProductCode\"";
        private const string ProjectTypePattern = OptionalWhitespacePattern + "\"ProjectType\"" + OptionalWhitespacePattern + "=" + OptionalWhitespacePattern + GuidPatternQuoted;
        private const string GuidPatternQuoted = "\"" + "8:" + GuidPattern + "\"";
	}
}
