/*
 * Filename:    InstallShieldLEVersionStream.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Reads and writes versions from/to InstallShield LE files.
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
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BuildAutoIncrement
{
    public class InstallShieldLEVersionStream : VersionStreamWGuid
    {
        #region Constructors

        public InstallShieldLEVersionStream(string filename)
            : base(filename)
        {
        }

        #endregion // Constructors

        #region Protected methods

        protected override string VersionPattern
        {
            get { return RegExVersionPattern; }
        }

        protected override string ProductVersionLinePattern
        {
            get { return OptionalWhitespacePattern + "<row><td>ProductVersion</td><td>" + RegExVersionPattern + "</td><td/></row>"; }
        }

        protected override void ModifyPackageAndProductCodes()
        {
            ModifyGuid(RegExProductCodeLinePattern);
        }

        #endregion // Protected methods

        #region Private fields

        private const string RegExVersionPattern = @"([0-9]+)([\.][0-9]+){1,3}";
        private const string RegExProductCodeLinePattern = @"\s*<row><td>ProductCode</td><td>{[0-9,A-F]{8}-[0-9,A-F]{4}-[0-9,A-F]{4}-[0-9,A-F]{4}-[0-9,A-F]{12}}</td><td/></row>";

        #endregion // Private fields
    }
}
