/*
 * Filename:    VcbConfiguration.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Tool configuration.
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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BuildAutoIncrement {

    [Serializable]
    public class VcbConfiguration : ICloneable {

        public VcbConfiguration() {
            m_listViewColumnWidths          = new ListViewColumnWidths();
            m_applyToAllTabsChecked         = true;
            m_numberingOptions              = new NumberingOptions();
            m_displayOptions                = new DisplayOptions();
            m_foldersConfigurations         = new FoldersConfigurations();
            m_configurationFileRead         = false;
            m_exportConfiguration           = new ExportConfiguration();
        }

        public System.Drawing.Size MainFormSize {
            get { return m_size; }
            set { m_size = value; }
        }

        public ListViewColumnWidths ListViewColumnWidths {
            get { return m_listViewColumnWidths; }
            set { m_listViewColumnWidths = value; }
        }

        public bool ApplyToAllTabsChecked {
            get { return m_applyToAllTabsChecked; }
            set { m_applyToAllTabsChecked = value; }
        }

        public NumberingOptions NumberingOptions {
            get { return m_numberingOptions; }
            set { m_numberingOptions = value; }
        }

        public DisplayOptions DisplayOptions {
            get { return m_displayOptions; }
            set { m_displayOptions = value; }
        }

        public FoldersConfigurations FoldersConfigurations {
            get { return m_foldersConfigurations; }
            set { m_foldersConfigurations = value; }
        }

        public ExportConfiguration ExportConfiguration {
            get { return m_exportConfiguration; }
            set { m_exportConfiguration = value; }
        }

        /// <summary>
        ///   Flag used to identify if configuration has been read from file.
        /// </summary>
        [XmlIgnore]
        public bool ConfigurationFileRead {
            get { return m_configurationFileRead; }
            set { m_configurationFileRead = value; }
        }

        #region ICloneable implementation

        object ICloneable.Clone() {
            return Clone();
        }

        public VcbConfiguration Clone() {
            VcbConfiguration newConfig = (VcbConfiguration)this.MemberwiseClone();
            newConfig.NumberingOptions = NumberingOptions.Clone();
            newConfig.DisplayOptions = DisplayOptions.Clone();
            newConfig.FoldersConfigurations = FoldersConfigurations.Clone();
            return newConfig;
        }

        #endregion // ICloneable implementation

        public int[] RetrieveListViewColumnWidths() {
            int[] columnWidths = new int[4];
            columnWidths[0] = ListViewColumnWidths.ProjectName;
            columnWidths[1] = ListViewColumnWidths.CurrentVersion;
            columnWidths[2] = ListViewColumnWidths.Modified;
            columnWidths[3] = ListViewColumnWidths.ToBeVersion;
            return columnWidths;
        }

        public void StoreListViewColumnWidths(int[] columnWidths) {
            ListViewColumnWidths.ProjectName    = columnWidths[0];
            ListViewColumnWidths.CurrentVersion = columnWidths[1];
            ListViewColumnWidths.Modified       = columnWidths[2];
            ListViewColumnWidths.ToBeVersion    = columnWidths[3];
        }

        private Size                    m_size;
        private ListViewColumnWidths    m_listViewColumnWidths;
        private bool                    m_applyToAllTabsChecked;
        private NumberingOptions        m_numberingOptions;
        private DisplayOptions          m_displayOptions;
        private FoldersConfigurations   m_foldersConfigurations;
        private bool                    m_configurationFileRead;
        private ExportConfiguration     m_exportConfiguration;
    }
}