/*
 * Filename:    VisualStylesEnabler.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Utility class to enable visual styles. 
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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BuildAutoIncrement {
	/// <summary>
	///   Utility class that enables visual style for .NET 1.1 and higher.
	/// </summary>
	public struct VisualStyles {

        /// <summary>
        ///   Enables visual styles for .NET 1.1 and higher.
        /// </summary>
        public static void EnableStyles() {
            // since .NET Framework 1.0 does not have Application.EnableVisualStyles
            // method, we must not call this method or MissingMethodException
            // would be thrown when JIT Compiler invokes the method
            if (Environment.Version >= new Version(1, 1) && Enabled) {
                EnableVisualStyles();
            }
        }

        /// <summary>
        ///   Calls <c>Application.EnableVisualStyles</c> method. Placed in 
        ///   separate method in order to prevent MissingMethodException
        ///   to be thrown by JIT compiler if running on .NET 1.0
        /// </summary>
        private static void EnableVisualStyles() {
            // DoEvents must be called immediately after as a workaround for 
            // EnableVisualStyles bug:
            // http://www.codeproject.com/buglist/EnableVisualStylesBug.asp
            Application.EnableVisualStyles();
            Application.DoEvents();
        }

        /// <summary>
        ///   Checks if visual styles are supported.
        /// </summary>
        public static bool Supported {
            get {
                OperatingSystem os = Environment.OSVersion;
                bool isAppropriateOS = os.Platform == PlatformID.Win32NT && ((os.Version.Major == 5 && os.Version.Minor >= 1) || os.Version.Major > 5);
                if (!isAppropriateOS)
                    return false;
                if (OSFeature.Feature.GetVersionPresent(OSFeature.Themes) == null)
                    return false;
                Win32Api.DLLVersionInfo dllVersion = new Win32Api.DLLVersionInfo();
                dllVersion.cbSize = Marshal.SizeOf(typeof(Win32Api.DLLVersionInfo));
                Win32Api.DllGetVersion(ref dllVersion);
                return (dllVersion.dwMajorVersion >= 6);
            }
        }

        /// <summary>
        ///   Checks if visual styles are enabled.
        /// </summary>
        public static bool Enabled {
            get {
                return Win32Api.IsAppThemed() && Win32Api.IsThemeActive();
            }
        }

        /// <summary>
        ///   Sets <c>UseVisualStyleBackColor</c> to true when running 
        ///   application on .NET 2.0
        /// </summary>
        /// <param name="tc"></param>
        public static void SetUseVisualStyleBackColor(TabControl tc) {
            if (Environment.Version >= new Version(2, 0)) {
                TabControlUseVisualStyleBackColor(tc);
            }
        }

        /// <summary>
        ///   Sets <c>FlatStyle</c> for buttons to <c>FlatStyle.System</c> 
        ///   when running on versions of .NET earlier than 2.0
        /// </summary>
        /// <param name="form"></param>
        public static void SetButtonFlatStyleSystem(Form form) {
            if (Environment.Version < new Version(2, 0) && Supported && Enabled) {
                RecursivelySetButtonFlatStyleSystem(form);
            }
        }

        private static void TabControlUseVisualStyleBackColor(TabControl tc) {
            PropertyInfo pi = typeof(TabPage).GetProperty("UseVisualStyleBackColor", BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            Debug.Assert(pi != null);
            foreach (TabPage tp in tc.TabPages) {
                pi.SetValue(tp, true, null);
                RecursivelyUseVisualStyleBackColor(tp);
            }
        }

        private static void RecursivelyUseVisualStyleBackColor(Control control) {
            foreach (Control subControl in control.Controls) {
                ButtonBase bb = subControl as ButtonBase;
                if (bb != null) {
                    ButtonUseVisualStyleBackColorProperty.SetValue(bb, true, null);
                }
                if (subControl.Controls.Count > 0) {
                    RecursivelyUseVisualStyleBackColor(subControl);
                }
            }
        }

        private static void RecursivelySetButtonFlatStyleSystem(Control control) {
            foreach (Control subControl in control.Controls) {
                ButtonBase bb = subControl as ButtonBase;
                if (bb != null) {
                    ButtonFlatStyleProperty.SetValue(bb, FlatStyle.System, null);
                }
                if (subControl.Controls.Count > 0) {
                    RecursivelySetButtonFlatStyleSystem(subControl);
                }
            }
        }

        private static readonly PropertyInfo ButtonUseVisualStyleBackColorProperty = typeof(ButtonBase).GetProperty("UseVisualStyleBackColor", BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
        private static readonly PropertyInfo ButtonFlatStyleProperty = typeof(ButtonBase).GetProperty("FlatStyle", BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
    }
}