/*
 * Filename:    InetRootLocator.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Gets path to IIS root from registry.
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

namespace BuildAutoIncrement {
	/// <summary>
	///   Provides path to IIS root. Implemented as a singleton.
	/// </summary>
	public sealed class InetRootLocator {

        private static InetRootLocator m_instance = null;

		private InetRootLocator() {
            m_pathWwwRoot = null;
            try {
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(InetStpPathKey)) {
                    if (rk != null) {
                        string pathWwwRoot = (string)rk.GetValue(PathSubKey);
                        if (Directory.Exists(pathWwwRoot)) {
                            m_pathWwwRoot = pathWwwRoot;
                        }
                        else {
                            Debug.Assert(false, string.Format("IIS WWW root folder {0} does not exist", pathWwwRoot));
                        }
                    }
                }
            }
            catch (Exception exception) {
                Trace.WriteLine(exception.ToString());
            }
        }

        /// <summary>
        ///   Gets the only instance of <c>InetRootLocator</c> class.
        /// </summary>
        public static InetRootLocator Instance {
            get {
                if (m_instance == null)
                    m_instance = new InetRootLocator();
                return m_instance;
            }
        }

        /// <summary>
        ///   Converts http address to actual path on the local machine. 
        /// </summary>
        /// <param name="address">
        ///   Address for which local filename is requested.
        /// </param>
        /// <returns>
        ///   Full path to the file.
        /// </returns>
        public string GetLocalPath(string address) {
            Debug.Assert(address.StartsWith(Http));
            Debug.Assert(IsIisAvailable);
            if (!address.StartsWith(Localhost))
                return string.Empty;
            string relPath = address.Substring(Localhost.Length).Replace("/", "\\");
            string path = Path.Combine(PathWwwRoot, relPath);
            Debug.Assert(Directory.Exists(path) || File.Exists(path));
            return path;
        }

        /// <summary>
        ///   Gets flag indicating if IIS is installed and its WWW root is available.
        /// </summary>
        public bool IsIisAvailable {
            get {
                return PathWwwRoot.Length > 0;
            }
        }

        /// <summary>
        ///   Gets path to the local WWW root path or empty string if not available.
        /// </summary>
        public string PathWwwRoot {
            get {
                return (m_pathWwwRoot != null) ? m_pathWwwRoot : string.Empty;
            }
        }

        private string m_pathWwwRoot;

        private const string InetStpPathKey = "SOFTWARE\\Microsoft\\InetStp";

        private const string PathSubKey     = "PathWWWRoot";

        public  const string Http           = "http://";

        public  const string Localhost      = "http://localhost/";
    }
}
