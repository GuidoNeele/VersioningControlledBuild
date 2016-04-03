/*
 * Filename:    DisplayOptions.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Display options for main GUI form of the tool.
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
	///   Display options for main GUI form of the tool.
	/// </summary>
	[Serializable]
	public class DisplayOptions : ICloneable {
		public DisplayOptions() {
            m_listViewColors                = ProjectsListViewColorsConfiguration.Default;
            m_indentSubProjectItems         = true;
            m_subProjectsIndentation        = 10;
            m_showSubprojectRoot            = true;
            m_showEnterpriseTemplateProjectRoot = true;
            m_showEmptyFolders              = true;
            m_showNonVersionableProjects    = true;
            m_displaySuccessDialog          = true;
        }

        public ProjectsListViewColorsConfiguration Colors {
            get { return m_listViewColors; }
            set { m_listViewColors = value; }
        }

        public bool IndentSubProjectItems {
            get { return m_indentSubProjectItems; }
            set { m_indentSubProjectItems = value; }
        }

        public int SubProjectIndentation {
            get { return m_subProjectsIndentation; }
            set { m_subProjectsIndentation = value; }
        }

        public bool ShowSubProjectRoot {
            get { return m_showSubprojectRoot; }
            set { m_showSubprojectRoot = value; }
        }

        public bool ShowEnterpriseTemplateProjectRoot {
            get { return m_showEnterpriseTemplateProjectRoot; }
            set { m_showEnterpriseTemplateProjectRoot = value; }
        }

        public bool ShowEmptyFolders {
            get { return m_showEmptyFolders; }
            set { m_showEmptyFolders = value; }
        }

        public bool ShowNonVersionableProjects {
            get { return m_showNonVersionableProjects; }
            set { m_showNonVersionableProjects = value; }
        }

        public bool ShowSuccessDialog {
            get { return m_displaySuccessDialog; }
            set { m_displaySuccessDialog = value; }
        }

        #region ICloneable implementation

        object ICloneable.Clone() {
            return Clone();
        }

        public DisplayOptions Clone() {
            DisplayOptions newOptions = (DisplayOptions)this.MemberwiseClone();
            newOptions.Colors = Colors.Clone();
            return newOptions;
        }

        #endregion // ICloneable implementation


        private ProjectsListViewColorsConfiguration m_listViewColors;
        private bool                                m_indentSubProjectItems;
        private int                                 m_subProjectsIndentation;
        private bool                                m_showSubprojectRoot;
        private bool                                m_showEnterpriseTemplateProjectRoot;
        private bool                                m_showNonVersionableProjects;
        private bool                                m_showEmptyFolders;
        private bool                                m_displaySuccessDialog;
    }
}