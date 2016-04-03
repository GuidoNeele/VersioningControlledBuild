/*
 * Filename:    ListExporterToTextFile.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Exports projects list to a text file.
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
using System.IO;

namespace BuildAutoIncrement {
	/// <summary>
	/// Summary description for ListExporterToTextFile.
	/// </summary>
	public abstract class ListExporterToTextFile : ListExporterToFile {

        public ListExporterToTextFile() {
		}

        /// <summary>
        ///   Saves list to a file.
        /// </summary>
        /// <param name="filename">
        ///   Name of the file to save.
        /// </param>
        override protected void DoSave(string filename) {
            using (StreamWriter sw = new StreamWriter(filename)) {
                WriteHeader(sw);
                WriteHeading(sw);
                WriteItems(sw);
            }
        }

        abstract protected void WriteHeader(StreamWriter streamWriter);

        abstract protected void WriteHeading(StreamWriter streamWriter);

        abstract protected void WriteItems(StreamWriter streamWriter);
	}
}
