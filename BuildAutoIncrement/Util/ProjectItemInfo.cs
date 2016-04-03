/*
 * Filename:    ProjectItemInfo.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     AddinImplementation
 * Description: Utility class that retains information on EnvDTE.ProjectItem.
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
using EnvDTE;

#if !FX1_1
using EnvDTE80;
using DTE = EnvDTE80.DTE2;
#endif

using System;
using System.Diagnostics;

namespace BuildAutoIncrement {

    public sealed class ProjectItemInfo {
        /// <summary>
        ///   Gets the "FullPath" property for the <c>ProjectItem</c> provided.
        /// </summary>
        /// <param name="projectItem">
        ///   <c>ProjectItem</c> for which full path is requested.
        /// </param>
        /// <returns>
        ///   Full path (for a file) or <c>null</c>.
        /// </returns>
        public static string GetItemFullPath(ProjectItem projectItem) {
            try {
                Property property = projectItem.Properties.Item("FullPath");
                Debug.Assert(property != null);
                return (string)property.Value;
            }
            catch {
            }
            return null;
        }
	}
}
