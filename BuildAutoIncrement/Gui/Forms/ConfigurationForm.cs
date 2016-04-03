/*
 * Filename:    ConfigurationForm.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Tool configuration form.
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using System.Windows.Forms.ColorPicker;

namespace BuildAutoIncrement {

	/// <summary>
	///   Application settings form.
	/// </summary>
    public class ConfigurationForm : System.Windows.Forms.Form {

        private enum ProjectListViewItemTypes {
            NotModifiedMarked,
            NotModifiedNotMarked,
            ModifiedMarked,
            ModifiedNotMarked,
            IllegalVersionMarked,
            IllegalVersionUnmarked,
            NoVersion,
            SubProjectRoot,
            ReportVersionUpdated,
            ReportNoVersionChange,
            ReportVersionUpdateFailed
        }

        #region Controls

        private System.Windows.Forms.TabControl m_tabControlSettings;

        private System.Windows.Forms.TabPage m_tabPageGeneral;
        private System.Windows.Forms.CheckBox m_checkBoxSaveFilesBeforeRunningAddinCommand;
        private System.Windows.Forms.Label m_labelDefaultVersionAttribute;
        private System.Windows.Forms.ComboBox m_comboBoxDefaulVersionAttribute;
        private System.Windows.Forms.CheckBox m_checkBoxApplyToAllTabs;
        private System.Windows.Forms.CheckBox m_checkBoxSynchronizeAllTypes;
        private System.Windows.Forms.CheckBox m_checkBoxDontWarnInvalidInformationalVersion;
        private BuildAutoIncrement.LabelWithDivider m_labelAdditionalVersionFiles;
        private System.Windows.Forms.CheckBox m_checkBoxIncludeVcppResources;
        private BuildAutoIncrement.LabelWithDivider m_labelSetupProjects;
        private System.Windows.Forms.CheckBox m_checkBoxIncludeSetupProjects;
        private System.Windows.Forms.CheckBox m_checkBoxGenerateProductAndPackageCode;

        private System.Windows.Forms.TabPage m_tabPageNumberingScheme;
        private System.Windows.Forms.Panel m_panelNumberingStyle;
        private System.Windows.Forms.RadioButton m_radioButtonIncrement;
        private System.Windows.Forms.ComboBox m_comboBoxIncrement;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown m_numericUpDownIncrementBy;
        private System.Windows.Forms.RadioButton m_radioButtonMicrosoftScheme;
        private System.Windows.Forms.GroupBox m_groupBoxResetBuild;
        private System.Windows.Forms.CheckBox m_checkBoxResetBuildOnMajor;
        private System.Windows.Forms.CheckBox m_checkBoxResetBuildOnMinor;
        private System.Windows.Forms.GroupBox m_groupBoxResetRevision;
        private System.Windows.Forms.CheckBox m_checkBoxResetRevisionOnMajor;
        private System.Windows.Forms.CheckBox m_checkBoxResetRevisionOnMinor;
        private System.Windows.Forms.CheckBox m_checkBoxResetRevisionOnBuild;
        private System.Windows.Forms.GroupBox m_groupBoxRestartBuildAndVersion;
        private System.Windows.Forms.RadioButton m_radioButtonResetTo0;
        private System.Windows.Forms.RadioButton m_radioButtonResetTo1;
        private System.Windows.Forms.CheckBox m_checkBoxReplaceWildcards;

        private System.Windows.Forms.TabPage m_tabPageAppearance;
        private System.Windows.Forms.GroupBox m_groupBoxColors;
        private System.Windows.Forms.Label m_labelProjectItems;
        private System.Windows.Forms.ListBox m_listBoxProjectListViewItems;
        private System.Windows.Forms.Label m_labelSample;
        private System.Windows.Forms.Button m_buttonSelectColor;
        private System.Windows.Forms.CheckBox m_checkBoxIndentSubProjects;
        private System.Windows.Forms.CheckBox m_checkBoxShowSubProjectRoot;
        private System.Windows.Forms.CheckBox m_checkBoxShowEnterpriseTemplateRoot;
        private System.Windows.Forms.NumericUpDown m_numericUpDownSubProjectIndentation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox m_checkBoxShowEmptyFolder;
        private System.Windows.Forms.CheckBox m_checkBoxShowNonVersionableProjects;

        private System.Windows.Forms.TabPage m_tabPageBatchCommands;
        private System.Windows.Forms.GroupBox m_groupBoxBatchIncrementScheme;
        private System.Windows.Forms.RadioButton m_radioButtonIncrementAllIndependently;
        private System.Windows.Forms.RadioButton m_radioButtonSynchronizeToHighestValue;
        private System.Windows.Forms.RadioButton m_radioButtonIncrementAndSynchronize;
        private System.Windows.Forms.RadioButton m_radioButtonIncrementModifiedIndependently;
        private System.Windows.Forms.CheckBox m_checkBoxFinalNotification;

        private System.Windows.Forms.TabPage m_tabPageFolders;
        private System.Windows.Forms.GroupBox m_groupBoxSourceSafe;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox m_checkBoxSourceSafeInstalled;
        private System.Windows.Forms.TextBox m_textBoxSourceSafePath;
        private System.Windows.Forms.Button m_buttonBrowseSourceSafe;
        private System.Windows.Forms.GroupBox m_groupBoxIis;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox m_checkBoxIisInstalled;
        private System.Windows.Forms.TextBox m_textBoxIisRoot;
        private System.Windows.Forms.Button m_buttonBrowseIisRoot;

        private System.Windows.Forms.TabPage m_tabPageExport;
        private BuildAutoIncrement.VcbExportOptionsUserControl m_userControlExportOptions;

        private System.Windows.Forms.Button m_buttonOK;
        private System.Windows.Forms.Button m_buttonCancel;
        #endregion // Controls

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Constructor

        public ConfigurationForm() {
            InitializeComponent();
            VisualStyles.SetUseVisualStyleBackColor(m_tabControlSettings);
            VisualStyles.SetButtonFlatStyleSystem(this);
            m_configuration = ConfigurationPersister.Instance.Configuration.Clone();
            SetControlValues();
            m_colorsForm.ColorSelected += new ColorSelectedEventHandler(this.OnColorSelected);
        }

        #endregion // Constructor

        #region Public methods 

        public VcbConfiguration GetConfiguration() {
            GetControlValues();
            return m_configuration; 
        }

        public bool NumberingSchemeHasChanged(NumberingOptions numberingOptions) {
            return numberingOptions.UseDateTimeBasedBuildAndRevisionNumbering   != UseDateTimeBasedBuildAndRevisionNumbering
                || numberingOptions.IncludeVCppResourceFiles                    != IncludeVCppResourceFiles
                || numberingOptions.IncludeSetupProjects                        != IncludeSetupProjects
                || numberingOptions.ApplyToAllTypes                             != ApplyToAllTabs
                || numberingOptions.SynchronizeAllVersionTypes                  != SynchronizeAllVersionTypes
                || numberingOptions.IncrementScheme                             != IncrementScheme
                || numberingOptions.ResetBuildOnMajorIncrement                  != ResetBuildOnMajor
                || numberingOptions.ResetBuildOnMinorIncrement                  != ResetBuildOnMinor
                || numberingOptions.ResetRevisionOnMajorIncrement               != ResetRevisionOnMajor
                || numberingOptions.ResetRevisionOnMinorIncrement               != ResetRevisionOnMinor
                || numberingOptions.ResetRevisionOnBuildIncrement               != ResetRevisionOnBuild
                || numberingOptions.ResetBuildAndRevisionTo                     != ResetBuildAndRevisionTo
                || numberingOptions.ReplaceAsteriskWithVersionComponents        != ReplaceAsteriskWithComponentVersions;
        }

        public bool ListViewOptionsChanged(DisplayOptions options) {
            return IndentSubProjectItems != options.IndentSubProjectItems || SubProjectsIndentation != options.SubProjectIndentation;
        }

        public bool ProjectItemsToDisplayChanged(DisplayOptions options) {
            return ShowSubProjectRoots != options.ShowSubProjectRoot || ShowEnterpriseTemplateProjectRoot != options.ShowEnterpriseTemplateProjectRoot || ShowNonVersionableProjects != options.ShowNonVersionableProjects || ShowEmptyFolders != options.ShowEmptyFolders;
        }

        public bool ListViewColorsHasChanged(ProjectsListViewColorsConfiguration colors) {
            return m_configuration.DisplayOptions.Colors != colors;
        }

        #endregion // Public methods 

        #region Overriden methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            if (m_listBoxProjectListViewItems.SelectedIndex == -1) 
                m_listBoxProjectListViewItems.SelectedIndex = 0;
        }

        #endregion // Overriden methods

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ConfigurationForm));
            this.m_checkBoxResetBuildOnMinor = new System.Windows.Forms.CheckBox();
            this.m_checkBoxResetRevisionOnBuild = new System.Windows.Forms.CheckBox();
            this.m_panelNumberingStyle = new System.Windows.Forms.Panel();
            this.m_numericUpDownIncrementBy = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.m_radioButtonMicrosoftScheme = new System.Windows.Forms.RadioButton();
            this.m_comboBoxIncrement = new System.Windows.Forms.ComboBox();
            this.m_radioButtonIncrement = new System.Windows.Forms.RadioButton();
            this.m_checkBoxResetRevisionOnMinor = new System.Windows.Forms.CheckBox();
            this.m_checkBoxResetRevisionOnMajor = new System.Windows.Forms.CheckBox();
            this.m_checkBoxResetBuildOnMajor = new System.Windows.Forms.CheckBox();
            this.m_buttonOK = new System.Windows.Forms.Button();
            this.m_buttonCancel = new System.Windows.Forms.Button();
            this.m_checkBoxApplyToAllTabs = new System.Windows.Forms.CheckBox();
            this.m_comboBoxDefaulVersionAttribute = new System.Windows.Forms.ComboBox();
            this.m_labelDefaultVersionAttribute = new System.Windows.Forms.Label();
            this.m_tabControlSettings = new System.Windows.Forms.TabControl();
            this.m_tabPageGeneral = new System.Windows.Forms.TabPage();
            this.m_checkBoxDontWarnInvalidInformationalVersion = new System.Windows.Forms.CheckBox();
            this.m_checkBoxSaveFilesBeforeRunningAddinCommand = new System.Windows.Forms.CheckBox();
            this.m_labelAdditionalVersionFiles = new BuildAutoIncrement.LabelWithDivider();
            this.m_labelSetupProjects = new BuildAutoIncrement.LabelWithDivider();
            this.m_checkBoxGenerateProductAndPackageCode = new System.Windows.Forms.CheckBox();
            this.m_checkBoxIncludeSetupProjects = new System.Windows.Forms.CheckBox();
            this.m_checkBoxIncludeVcppResources = new System.Windows.Forms.CheckBox();
            this.m_checkBoxSynchronizeAllTypes = new System.Windows.Forms.CheckBox();
            this.m_tabPageNumberingScheme = new System.Windows.Forms.TabPage();
            this.m_groupBoxRestartBuildAndVersion = new System.Windows.Forms.GroupBox();
            this.m_radioButtonResetTo1 = new System.Windows.Forms.RadioButton();
            this.m_radioButtonResetTo0 = new System.Windows.Forms.RadioButton();
            this.m_checkBoxReplaceWildcards = new System.Windows.Forms.CheckBox();
            this.m_groupBoxResetRevision = new System.Windows.Forms.GroupBox();
            this.m_groupBoxResetBuild = new System.Windows.Forms.GroupBox();
            this.m_tabPageAppearance = new System.Windows.Forms.TabPage();
            this.m_checkBoxShowEnterpriseTemplateRoot = new System.Windows.Forms.CheckBox();
            this.m_checkBoxShowEmptyFolder = new System.Windows.Forms.CheckBox();
            this.m_checkBoxShowNonVersionableProjects = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_numericUpDownSubProjectIndentation = new System.Windows.Forms.NumericUpDown();
            this.m_checkBoxShowSubProjectRoot = new System.Windows.Forms.CheckBox();
            this.m_checkBoxIndentSubProjects = new System.Windows.Forms.CheckBox();
            this.m_groupBoxColors = new System.Windows.Forms.GroupBox();
            this.m_listBoxProjectListViewItems = new System.Windows.Forms.ListBox();
            this.m_labelSample = new System.Windows.Forms.Label();
            this.m_buttonSelectColor = new System.Windows.Forms.Button();
            this.m_labelProjectItems = new System.Windows.Forms.Label();
            this.m_tabPageBatchCommands = new System.Windows.Forms.TabPage();
            this.m_checkBoxFinalNotification = new System.Windows.Forms.CheckBox();
            this.m_groupBoxBatchIncrementScheme = new System.Windows.Forms.GroupBox();
            this.m_radioButtonIncrementAllIndependently = new System.Windows.Forms.RadioButton();
            this.m_radioButtonSynchronizeToHighestValue = new System.Windows.Forms.RadioButton();
            this.m_radioButtonIncrementAndSynchronize = new System.Windows.Forms.RadioButton();
            this.m_radioButtonIncrementModifiedIndependently = new System.Windows.Forms.RadioButton();
            this.m_tabPageFolders = new System.Windows.Forms.TabPage();
            this.m_groupBoxSourceSafe = new System.Windows.Forms.GroupBox();
            this.m_buttonBrowseSourceSafe = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.m_textBoxSourceSafePath = new System.Windows.Forms.TextBox();
            this.m_checkBoxSourceSafeInstalled = new System.Windows.Forms.CheckBox();
            this.m_groupBoxIis = new System.Windows.Forms.GroupBox();
            this.m_buttonBrowseIisRoot = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.m_textBoxIisRoot = new System.Windows.Forms.TextBox();
            this.m_checkBoxIisInstalled = new System.Windows.Forms.CheckBox();
            this.m_tabPageExport = new System.Windows.Forms.TabPage();
            this.m_userControlExportOptions = new BuildAutoIncrement.VcbExportOptionsUserControl();
            this.m_panelNumberingStyle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDownIncrementBy)).BeginInit();
            this.m_tabControlSettings.SuspendLayout();
            this.m_tabPageGeneral.SuspendLayout();
            this.m_tabPageNumberingScheme.SuspendLayout();
            this.m_groupBoxRestartBuildAndVersion.SuspendLayout();
            this.m_groupBoxResetRevision.SuspendLayout();
            this.m_groupBoxResetBuild.SuspendLayout();
            this.m_tabPageAppearance.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDownSubProjectIndentation)).BeginInit();
            this.m_groupBoxColors.SuspendLayout();
            this.m_tabPageBatchCommands.SuspendLayout();
            this.m_groupBoxBatchIncrementScheme.SuspendLayout();
            this.m_tabPageFolders.SuspendLayout();
            this.m_groupBoxSourceSafe.SuspendLayout();
            this.m_groupBoxIis.SuspendLayout();
            this.m_tabPageExport.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_checkBoxResetBuildOnMinor
            // 
            this.m_checkBoxResetBuildOnMinor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_checkBoxResetBuildOnMinor.Location = new System.Drawing.Point(16, 38);
            this.m_checkBoxResetBuildOnMinor.Name = "m_checkBoxResetBuildOnMinor";
            this.m_checkBoxResetBuildOnMinor.Size = new System.Drawing.Size(296, 24);
            this.m_checkBoxResetBuildOnMinor.TabIndex = 1;
            this.m_checkBoxResetBuildOnMinor.Text = "On Mi&nor increment";
            // 
            // m_checkBoxResetRevisionOnBuild
            // 
            this.m_checkBoxResetRevisionOnBuild.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_checkBoxResetRevisionOnBuild.Location = new System.Drawing.Point(16, 60);
            this.m_checkBoxResetRevisionOnBuild.Name = "m_checkBoxResetRevisionOnBuild";
            this.m_checkBoxResetRevisionOnBuild.Size = new System.Drawing.Size(296, 24);
            this.m_checkBoxResetRevisionOnBuild.TabIndex = 2;
            this.m_checkBoxResetRevisionOnBuild.Text = "On &Build increment";
            // 
            // m_panelNumberingStyle
            // 
            this.m_panelNumberingStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_panelNumberingStyle.Controls.Add(this.m_numericUpDownIncrementBy);
            this.m_panelNumberingStyle.Controls.Add(this.label4);
            this.m_panelNumberingStyle.Controls.Add(this.m_radioButtonMicrosoftScheme);
            this.m_panelNumberingStyle.Controls.Add(this.m_comboBoxIncrement);
            this.m_panelNumberingStyle.Controls.Add(this.m_radioButtonIncrement);
            this.m_panelNumberingStyle.Location = new System.Drawing.Point(16, 8);
            this.m_panelNumberingStyle.Name = "m_panelNumberingStyle";
            this.m_panelNumberingStyle.Size = new System.Drawing.Size(312, 96);
            this.m_panelNumberingStyle.TabIndex = 0;
            // 
            // m_numericUpDownIncrementBy
            // 
            this.m_numericUpDownIncrementBy.Location = new System.Drawing.Point(144, 33);
            this.m_numericUpDownIncrementBy.Minimum = new System.Decimal(new int[] {
                                                                                       1,
                                                                                       0,
                                                                                       0,
                                                                                       0});
            this.m_numericUpDownIncrementBy.Name = "m_numericUpDownIncrementBy";
            this.m_numericUpDownIncrementBy.Size = new System.Drawing.Size(48, 20);
            this.m_numericUpDownIncrementBy.TabIndex = 3;
            this.m_numericUpDownIncrementBy.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.m_numericUpDownIncrementBy.Value = new System.Decimal(new int[] {
                                                                                     1,
                                                                                     0,
                                                                                     0,
                                                                                     0});
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 22);
            this.label4.TabIndex = 2;
            this.label4.Text = "&Increment by:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_radioButtonMicrosoftScheme
            // 
            this.m_radioButtonMicrosoftScheme.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioButtonMicrosoftScheme.Location = new System.Drawing.Point(0, 64);
            this.m_radioButtonMicrosoftScheme.Name = "m_radioButtonMicrosoftScheme";
            this.m_radioButtonMicrosoftScheme.Size = new System.Drawing.Size(312, 32);
            this.m_radioButtonMicrosoftScheme.TabIndex = 4;
            this.m_radioButtonMicrosoftScheme.Text = "&Date&&time based Build and Revision numbering (Microsoft\'s scheme)";
            this.m_radioButtonMicrosoftScheme.CheckedChanged += new System.EventHandler(this.m_radioButtonMicrosoftScheme_CheckedChanged);
            // 
            // m_comboBoxIncrement
            // 
            this.m_comboBoxIncrement.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_comboBoxIncrement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboBoxIncrement.Items.AddRange(new object[] {
                                                                     "Major version",
                                                                     "Minor version",
                                                                     "Build",
                                                                     "Revision"});
            this.m_comboBoxIncrement.Location = new System.Drawing.Point(144, 6);
            this.m_comboBoxIncrement.Name = "m_comboBoxIncrement";
            this.m_comboBoxIncrement.Size = new System.Drawing.Size(168, 20);
            this.m_comboBoxIncrement.TabIndex = 1;
            // 
            // m_radioButtonIncrement
            // 
            this.m_radioButtonIncrement.Checked = true;
            this.m_radioButtonIncrement.Location = new System.Drawing.Point(0, 4);
            this.m_radioButtonIncrement.Name = "m_radioButtonIncrement";
            this.m_radioButtonIncrement.Size = new System.Drawing.Size(144, 24);
            this.m_radioButtonIncrement.TabIndex = 0;
            this.m_radioButtonIncrement.TabStop = true;
            this.m_radioButtonIncrement.Text = "&By default, increment:";
            // 
            // m_checkBoxResetRevisionOnMinor
            // 
            this.m_checkBoxResetRevisionOnMinor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_checkBoxResetRevisionOnMinor.Location = new System.Drawing.Point(16, 38);
            this.m_checkBoxResetRevisionOnMinor.Name = "m_checkBoxResetRevisionOnMinor";
            this.m_checkBoxResetRevisionOnMinor.Size = new System.Drawing.Size(296, 24);
            this.m_checkBoxResetRevisionOnMinor.TabIndex = 1;
            this.m_checkBoxResetRevisionOnMinor.Text = "On Mi&nor increment";
            // 
            // m_checkBoxResetRevisionOnMajor
            // 
            this.m_checkBoxResetRevisionOnMajor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_checkBoxResetRevisionOnMajor.Location = new System.Drawing.Point(16, 16);
            this.m_checkBoxResetRevisionOnMajor.Name = "m_checkBoxResetRevisionOnMajor";
            this.m_checkBoxResetRevisionOnMajor.Size = new System.Drawing.Size(296, 24);
            this.m_checkBoxResetRevisionOnMajor.TabIndex = 0;
            this.m_checkBoxResetRevisionOnMajor.Text = "On M&ajor increment";
            // 
            // m_checkBoxResetBuildOnMajor
            // 
            this.m_checkBoxResetBuildOnMajor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_checkBoxResetBuildOnMajor.Location = new System.Drawing.Point(16, 16);
            this.m_checkBoxResetBuildOnMajor.Name = "m_checkBoxResetBuildOnMajor";
            this.m_checkBoxResetBuildOnMajor.Size = new System.Drawing.Size(296, 24);
            this.m_checkBoxResetBuildOnMajor.TabIndex = 0;
            this.m_checkBoxResetBuildOnMajor.Text = "On M&ajor increment";
            // 
            // m_buttonOK
            // 
            this.m_buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_buttonOK.Location = new System.Drawing.Point(100, 432);
            this.m_buttonOK.Name = "m_buttonOK";
            this.m_buttonOK.TabIndex = 1;
            this.m_buttonOK.Text = "OK";
            this.m_buttonOK.Click += new System.EventHandler(this.m_buttonOK_Click);
            // 
            // m_buttonCancel
            // 
            this.m_buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_buttonCancel.Location = new System.Drawing.Point(188, 432);
            this.m_buttonCancel.Name = "m_buttonCancel";
            this.m_buttonCancel.TabIndex = 2;
            this.m_buttonCancel.Text = "Cancel";
            // 
            // m_checkBoxApplyToAllTabs
            // 
            this.m_checkBoxApplyToAllTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_checkBoxApplyToAllTabs.Location = new System.Drawing.Point(16, 80);
            this.m_checkBoxApplyToAllTabs.Name = "m_checkBoxApplyToAllTabs";
            this.m_checkBoxApplyToAllTabs.Size = new System.Drawing.Size(312, 24);
            this.m_checkBoxApplyToAllTabs.TabIndex = 3;
            this.m_checkBoxApplyToAllTabs.Text = "&Apply changes to all version types";
            // 
            // m_comboBoxDefaulVersionAttribute
            // 
            this.m_comboBoxDefaulVersionAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_comboBoxDefaulVersionAttribute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comboBoxDefaulVersionAttribute.Items.AddRange(new object[] {
                                                                                  "Assembly Version",
                                                                                  "File Version",
                                                                                  "Informational (Product) Version"});
            this.m_comboBoxDefaulVersionAttribute.Location = new System.Drawing.Point(136, 48);
            this.m_comboBoxDefaulVersionAttribute.Name = "m_comboBoxDefaulVersionAttribute";
            this.m_comboBoxDefaulVersionAttribute.Size = new System.Drawing.Size(192, 21);
            this.m_comboBoxDefaulVersionAttribute.TabIndex = 2;
            // 
            // m_labelDefaultVersionAttribute
            // 
            this.m_labelDefaultVersionAttribute.Location = new System.Drawing.Point(16, 48);
            this.m_labelDefaultVersionAttribute.Name = "m_labelDefaultVersionAttribute";
            this.m_labelDefaultVersionAttribute.Size = new System.Drawing.Size(112, 21);
            this.m_labelDefaultVersionAttribute.TabIndex = 1;
            this.m_labelDefaultVersionAttribute.Text = "Default &version:";
            this.m_labelDefaultVersionAttribute.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_tabControlSettings
            // 
            this.m_tabControlSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tabControlSettings.Controls.Add(this.m_tabPageGeneral);
            this.m_tabControlSettings.Controls.Add(this.m_tabPageNumberingScheme);
            this.m_tabControlSettings.Controls.Add(this.m_tabPageAppearance);
            this.m_tabControlSettings.Controls.Add(this.m_tabPageBatchCommands);
            this.m_tabControlSettings.Controls.Add(this.m_tabPageFolders);
            this.m_tabControlSettings.Controls.Add(this.m_tabPageExport);
            this.m_tabControlSettings.Location = new System.Drawing.Point(8, 8);
            this.m_tabControlSettings.Multiline = true;
            this.m_tabControlSettings.Name = "m_tabControlSettings";
            this.m_tabControlSettings.SelectedIndex = 0;
            this.m_tabControlSettings.Size = new System.Drawing.Size(346, 408);
            this.m_tabControlSettings.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.m_tabControlSettings.TabIndex = 0;
            // 
            // m_tabPageGeneral
            // 
            this.m_tabPageGeneral.Controls.Add(this.m_checkBoxDontWarnInvalidInformationalVersion);
            this.m_tabPageGeneral.Controls.Add(this.m_checkBoxSaveFilesBeforeRunningAddinCommand);
            this.m_tabPageGeneral.Controls.Add(this.m_labelAdditionalVersionFiles);
            this.m_tabPageGeneral.Controls.Add(this.m_labelSetupProjects);
            this.m_tabPageGeneral.Controls.Add(this.m_checkBoxGenerateProductAndPackageCode);
            this.m_tabPageGeneral.Controls.Add(this.m_checkBoxIncludeSetupProjects);
            this.m_tabPageGeneral.Controls.Add(this.m_checkBoxIncludeVcppResources);
            this.m_tabPageGeneral.Controls.Add(this.m_checkBoxSynchronizeAllTypes);
            this.m_tabPageGeneral.Controls.Add(this.m_comboBoxDefaulVersionAttribute);
            this.m_tabPageGeneral.Controls.Add(this.m_labelDefaultVersionAttribute);
            this.m_tabPageGeneral.Controls.Add(this.m_checkBoxApplyToAllTabs);
            this.m_tabPageGeneral.Location = new System.Drawing.Point(4, 40);
            this.m_tabPageGeneral.Name = "m_tabPageGeneral";
            this.m_tabPageGeneral.Size = new System.Drawing.Size(338, 364);
            this.m_tabPageGeneral.TabIndex = 0;
            this.m_tabPageGeneral.Text = "General";
            // 
            // m_checkBoxDontWarnInvalidInformationalVersion
            // 
            this.m_checkBoxDontWarnInvalidInformationalVersion.Location = new System.Drawing.Point(16, 144);
            this.m_checkBoxDontWarnInvalidInformationalVersion.Name = "m_checkBoxDontWarnInvalidInformationalVersion";
            this.m_checkBoxDontWarnInvalidInformationalVersion.Size = new System.Drawing.Size(304, 24);
            this.m_checkBoxDontWarnInvalidInformationalVersion.TabIndex = 5;
            this.m_checkBoxDontWarnInvalidInformationalVersion.Text = "Allow ar&bitrary Informational Version";
            // 
            // m_checkBoxSaveFilesBeforeRunningAddinCommand
            // 
            this.m_checkBoxSaveFilesBeforeRunningAddinCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_checkBoxSaveFilesBeforeRunningAddinCommand.Checked = true;
            this.m_checkBoxSaveFilesBeforeRunningAddinCommand.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_checkBoxSaveFilesBeforeRunningAddinCommand.Location = new System.Drawing.Point(16, 16);
            this.m_checkBoxSaveFilesBeforeRunningAddinCommand.Name = "m_checkBoxSaveFilesBeforeRunningAddinCommand";
            this.m_checkBoxSaveFilesBeforeRunningAddinCommand.Size = new System.Drawing.Size(312, 24);
            this.m_checkBoxSaveFilesBeforeRunningAddinCommand.TabIndex = 0;
            this.m_checkBoxSaveFilesBeforeRunningAddinCommand.Text = "&Save modified files before add-in command execution";
            // 
            // m_labelAdditionalVersionFiles
            // 
            this.m_labelAdditionalVersionFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelAdditionalVersionFiles.Location = new System.Drawing.Point(8, 182);
            this.m_labelAdditionalVersionFiles.Name = "m_labelAdditionalVersionFiles";
            this.m_labelAdditionalVersionFiles.Size = new System.Drawing.Size(320, 23);
            this.m_labelAdditionalVersionFiles.TabIndex = 6;
            this.m_labelAdditionalVersionFiles.Text = "Additional Version Files";
            // 
            // m_labelSetupProjects
            // 
            this.m_labelSetupProjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelSetupProjects.Location = new System.Drawing.Point(8, 244);
            this.m_labelSetupProjects.Name = "m_labelSetupProjects";
            this.m_labelSetupProjects.Size = new System.Drawing.Size(320, 23);
            this.m_labelSetupProjects.TabIndex = 8;
            this.m_labelSetupProjects.Text = "Setup Projects";
            // 
            // m_checkBoxGenerateProductAndPackageCode
            // 
            this.m_checkBoxGenerateProductAndPackageCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_checkBoxGenerateProductAndPackageCode.Enabled = false;
            this.m_checkBoxGenerateProductAndPackageCode.Location = new System.Drawing.Point(16, 300);
            this.m_checkBoxGenerateProductAndPackageCode.Name = "m_checkBoxGenerateProductAndPackageCode";
            this.m_checkBoxGenerateProductAndPackageCode.Size = new System.Drawing.Size(312, 24);
            this.m_checkBoxGenerateProductAndPackageCode.TabIndex = 10;
            this.m_checkBoxGenerateProductAndPackageCode.Text = "Generate new &ProductCode and PackageCode";
            // 
            // m_checkBoxIncludeSetupProjects
            // 
            this.m_checkBoxIncludeSetupProjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_checkBoxIncludeSetupProjects.Location = new System.Drawing.Point(16, 268);
            this.m_checkBoxIncludeSetupProjects.Name = "m_checkBoxIncludeSetupProjects";
            this.m_checkBoxIncludeSetupProjects.Size = new System.Drawing.Size(312, 24);
            this.m_checkBoxIncludeSetupProjects.TabIndex = 9;
            this.m_checkBoxIncludeSetupProjects.Text = "Include set&up projects";
            this.m_checkBoxIncludeSetupProjects.CheckedChanged += new System.EventHandler(this.m_checkBoxIncludeSetupProjects_CheckedChanged);
            // 
            // m_checkBoxIncludeVcppResources
            // 
            this.m_checkBoxIncludeVcppResources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_checkBoxIncludeVcppResources.Location = new System.Drawing.Point(16, 204);
            this.m_checkBoxIncludeVcppResources.Name = "m_checkBoxIncludeVcppResources";
            this.m_checkBoxIncludeVcppResources.Size = new System.Drawing.Size(312, 24);
            this.m_checkBoxIncludeVcppResources.TabIndex = 7;
            this.m_checkBoxIncludeVcppResources.Text = "Include V&C++ (.RC) resource files";
            // 
            // m_checkBoxSynchronizeAllTypes
            // 
            this.m_checkBoxSynchronizeAllTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_checkBoxSynchronizeAllTypes.Location = new System.Drawing.Point(16, 112);
            this.m_checkBoxSynchronizeAllTypes.Name = "m_checkBoxSynchronizeAllTypes";
            this.m_checkBoxSynchronizeAllTypes.Size = new System.Drawing.Size(312, 24);
            this.m_checkBoxSynchronizeAllTypes.TabIndex = 4;
            this.m_checkBoxSynchronizeAllTypes.Text = "For each project sy&nchronize all version types ";
            this.m_checkBoxSynchronizeAllTypes.CheckedChanged += new System.EventHandler(this.m_checkBoxSynchronizeAllTypes_CheckedChanged);
            // 
            // m_tabPageNumberingScheme
            // 
            this.m_tabPageNumberingScheme.Controls.Add(this.m_groupBoxRestartBuildAndVersion);
            this.m_tabPageNumberingScheme.Controls.Add(this.m_checkBoxReplaceWildcards);
            this.m_tabPageNumberingScheme.Controls.Add(this.m_groupBoxResetRevision);
            this.m_tabPageNumberingScheme.Controls.Add(this.m_groupBoxResetBuild);
            this.m_tabPageNumberingScheme.Controls.Add(this.m_panelNumberingStyle);
            this.m_tabPageNumberingScheme.Location = new System.Drawing.Point(4, 40);
            this.m_tabPageNumberingScheme.Name = "m_tabPageNumberingScheme";
            this.m_tabPageNumberingScheme.Size = new System.Drawing.Size(338, 364);
            this.m_tabPageNumberingScheme.TabIndex = 1;
            this.m_tabPageNumberingScheme.Text = "Numbering Scheme";
            // 
            // m_groupBoxRestartBuildAndVersion
            // 
            this.m_groupBoxRestartBuildAndVersion.Controls.Add(this.m_radioButtonResetTo1);
            this.m_groupBoxRestartBuildAndVersion.Controls.Add(this.m_radioButtonResetTo0);
            this.m_groupBoxRestartBuildAndVersion.Location = new System.Drawing.Point(8, 276);
            this.m_groupBoxRestartBuildAndVersion.Name = "m_groupBoxRestartBuildAndVersion";
            this.m_groupBoxRestartBuildAndVersion.Size = new System.Drawing.Size(320, 48);
            this.m_groupBoxRestartBuildAndVersion.TabIndex = 3;
            this.m_groupBoxRestartBuildAndVersion.TabStop = false;
            this.m_groupBoxRestartBuildAndVersion.Text = "Reset Build And Revision";
            // 
            // m_radioButtonResetTo1
            // 
            this.m_radioButtonResetTo1.Location = new System.Drawing.Point(184, 16);
            this.m_radioButtonResetTo1.Name = "m_radioButtonResetTo1";
            this.m_radioButtonResetTo1.TabIndex = 1;
            this.m_radioButtonResetTo1.Text = "to &1";
            // 
            // m_radioButtonResetTo0
            // 
            this.m_radioButtonResetTo0.Checked = true;
            this.m_radioButtonResetTo0.Location = new System.Drawing.Point(16, 16);
            this.m_radioButtonResetTo0.Name = "m_radioButtonResetTo0";
            this.m_radioButtonResetTo0.TabIndex = 0;
            this.m_radioButtonResetTo0.TabStop = true;
            this.m_radioButtonResetTo0.Text = "to &0";
            // 
            // m_checkBoxReplaceWildcards
            // 
            this.m_checkBoxReplaceWildcards.Location = new System.Drawing.Point(16, 332);
            this.m_checkBoxReplaceWildcards.Name = "m_checkBoxReplaceWildcards";
            this.m_checkBoxReplaceWildcards.Size = new System.Drawing.Size(312, 24);
            this.m_checkBoxReplaceWildcards.TabIndex = 4;
            this.m_checkBoxReplaceWildcards.Text = "&Replace asterisk with component values";
            // 
            // m_groupBoxResetRevision
            // 
            this.m_groupBoxResetRevision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_groupBoxResetRevision.Controls.Add(this.m_checkBoxResetRevisionOnMajor);
            this.m_groupBoxResetRevision.Controls.Add(this.m_checkBoxResetRevisionOnMinor);
            this.m_groupBoxResetRevision.Controls.Add(this.m_checkBoxResetRevisionOnBuild);
            this.m_groupBoxResetRevision.Location = new System.Drawing.Point(8, 184);
            this.m_groupBoxResetRevision.Name = "m_groupBoxResetRevision";
            this.m_groupBoxResetRevision.Size = new System.Drawing.Size(320, 88);
            this.m_groupBoxResetRevision.TabIndex = 2;
            this.m_groupBoxResetRevision.TabStop = false;
            this.m_groupBoxResetRevision.Text = "Reset Revision";
            // 
            // m_groupBoxResetBuild
            // 
            this.m_groupBoxResetBuild.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_groupBoxResetBuild.Controls.Add(this.m_checkBoxResetBuildOnMajor);
            this.m_groupBoxResetBuild.Controls.Add(this.m_checkBoxResetBuildOnMinor);
            this.m_groupBoxResetBuild.Location = new System.Drawing.Point(8, 112);
            this.m_groupBoxResetBuild.Name = "m_groupBoxResetBuild";
            this.m_groupBoxResetBuild.Size = new System.Drawing.Size(320, 66);
            this.m_groupBoxResetBuild.TabIndex = 1;
            this.m_groupBoxResetBuild.TabStop = false;
            this.m_groupBoxResetBuild.Text = "Reset Build";
            // 
            // m_tabPageAppearance
            // 
            this.m_tabPageAppearance.Controls.Add(this.m_checkBoxShowEnterpriseTemplateRoot);
            this.m_tabPageAppearance.Controls.Add(this.m_checkBoxShowEmptyFolder);
            this.m_tabPageAppearance.Controls.Add(this.m_checkBoxShowNonVersionableProjects);
            this.m_tabPageAppearance.Controls.Add(this.label1);
            this.m_tabPageAppearance.Controls.Add(this.m_numericUpDownSubProjectIndentation);
            this.m_tabPageAppearance.Controls.Add(this.m_checkBoxShowSubProjectRoot);
            this.m_tabPageAppearance.Controls.Add(this.m_checkBoxIndentSubProjects);
            this.m_tabPageAppearance.Controls.Add(this.m_groupBoxColors);
            this.m_tabPageAppearance.Location = new System.Drawing.Point(4, 40);
            this.m_tabPageAppearance.Name = "m_tabPageAppearance";
            this.m_tabPageAppearance.Size = new System.Drawing.Size(338, 364);
            this.m_tabPageAppearance.TabIndex = 2;
            this.m_tabPageAppearance.Text = "Appearance";
            // 
            // m_checkBoxShowEnterpriseTemplateRoot
            // 
            this.m_checkBoxShowEnterpriseTemplateRoot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_checkBoxShowEnterpriseTemplateRoot.Location = new System.Drawing.Point(16, 272);
            this.m_checkBoxShowEnterpriseTemplateRoot.Name = "m_checkBoxShowEnterpriseTemplateRoot";
            this.m_checkBoxShowEnterpriseTemplateRoot.Size = new System.Drawing.Size(296, 20);
            this.m_checkBoxShowEnterpriseTemplateRoot.TabIndex = 6;
            this.m_checkBoxShowEnterpriseTemplateRoot.Text = "Show &Enterprise Template Project roots";
            this.m_checkBoxShowEnterpriseTemplateRoot.CheckedChanged += new System.EventHandler(this.m_checkBoxShowEnterpriseTemplateRoot_CheckedChanged);
            // 
            // m_checkBoxShowEmptyFolder
            // 
            this.m_checkBoxShowEmptyFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_checkBoxShowEmptyFolder.Location = new System.Drawing.Point(16, 296);
            this.m_checkBoxShowEmptyFolder.Name = "m_checkBoxShowEmptyFolder";
            this.m_checkBoxShowEmptyFolder.Size = new System.Drawing.Size(296, 20);
            this.m_checkBoxShowEmptyFolder.TabIndex = 7;
            this.m_checkBoxShowEmptyFolder.Text = "Show &empty folders";
            // 
            // m_checkBoxShowNonVersionableProjects
            // 
            this.m_checkBoxShowNonVersionableProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_checkBoxShowNonVersionableProjects.Location = new System.Drawing.Point(16, 320);
            this.m_checkBoxShowNonVersionableProjects.Name = "m_checkBoxShowNonVersionableProjects";
            this.m_checkBoxShowNonVersionableProjects.Size = new System.Drawing.Size(296, 20);
            this.m_checkBoxShowNonVersionableProjects.TabIndex = 8;
            this.m_checkBoxShowNonVersionableProjects.Text = "Show &non-versionable projects ";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Location = new System.Drawing.Point(232, 224);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "pixels";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_numericUpDownSubProjectIndentation
            // 
            this.m_numericUpDownSubProjectIndentation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_numericUpDownSubProjectIndentation.Enabled = false;
            this.m_numericUpDownSubProjectIndentation.Location = new System.Drawing.Point(184, 224);
            this.m_numericUpDownSubProjectIndentation.Name = "m_numericUpDownSubProjectIndentation";
            this.m_numericUpDownSubProjectIndentation.Size = new System.Drawing.Size(48, 20);
            this.m_numericUpDownSubProjectIndentation.TabIndex = 3;
            this.m_numericUpDownSubProjectIndentation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.m_numericUpDownSubProjectIndentation.Value = new System.Decimal(new int[] {
                                                                                               10,
                                                                                               0,
                                                                                               0,
                                                                                               0});
            // 
            // m_checkBoxShowSubProjectRoot
            // 
            this.m_checkBoxShowSubProjectRoot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_checkBoxShowSubProjectRoot.Location = new System.Drawing.Point(16, 248);
            this.m_checkBoxShowSubProjectRoot.Name = "m_checkBoxShowSubProjectRoot";
            this.m_checkBoxShowSubProjectRoot.Size = new System.Drawing.Size(296, 20);
            this.m_checkBoxShowSubProjectRoot.TabIndex = 5;
            this.m_checkBoxShowSubProjectRoot.Text = "Show &root folders";
            this.m_checkBoxShowSubProjectRoot.CheckedChanged += new System.EventHandler(this.m_checkBoxShowSubProjectRoot_CheckedChanged);
            // 
            // m_checkBoxIndentSubProjects
            // 
            this.m_checkBoxIndentSubProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_checkBoxIndentSubProjects.Location = new System.Drawing.Point(16, 224);
            this.m_checkBoxIndentSubProjects.Name = "m_checkBoxIndentSubProjects";
            this.m_checkBoxIndentSubProjects.Size = new System.Drawing.Size(168, 20);
            this.m_checkBoxIndentSubProjects.TabIndex = 2;
            this.m_checkBoxIndentSubProjects.Text = "&Indent subproject items by:";
            this.m_checkBoxIndentSubProjects.CheckedChanged += new System.EventHandler(this.m_checkBoxIndentSubProjects_CheckedChanged);
            // 
            // m_groupBoxColors
            // 
            this.m_groupBoxColors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_groupBoxColors.Controls.Add(this.m_listBoxProjectListViewItems);
            this.m_groupBoxColors.Controls.Add(this.m_labelSample);
            this.m_groupBoxColors.Controls.Add(this.m_buttonSelectColor);
            this.m_groupBoxColors.Controls.Add(this.m_labelProjectItems);
            this.m_groupBoxColors.Location = new System.Drawing.Point(8, 8);
            this.m_groupBoxColors.Name = "m_groupBoxColors";
            this.m_groupBoxColors.Size = new System.Drawing.Size(322, 198);
            this.m_groupBoxColors.TabIndex = 0;
            this.m_groupBoxColors.TabStop = false;
            this.m_groupBoxColors.Text = "Highlightning";
            // 
            // m_listBoxProjectListViewItems
            // 
            this.m_listBoxProjectListViewItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_listBoxProjectListViewItems.Items.AddRange(new object[] {
                                                                               "Not modified project marked",
                                                                               "Not modified project not marked",
                                                                               "Modified project marked",
                                                                               "Modified project not marked",
                                                                               "Project with invalid version, marked",
                                                                               "Project with invalid version, not marked",
                                                                               "Project without version",
                                                                               "Project root",
                                                                               "Version changed (in batch command report)",
                                                                               "Version not modified (in batch command report)",
                                                                               "Version update failed (in batch command report)"});
            this.m_listBoxProjectListViewItems.Location = new System.Drawing.Point(16, 40);
            this.m_listBoxProjectListViewItems.Name = "m_listBoxProjectListViewItems";
            this.m_listBoxProjectListViewItems.Size = new System.Drawing.Size(290, 95);
            this.m_listBoxProjectListViewItems.TabIndex = 1;
            this.m_listBoxProjectListViewItems.SelectedIndexChanged += new System.EventHandler(this.m_listBoxProjectListViewItems_SelectedIndexChanged);
            // 
            // m_labelSample
            // 
            this.m_labelSample.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_labelSample.BackColor = System.Drawing.SystemColors.Window;
            this.m_labelSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_labelSample.Location = new System.Drawing.Point(16, 166);
            this.m_labelSample.Name = "m_labelSample";
            this.m_labelSample.Size = new System.Drawing.Size(176, 24);
            this.m_labelSample.TabIndex = 2;
            this.m_labelSample.Text = "Sample text";
            this.m_labelSample.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_buttonSelectColor
            // 
            this.m_buttonSelectColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonSelectColor.Location = new System.Drawing.Point(224, 166);
            this.m_buttonSelectColor.Name = "m_buttonSelectColor";
            this.m_buttonSelectColor.TabIndex = 3;
            this.m_buttonSelectColor.Text = "&Select...";
            this.m_buttonSelectColor.Click += new System.EventHandler(this.m_buttonSelectColor_Click);
            // 
            // m_labelProjectItems
            // 
            this.m_labelProjectItems.Location = new System.Drawing.Point(16, 22);
            this.m_labelProjectItems.Name = "m_labelProjectItems";
            this.m_labelProjectItems.Size = new System.Drawing.Size(136, 16);
            this.m_labelProjectItems.TabIndex = 0;
            this.m_labelProjectItems.Text = "&Project items:";
            // 
            // m_tabPageBatchCommands
            // 
            this.m_tabPageBatchCommands.Controls.Add(this.m_checkBoxFinalNotification);
            this.m_tabPageBatchCommands.Controls.Add(this.m_groupBoxBatchIncrementScheme);
            this.m_tabPageBatchCommands.Location = new System.Drawing.Point(4, 40);
            this.m_tabPageBatchCommands.Name = "m_tabPageBatchCommands";
            this.m_tabPageBatchCommands.Size = new System.Drawing.Size(338, 364);
            this.m_tabPageBatchCommands.TabIndex = 3;
            this.m_tabPageBatchCommands.Text = "Batch Commands";
            // 
            // m_checkBoxFinalNotification
            // 
            this.m_checkBoxFinalNotification.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_checkBoxFinalNotification.Location = new System.Drawing.Point(16, 192);
            this.m_checkBoxFinalNotification.Name = "m_checkBoxFinalNotification";
            this.m_checkBoxFinalNotification.Size = new System.Drawing.Size(312, 24);
            this.m_checkBoxFinalNotification.TabIndex = 1;
            this.m_checkBoxFinalNotification.Text = "&Display batch operation success dialog";
            // 
            // m_groupBoxBatchIncrementScheme
            // 
            this.m_groupBoxBatchIncrementScheme.Controls.Add(this.m_radioButtonIncrementAllIndependently);
            this.m_groupBoxBatchIncrementScheme.Controls.Add(this.m_radioButtonSynchronizeToHighestValue);
            this.m_groupBoxBatchIncrementScheme.Controls.Add(this.m_radioButtonIncrementAndSynchronize);
            this.m_groupBoxBatchIncrementScheme.Controls.Add(this.m_radioButtonIncrementModifiedIndependently);
            this.m_groupBoxBatchIncrementScheme.Location = new System.Drawing.Point(8, 8);
            this.m_groupBoxBatchIncrementScheme.Name = "m_groupBoxBatchIncrementScheme";
            this.m_groupBoxBatchIncrementScheme.Size = new System.Drawing.Size(320, 160);
            this.m_groupBoxBatchIncrementScheme.TabIndex = 0;
            this.m_groupBoxBatchIncrementScheme.TabStop = false;
            this.m_groupBoxBatchIncrementScheme.Text = "Increment Scheme";
            // 
            // m_radioButtonIncrementAllIndependently
            // 
            this.m_radioButtonIncrementAllIndependently.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioButtonIncrementAllIndependently.Location = new System.Drawing.Point(16, 56);
            this.m_radioButtonIncrementAllIndependently.Name = "m_radioButtonIncrementAllIndependently";
            this.m_radioButtonIncrementAllIndependently.Size = new System.Drawing.Size(296, 22);
            this.m_radioButtonIncrementAllIndependently.TabIndex = 1;
            this.m_radioButtonIncrementAllIndependently.Text = "Increment &all projects independently";
            // 
            // m_radioButtonSynchronizeToHighestValue
            // 
            this.m_radioButtonSynchronizeToHighestValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioButtonSynchronizeToHighestValue.Location = new System.Drawing.Point(16, 88);
            this.m_radioButtonSynchronizeToHighestValue.Name = "m_radioButtonSynchronizeToHighestValue";
            this.m_radioButtonSynchronizeToHighestValue.Size = new System.Drawing.Size(296, 22);
            this.m_radioButtonSynchronizeToHighestValue.TabIndex = 2;
            this.m_radioButtonSynchronizeToHighestValue.Text = "Synchronize projects to &highest version";
            // 
            // m_radioButtonIncrementAndSynchronize
            // 
            this.m_radioButtonIncrementAndSynchronize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioButtonIncrementAndSynchronize.Location = new System.Drawing.Point(16, 120);
            this.m_radioButtonIncrementAndSynchronize.Name = "m_radioButtonIncrementAndSynchronize";
            this.m_radioButtonIncrementAndSynchronize.Size = new System.Drawing.Size(296, 24);
            this.m_radioButtonIncrementAndSynchronize.TabIndex = 3;
            this.m_radioButtonIncrementAndSynchronize.Text = "Increment all projects and s&ynchronize";
            // 
            // m_radioButtonIncrementModifiedIndependently
            // 
            this.m_radioButtonIncrementModifiedIndependently.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_radioButtonIncrementModifiedIndependently.Checked = true;
            this.m_radioButtonIncrementModifiedIndependently.Location = new System.Drawing.Point(16, 24);
            this.m_radioButtonIncrementModifiedIndependently.Name = "m_radioButtonIncrementModifiedIndependently";
            this.m_radioButtonIncrementModifiedIndependently.Size = new System.Drawing.Size(296, 22);
            this.m_radioButtonIncrementModifiedIndependently.TabIndex = 0;
            this.m_radioButtonIncrementModifiedIndependently.TabStop = true;
            this.m_radioButtonIncrementModifiedIndependently.Text = "Increment &modified projects independently";
            // 
            // m_tabPageFolders
            // 
            this.m_tabPageFolders.Controls.Add(this.m_groupBoxSourceSafe);
            this.m_tabPageFolders.Controls.Add(this.m_groupBoxIis);
            this.m_tabPageFolders.Location = new System.Drawing.Point(4, 40);
            this.m_tabPageFolders.Name = "m_tabPageFolders";
            this.m_tabPageFolders.Size = new System.Drawing.Size(338, 364);
            this.m_tabPageFolders.TabIndex = 4;
            this.m_tabPageFolders.Text = "Folders";
            // 
            // m_groupBoxSourceSafe
            // 
            this.m_groupBoxSourceSafe.Controls.Add(this.m_buttonBrowseSourceSafe);
            this.m_groupBoxSourceSafe.Controls.Add(this.label2);
            this.m_groupBoxSourceSafe.Controls.Add(this.m_textBoxSourceSafePath);
            this.m_groupBoxSourceSafe.Controls.Add(this.m_checkBoxSourceSafeInstalled);
            this.m_groupBoxSourceSafe.Location = new System.Drawing.Point(8, 8);
            this.m_groupBoxSourceSafe.Name = "m_groupBoxSourceSafe";
            this.m_groupBoxSourceSafe.Size = new System.Drawing.Size(322, 96);
            this.m_groupBoxSourceSafe.TabIndex = 0;
            this.m_groupBoxSourceSafe.TabStop = false;
            this.m_groupBoxSourceSafe.Text = "Source Safe Command Line";
            // 
            // m_buttonBrowseSourceSafe
            // 
            this.m_buttonBrowseSourceSafe.Location = new System.Drawing.Point(280, 54);
            this.m_buttonBrowseSourceSafe.Name = "m_buttonBrowseSourceSafe";
            this.m_buttonBrowseSourceSafe.Size = new System.Drawing.Size(32, 22);
            this.m_buttonBrowseSourceSafe.TabIndex = 3;
            this.m_buttonBrowseSourceSafe.Text = "...";
            this.m_buttonBrowseSourceSafe.Click += new System.EventHandler(this.m_buttonBrowseSourceSafe_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "&Folder:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textBoxSourceSafePath
            // 
            this.m_textBoxSourceSafePath.Location = new System.Drawing.Point(64, 55);
            this.m_textBoxSourceSafePath.Name = "m_textBoxSourceSafePath";
            this.m_textBoxSourceSafePath.Size = new System.Drawing.Size(208, 20);
            this.m_textBoxSourceSafePath.TabIndex = 2;
            this.m_textBoxSourceSafePath.Text = "";
            // 
            // m_checkBoxSourceSafeInstalled
            // 
            this.m_checkBoxSourceSafeInstalled.Checked = true;
            this.m_checkBoxSourceSafeInstalled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_checkBoxSourceSafeInstalled.Location = new System.Drawing.Point(16, 24);
            this.m_checkBoxSourceSafeInstalled.Name = "m_checkBoxSourceSafeInstalled";
            this.m_checkBoxSourceSafeInstalled.Size = new System.Drawing.Size(160, 24);
            this.m_checkBoxSourceSafeInstalled.TabIndex = 0;
            this.m_checkBoxSourceSafeInstalled.Text = "&SourceSafe installed";
            this.m_checkBoxSourceSafeInstalled.CheckedChanged += new System.EventHandler(this.m_checkBoxSourceSafeInstalled_CheckedChanged);
            // 
            // m_groupBoxIis
            // 
            this.m_groupBoxIis.Controls.Add(this.m_buttonBrowseIisRoot);
            this.m_groupBoxIis.Controls.Add(this.label3);
            this.m_groupBoxIis.Controls.Add(this.m_textBoxIisRoot);
            this.m_groupBoxIis.Controls.Add(this.m_checkBoxIisInstalled);
            this.m_groupBoxIis.Location = new System.Drawing.Point(8, 120);
            this.m_groupBoxIis.Name = "m_groupBoxIis";
            this.m_groupBoxIis.Size = new System.Drawing.Size(322, 96);
            this.m_groupBoxIis.TabIndex = 1;
            this.m_groupBoxIis.TabStop = false;
            this.m_groupBoxIis.Text = "IIS";
            // 
            // m_buttonBrowseIisRoot
            // 
            this.m_buttonBrowseIisRoot.Location = new System.Drawing.Point(280, 54);
            this.m_buttonBrowseIisRoot.Name = "m_buttonBrowseIisRoot";
            this.m_buttonBrowseIisRoot.Size = new System.Drawing.Size(32, 22);
            this.m_buttonBrowseIisRoot.TabIndex = 3;
            this.m_buttonBrowseIisRoot.Text = "...";
            this.m_buttonBrowseIisRoot.Click += new System.EventHandler(this.m_buttonBrowseIisRoot_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 18);
            this.label3.TabIndex = 1;
            this.label3.Text = "&Root:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textBoxIisRoot
            // 
            this.m_textBoxIisRoot.Location = new System.Drawing.Point(64, 55);
            this.m_textBoxIisRoot.Name = "m_textBoxIisRoot";
            this.m_textBoxIisRoot.Size = new System.Drawing.Size(208, 20);
            this.m_textBoxIisRoot.TabIndex = 2;
            this.m_textBoxIisRoot.Text = "";
            // 
            // m_checkBoxIisInstalled
            // 
            this.m_checkBoxIisInstalled.Checked = true;
            this.m_checkBoxIisInstalled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_checkBoxIisInstalled.Location = new System.Drawing.Point(16, 24);
            this.m_checkBoxIisInstalled.Name = "m_checkBoxIisInstalled";
            this.m_checkBoxIisInstalled.Size = new System.Drawing.Size(160, 24);
            this.m_checkBoxIisInstalled.TabIndex = 0;
            this.m_checkBoxIisInstalled.Text = "&IIS installed";
            this.m_checkBoxIisInstalled.CheckedChanged += new System.EventHandler(this.m_checkBoxIisInstalled_CheckedChanged);
            // 
            // m_tabPageExport
            // 
            this.m_tabPageExport.Controls.Add(this.m_userControlExportOptions);
            this.m_tabPageExport.Location = new System.Drawing.Point(4, 40);
            this.m_tabPageExport.Name = "m_tabPageExport";
            this.m_tabPageExport.Size = new System.Drawing.Size(338, 364);
            this.m_tabPageExport.TabIndex = 5;
            this.m_tabPageExport.Text = "Export";
            // 
            // m_userControlExportOptions
            // 
            this.m_userControlExportOptions.Location = new System.Drawing.Point(8, 8);
            this.m_userControlExportOptions.Name = "m_userControlExportOptions";
            this.m_userControlExportOptions.Size = new System.Drawing.Size(320, 346);
            this.m_userControlExportOptions.TabIndex = 0;
            this.m_userControlExportOptions.TabStop = false;
            // 
            // ConfigurationForm
            // 
            this.AcceptButton = this.m_buttonOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_buttonCancel;
            this.ClientSize = new System.Drawing.Size(362, 471);
            this.Controls.Add(this.m_tabControlSettings);
            this.Controls.Add(this.m_buttonCancel);
            this.Controls.Add(this.m_buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "VCB Settings";
            this.m_panelNumberingStyle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDownIncrementBy)).EndInit();
            this.m_tabControlSettings.ResumeLayout(false);
            this.m_tabPageGeneral.ResumeLayout(false);
            this.m_tabPageNumberingScheme.ResumeLayout(false);
            this.m_groupBoxRestartBuildAndVersion.ResumeLayout(false);
            this.m_groupBoxResetRevision.ResumeLayout(false);
            this.m_groupBoxResetBuild.ResumeLayout(false);
            this.m_tabPageAppearance.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDownSubProjectIndentation)).EndInit();
            this.m_groupBoxColors.ResumeLayout(false);
            this.m_tabPageBatchCommands.ResumeLayout(false);
            this.m_groupBoxBatchIncrementScheme.ResumeLayout(false);
            this.m_tabPageFolders.ResumeLayout(false);
            this.m_groupBoxSourceSafe.ResumeLayout(false);
            this.m_groupBoxIis.ResumeLayout(false);
            this.m_tabPageExport.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Private properties

        private bool SaveFilesBeforeRunningAddinCommand {
            get { return m_checkBoxSaveFilesBeforeRunningAddinCommand.Checked; }
            set { m_checkBoxSaveFilesBeforeRunningAddinCommand.Checked = value; }
        }

        private AssemblyVersionType DefaultVersionType {
            get { 
                switch (m_comboBoxDefaulVersionAttribute.SelectedIndex) {
                case 0:
                    return AssemblyVersionType.AssemblyVersion;
                case 1:
                    return AssemblyVersionType.AssemblyFileVersion;
                case 2:
                    return AssemblyVersionType.AssemblyInformationalVersion;
                }
                return AssemblyVersionType.All;
            }
            set { 
                switch (value) {
                case AssemblyVersionType.AssemblyVersion:
                    m_comboBoxDefaulVersionAttribute.SelectedIndex = 0;
                    break;
                case AssemblyVersionType.AssemblyFileVersion:
                    m_comboBoxDefaulVersionAttribute.SelectedIndex = 1;
                    break;
                case AssemblyVersionType.AssemblyInformationalVersion:
                    m_comboBoxDefaulVersionAttribute.SelectedIndex = 2;
                    break;
                }
            }
        }

        private int IncrementBy {
            get { return (int)m_numericUpDownIncrementBy.Value; }
            set { m_numericUpDownIncrementBy.Value = value; }
        }

        private bool ApplyToAllTabs {
            get { return m_checkBoxApplyToAllTabs.Checked; }
            set { m_checkBoxApplyToAllTabs.Checked = value; }
        }

        private bool SynchronizeAllVersionTypes {
            get { return m_checkBoxSynchronizeAllTypes.Checked; }
            set { m_checkBoxSynchronizeAllTypes.Checked = value; }
        }

        private bool DontWarnInvalidInformationalVersion {
            get { return m_checkBoxDontWarnInvalidInformationalVersion.Checked; }
            set { m_checkBoxDontWarnInvalidInformationalVersion.Checked = value; }
        }

        private bool IncludeVCppResourceFiles {
            get { return m_checkBoxIncludeVcppResources.Checked; }
            set { m_checkBoxIncludeVcppResources.Checked = value; }
        }

        private bool IncludeSetupProjects {
            get { return m_checkBoxIncludeSetupProjects.Checked; }
            set { m_checkBoxIncludeSetupProjects.Checked = value; }
        }

        private bool GeneratePackageAndProductCodes {
            get { return m_checkBoxGenerateProductAndPackageCode.Checked; }
            set { m_checkBoxGenerateProductAndPackageCode.Checked = value; }
        }

        private IncrementScheme IncrementScheme {
            get { return (IncrementScheme)m_comboBoxIncrement.SelectedIndex; }
            set { m_comboBoxIncrement.SelectedIndex = (int)value; }
        }

        private BatchCommandIncrementScheme BatchCommandIncrementScheme {
            get {
                if (m_radioButtonIncrementModifiedIndependently.Checked)
                    return BatchCommandIncrementScheme.IncrementModifiedIndependently;
                if (m_radioButtonIncrementAllIndependently.Checked)
                    return BatchCommandIncrementScheme.IncrementAllIndependently;
                if (m_radioButtonSynchronizeToHighestValue.Checked)
                    return BatchCommandIncrementScheme.IncrementModifiedOnlyAndSynchronize;
                if (m_radioButtonIncrementAndSynchronize.Checked)
                    return BatchCommandIncrementScheme.IncrementAllAndSynchronize;
                Debug.Assert(false, "Not supported option");
                return BatchCommandIncrementScheme.IncrementModifiedIndependently;
            }
            set {
                switch (value) {
                case BatchCommandIncrementScheme.IncrementModifiedIndependently:
                    m_radioButtonIncrementModifiedIndependently.Checked = true;
                    break;
                case BatchCommandIncrementScheme.IncrementAllIndependently:
                    m_radioButtonIncrementAllIndependently.Checked = true;
                    break;
                case BatchCommandIncrementScheme.IncrementModifiedOnlyAndSynchronize:
                    m_radioButtonSynchronizeToHighestValue.Checked = true;
                    break;
                case BatchCommandIncrementScheme.IncrementAllAndSynchronize:
                    m_radioButtonIncrementAndSynchronize.Checked = true;
                    break;
                default:
                    Debug.Assert(false, "Not supported option");
                    break;
                }
            }
        }

        private bool ShowSuccessDialog {
            get { return m_checkBoxFinalNotification.Checked; }
            set { m_checkBoxFinalNotification.Checked = value; }
        }

        private bool UseDateTimeBasedBuildAndRevisionNumbering {
            get { return m_radioButtonMicrosoftScheme.Checked; }
            set { m_radioButtonMicrosoftScheme.Checked = value; }
        }

        private bool ResetBuildOnMajor {
            get { return m_checkBoxResetBuildOnMajor.Checked; }
            set { m_checkBoxResetBuildOnMajor.Checked = value; }
        }

        private bool ResetBuildOnMinor {
            get { return m_checkBoxResetBuildOnMinor.Checked; }
            set { m_checkBoxResetBuildOnMinor.Checked = value; }
        }

        private bool ResetRevisionOnMajor {
            get { return m_checkBoxResetRevisionOnMajor.Checked; }
            set { m_checkBoxResetRevisionOnMajor.Checked = value; }
        }

        private bool ResetRevisionOnMinor {
            get { return m_checkBoxResetRevisionOnMinor.Checked; }
            set { m_checkBoxResetRevisionOnMinor.Checked = value; }
        }

        private bool ResetRevisionOnBuild {
            get { return m_checkBoxResetRevisionOnBuild.Checked; }
            set { m_checkBoxResetRevisionOnBuild.Checked = value; }
        }

        private ResetBuildAndRevision ResetBuildAndRevisionTo {
            get { return m_radioButtonResetTo0.Checked ? ResetBuildAndRevision.ToZero : ResetBuildAndRevision.ToOne; }
            set { 
                m_radioButtonResetTo0.Checked = value == ResetBuildAndRevision.ToZero; 
                m_radioButtonResetTo1.Checked = value == ResetBuildAndRevision.ToOne; 
            }
        }

        private bool ReplaceAsteriskWithComponentVersions {
            get { return m_checkBoxReplaceWildcards.Checked; }
            set { m_checkBoxReplaceWildcards.Checked = value; }
        }

        private bool IndentSubProjectItems {
            get { return m_checkBoxIndentSubProjects.Checked; }
            set { m_checkBoxIndentSubProjects.Checked = value; }
        }

        private int SubProjectsIndentation {
            get { return (int)m_numericUpDownSubProjectIndentation.Value; }
            set { m_numericUpDownSubProjectIndentation.Value = value; }
        }

        private bool ShowSubProjectRoots {
            get { return m_checkBoxShowSubProjectRoot.Checked; }
            set { 
                m_checkBoxShowSubProjectRoot.Checked = value; 
                EnableShowEmptySubfolderCheckbox();
            }
        }

        private bool ShowEnterpriseTemplateProjectRoot {
            get { return m_checkBoxShowEnterpriseTemplateRoot.Checked; }
            set { 
                m_checkBoxShowEnterpriseTemplateRoot.Checked = value; 
                EnableShowEmptySubfolderCheckbox();
            }
        }

        private bool ShowEmptyFolders {
            get { return m_checkBoxShowEmptyFolder.Checked; }
            set { m_checkBoxShowEmptyFolder.Checked = value; }
        }

        private bool ShowNonVersionableProjects {
            get { return m_checkBoxShowNonVersionableProjects.Checked; }
            set { m_checkBoxShowNonVersionableProjects.Checked = value; }
        }

        private FolderConfiguration SourceSafeFolderConfiguration {
            get {
                FolderConfiguration fc = new FolderConfiguration();
                fc.IsAvailable = m_checkBoxSourceSafeInstalled.Checked;
                fc.Folder = m_textBoxSourceSafePath.Text;
                return fc; 
            }
            set {
                m_checkBoxSourceSafeInstalled.Checked = value.IsAvailable;
                m_textBoxSourceSafePath.Text = value.Folder;
            }
        }
        
        private FolderConfiguration IisFolderConfiguration {
            get {
                FolderConfiguration fc = new FolderConfiguration();
                fc.IsAvailable = m_checkBoxIisInstalled.Checked;
                fc.Folder = m_textBoxIisRoot.Text;
                return fc; 
            }
            set {
                m_checkBoxIisInstalled.Checked = value.IsAvailable;
                m_textBoxIisRoot.Text = value.Folder;
            }
        }

        private ExportConfiguration ExportConfiguration {
            get {
                return m_userControlExportOptions.ExportConfiguration;
            }
            set {
                m_userControlExportOptions.ExportConfiguration = value;
            }
        }
        
        #endregion // Private properties

        #region Control event handlers 

        private void m_radioButtonMicrosoftScheme_CheckedChanged(object sender, System.EventArgs e) {
            m_comboBoxIncrement.Enabled             = !m_radioButtonMicrosoftScheme.Checked;
            m_numericUpDownIncrementBy.Enabled      = !m_radioButtonMicrosoftScheme.Checked;
            m_checkBoxResetBuildOnMajor.Enabled     = !m_radioButtonMicrosoftScheme.Checked;
            m_checkBoxResetBuildOnMinor.Enabled     = !m_radioButtonMicrosoftScheme.Checked;
            m_checkBoxResetRevisionOnMajor.Enabled  = !m_radioButtonMicrosoftScheme.Checked;
            m_checkBoxResetRevisionOnMinor.Enabled  = !m_radioButtonMicrosoftScheme.Checked;
            m_checkBoxResetRevisionOnBuild.Enabled  = !m_radioButtonMicrosoftScheme.Checked;
            m_radioButtonResetTo0.Enabled           = !m_radioButtonMicrosoftScheme.Checked;
            m_radioButtonResetTo1.Enabled           = !m_radioButtonMicrosoftScheme.Checked;
        }

        private void OnColorSelected(object sender, ColorSelectedEventArgs e) {
            Debug.Assert(m_configuration.DisplayOptions.Colors != null);
            switch (m_listBoxProjectListViewItems.SelectedIndex) {
            case (int)ProjectListViewItemTypes.NotModifiedMarked:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.NotModifiedMarked = e.ColorSelected;
                break;
            case (int)ProjectListViewItemTypes.NotModifiedNotMarked:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.NotModifiedNotMarked = e.ColorSelected;
                break;
            case (int)ProjectListViewItemTypes.ModifiedMarked:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.ModifiedMarked = e.ColorSelected;
                break;
            case (int)ProjectListViewItemTypes.ModifiedNotMarked:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.ModifiedNotMarked = e.ColorSelected;
                break;
            case (int)ProjectListViewItemTypes.IllegalVersionMarked:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.InvalidVersionMarked = e.ColorSelected;
                break;
            case (int)ProjectListViewItemTypes.IllegalVersionUnmarked:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.InvalidVersionNotMarked = e.ColorSelected;
                break;
            case (int)ProjectListViewItemTypes.NoVersion:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.NoVersion = e.ColorSelected;
                break;
            case (int)ProjectListViewItemTypes.SubProjectRoot:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.SubProjectRoot = e.ColorSelected;
                break;
            case (int)ProjectListViewItemTypes.ReportVersionUpdated:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.ReportUpdatedVersion = e.ColorSelected;
                break;
            case (int)ProjectListViewItemTypes.ReportNoVersionChange:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.ReportVersionNotChanged = e.ColorSelected;
                break;
            case (int)ProjectListViewItemTypes.ReportVersionUpdateFailed:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.ReportVersionUpdateFailed = e.ColorSelected;
                break;
            default:
                Debug.Assert(false, "Not supported color configuration.");
                break;
            }
        }

        private void m_listBoxProjectListViewItems_SelectedIndexChanged(object sender, System.EventArgs e) {
            Debug.Assert(m_configuration.DisplayOptions.Colors != null);
            Debug.Assert(m_listBoxProjectListViewItems.SelectedIndex < Enum.GetValues(typeof(ProjectListViewItemTypes)).Length);
            switch (m_listBoxProjectListViewItems.SelectedIndex) {
            case (int)ProjectListViewItemTypes.NotModifiedMarked:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.NotModifiedMarked;
                break;
            case (int)ProjectListViewItemTypes.NotModifiedNotMarked:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.NotModifiedNotMarked;
                break;
            case (int)ProjectListViewItemTypes.ModifiedMarked:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.ModifiedMarked;
                break;
            case (int)ProjectListViewItemTypes.ModifiedNotMarked:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.ModifiedNotMarked;
                break;
            case (int)ProjectListViewItemTypes.IllegalVersionMarked:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.InvalidVersionMarked;
                break;
            case (int)ProjectListViewItemTypes.IllegalVersionUnmarked:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.InvalidVersionNotMarked;
                break;
            case (int)ProjectListViewItemTypes.NoVersion:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.NoVersion;
                break;
            case (int)ProjectListViewItemTypes.SubProjectRoot:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.SubProjectRoot;
                break;
            case (int)ProjectListViewItemTypes.ReportVersionUpdated:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.ReportUpdatedVersion;
                break;
            case (int)ProjectListViewItemTypes.ReportNoVersionChange:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.ReportVersionNotChanged;
                break;
            case (int)ProjectListViewItemTypes.ReportVersionUpdateFailed:
                m_labelSample.ForeColor = m_configuration.DisplayOptions.Colors.ReportVersionUpdateFailed = m_configuration.DisplayOptions.Colors.ReportVersionUpdateFailed;
                break;
            default:
                Debug.Assert(false, "Not supported color configuration.");
                break;
            }
        }

        private void m_buttonSelectColor_Click(object sender, System.EventArgs e) {
            m_colorsForm.Color = m_labelSample.ForeColor;
            // places the window right to the button
            m_colorsForm.Location = PointToScreen(new Point(m_tabControlSettings.Left + m_tabPageAppearance.Left + m_groupBoxColors.Left + m_buttonSelectColor.Right, m_tabControlSettings.Top + m_tabPageAppearance.Top + m_groupBoxColors.Top + m_buttonSelectColor.Top /*- m_colorsForm.ClientRectangle.Height*/));
            if (!m_colorsForm.Visible)
                m_colorsForm.Show();
        }

        private void m_checkBoxSynchronizeAllTypes_CheckedChanged(object sender, System.EventArgs e) {
            if (m_checkBoxSynchronizeAllTypes.Checked)
                m_checkBoxApplyToAllTabs.Checked = true;;
            m_checkBoxApplyToAllTabs.Enabled = !m_checkBoxSynchronizeAllTypes.Checked;
        }

        private void m_checkBoxIndentSubProjects_CheckedChanged(object sender, System.EventArgs e) {
            m_numericUpDownSubProjectIndentation.Enabled = m_checkBoxIndentSubProjects.Checked;
        }

        private void m_checkBoxIncludeSetupProjects_CheckedChanged(object sender, System.EventArgs e) {
            m_checkBoxGenerateProductAndPackageCode.Enabled = m_checkBoxIncludeSetupProjects.Checked;
        }

        private void m_checkBoxShowSubProjectRoot_CheckedChanged(object sender, System.EventArgs e) {
            EnableShowEmptySubfolderCheckbox();
        }

        private void m_checkBoxShowEnterpriseTemplateRoot_CheckedChanged(object sender, System.EventArgs e) {
            EnableShowEmptySubfolderCheckbox();
        }

        private void EnableShowEmptySubfolderCheckbox() {
            m_checkBoxShowEmptyFolder.Enabled = m_checkBoxShowSubProjectRoot.Checked || m_checkBoxShowEnterpriseTemplateRoot.Checked;
        }

        private void m_checkBoxSourceSafeInstalled_CheckedChanged(object sender, System.EventArgs e) {
            m_textBoxSourceSafePath.Enabled = m_checkBoxSourceSafeInstalled.Checked;
            m_buttonBrowseSourceSafe.Enabled = m_checkBoxSourceSafeInstalled.Checked;
        }

        private void m_checkBoxIisInstalled_CheckedChanged(object sender, System.EventArgs e) {
            m_textBoxIisRoot.Enabled = m_checkBoxIisInstalled.Checked;
            m_buttonBrowseIisRoot.Enabled = m_checkBoxIisInstalled.Checked;
        }

        private void m_buttonBrowseSourceSafe_Click(object sender, System.EventArgs e) {
            string path = m_textBoxSourceSafePath.Text;
            if (!Path.IsPathRooted(path)) {
                path = SourceSafeLocator.Instance.SourceSafeRoot;
            }
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = path;
            dlg.FileName = "ss.exe";
            dlg.Filter = m_txtSourceSafeFolderFilter;
            dlg.Title = m_txtSourceSafeFolderTitle;
            if (dlg.ShowDialog() == DialogResult.OK) {
                m_textBoxSourceSafePath.Text = Path.GetDirectoryName(dlg.FileName);
            }
        }

        private void m_buttonBrowseIisRoot_Click(object sender, System.EventArgs e) {
            string path = m_textBoxIisRoot.Text;
            if (!Path.IsPathRooted(path) || !Directory.Exists(path)) {
                if (InetRootLocator.Instance.IsIisAvailable)
                    path = InetRootLocator.Instance.PathWwwRoot;
                else
                    path = string.Empty;
            }
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = path;
            dlg.Description = m_txtSourceIisFolderDescription;
            dlg.ShowNewFolderButton = false;
            if (dlg.ShowDialog() == DialogResult.OK) {
                m_textBoxIisRoot.Text = dlg.SelectedPath;
            }
        }
        
        #endregion // Control event handlers

        #region Private methods

        private void SetControlValues() {
            SaveFilesBeforeRunningAddinCommand          = m_configuration.NumberingOptions.SaveModifiedFilesBeforeRunningAddinCommand;
            DefaultVersionType                          = m_configuration.NumberingOptions.DefaultVersionType;
            ApplyToAllTabs                              = m_configuration.NumberingOptions.ApplyToAllTypes;
            IncrementBy                                 = m_configuration.NumberingOptions.IncrementBy;
            SynchronizeAllVersionTypes                  = m_configuration.NumberingOptions.SynchronizeAllVersionTypes;
            DontWarnInvalidInformationalVersion         = m_configuration.NumberingOptions.AllowArbitraryInformationalVersion;
            IncludeVCppResourceFiles                    = m_configuration.NumberingOptions.IncludeVCppResourceFiles;
            IncludeSetupProjects                        = m_configuration.NumberingOptions.IncludeSetupProjects;
            GeneratePackageAndProductCodes              = m_configuration.NumberingOptions.GeneratePackageAndProductCodes;
            BatchCommandIncrementScheme                 = m_configuration.NumberingOptions.BatchCommandIncrementScheme;
            IncrementScheme                             = m_configuration.NumberingOptions.IncrementScheme;
            UseDateTimeBasedBuildAndRevisionNumbering   = m_configuration.NumberingOptions.UseDateTimeBasedBuildAndRevisionNumbering;
            ResetBuildOnMajor                           = m_configuration.NumberingOptions.ResetBuildOnMajorIncrement;
            ResetBuildOnMinor                           = m_configuration.NumberingOptions.ResetBuildOnMinorIncrement;
            ResetRevisionOnMajor                        = m_configuration.NumberingOptions.ResetRevisionOnMajorIncrement;
            ResetRevisionOnMinor                        = m_configuration.NumberingOptions.ResetRevisionOnMinorIncrement;
            ResetRevisionOnBuild                        = m_configuration.NumberingOptions.ResetRevisionOnBuildIncrement;
            ResetBuildAndRevisionTo                     = m_configuration.NumberingOptions.ResetBuildAndRevisionTo;
            ReplaceAsteriskWithComponentVersions        = m_configuration.NumberingOptions.ReplaceAsteriskWithVersionComponents;
            IndentSubProjectItems                       = m_configuration.DisplayOptions.IndentSubProjectItems;
            SubProjectsIndentation                      = m_configuration.DisplayOptions.SubProjectIndentation;
            ShowSubProjectRoots                         = m_configuration.DisplayOptions.ShowSubProjectRoot;
            ShowEnterpriseTemplateProjectRoot           = m_configuration.DisplayOptions.ShowEnterpriseTemplateProjectRoot;
            ShowNonVersionableProjects                  = m_configuration.DisplayOptions.ShowNonVersionableProjects;
            ShowSuccessDialog                           = m_configuration.DisplayOptions.ShowSuccessDialog;
            ShowEmptyFolders                            = m_configuration.DisplayOptions.ShowEmptyFolders;
            SourceSafeFolderConfiguration               = m_configuration.FoldersConfigurations.SourceSafeFolder;
            IisFolderConfiguration                      = m_configuration.FoldersConfigurations.IisFolder;
            ExportConfiguration                         = m_configuration.ExportConfiguration;
        }

        private void GetControlValues() {
            m_configuration.NumberingOptions.SaveModifiedFilesBeforeRunningAddinCommand = SaveFilesBeforeRunningAddinCommand;
            m_configuration.NumberingOptions.DefaultVersionType                         = DefaultVersionType;
            m_configuration.NumberingOptions.ApplyToAllTypes                            = ApplyToAllTabs;
            m_configuration.NumberingOptions.IncrementBy                                = IncrementBy;
            m_configuration.NumberingOptions.AllowArbitraryInformationalVersion         = DontWarnInvalidInformationalVersion;
            m_configuration.NumberingOptions.SynchronizeAllVersionTypes                 = SynchronizeAllVersionTypes;
            m_configuration.NumberingOptions.IncludeVCppResourceFiles                   = IncludeVCppResourceFiles;
            m_configuration.NumberingOptions.IncludeSetupProjects                       = IncludeSetupProjects;
            m_configuration.NumberingOptions.GeneratePackageAndProductCodes             = GeneratePackageAndProductCodes;
            m_configuration.NumberingOptions.BatchCommandIncrementScheme                = BatchCommandIncrementScheme;
            m_configuration.NumberingOptions.IncrementScheme                            = IncrementScheme;
            m_configuration.NumberingOptions.UseDateTimeBasedBuildAndRevisionNumbering  = UseDateTimeBasedBuildAndRevisionNumbering;
            m_configuration.NumberingOptions.ResetBuildOnMajorIncrement                 = ResetBuildOnMajor;
            m_configuration.NumberingOptions.ResetBuildOnMinorIncrement                 = ResetBuildOnMinor;
            m_configuration.NumberingOptions.ResetRevisionOnMajorIncrement              = ResetRevisionOnMajor;
            m_configuration.NumberingOptions.ResetRevisionOnMinorIncrement              = ResetRevisionOnMinor;
            m_configuration.NumberingOptions.ResetRevisionOnBuildIncrement              = ResetRevisionOnBuild;
            m_configuration.NumberingOptions.ResetBuildAndRevisionTo                    = ResetBuildAndRevisionTo;
            m_configuration.NumberingOptions.ReplaceAsteriskWithVersionComponents       = ReplaceAsteriskWithComponentVersions;
            m_configuration.DisplayOptions.IndentSubProjectItems                        = IndentSubProjectItems;
            m_configuration.DisplayOptions.SubProjectIndentation                        = SubProjectsIndentation;
            m_configuration.DisplayOptions.ShowSubProjectRoot                           = ShowSubProjectRoots;
            m_configuration.DisplayOptions.ShowEnterpriseTemplateProjectRoot            = ShowEnterpriseTemplateProjectRoot;
            m_configuration.DisplayOptions.ShowNonVersionableProjects                   = ShowNonVersionableProjects;
            m_configuration.DisplayOptions.ShowSuccessDialog                            = ShowSuccessDialog;
            m_configuration.DisplayOptions.ShowEmptyFolders                             = ShowEmptyFolders;
            m_configuration.FoldersConfigurations.SourceSafeFolder                      = SourceSafeFolderConfiguration;
            m_configuration.FoldersConfigurations.IisFolder                             = IisFolderConfiguration;
            m_configuration.ExportConfiguration                                         = ExportConfiguration;
        }

        #endregion // private methods

        #region Private fields

        private VcbConfiguration m_configuration = new VcbConfiguration();

        private ColorsForm m_colorsForm = new ColorsForm();

        #endregion // Private fields

        #region Static constructor and fields

        static ConfigurationForm() {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager("BuildAutoIncrement.Resources.Shared", typeof(ConfigurationForm).Assembly);
            Debug.Assert(resources != null);

            m_txtSourceSafeFolderFilter = resources.GetString("SoursceSafe exe Filter");
            Debug.Assert(m_txtSourceSafeFolderFilter != null);

            m_txtSourceSafeFolderTitle = resources.GetString("Select SourceSafe Folder caption");
            Debug.Assert(m_txtSourceSafeFolderTitle != null);

            m_txtSourceIisFolderDescription = resources.GetString("Select IIS Root description");
            Debug.Assert(m_txtSourceIisFolderDescription != null);

            //m_txtFileFilterCannotContainCharacters = resources.GetString("File filter cannot contain any of the characters");
            //Debug.Assert(m_txtFileFilterCannotContainCharacters != null);
        }

        private static readonly string m_txtSourceSafeFolderFilter;
        private static readonly string m_txtSourceSafeFolderTitle;
        private static readonly string m_txtSourceIisFolderDescription;

        private void m_buttonOK_Click(object sender, System.EventArgs e) {
        
        }

        //private static string m_txtFileFilterCannotContainCharacters;

        #endregion // Static constructor and fields
    }
}