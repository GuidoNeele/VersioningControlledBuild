/*
 * Filename:    ImplementationAssemblyLoader.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Utility to load the implementation assembly depending on the 
 *              runtime version.
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace BuildAutoIncrement {
	/// <summary>
	///   Utility structure to load corresponding implementation assembly for
	///   runtime version provided. Used during uninstallation to assure that
	///   toolbar has been removed.
	/// </summary>
	public struct ImplementationAssemblyLoader {

        private const string AddinImplementationAssemblyBasename    = "AddinImplementation.";

        private const string FrameworkNotSupported                  = ".NET version {0} not supported.";

        public static Assembly LoadMainAssembly(int runtimeVersion) {
            Debug.Assert(runtimeVersion >= 1 && runtimeVersion <= 4);

            string addinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            StringBuilder mainAssemblyFile = new StringBuilder(Path.Combine(addinPath, AddinImplementationAssemblyBasename));
            switch (runtimeVersion) {
                case 1:
                    mainAssemblyFile.Append("VS7");
                    break;
                case 2:
                case 3:
                case 4:
                    mainAssemblyFile.Append("VS8");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("runtimeVersion", string.Format(FrameworkNotSupported, runtimeVersion));
            }
            mainAssemblyFile.Append(".dll");
            return Assembly.LoadFrom(mainAssemblyFile.ToString());
        }
	}
}