/*
 * Filename:    SourceSafeLocator.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Locates the path to Source Safe command-line utility.
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
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.ComponentModel;

namespace BuildAutoIncrement {
	/// <summary>
	///   Class that gets the path to Source Safe command-line utility from
	///   registry. Implemented as a singleton.
	/// </summary>
	public sealed class SourceSafeLocator {

        private static SourceSafeLocator m_instance = null;

		private SourceSafeLocator() {
            m_sourceSafeRoot = GetSourceSafeRoot();
            if (m_sourceSafeRoot != null) {
                string sourceSafeExecutable = Path.Combine(m_sourceSafeRoot, "ss.exe");
                if (File.Exists(sourceSafeExecutable))
                    SourceSafeExecutable = sourceSafeExecutable;
            }
		}

        /// <summary>
        ///   Gets the only instance of <c>SourceSafeLocator</c> class.
        /// </summary>
        public static SourceSafeLocator Instance {
            get {
                if (m_instance == null)
                    m_instance = new SourceSafeLocator();
                return m_instance;
            }
        }

        /// <summary>
        ///   Returns flag indicating if SourceSafe is available.
        /// </summary>
        public bool IsSourceSafeAvailable {
            get {
                return File.Exists(SourceSafeExecutable);
            }
        }

        /// <summary>
        ///   Gets SourceSafe root folder or empty string if SourceSafe 
        ///   command line is not installed.
        /// </summary>
        public string SourceSafeRoot {
            get {
                return m_sourceSafeRoot != null ? m_sourceSafeRoot : string.Empty;
            }
        }

        /// <summary>
        ///   Reads Source Safe path from the registry.
        /// </summary>
        /// <returns>
        ///   Source safe executable path if successful, otherwise <c>null</c>.
        /// </returns>
        private string GetSourceSafeRoot() {
            // Win API call must be invoked to get correct key on 64-bit systems
            using (WoW64RegistryKey rk = new WoW64RegistryKey()) {
                try {
                    rk.Open(WoW64RegistryKey.HKEY_LOCAL_MACHINE, SourceSafeSubKey);
                    string path = rk.ReadString(SCCServerPathValue);
                    if (path != null && File.Exists(path)) {
                        return Path.GetDirectoryName(path);
                    }
                }
                catch (Win32Exception) {
                }
                return null;
            }
        }

        /// <summary>
        ///   Full path to SourceSafe command line executable, or <c>null</c> if 
        ///   it is not available.
        /// </summary>
        public readonly string SourceSafeExecutable = null;

        private readonly string m_sourceSafeRoot = null;

        private const string SourceSafeSubKey               = "SOFTWARE\\Microsoft\\SourceSafe";
        private const string SCCServerPathValue             = "SCCServerPath";
	}
}