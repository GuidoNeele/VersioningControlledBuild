/*
 * Filename:    ListExporterToFile.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Exports projects list to a file.
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
using System.Windows.Forms;

namespace BuildAutoIncrement {
	/// <summary>
	///   Exports projects list to file.
	/// </summary>
	public abstract class ListExporterToFile : ListExporter {

        /// <summary>
        ///   Initializes <c>ListExporterToFile</c> instance.
        /// </summary>
		public ListExporterToFile() {
            m_separator = string.Empty;
		}

        /// <summary>
        ///   Sets separator between columns (important for CSV only)
        /// </summary>
        public string Separator {
            get {
                return m_separator;
            }
            set {
                m_separator = value;
            }
        }

        /// <summary>
        ///   Exports list to a file. File name is created from solution 
        ///   filename provided.
        /// </summary>
        /// <param name="solutionName">
        ///   Name of the corresponding solution.
        /// </param>
        /// <param name="solutionFilename">
        ///   Full name of the solution file.
        /// </param>
        /// <param name="projectInfoList">
        ///   List of filtered projects.
        /// </param>
        public void Export(string solutionName, string solutionFilename, ProjectInfoList projectInfoList) {
            m_solutionName = solutionName;
            m_projectInfoList = projectInfoList;
            m_exportDateTime = DateTime.Now;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = CreateExportFilename(solutionFilename);
            sfd.Filter = ExportFileFilter;
            if (sfd.ShowDialog() == DialogResult.OK) {
                try {
                    DoSave(sfd.FileName);
                }
                catch (Exception e) {
                    MessageBox.Show(string.Format("Failed to save the file.{0}{0}{1}", Environment.NewLine, e.Message), "File Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        ///   Creates output filename from solution file name provided.
        /// </summary>
        /// <param name="solutionFilename">
        ///   Solution file name.
        /// </param>
        /// <returns>
        ///   Output file name.
        /// </returns>
        private string CreateExportFilename(string solutionFilename) {
            return solutionFilename.Replace(".sln", ExportFileExtension);
        }

        abstract protected void DoSave(string filename);

        abstract protected string ExportFileFilter { get; }

        abstract protected string ExportFileExtension { get; }

        private string m_separator;
    }
}