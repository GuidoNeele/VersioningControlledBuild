/*
 * Filename:    ListExporter.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Base class for ListPrinter and ListExporterToFile.
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
using System.Collections;

namespace BuildAutoIncrement {
	/// <summary>
	///   Abstract class inherited by <c>ListPrinter</c> and <c>ListExporterToFile</c>.
	/// </summary>
	public abstract class ListExporter {

        /// <summary>
        ///   Column types.
        /// </summary>
        protected enum ColumnName {
            ProjectName,
            Version,
        }

        /// <summary>
        ///   Initializes <c>ListExporter</c> using configuration settings.
        /// </summary>
        public ListExporter() {
            ExportConfiguration ec = ConfigurationPersister.Instance.Configuration.ExportConfiguration;
            m_dontExportNonversionable = ec.ExcludeNonversionableItems;
            m_indentBy = ec.IndentSubItems ? ec.IndentSubItemsBy : 0;
            m_assemblyVersionTypes = ec.AssemblyVersionTypes;
        }

        /// <summary>
        ///   Sets indentation depth.
        /// </summary>
        public int IndentBy {
            set {
                m_indentBy = value;
            }
        }

        /// <summary>
        ///   Sets the flag if nonversionable items should be exported.
        /// </summary>
        public bool DontExportNonversionableItems {
            set {
                m_dontExportNonversionable = value;
            }
        }

        /// <summary>
        ///   List of currently filtered projects.
        /// </summary>
        protected ProjectInfoList m_projectInfoList;

        protected bool m_dontExportNonversionable;

        protected int m_indentBy;

        protected AssemblyVersionTypeSelection[] m_assemblyVersionTypes;

        /// <summary>
        ///   Name of the current solution.
        /// </summary>
        protected string m_solutionName;

        /// <summary>
        ///   Export date and time.
        /// </summary>
        protected DateTime m_exportDateTime;

        /// <summary>
        ///   Column titles.
        /// </summary>
        protected const string HeaderProjectName      = "Project Name";
        protected const string HeaderAssemblyVersion  = "Assembly Version";
        protected const string HeaderFileVersion      = "File Version";
        protected const string HeaderProductVersion   = "Product Version";

        protected static readonly Hashtable m_headings;
        
        static ListExporter() {
            m_headings = new Hashtable();
            m_headings[AssemblyVersionType.AssemblyVersion] = HeaderAssemblyVersion;
            m_headings[AssemblyVersionType.AssemblyFileVersion] = HeaderFileVersion;
            m_headings[AssemblyVersionType.AssemblyInformationalVersion] = HeaderProductVersion;
        }
        
	}
}
