/*
 * Filename:    MainForm.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Main GUI form displaying all projects in the solution.
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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BuildAutoIncrement {

    public class MainForm : System.Windows.Forms.Form {

        #region Controls

        private System.Windows.Forms.GroupBox m_groupBoxProjects;
        private BuildAutoIncrement.AssemblyInfoListViewsControl m_assemblyInfoListViewsControl;
        private System.Windows.Forms.Button m_buttonSelectAllProjects;
        private System.Windows.Forms.Button m_buttonDeselectAll;
        private System.Windows.Forms.Button m_buttonResetChecks;
        private System.Windows.Forms.Button m_buttonResetListView;
        private System.Windows.Forms.Button m_buttonResetVersions;
        private System.Windows.Forms.Button m_buttonExport;

        private System.Windows.Forms.ContextMenu m_contextMenuExport;
        private System.Windows.Forms.MenuItem m_menuItemExportPrint;
        private System.Windows.Forms.MenuItem m_menuItemExportFile;

        private System.Windows.Forms.GroupBox m_groupBoxMarkedProjects;
        private System.Windows.Forms.Label label1;
        private BuildAutoIncrement.VersionUpDown m_versionEditBoxHighestVersion;
        private System.Windows.Forms.Button m_buttonApplyToVersion;
        
        private System.Windows.Forms.GroupBox m_groupBoxGetVersion;
        private System.Windows.Forms.Button m_buttonGetSelected;
        private System.Windows.Forms.Button m_buttonGetHighestMarked;
        private System.Windows.Forms.Button m_buttonGetHighest;
        private System.Windows.Forms.Button m_buttonGetHighestAll;

        private System.Windows.Forms.GroupBox m_groupBoxIncrement;
        private System.Windows.Forms.Button m_buttonIncrementMajorVersion;
        private System.Windows.Forms.Button m_buttonIncrementMinorVersion;
        private System.Windows.Forms.Button m_buttonIncrementBuildVersion;
        private System.Windows.Forms.Button m_buttonIncrementRevisionVersion;

        private System.Windows.Forms.GroupBox m_groupBoxAllProjects;
        private System.Windows.Forms.Button m_buttonAllToHighest;
        private System.Windows.Forms.Button m_buttonAllProjectsIncrementAndSynchronize;

        private System.Windows.Forms.CheckBox m_checkBoxApplyToAllTabs;
        private System.Windows.Forms.Button m_buttonSave;
        private System.Windows.Forms.Button m_buttonBuild;
        private System.Windows.Forms.Button m_buttonCancel;
        private System.Windows.Forms.Button m_buttonRebuildAll;
        private System.Windows.Forms.Button m_buttonInvertChecks;

        #endregion // Controls
        
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///   Hides the empty constructor.
        /// </summary>
        private MainForm() {
            InitializeComponent();
            VisualStyles.SetButtonFlatStyleSystem(this);
            m_configuration = ConfigurationPersister.Instance.Configuration;
            Debug.Assert(m_configuration != null);
            m_commandToPerform = "";
            AttachEventListeners();
        }

        /// <summary>
        ///   Creates MainForm object.
        /// </summary>
        /// <param name="devEnvApplicationObject">
        ///   <c>DTE</c> object representing the enclosing development environment.
        /// </param>
        public MainForm(ISolutionBrowser solutionBrowser) : this() {
            Debug.Assert(solutionBrowser != null);
            m_projectBrowser = solutionBrowser;
            Debug.Assert(m_projectBrowser != null);
        }

        #region Public properties

        /// <summary>
        ///   Gets the string of the command to perform after form is closed.
        /// </summary>
        public string CommandToPerform {
            get { return m_commandToPerform; }
        }

        /// <summary>
        ///   Gets an array of <c>ProjectInfo</c> objects for projects marked for update.
        /// </summary>
        public ProjectInfo[] MarkedProjects {
            get { return m_assemblyInfoListViewsControl.GetMarkedProjectsInformation(m_checkBoxApplyToAllTabs.Checked); }
        }

        /// <summary>
        ///   Sets visibility of Build and Rebuild buttons. Used by command line tool.
        /// </summary>
        public bool BuildButtonsVisible {
            set {
                m_buttonBuild.Visible = value;
                m_buttonRebuildAll.Visible = value;
            }
        }

        #endregion Public properties

        #region Public methods

        /// <summary>
        ///   Saves versions of checked projects to corresponding <c>AssemblyInfo</c> files.
        /// </summary>
        public void SaveVersions() {
            Debug.Assert(m_assemblyInfoListViewsControl != null);
            Debug.Assert(m_checkBoxApplyToAllTabs != null);
            m_assemblyInfoListViewsControl.SaveVersions(m_checkBoxApplyToAllTabs.Checked);
        }

        #endregion // Public methods

        #region Configuration

        /// <summary>
        ///   Stores data to configuration file.
        /// </summary>
        private void StoreConfiguration() {
            m_configuration.MainFormSize = this.Size;
            m_configuration.StoreListViewColumnWidths(m_assemblyInfoListViewsControl.ListViewColumnWidths);
            m_configuration.ApplyToAllTabsChecked = m_checkBoxApplyToAllTabs.Checked;
            try {
                ConfigurationPersister.Instance.StoreConfiguration();
            }
            catch (Exception exception) {
                string message = s_txtCannotSaveConfiguration + Environment.NewLine + exception.Message + Environment.NewLine + exception.StackTrace.ToString();
                MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void ShowConfigureDialog() {
            ConfigurationForm configurationForm = new ConfigurationForm();
            if (configurationForm.ShowDialog(this) == DialogResult.OK) {
                this.Update();
                m_configuration = configurationForm.GetConfiguration();
                ConfigurationPersister.Instance.Configuration.DisplayOptions = m_configuration.DisplayOptions;
                ConfigurationPersister.Instance.Configuration.NumberingOptions = m_configuration.NumberingOptions;
                ConfigurationPersister.Instance.Configuration.FoldersConfigurations = m_configuration.FoldersConfigurations;
                ConfigurationPersister.Instance.Configuration.ExportConfiguration = m_configuration.ExportConfiguration;
                StoreConfiguration();
                m_projectBrowser.ApplyConfiguration(m_configuration);
                IProjectFilter projectFilter = new ProjectFilterByType(m_configuration.NumberingOptions.IncludeSetupProjects, m_configuration.DisplayOptions.ShowNonVersionableProjects, m_configuration.DisplayOptions.ShowSubProjectRoot, m_configuration.DisplayOptions.ShowEnterpriseTemplateProjectRoot);
                m_projectBrowser.ApplyFilter(projectFilter);
                FillControls();
            }
        }

        #endregion Configuration

        #region Control message handlers

        /// <summary>
        ///     Build button click handler. Saves requested versions to 
        ///     AssemblyInfo files and performs solution build.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonBuild_Click(object sender, System.EventArgs e) {
            //CheckOutAssemblyInfos();
            m_commandToPerform = "Build.BuildSolution";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        ///     BuildAll button click handler. Cleans all projects and rebuilds
        ///     the whole solution
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonBuildAll_Click(object sender, System.EventArgs e) {
            //CheckOutAssemblyInfos();
            m_commandToPerform = "Build.RebuildSolution";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        ///     OK button click handler. Saves versions into selected Assemby 
        ///     Version files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonSave_Click(object sender, System.EventArgs e) {
            Cursor.Current = Cursors.WaitCursor;
            //CheckOutAssemblyInfos();
            m_commandToPerform = "";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        
        /// <summary>
        ///     SelectAll button click handler. Marks all projects in the 
        ///     projects listview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonSelectAllProjects_Click(object sender, System.EventArgs e) {
            m_assemblyInfoListViewsControl.MarkAllProjects(m_checkBoxApplyToAllTabs.Checked);
        }

        /// <summary>
        /// DeselectAll button click handler. Deselects all projects from the 
        /// projects listview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonDeselectAll_Click(object sender, System.EventArgs e) {
            m_assemblyInfoListViewsControl.UnmarkAllProjects(m_checkBoxApplyToAllTabs.Checked);
		}

        /// <summary>
        ///   Inverts checkmark states in listviews.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonInvertChecks_Click(object sender, System.EventArgs e) {
            m_assemblyInfoListViewsControl.InvertProjectMarks(m_checkBoxApplyToAllTabs.Checked);
        }

        /// <summary>
        /// Reset button click handler. Resets versions of all projects to their 
        /// initial values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonResetVersions_Click(object sender, System.EventArgs e) {
            m_assemblyInfoListViewsControl.ResetVersions(m_checkBoxApplyToAllTabs.Checked);
		}

        /// <summary>
        /// ResetListView button click handler. Resets the projects listview to 
        /// initial state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonResetListView_Click(object sender, System.EventArgs e) {
            m_assemblyInfoListViewsControl.ResetListView(m_checkBoxApplyToAllTabs.Checked);
            UpdateControlsStates();
        }

        /// <summary>
        /// ResetChecks button click handler. Resets chacked state of items to
        /// initial value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonResetChecks_Click(object sender, System.EventArgs e) {
            m_assemblyInfoListViewsControl.ResetMarks(m_checkBoxApplyToAllTabs.Checked);
        }

        /// <summary>
        /// ToBeVersion control content event handler. Compares version with 
        /// each item in the projects listview and marks red those that have
        /// version smaller than the one in the "To be version" textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="toBeVersionPattern">Pattern in the "to be version" textbox.</param>
        protected void OnToBeVersionChanged(object sender, string toBeVersionPattern) {
            Debug.Assert(toBeVersionPattern != null && toBeVersionPattern.Length > 0);
            m_assemblyInfoListViewsControl.ProposeToBeVersion(toBeVersionPattern);
        }

        /// <summary>
        /// Projects listview item check handler. Required to update button states.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewProjectsItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e) {
            Debug.Assert(m_configuration != null && m_configuration.NumberingOptions != null);
            // process only if state has changed (i.e. prevent processing after click on disabled item)
            if (e.NewValue != e.CurrentValue) {
                int delta = e.NewValue == CheckState.Checked ? +1 : -1;
                bool somethingChecked = m_assemblyInfoListViewsControl.GetMarkedItemsCount(m_checkBoxApplyToAllTabs.Checked) + delta > 0;
                UpdateMarksDependantControlStates(somethingChecked);
            }
        }

        /// <summary>
        ///   Handler for <c>SelectedIndexChanged</c> event coming from listview. 
        ///   Enables/disables "Get Selected" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewProjectsSelectedIndexChanged(object sender, System.EventArgs e) {
            m_buttonGetSelected.Enabled = m_assemblyInfoListViewsControl.IsSingleSelectionWithValidAssemblyVersion;
        }

        /// <summary>
        ///   After new tab has been selected, synchronize the scroll position 
        ///   with previously active listview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedTabIndexChanged(object sender, System.EventArgs e) {
            UpdateControlsStates();
        }

        /// <summary>
        /// Apply button click handler. Applies new version to all selected 
        /// items in the project listview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonApplyToVersion_Click(object sender, System.EventArgs e) {
            if (HaveAllListViewItemsLowerCurrentVersion(false) == false) {
                if (MessageBox.Show(this, s_txtHigherVersionWarning + Environment.NewLine + s_txtAreYouSureToContinue, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
            }
            m_assemblyInfoListViewsControl.ApplyVersion(m_versionEditBoxHighestVersion.Text, m_checkBoxApplyToAllTabs.Checked);
        }

        /// <summary>
        /// GetSelected button click handler. Copies version of the first 
        /// selected item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonGetSelected_Click(object sender, System.EventArgs e) {
            m_versionEditBoxHighestVersion.Text = m_assemblyInfoListViewsControl.SelectedVersion.ToString(true);
        }

        /// <summary>
        ///   GetHighest button click handler. Searches for the largest version 
        ///   in the projects listview and copies it to the textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonGetHighest_Click(object sender, System.EventArgs e) {
            m_versionEditBoxHighestVersion.Text = m_assemblyInfoListViewsControl.GetHighestToBecomeVersion(m_checkBoxApplyToAllTabs.Checked).ToString(true);
        }

        private void m_buttonGetHighestAll_Click(object sender, System.EventArgs e) {
            m_versionEditBoxHighestVersion.Text = m_assemblyInfoListViewsControl.GetHighestToBecomeVersion(true).ToString(true);
        }

        private void m_buttonGetHighestMarked_Click(object sender, System.EventArgs e) {
            m_versionEditBoxHighestVersion.Text = m_assemblyInfoListViewsControl.GetHighestMarkedVersion(m_checkBoxApplyToAllTabs.Checked).ToString(true);
        }

        /// <summary>
        ///   Increments Build numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonIncrementBuildVersion_Click(object sender, System.EventArgs e) {
            IncrementVersions(ProjectVersion.VersionComponent.Build);
        }

        /// <summary>
        ///   Increments Revision numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonIncrementRevision_Click(object sender, System.EventArgs e) {
            IncrementVersions(ProjectVersion.VersionComponent.Revision);
        }

        /// <summary>
        ///   Increments Major numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonIncrementMajorVersion_Click(object sender, System.EventArgs e) {
            IncrementVersions(ProjectVersion.VersionComponent.Major);
        }

        /// <summary>
        ///   Increments Minor numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonIncrementMinorVersion_Click(object sender, System.EventArgs e) {
            IncrementVersions(ProjectVersion.VersionComponent.Minor);
        }

        /// <summary>
        ///   Increments versions for projects checked.
        /// </summary>
        /// <param name="toIncrement">
        ///   Object defining which part of version to increment.
        /// </param>
        private void IncrementVersions(ProjectVersion.VersionComponent toIncrement) {
            m_assemblyInfoListViewsControl.IncrementVersions(toIncrement, m_checkBoxApplyToAllTabs.Checked);
        }

        private void m_buttonAllToLargest_Click(object sender, System.EventArgs e) {
            m_assemblyInfoListViewsControl.SynchronizeAllVersions(false, m_checkBoxApplyToAllTabs.Checked);
        }

        private void m_buttonAllProjectsIncrementAndSynchronize_Click(object sender, System.EventArgs e) {
            m_assemblyInfoListViewsControl.SynchronizeAllVersions(true, m_checkBoxApplyToAllTabs.Checked);
        }

        private void m_checkBoxApplyToAllTabs_CheckedChanged(object sender, System.EventArgs e) {
            bool somethingChecked = m_assemblyInfoListViewsControl.GetMarkedItemsCount(m_checkBoxApplyToAllTabs.Checked) > 0;
            UpdateMarksDependantControlStates(somethingChecked);
        }

        private void m_buttonExport_Click(object sender, System.EventArgs e) {
            m_contextMenuExport.Show(m_buttonExport, new Point(0, m_buttonExport.Height));
        }

        private void m_menuItemExportPrint_Click(object sender, System.EventArgs e) {
            ListPrinter lp = new ListPrinter(this);
            lp.Print(string.Empty, m_projectBrowser.SolutionName, m_projectBrowser.ProjectInfoList);
        }

        private void m_menuItemExportFile_Click(object sender, System.EventArgs e) {
            ListExporterToFile lef = null;
            switch (ConfigurationPersister.Instance.Configuration.ExportConfiguration.ExportFileFormat) {
            case ExportFileFormat.PlainText:
                lef = new ListExporterToPlainTextFile();
                break;
            case ExportFileFormat.CSV:
                lef = new ListExporterToCsvFile();
                lef.Separator = ConfigurationPersister.Instance.Configuration.ExportConfiguration.CsvSeparator;
                break;
            default:
                Debug.Assert(false, "Not supported ExportFileFormat");
                break;
            }
            lef.Export(m_projectBrowser.SolutionName, m_projectBrowser.SolutionFilename, m_projectBrowser.ProjectInfoList);
        }

        #endregion // Control message handlers

        #region Form class overrides

        /// <summary>
		///   Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
            if (!m_disposed) {
                try {
                    if (disposing) {
                        DetachEventListeners();
                        if (components != null) {
                            components.Dispose();
                        }
                    }
                    m_disposed = true;
                }
                finally {
                    base.Dispose(disposing);
                }
            }
		}

        /// <summary>
        ///   Overrides <c>WndProc</c> to detect if "About" item in the system 
        ///   menu has been selected.
        /// </summary>
        /// <param name="msg"></param>
        protected override void WndProc(ref Message msg) {
            base.WndProc(ref msg);
            switch (msg.Msg) {
            case (int)Win32Api.WM.SYSCOMMAND:
                if ((uint)msg.WParam == Win32Api.IDM_CUSTOM) {
                    ShowConfigureDialog();
                }
                else if ((uint)msg.WParam == (Win32Api.IDM_CUSTOM + 1)) {
                    AboutBox.Show(this);
                }
                break;
            }
        }
        
        /// <summary>
        ///   Load form event handler. Adds "About..." item to the system menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e) {
            Debug.Assert(m_configuration != null);
            if (!m_configuration.ConfigurationFileRead)
                MessageBox.Show(this, s_txtFailedToLoadConfiguration + Environment.NewLine + s_txtUsingDefaultConfiguration, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            FillControls();
            if (m_projectBrowser.SolutionName != null && m_projectBrowser.SolutionName.Length > 0)
                Text = string.Format("\'{0}\' {1}", m_projectBrowser.SolutionName, s_txtTitle);
            else
                Text = s_txtEmpty + " " + s_txtTitle;
            AddAboutSystemMenuItem();
            base.OnLoad(e);
            AdjustSizeAndPosition();
            SelectDefaultTab();
            m_checkBoxApplyToAllTabs.Checked = m_configuration.ApplyToAllTabsChecked;
        }

        /// <summary>
        ///   Closing event handler stores current settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            StoreConfiguration();
            base.OnClosing(e);
        }

        /// <summary>
        ///   Closed event handler resets VS settings to inital state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
        }

        /// <summary>
        ///   Overriden to process CONTROL-TAB key to switch between listviews.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == (Keys.Control | Keys.Tab)) {
                m_assemblyInfoListViewsControl.SelectNextListView(true);
                return true;
            }
            else if (keyData == (Keys.Control | Keys.Shift | Keys.Tab)) {
                m_assemblyInfoListViewsControl.SelectNextListView(false);
                return true;
            }
            return base.ProcessCmdKey (ref msg, keyData);
        }

        #endregion // Form class overrides

        #region Private methods

        /// <summary>
        ///   Fills form controls with current solution data.
        /// </summary>
        private void FillControls() {
            Debug.Assert(m_projectBrowser != null);
            Cursor.Current = Cursors.WaitCursor;
            m_assemblyInfoListViewsControl.BeginUpdate();
            ProjectVersion pv = FillProjectsListView();
            if (m_versionEditBoxHighestVersion.Text == pv.ToString(true))
                m_assemblyInfoListViewsControl.ProposeToBeVersion(m_versionEditBoxHighestVersion.Text);
            else
                m_versionEditBoxHighestVersion.Text = pv.ToString(true);
            m_assemblyInfoListViewsControl.EndUpdate();
            if (m_configuration.NumberingOptions.ApplyToAllTypes) {
                m_checkBoxApplyToAllTabs.Checked = true;
            }
            m_checkBoxApplyToAllTabs.Enabled = !m_configuration.NumberingOptions.ApplyToAllTypes;
            UpdateControlsStates();
        }

        private void AdjustSizeAndPosition() {
            this.Size = m_configuration.MainFormSize;
            // shrink the form if too large 
            if (this.Width > Screen.GetWorkingArea(this).Width) {
                this.Width = Screen.GetWorkingArea(this).Width * 8 / 10;
            }
            if (this.Height > Screen.GetWorkingArea(this).Height) {
                this.Height = Screen.GetWorkingArea(this).Height * 8 / 10;
            }
            /*
            this.Left = m_devEnvApplicationObject.MainWindow.Left + (m_devEnvApplicationObject.MainWindow.Width - this.Width) / 2;
            this.Top  = m_devEnvApplicationObject.MainWindow.Top + (m_devEnvApplicationObject.MainWindow.Height - this.Height) / 2;
            */
            this.Left = (Screen.GetWorkingArea(this).Width - Width) / 2;
            this.Top = (Screen.GetWorkingArea(this).Height - Height) / 2;
            m_assemblyInfoListViewsControl.ListViewColumnWidths = m_configuration.RetrieveListViewColumnWidths();
        }

        private void SelectDefaultTab() {
            m_assemblyInfoListViewsControl.SelectListView(m_configuration.NumberingOptions.DefaultVersionType);
        }

        /// <summary>
        ///   Update (enabled/disabled) status of buttons depending on the
        ///   number of selected items in the listview.
        /// </summary>
        private void UpdateControlsStates() {
            bool anyValidVersion = m_assemblyInfoListViewsControl.GetValidVersionsCount(m_checkBoxApplyToAllTabs.Checked) > 0;
            m_buttonDeselectAll.Enabled              = anyValidVersion;
            m_buttonResetChecks.Enabled              = anyValidVersion;
            m_buttonInvertChecks.Enabled             = anyValidVersion;
            m_buttonResetListView.Enabled            = anyValidVersion;
            m_buttonSelectAllProjects.Enabled        = anyValidVersion;

            m_buttonGetSelected.Enabled              = m_assemblyInfoListViewsControl.IsSingleSelectionWithValidAssemblyVersion;
            m_buttonGetHighest.Enabled               = anyValidVersion;
            m_versionEditBoxHighestVersion.Enabled   = anyValidVersion;
            m_buttonGetHighestAll.Enabled            = m_assemblyInfoListViewsControl.GetValidVersionsCount(true) > 0;

            m_buttonAllToHighest.Enabled             = anyValidVersion && m_assemblyInfoListViewsControl.ItemsCount > 1;
            m_buttonAllProjectsIncrementAndSynchronize.Enabled = anyValidVersion && m_assemblyInfoListViewsControl.ItemsCount > 1;

            m_buttonBuild.Enabled                    = m_assemblyInfoListViewsControl.ItemsCount > 0;
            m_buttonRebuildAll.Enabled               = m_assemblyInfoListViewsControl.ItemsCount > 0;

            bool somethingChecked = m_assemblyInfoListViewsControl.GetMarkedItemsCount(m_checkBoxApplyToAllTabs.Checked) > 0;
            UpdateMarksDependantControlStates(somethingChecked);
        }

        private void UpdateMarksDependantControlStates(bool somethingChecked) {
            m_buttonResetVersions.Enabled               = somethingChecked;

            m_buttonIncrementMajorVersion.Enabled       = somethingChecked;
            m_buttonIncrementMinorVersion.Enabled       = somethingChecked;
            m_buttonIncrementBuildVersion.Enabled       = somethingChecked && !m_configuration.NumberingOptions.UseDateTimeBasedBuildAndRevisionNumbering;
            m_buttonIncrementRevisionVersion.Enabled    = somethingChecked && !m_configuration.NumberingOptions.UseDateTimeBasedBuildAndRevisionNumbering;

            m_buttonApplyToVersion.Enabled              = somethingChecked;
            m_buttonGetHighestMarked.Enabled            = somethingChecked;
            m_buttonSave.Enabled                        = somethingChecked;
        }

        /// <summary>
        ///   Fills the project listview with corresponding data.
        /// </summary>
        private ProjectVersion FillProjectsListView() {
            Debug.Assert(m_projectBrowser != null);
            ProjectInfoList pil = m_projectBrowser.ProjectInfoList;
            m_assemblyInfoListViewsControl.FillProjectsListView(pil);
            return pil.HighestToBeAssemblyVersions.HighestProjectVersion;
        }

        /// <summary>
        ///   Adds "Customize" and "About" items to the system menu.
        /// </summary>
        private void AddAboutSystemMenuItem() {
            Int32 hSystemMenu = Win32Api.GetSystemMenu(this.Handle, false);
            Win32Api.AppendMenu(hSystemMenu, (int)Win32Api.MF.SEPARATOR, 0, null);
#if DEBUG
            Win32Api.AppendMenu(hSystemMenu, (int)Win32Api.MF.STRING, Win32Api.IDM_CUSTOM, s_txtSettings);
#endif
            Win32Api.AppendMenu(hSystemMenu, (int)Win32Api.MF.STRING, Win32Api.IDM_CUSTOM+1, s_txtAbout);
        }

        private void AttachEventListeners() {
            m_assemblyInfoListViewsControl.ItemCheck                    += new ItemCheckEventHandler(this.ListViewProjectsItemCheck);
            m_assemblyInfoListViewsControl.SelectedIndexChanged         += new EventHandler(this.ListViewProjectsSelectedIndexChanged);
            m_assemblyInfoListViewsControl.SelectedTabIndexChanged      += new EventHandler(this.SelectedTabIndexChanged);
            m_versionEditBoxHighestVersion.ToBeVersionChanged           += new ToBeVersionChangedHandler(OnToBeVersionChanged);
        }

        private void DetachEventListeners() {
            m_assemblyInfoListViewsControl.ItemCheck                    -= new ItemCheckEventHandler(this.ListViewProjectsItemCheck);
            m_assemblyInfoListViewsControl.SelectedIndexChanged         -= new EventHandler(this.ListViewProjectsSelectedIndexChanged);
            m_assemblyInfoListViewsControl.SelectedTabIndexChanged      -= new EventHandler(this.SelectedTabIndexChanged);
            m_versionEditBoxHighestVersion.ToBeVersionChanged           -= new ToBeVersionChangedHandler(OnToBeVersionChanged);
        }

        /// <summary>
        ///   Compares versions of each project to the version in the edit box.
        /// </summary>
        /// <param name="checkAll">
        ///   Flag indicating if all items should be compared. If <c>false</c>,
        ///   only selected items are compared.
        /// </param>
        /// <returns>
        ///   <c>true</c> if all projects have larger version.
        /// </returns>
        private bool HaveAllListViewItemsLowerCurrentVersion(bool includeNotMarked) {
            return m_assemblyInfoListViewsControl.HaveAllListViewItemsLowerCurrentVersion(includeNotMarked, m_versionEditBoxHighestVersion.Text, m_checkBoxApplyToAllTabs.Checked);
        }

        #endregion // Private methods

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
            this.m_buttonBuild = new System.Windows.Forms.Button();
            this.m_buttonCancel = new System.Windows.Forms.Button();
            this.m_buttonApplyToVersion = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.m_buttonResetChecks = new System.Windows.Forms.Button();
            this.m_buttonResetListView = new System.Windows.Forms.Button();
            this.m_buttonDeselectAll = new System.Windows.Forms.Button();
            this.m_buttonSelectAllProjects = new System.Windows.Forms.Button();
            this.m_buttonResetVersions = new System.Windows.Forms.Button();
            this.m_groupBoxMarkedProjects = new System.Windows.Forms.GroupBox();
            this.m_groupBoxGetVersion = new System.Windows.Forms.GroupBox();
            this.m_buttonGetHighestAll = new System.Windows.Forms.Button();
            this.m_buttonGetHighestMarked = new System.Windows.Forms.Button();
            this.m_buttonGetSelected = new System.Windows.Forms.Button();
            this.m_buttonGetHighest = new System.Windows.Forms.Button();
            this.m_groupBoxIncrement = new System.Windows.Forms.GroupBox();
            this.m_buttonIncrementRevisionVersion = new System.Windows.Forms.Button();
            this.m_buttonIncrementMajorVersion = new System.Windows.Forms.Button();
            this.m_buttonIncrementMinorVersion = new System.Windows.Forms.Button();
            this.m_buttonIncrementBuildVersion = new System.Windows.Forms.Button();
            this.m_versionEditBoxHighestVersion = new BuildAutoIncrement.VersionUpDown();
            this.m_buttonRebuildAll = new System.Windows.Forms.Button();
            this.m_buttonSave = new System.Windows.Forms.Button();
            this.m_groupBoxProjects = new System.Windows.Forms.GroupBox();
            this.m_buttonExport = new System.Windows.Forms.Button();
            this.m_assemblyInfoListViewsControl = new BuildAutoIncrement.AssemblyInfoListViewsControl();
            this.m_buttonInvertChecks = new System.Windows.Forms.Button();
            this.m_groupBoxAllProjects = new System.Windows.Forms.GroupBox();
            this.m_buttonAllProjectsIncrementAndSynchronize = new System.Windows.Forms.Button();
            this.m_buttonAllToHighest = new System.Windows.Forms.Button();
            this.m_checkBoxApplyToAllTabs = new System.Windows.Forms.CheckBox();
            this.m_contextMenuExport = new System.Windows.Forms.ContextMenu();
            this.m_menuItemExportPrint = new System.Windows.Forms.MenuItem();
            this.m_menuItemExportFile = new System.Windows.Forms.MenuItem();
            this.m_groupBoxMarkedProjects.SuspendLayout();
            this.m_groupBoxGetVersion.SuspendLayout();
            this.m_groupBoxIncrement.SuspendLayout();
            this.m_groupBoxProjects.SuspendLayout();
            this.m_groupBoxAllProjects.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_buttonBuild
            // 
            this.m_buttonBuild.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonBuild.Enabled = false;
            this.m_buttonBuild.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonBuild.Location = new System.Drawing.Point(502, 312);
            this.m_buttonBuild.Name = "m_buttonBuild";
            this.m_buttonBuild.TabIndex = 1;
            this.m_buttonBuild.Text = "Build";
            this.m_buttonBuild.Click += new System.EventHandler(this.m_buttonBuild_Click);
            // 
            // m_buttonCancel
            // 
            this.m_buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_buttonCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonCancel.Location = new System.Drawing.Point(502, 392);
            this.m_buttonCancel.Name = "m_buttonCancel";
            this.m_buttonCancel.TabIndex = 3;
            this.m_buttonCancel.Text = "Cancel";
            // 
            // m_buttonApplyToVersion
            // 
            this.m_buttonApplyToVersion.Enabled = false;
            this.m_buttonApplyToVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonApplyToVersion.Location = new System.Drawing.Point(148, 36);
            this.m_buttonApplyToVersion.Name = "m_buttonApplyToVersion";
            this.m_buttonApplyToVersion.TabIndex = 2;
            this.m_buttonApplyToVersion.Text = "&Apply";
            this.m_buttonApplyToVersion.Click += new System.EventHandler(this.m_buttonApplyToVersion_Click);
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(16, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "&To version:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_buttonResetChecks
            // 
            this.m_buttonResetChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonResetChecks.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonResetChecks.Location = new System.Drawing.Point(488, 138);
            this.m_buttonResetChecks.Name = "m_buttonResetChecks";
            this.m_buttonResetChecks.Size = new System.Drawing.Size(80, 23);
            this.m_buttonResetChecks.TabIndex = 4;
            this.m_buttonResetChecks.Text = "Re&set Marks";
            this.m_buttonResetChecks.Click += new System.EventHandler(this.m_buttonResetChecks_Click);
            // 
            // m_buttonResetListView
            // 
            this.m_buttonResetListView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonResetListView.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonResetListView.Location = new System.Drawing.Point(488, 194);
            this.m_buttonResetListView.Name = "m_buttonResetListView";
            this.m_buttonResetListView.Size = new System.Drawing.Size(80, 23);
            this.m_buttonResetListView.TabIndex = 6;
            this.m_buttonResetListView.Text = "Reset A&ll";
            this.m_buttonResetListView.Click += new System.EventHandler(this.m_buttonResetListView_Click);
            // 
            // m_buttonDeselectAll
            // 
            this.m_buttonDeselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonDeselectAll.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonDeselectAll.Location = new System.Drawing.Point(488, 86);
            this.m_buttonDeselectAll.Name = "m_buttonDeselectAll";
            this.m_buttonDeselectAll.Size = new System.Drawing.Size(80, 23);
            this.m_buttonDeselectAll.TabIndex = 2;
            this.m_buttonDeselectAll.Text = "U&nmark All";
            this.m_buttonDeselectAll.Click += new System.EventHandler(this.m_buttonDeselectAll_Click);
            // 
            // m_buttonSelectAllProjects
            // 
            this.m_buttonSelectAllProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonSelectAllProjects.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonSelectAllProjects.Location = new System.Drawing.Point(488, 60);
            this.m_buttonSelectAllProjects.Name = "m_buttonSelectAllProjects";
            this.m_buttonSelectAllProjects.Size = new System.Drawing.Size(80, 23);
            this.m_buttonSelectAllProjects.TabIndex = 1;
            this.m_buttonSelectAllProjects.Text = "&Mark All";
            this.m_buttonSelectAllProjects.Click += new System.EventHandler(this.m_buttonSelectAllProjects_Click);
            // 
            // m_buttonResetVersions
            // 
            this.m_buttonResetVersions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonResetVersions.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonResetVersions.Location = new System.Drawing.Point(488, 168);
            this.m_buttonResetVersions.Name = "m_buttonResetVersions";
            this.m_buttonResetVersions.Size = new System.Drawing.Size(80, 23);
            this.m_buttonResetVersions.TabIndex = 5;
            this.m_buttonResetVersions.Text = "Reset &Vers.";
            this.m_buttonResetVersions.Click += new System.EventHandler(this.m_buttonResetVersions_Click);
            // 
            // m_groupBoxMarkedProjects
            // 
            this.m_groupBoxMarkedProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_groupBoxMarkedProjects.Controls.Add(this.m_groupBoxGetVersion);
            this.m_groupBoxMarkedProjects.Controls.Add(this.m_groupBoxIncrement);
            this.m_groupBoxMarkedProjects.Controls.Add(this.m_versionEditBoxHighestVersion);
            this.m_groupBoxMarkedProjects.Controls.Add(this.m_buttonApplyToVersion);
            this.m_groupBoxMarkedProjects.Controls.Add(this.label1);
            this.m_groupBoxMarkedProjects.Location = new System.Drawing.Point(8, 272);
            this.m_groupBoxMarkedProjects.Name = "m_groupBoxMarkedProjects";
            this.m_groupBoxMarkedProjects.Size = new System.Drawing.Size(344, 148);
            this.m_groupBoxMarkedProjects.TabIndex = 5;
            this.m_groupBoxMarkedProjects.TabStop = false;
            this.m_groupBoxMarkedProjects.Text = "Marked projects ";
            // 
            // m_groupBoxGetVersion
            // 
            this.m_groupBoxGetVersion.Controls.Add(this.m_buttonGetHighestAll);
            this.m_groupBoxGetVersion.Controls.Add(this.m_buttonGetHighestMarked);
            this.m_groupBoxGetVersion.Controls.Add(this.m_buttonGetSelected);
            this.m_groupBoxGetVersion.Controls.Add(this.m_buttonGetHighest);
            this.m_groupBoxGetVersion.Location = new System.Drawing.Point(8, 64);
            this.m_groupBoxGetVersion.Name = "m_groupBoxGetVersion";
            this.m_groupBoxGetVersion.Size = new System.Drawing.Size(216, 76);
            this.m_groupBoxGetVersion.TabIndex = 3;
            this.m_groupBoxGetVersion.TabStop = false;
            this.m_groupBoxGetVersion.Text = "Get version from listview";
            // 
            // m_buttonGetHighestAll
            // 
            this.m_buttonGetHighestAll.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonGetHighestAll.Location = new System.Drawing.Point(116, 46);
            this.m_buttonGetHighestAll.Name = "m_buttonGetHighestAll";
            this.m_buttonGetHighestAll.Size = new System.Drawing.Size(88, 23);
            this.m_buttonGetHighestAll.TabIndex = 3;
            this.m_buttonGetHighestAll.Text = "&Highest all";
            this.m_buttonGetHighestAll.Click += new System.EventHandler(this.m_buttonGetHighestAll_Click);
            // 
            // m_buttonGetHighestMarked
            // 
            this.m_buttonGetHighestMarked.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonGetHighestMarked.Location = new System.Drawing.Point(12, 46);
            this.m_buttonGetHighestMarked.Name = "m_buttonGetHighestMarked";
            this.m_buttonGetHighestMarked.Size = new System.Drawing.Size(88, 23);
            this.m_buttonGetHighestMarked.TabIndex = 2;
            this.m_buttonGetHighestMarked.Text = "High. mar&ked";
            this.m_buttonGetHighestMarked.Click += new System.EventHandler(this.m_buttonGetHighestMarked_Click);
            // 
            // m_buttonGetSelected
            // 
            this.m_buttonGetSelected.Enabled = false;
            this.m_buttonGetSelected.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonGetSelected.Location = new System.Drawing.Point(12, 16);
            this.m_buttonGetSelected.Name = "m_buttonGetSelected";
            this.m_buttonGetSelected.Size = new System.Drawing.Size(88, 23);
            this.m_buttonGetSelected.TabIndex = 0;
            this.m_buttonGetSelected.Text = "Selecte&d";
            this.m_buttonGetSelected.Click += new System.EventHandler(this.m_buttonGetSelected_Click);
            // 
            // m_buttonGetHighest
            // 
            this.m_buttonGetHighest.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonGetHighest.Location = new System.Drawing.Point(116, 16);
            this.m_buttonGetHighest.Name = "m_buttonGetHighest";
            this.m_buttonGetHighest.Size = new System.Drawing.Size(88, 23);
            this.m_buttonGetHighest.TabIndex = 1;
            this.m_buttonGetHighest.Text = "Hi&ghest";
            this.m_buttonGetHighest.Click += new System.EventHandler(this.m_buttonGetHighest_Click);
            // 
            // m_groupBoxIncrement
            // 
            this.m_groupBoxIncrement.Controls.Add(this.m_buttonIncrementRevisionVersion);
            this.m_groupBoxIncrement.Controls.Add(this.m_buttonIncrementMajorVersion);
            this.m_groupBoxIncrement.Controls.Add(this.m_buttonIncrementMinorVersion);
            this.m_groupBoxIncrement.Controls.Add(this.m_buttonIncrementBuildVersion);
            this.m_groupBoxIncrement.Location = new System.Drawing.Point(236, 8);
            this.m_groupBoxIncrement.Name = "m_groupBoxIncrement";
            this.m_groupBoxIncrement.Size = new System.Drawing.Size(94, 132);
            this.m_groupBoxIncrement.TabIndex = 4;
            this.m_groupBoxIncrement.TabStop = false;
            this.m_groupBoxIncrement.Text = "Increment";
            // 
            // m_buttonIncrementRevisionVersion
            // 
            this.m_buttonIncrementRevisionVersion.Enabled = false;
            this.m_buttonIncrementRevisionVersion.Location = new System.Drawing.Point(10, 102);
            this.m_buttonIncrementRevisionVersion.Name = "m_buttonIncrementRevisionVersion";
            this.m_buttonIncrementRevisionVersion.TabIndex = 3;
            this.m_buttonIncrementRevisionVersion.Text = "&Revision";
            this.m_buttonIncrementRevisionVersion.Click += new System.EventHandler(this.m_buttonIncrementRevision_Click);
            // 
            // m_buttonIncrementMajorVersion
            // 
            this.m_buttonIncrementMajorVersion.Enabled = false;
            this.m_buttonIncrementMajorVersion.Location = new System.Drawing.Point(10, 18);
            this.m_buttonIncrementMajorVersion.Name = "m_buttonIncrementMajorVersion";
            this.m_buttonIncrementMajorVersion.TabIndex = 0;
            this.m_buttonIncrementMajorVersion.Text = "Maj&or";
            this.m_buttonIncrementMajorVersion.Click += new System.EventHandler(this.m_buttonIncrementMajorVersion_Click);
            // 
            // m_buttonIncrementMinorVersion
            // 
            this.m_buttonIncrementMinorVersion.Enabled = false;
            this.m_buttonIncrementMinorVersion.Location = new System.Drawing.Point(10, 46);
            this.m_buttonIncrementMinorVersion.Name = "m_buttonIncrementMinorVersion";
            this.m_buttonIncrementMinorVersion.TabIndex = 1;
            this.m_buttonIncrementMinorVersion.Text = "M&inor";
            this.m_buttonIncrementMinorVersion.Click += new System.EventHandler(this.m_buttonIncrementMinorVersion_Click);
            // 
            // m_buttonIncrementBuildVersion
            // 
            this.m_buttonIncrementBuildVersion.Enabled = false;
            this.m_buttonIncrementBuildVersion.Location = new System.Drawing.Point(10, 74);
            this.m_buttonIncrementBuildVersion.Name = "m_buttonIncrementBuildVersion";
            this.m_buttonIncrementBuildVersion.TabIndex = 2;
            this.m_buttonIncrementBuildVersion.Text = "B&uild";
            this.m_buttonIncrementBuildVersion.Click += new System.EventHandler(this.m_buttonIncrementBuildVersion_Click);
            // 
            // m_versionEditBoxHighestVersion
            // 
            this.m_versionEditBoxHighestVersion.Location = new System.Drawing.Point(16, 37);
            this.m_versionEditBoxHighestVersion.Name = "m_versionEditBoxHighestVersion";
            this.m_versionEditBoxHighestVersion.Size = new System.Drawing.Size(126, 20);
            this.m_versionEditBoxHighestVersion.TabIndex = 1;
            // 
            // m_buttonRebuildAll
            // 
            this.m_buttonRebuildAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonRebuildAll.Enabled = false;
            this.m_buttonRebuildAll.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonRebuildAll.Location = new System.Drawing.Point(502, 344);
            this.m_buttonRebuildAll.Name = "m_buttonRebuildAll";
            this.m_buttonRebuildAll.TabIndex = 2;
            this.m_buttonRebuildAll.Text = "Rebuild All";
            this.m_buttonRebuildAll.Click += new System.EventHandler(this.m_buttonBuildAll_Click);
            // 
            // m_buttonSave
            // 
            this.m_buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonSave.Location = new System.Drawing.Point(502, 280);
            this.m_buttonSave.Name = "m_buttonSave";
            this.m_buttonSave.TabIndex = 0;
            this.m_buttonSave.Text = "Save";
            this.m_buttonSave.Click += new System.EventHandler(this.m_buttonSave_Click);
            // 
            // m_groupBoxProjects
            // 
            this.m_groupBoxProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_groupBoxProjects.Controls.Add(this.m_buttonExport);
            this.m_groupBoxProjects.Controls.Add(this.m_assemblyInfoListViewsControl);
            this.m_groupBoxProjects.Controls.Add(this.m_buttonInvertChecks);
            this.m_groupBoxProjects.Controls.Add(this.m_buttonResetVersions);
            this.m_groupBoxProjects.Controls.Add(this.m_buttonResetChecks);
            this.m_groupBoxProjects.Controls.Add(this.m_buttonDeselectAll);
            this.m_groupBoxProjects.Controls.Add(this.m_buttonResetListView);
            this.m_groupBoxProjects.Controls.Add(this.m_buttonSelectAllProjects);
            this.m_groupBoxProjects.Location = new System.Drawing.Point(8, 4);
            this.m_groupBoxProjects.Name = "m_groupBoxProjects";
            this.m_groupBoxProjects.Size = new System.Drawing.Size(576, 260);
            this.m_groupBoxProjects.TabIndex = 4;
            this.m_groupBoxProjects.TabStop = false;
            this.m_groupBoxProjects.Text = "&Projects";
            // 
            // m_buttonExport
            // 
            this.m_buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonExport.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonExport.Location = new System.Drawing.Point(488, 226);
            this.m_buttonExport.Name = "m_buttonExport";
            this.m_buttonExport.Size = new System.Drawing.Size(80, 23);
            this.m_buttonExport.TabIndex = 7;
            this.m_buttonExport.Text = "E&xport";
            this.m_buttonExport.Click += new System.EventHandler(this.m_buttonExport_Click);
            // 
            // m_assemblyInfoListViewsControl
            // 
            this.m_assemblyInfoListViewsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_assemblyInfoListViewsControl.ListViewColumnWidths = new int[] {
                                                                                     174,
                                                                                     83,
                                                                                     120,
                                                                                     82};
            this.m_assemblyInfoListViewsControl.Location = new System.Drawing.Point(8, 16);
            this.m_assemblyInfoListViewsControl.Name = "m_assemblyInfoListViewsControl";
            this.m_assemblyInfoListViewsControl.Size = new System.Drawing.Size(472, 232);
            this.m_assemblyInfoListViewsControl.TabIndex = 0;
            // 
            // m_buttonInvertChecks
            // 
            this.m_buttonInvertChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonInvertChecks.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonInvertChecks.Location = new System.Drawing.Point(488, 112);
            this.m_buttonInvertChecks.Name = "m_buttonInvertChecks";
            this.m_buttonInvertChecks.Size = new System.Drawing.Size(80, 23);
            this.m_buttonInvertChecks.TabIndex = 3;
            this.m_buttonInvertChecks.Text = "In&vert Marks";
            this.m_buttonInvertChecks.Click += new System.EventHandler(this.m_buttonInvertChecks_Click);
            // 
            // m_groupBoxAllProjects
            // 
            this.m_groupBoxAllProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_groupBoxAllProjects.Controls.Add(this.m_buttonAllProjectsIncrementAndSynchronize);
            this.m_groupBoxAllProjects.Controls.Add(this.m_buttonAllToHighest);
            this.m_groupBoxAllProjects.Location = new System.Drawing.Point(368, 328);
            this.m_groupBoxAllProjects.Name = "m_groupBoxAllProjects";
            this.m_groupBoxAllProjects.Size = new System.Drawing.Size(104, 88);
            this.m_groupBoxAllProjects.TabIndex = 7;
            this.m_groupBoxAllProjects.TabStop = false;
            this.m_groupBoxAllProjects.Text = "All projects";
            // 
            // m_buttonAllProjectsIncrementAndSynchronize
            // 
            this.m_buttonAllProjectsIncrementAndSynchronize.Location = new System.Drawing.Point(16, 56);
            this.m_buttonAllProjectsIncrementAndSynchronize.Name = "m_buttonAllProjectsIncrementAndSynchronize";
            this.m_buttonAllProjectsIncrementAndSynchronize.TabIndex = 1;
            this.m_buttonAllProjectsIncrementAndSynchronize.Text = "In&cr && Sync";
            this.m_buttonAllProjectsIncrementAndSynchronize.Click += new System.EventHandler(this.m_buttonAllProjectsIncrementAndSynchronize_Click);
            // 
            // m_buttonAllToHighest
            // 
            this.m_buttonAllToHighest.Location = new System.Drawing.Point(16, 24);
            this.m_buttonAllToHighest.Name = "m_buttonAllToHighest";
            this.m_buttonAllToHighest.TabIndex = 0;
            this.m_buttonAllToHighest.Text = "S&ync";
            this.m_buttonAllToHighest.Click += new System.EventHandler(this.m_buttonAllToLargest_Click);
            // 
            // m_checkBoxApplyToAllTabs
            // 
            this.m_checkBoxApplyToAllTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_checkBoxApplyToAllTabs.Checked = true;
            this.m_checkBoxApplyToAllTabs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_checkBoxApplyToAllTabs.Location = new System.Drawing.Point(368, 280);
            this.m_checkBoxApplyToAllTabs.Name = "m_checkBoxApplyToAllTabs";
            this.m_checkBoxApplyToAllTabs.Size = new System.Drawing.Size(112, 24);
            this.m_checkBoxApplyToAllTabs.TabIndex = 6;
            this.m_checkBoxApplyToAllTabs.Text = "Apply to all ta&bs";
            this.m_checkBoxApplyToAllTabs.CheckedChanged += new System.EventHandler(this.m_checkBoxApplyToAllTabs_CheckedChanged);
            // 
            // m_contextMenuExport
            // 
            this.m_contextMenuExport.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
                                                                                                this.m_menuItemExportPrint,
                                                                                                this.m_menuItemExportFile});
            // 
            // m_menuItemExportPrint
            // 
            this.m_menuItemExportPrint.Index = 0;
            this.m_menuItemExportPrint.Text = "&Print...";
            this.m_menuItemExportPrint.Click += new System.EventHandler(this.m_menuItemExportPrint_Click);
            // 
            // m_menuItemExportFile
            // 
            this.m_menuItemExportFile.Index = 1;
            this.m_menuItemExportFile.Text = "To &File...";
            this.m_menuItemExportFile.Click += new System.EventHandler(this.m_menuItemExportFile_Click);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(592, 429);
            this.Controls.Add(this.m_checkBoxApplyToAllTabs);
            this.Controls.Add(this.m_groupBoxAllProjects);
            this.Controls.Add(this.m_groupBoxProjects);
            this.Controls.Add(this.m_buttonSave);
            this.Controls.Add(this.m_buttonRebuildAll);
            this.Controls.Add(this.m_groupBoxMarkedProjects);
            this.Controls.Add(this.m_buttonCancel);
            this.Controls.Add(this.m_buttonBuild);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(590, 456);
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Project Versions";
            this.m_groupBoxMarkedProjects.ResumeLayout(false);
            this.m_groupBoxGetVersion.ResumeLayout(false);
            this.m_groupBoxIncrement.ResumeLayout(false);
            this.m_groupBoxProjects.ResumeLayout(false);
            this.m_groupBoxAllProjects.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        #region Private fields

        /// <summary>
        ///   <c>ISolutionBrowser</c> object responsible for getting/setting 
        ///   information for projects in the solution.
        /// </summary>
        private ISolutionBrowser m_projectBrowser = null;
        /// <summary>
        ///   Configuration data object.
        /// </summary>
        private VcbConfiguration m_configuration = null;
        /// <summary>
        ///   Command string to perform 
        /// </summary>
        private string m_commandToPerform;
        /// <summary>
        ///   Flag indicating that form has been disposed.
        /// </summary>
        private bool m_disposed = false;

        #endregion // Private fields

        /// <summary>
        ///   Static constructor.
        /// </summary>
        static MainForm() {
            ResourceManager resources = new System.Resources.ResourceManager("BuildAutoIncrement.Resources.Shared", typeof(ResourceAccessor).Assembly);
            Debug.Assert(resources != null);
            s_txtTitle                      = resources.GetString("Title");
            s_txtEmpty                      = resources.GetString("Empty");
            s_txtSettings                   = resources.GetString("Settings menu");
            s_txtAbout                      = resources.GetString("About menu");
            s_txtCheckPermissions           = resources.GetString("Check permissions");
            s_txtHigherVersionWarning       = resources.GetString("Higher version warning");
            s_txtAreYouSureToContinue       = resources.GetString("Are you sure to continue");
            s_txtCannotSaveConfiguration    = resources.GetString("Cannot save configuration file");
            s_txtFailedToLoadConfiguration  = resources.GetString("Failed to load configuration file");
            s_txtUsingDefaultConfiguration  = resources.GetString("Using default configuration");

            Debug.Assert(s_txtTitle != null);
            Debug.Assert(s_txtEmpty != null);
            Debug.Assert(s_txtSettings != null);
            Debug.Assert(s_txtAbout != null);
            Debug.Assert(s_txtCheckPermissions != null);
            Debug.Assert(s_txtHigherVersionWarning != null);
            Debug.Assert(s_txtAreYouSureToContinue != null);
            Debug.Assert(s_txtCannotSaveConfiguration != null);
            Debug.Assert(s_txtFailedToLoadConfiguration != null);
            Debug.Assert(s_txtUsingDefaultConfiguration != null);
        }

        #region String constants

        private static readonly string s_txtTitle;
        private static readonly string s_txtEmpty;
        private static readonly string s_txtSettings;
        private static readonly string s_txtAbout;
        private static readonly string s_txtCheckPermissions;
        private static readonly string s_txtHigherVersionWarning;
        private static readonly string s_txtAreYouSureToContinue;
        private static readonly string s_txtCannotSaveConfiguration;
        private static readonly string s_txtFailedToLoadConfiguration;
        private static readonly string s_txtUsingDefaultConfiguration;

        #endregion // String constants

    }
}