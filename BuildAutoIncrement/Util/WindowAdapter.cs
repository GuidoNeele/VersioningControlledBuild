/*
 * Filename:    WindowAdapter.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Adapter class used to pass Environment Development object to
 *              ShowDialog method of the main form.
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

namespace BuildAutoIncrement {
    /// <summary>
    ///   Adapter class required to pass main window of Environment Development 
    ///   object as owner to <c>ShowDialog</c> method.
    /// </summary>
    public class WindowAdapter : System.Windows.Forms.IWin32Window {
        private IntPtr m_handle;

        public WindowAdapter(int handle) {
            m_handle = new IntPtr(handle);
        }

        public IntPtr Handle {
            get { return m_handle; }
        }
    }
}