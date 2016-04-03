/*
 * Filename:    ListExporterToPlainTextFile.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Exports projects list to a plain text file.
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
	/// Summary description for PlainTextListExporter.
	/// </summary>
	public class ListExporterToPlainTextFile : ListExporterToTextFile {

		public ListExporterToPlainTextFile() {
		}

        override protected string ExportFileFilter {
            get {
                return "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            }
        }

        override protected string ExportFileExtension { 
            get {
                return ".vcb.txt"; 
            }
        }

        /// <summary>
        ///   Gets format string for output of project items.
        /// </summary>
        /// <param name="width">
        ///   Width required for string output.
        /// </param>
        /// <returns>
        ///   Formatting string.
        /// </returns>
        private string GetFormatString(int width) {
            return string.Format("{{0,-{0}}}", width);
        }

        /// <summary>
        ///   Gets separator line with width provided.
        /// </summary>
        /// <param name="width">
        ///   Width of the line.
        /// </param>
        /// <returns>
        ///   Entire separator line.
        /// </returns>
        private string GetSeparator(int width) {
            StringBuilder sb = new StringBuilder();
            sb.Append('-', width);
            return sb.ToString();
        }

        override protected void DoSave(string filename) {
            CollectColumnWidths();
            base.DoSave(filename);
        }

        /// <summary>
        ///   Writes header with solution name and current date and time.
        /// </summary>
        /// <param name="streamWriter">
        ///   <c>StreamWriter</c> used to write header.
        /// </param>
        override protected void WriteHeader(StreamWriter streamWriter) {
            streamWriter.WriteLine(m_solutionName);
            streamWriter.WriteLine(m_exportDateTime.ToString("g"));
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
            string format = GetFormatString(m_columnWidths[(int)ColumnName.ProjectName] + m_columnSpacing);
            output.AppendFormat(format, HeaderProjectName);
            foreach (AssemblyVersionTypeSelection avts in m_assemblyVersionTypes) {
                if (avts.IsSelected) {
                    format = GetFormatString(m_columnWidths[(int)ColumnName.Version] + m_columnSpacing);
                    output.AppendFormat(format, m_headings[avts.AssemblyVersionType]);
                }
            }
            streamWriter.WriteLine(output);
            streamWriter.WriteLine(GetSeparator(output.Length));
        }

        override protected void WriteItems(StreamWriter streamWriter) {
            foreach (ProjectInfo pi in m_projectInfoList) {
                if (pi.IsVersionable || !m_dontExportNonversionable) {
                    // set indentation
                    StringBuilder projectName = new StringBuilder(pi.ProjectName);
                    if (pi.Level > 0) 
                        projectName.Insert(0, " ", pi.Level);
                    StringBuilder output = new StringBuilder();
                    string format = GetFormatString(m_columnWidths[(int)ColumnName.ProjectName] + m_columnSpacing);
                    output.AppendFormat(format, projectName);
                    foreach (AssemblyVersionTypeSelection avts in m_assemblyVersionTypes) {
                        if (avts.IsSelected) {
                            format = GetFormatString(m_columnWidths[(int)ColumnName.Version] + m_columnSpacing);
                            output.AppendFormat(format, pi.CurrentAssemblyVersions[avts.AssemblyVersionType]);
                        }
                    }
                    streamWriter.WriteLine(output.ToString());
                }
            }
        }

        /// <summary>
        ///   Evaluates required widths of individual columns,
        /// </summary>
        private void CollectColumnWidths() {
            m_columnWidths = new int[Enum.GetValues(typeof(ColumnName)).Length];
            m_columnWidths[(int)ColumnName.ProjectName] = HeaderProjectName.Length;
            // find largest version column header
            foreach (AssemblyVersionTypeSelection avts in m_assemblyVersionTypes) {
                if (avts.IsSelected) {
                    int width = ((string)m_headings[avts.AssemblyVersionType]).Length;
                    if (width > m_columnWidths[(int)ColumnName.Version])
                        m_columnWidths[(int)ColumnName.Version] = width;
                }
            }
            foreach (ProjectInfo pi in m_projectInfoList) {
                int width = pi.ProjectName.Length + pi.Level;
                if (width > m_columnWidths[(int)ColumnName.ProjectName])
                    m_columnWidths[(int)ColumnName.ProjectName] = width;

                // go through all selected version types
                foreach (AssemblyVersionTypeSelection avts in m_assemblyVersionTypes) {
                    if (avts.IsSelected) {
                        width = pi.CurrentAssemblyVersions[avts.AssemblyVersionType].ToString().Length;
                        if (width > m_columnWidths[(int)ColumnName.Version])
                            m_columnWidths[(int)ColumnName.Version] = width;
                    }
                }
            }
        }

        private int[] m_columnWidths;

        private const int m_columnSpacing = 4;

    }
}