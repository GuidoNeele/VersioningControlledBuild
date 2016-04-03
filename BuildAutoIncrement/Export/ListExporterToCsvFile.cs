/*
 * Filename:    ListExporterToCsvFile.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Exports projects list to a CSV file.
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
using System.Text;

namespace BuildAutoIncrement {
	/// <summary>
	/// Summary description for ListExporterToCsvFile.
	/// </summary>
	public class ListExporterToCsvFile : ListExporterToTextFile {

        public ListExporterToCsvFile() {
            Separator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
		}

        override protected string ExportFileFilter {
            get {
                return "Comma-separated values files (*.csv)|*.csv|All files (*.*)|*.*";
            }
        }

        override protected string ExportFileExtension { 
            get {
                return ".vcb.csv"; 
            }
        }

        /// <summary>
        ///   Writes header with solution name and current date and time.
        /// </summary>
        /// <param name="streamWriter">
        ///   <c>StreamWriter</c> used to write header.
        /// </param>
        override protected void WriteHeader(StreamWriter streamWriter) {
            streamWriter.WriteLine("\"{0}\"", m_solutionName);
            streamWriter.WriteLine("\"{0}\"", m_exportDateTime.ToString("g"));
            streamWriter.WriteLine();
        }

        /// <summary>
        ///   Writes heading with column captions.
        /// </summary>
        /// <param name="streamWriter">
        ///   <c>StreamWriter</c> used to write heading.
        /// </param>
        override protected void WriteHeading(StreamWriter streamWriter) {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("\"{0}\"", HeaderProjectName);
            foreach (AssemblyVersionTypeSelection avts in m_assemblyVersionTypes) {
                if (avts.IsSelected) {
                    output.Append(Separator);
                    output.AppendFormat("\"{0}\"", m_headings[avts.AssemblyVersionType]);
                }
            }
            streamWriter.WriteLine(output);
        }

        override protected void WriteItems(StreamWriter streamWriter) {
            foreach (ProjectInfo pi in m_projectInfoList) {
                if (pi.IsVersionable || !m_dontExportNonversionable) {
                    StringBuilder output = new StringBuilder();
                    output.AppendFormat("\"{0}\"", pi.ProjectName);
                    foreach (AssemblyVersionTypeSelection avts in m_assemblyVersionTypes) {
                        if (avts.IsSelected) {
                            output.Append(Separator);
                            output.AppendFormat("\"{0}\"", pi.CurrentAssemblyVersions[avts.AssemblyVersionType]);
                        }
                    }
                    streamWriter.WriteLine(output.ToString());
                }
            }
        }

	}
}