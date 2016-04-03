/*
 * Filename:    AssemblyInfoListViewsControl.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: User control consisting of three tabbed ProjectListView controls 
 *              and buttons used to activate corresponding listview.
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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BuildAutoIncrement {
	/// <summary>
	///   Tab control alike user control with three <c>ProjectsListView</c>s 
	///   and corresponding selection buttons. <c>ProjectsListView</c>s contain
	///   values of <c>AssemblyVersion</c>, <c>AssemblyFileVersion</c> and 
	///   <c>AssemblyInformationalVersion</c> (i.e. Product version) attributes 
	///   for projects.
	/// </summary>
	internal class AssemblyInfoListViewsControl : System.Windows.Forms.UserControl {
        
        /// <summary>
        ///   Event raised when selection in listview has changed.
        /// </summary>
        public event EventHandler SelectedIndexChanged;

        /// <summary>
        ///   Event raised when an item in the listview is checked.
        /// </summary>
        public event ItemCheckEventHandler ItemCheck;

        /// <summary>
        ///   Event raised when listview shown is changed.
        /// </summary>
        public event EventHandler SelectedTabIndexChanged;

        #region Controls

        private System.Windows.Forms.Panel m_panelSelectionButtons;
        private System.Windows.Forms.RadioButton m_radioButtonAssemblyVersions;
        private System.Windows.Forms.RadioButton m_radioButtonAssemblyFileVersions;
        private System.Windows.Forms.RadioButton m_radioButtonAssemblyInformationalVersions;

        private System.Windows.Forms.Panel m_panelListViews;
        private BuildAutoIncrement.ProjectsListView m_listViewAssemblyVersions;
        private BuildAutoIncrement.ProjectsListView m_listViewFileVersions;
        private BuildAutoIncrement.ProjectsListView m_listViewInformationalVersions;

        private System.Windows.Forms.ImageList m_imageListProjectIcons;

        #endregion // Controls

        private System.ComponentModel.IContainer components;

		/// <summary>
		///   Initializes <c>AssemblyInfoListViewsControl</c>.
		/// </summary>
        public AssemblyInfoListViewsControl() {
			InitializeComponent();
            SetupListViews();
            InitializeFields();
        }

        #region Public properties

        /// <summary>
        ///   Gets a flag indicating if currently selected item is the only one 
        ///   and if it contains a valid version.
        /// </summary>
        public bool IsSingleSelectionWithValidAssemblyVersion {
            get {
                return SelectedListView.IsSingleSelectionWithValidAssemblyVersion;
            }
        }

        /// <summary>
        ///   Gets the number of items in the listview. Note that all listviews 
        ///   should contain the same number of items.
        /// </summary>
        public int ItemsCount {
            get { 
                return SelectedListView.Items.Count; 
            }
        }

        /// <summary>
        ///   Gets or sets column widths of listviews.
        /// </summary>
        public int[] ListViewColumnWidths {
            get {
                return GetSelectedListViewColumnWidths();
            }
            set {
                SetAllListViewColumnWidths(value);
            }
        }

        #endregion // Public properties

        /// <summary>
        ///   Gets the selected version.
        /// </summary>
        /// <returns>
        ///   <c>ProjectVersion</c> object currently selected.
        /// </returns>
        public ProjectVersion SelectedVersion {
            get {
                return SelectedListView.GetSelectedVersion();
            }
        }

        public void BeginUpdate() {
            m_listViewAssemblyVersions.BeginUpdate();
            m_listViewFileVersions.BeginUpdate();
            m_listViewInformationalVersions.BeginUpdate();
        }

        public void EndUpdate() {
            m_listViewAssemblyVersions.EndUpdate();
            m_listViewFileVersions.EndUpdate();
            m_listViewInformationalVersions.EndUpdate();
        }

        /// <summary>
        ///   Selects the listview with corresponding assembly version type.
        /// </summary>
        /// <param name="assemblyVersionType">
        ///   <c>AssemblyVersionType</c> for which listview should be selected.
        /// </param>
        public void SelectListView(AssemblyVersionType assemblyVersionType) {
            switch (assemblyVersionType) {
            case AssemblyVersionType.AssemblyVersion:
                m_radioButtonAssemblyVersions.PerformClick();
                break;
            case AssemblyVersionType.AssemblyFileVersion:
                m_radioButtonAssemblyFileVersions.PerformClick();
                break;
            case AssemblyVersionType.AssemblyInformationalVersion:
                m_radioButtonAssemblyInformationalVersions.PerformClick();
                break;
            }
        }

        public void SelectNextListView(bool forward) {
            Debug.Assert(m_radioButtonAssemblyVersions != null && m_radioButtonAssemblyFileVersions != null && m_radioButtonAssemblyInformationalVersions != null);
            Debug.Assert(m_radioButtonAssemblyVersions.Checked || m_radioButtonAssemblyFileVersions.Checked || m_radioButtonAssemblyInformationalVersions.Checked);
            if (forward) {
                if (m_radioButtonAssemblyVersions.Checked)
                    m_radioButtonAssemblyFileVersions.PerformClick();
                else if (m_radioButtonAssemblyFileVersions.Checked)
                    m_radioButtonAssemblyInformationalVersions.PerformClick();
                else if (m_radioButtonAssemblyInformationalVersions.Checked)
                    m_radioButtonAssemblyVersions.PerformClick();
            }
            else {
                if (m_radioButtonAssemblyVersions.Checked)
                    m_radioButtonAssemblyInformationalVersions.PerformClick();
                else if (m_radioButtonAssemblyInformationalVersions.Checked)
                    m_radioButtonAssemblyFileVersions.PerformClick();
                else if (m_radioButtonAssemblyFileVersions.Checked)
                    m_radioButtonAssemblyVersions.PerformClick();
            }
        }

        public bool HaveAllListViewItemsLowerCurrentVersion(bool includeNotMarked, string referenceVersion, bool allListViews) {
            if (allListViews) {
                foreach (ProjectsListView listView in m_listViews) {
                    if (!listView.HaveAllListViewItemsLowerCurrentVersion(includeNotMarked, referenceVersion))
                        return false;
                }
                return true;
            }
            else {
                return SelectedListView.HaveAllListViewItemsLowerCurrentVersion(includeNotMarked, referenceVersion);
            }
        }

        private class UniqueArrayList : ArrayList {

            public override void AddRange(ICollection c) {
                foreach (object obj in c) {
                    Add(obj);
                }
            }

            public override int Add(object value) {
                if (this.Contains(value))
                    return -1;
                return base.Add(value);
            }
        }

        public ProjectInfo[] GetMarkedProjectsInformation(bool allListViews) {
            UniqueArrayList markedProjectsInforamtion = new UniqueArrayList();
            if (allListViews) {
                foreach (ProjectsListView listView in m_listViews) {
                    markedProjectsInforamtion.AddRange(listView.MarkedProjectInfos);
                }
            }
            else {
                markedProjectsInforamtion.AddRange(SelectedListView.MarkedProjectInfos);
            }
            return (ProjectInfo[])markedProjectsInforamtion.ToArray(typeof(ProjectInfo));
        }

        #region Methods that iterate through ProjectsListViews using delegates

        public void FillProjectsListView(ProjectInfoList projectInfoList) {
            m_disableCheckSynchronization = true;
            ProposeToBeVersion(projectInfoList.HighestToBeAssemblyVersions.HighestProjectVersion.ToString(true));
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.FillListView), true, projectInfoList.ProjectInfos);
            m_disableCheckSynchronization = false;
        }

        public void MarkAllProjects(bool allListViews) {
            m_disableCheckSynchronization = true;
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.MarkAllProjects), allListViews, null);
            m_disableCheckSynchronization = false;
        }

        public void UnmarkAllProjects(bool allListViews) {
            m_disableCheckSynchronization = true;
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.UnmarkAllProjects), allListViews, null);
            m_disableCheckSynchronization = false;
        }

        public void InvertProjectMarks(bool allListViews) {
            m_disableCheckSynchronization = true;
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.InvertProjectMarks), allListViews, null);
            m_disableCheckSynchronization = false;
        }

        public void ResetVersions(bool allListViews) {
            m_disableCheckSynchronization = true;
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.ResetVersions), allListViews, null);
            m_disableCheckSynchronization = false;
        }

        public void ResetListView(bool allListViews) {
            m_disableCheckSynchronization = true;
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.ResetListView), allListViews, null);
            m_disableCheckSynchronization = false;
        }

        public void ResetMarks(bool allListViews) {
            m_disableCheckSynchronization = true;
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.ResetMarks), allListViews, null);
            m_disableCheckSynchronization = false;
        }

        public void SaveVersions(bool allListViews) {
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.SaveVersions), allListViews, null);
        }

        /// <summary>
        ///   Sets "to be version" used as a reference for items coloring
        /// </summary>
        /// <param name="versionPattern"></param>
        public void ProposeToBeVersion(string versionPattern) {
            Debug.Assert(versionPattern != null && versionPattern.Length > 0);
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.ProposeToBeVersion), true, versionPattern);
        }

        public void ApplyVersion(string versionPattern, bool allListViews) {
            Debug.Assert(versionPattern != null && versionPattern.Length > 0);
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.ApplyVersion), allListViews, versionPattern);
        }

        public void IncrementVersions(ProjectVersion.VersionComponent toIncrement, bool allListViews) {
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.IncrementVersions), allListViews, toIncrement);
        }

        public int GetValidVersionsCount(bool allListViews) {
            return ProcessListViewsReturningInt(new ProcessProjectsListViewReturningIntDelegate(ProjectsListView.GetValidVersionsCount), allListViews);
        }

        public int GetMarkedItemsCount(bool allListViews) {
            return ProcessListViewsReturningInt(new ProcessProjectsListViewReturningIntDelegate(ProjectsListView.GetMarkedItemsCount), allListViews);
        }

        public ProjectVersion GetHighestToBecomeVersion(bool allListViews) {
            return ProcessListViewsReturningHighestProjectVersion(new ProcessProjectsListViewReturningProjectVersionDelegate(ProjectsListView.GetHighestToBecomeVersion), allListViews, null);
        }

        public ProjectVersion GetHighestMarkedVersion(bool allListViews) {
            return ProcessListViewsReturningHighestProjectVersion(new ProcessProjectsListViewReturningProjectVersionDelegate(ProjectsListView.GetHighestMarkedVersion), allListViews, null);
        }

        public void SynchronizeAllVersions(bool includeProjectsNotForUpdate, bool allListViews) {
            m_disableCheckSynchronization = true;
            ProjectVersion highestToUpdateVersion = ProcessListViewsReturningHighestProjectVersion(new ProcessProjectsListViewReturningProjectVersionDelegate(ProjectsListView.GetHighestToUpdateVersion), allListViews, includeProjectsNotForUpdate);
            if (!includeProjectsNotForUpdate && highestToUpdateVersion.ContainsWildCard())
                highestToUpdateVersion = ProcessListViewsReturningHighestProjectVersion(new ProcessProjectsListViewReturningProjectVersionDelegate(ProjectsListView.GetHighestToUpdateVersion), allListViews, true);
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.MarkProjectsWithLowerVersion), allListViews, highestToUpdateVersion);
            ProcessListViews(new ProcessProjectsListViewDelegate(ProjectsListView.ApplyVersion), allListViews, highestToUpdateVersion.ToString(true));
            m_disableCheckSynchronization = false;
        }

        #endregion // Methods that iterate through ProjectsListViews using delegates

        #region Overriden and virtual methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///   Handler invoked when selection changes in one of the listviews.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSelectedIndexChanged(object sender, EventArgs e) {
            if (SelectedIndexChanged != null) {
                SelectedIndexChanged(sender, e);
            }
        }

        /// <summary>
        ///   Handler invoked when item check changes. Called when checking and
        ///   unchecking must be synchronized between listviews.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnItemCheck(object sender, ItemCheckEventArgs ice) {
            ProjectsListView senderListView = sender as ProjectsListView;
            Debug.Assert(senderListView != null);
            if (!m_disableCheckSynchronization) {
                if (ConfigurationPersister.Instance.Configuration.NumberingOptions.ApplyToAllTypes) {
                    m_disableCheckSynchronization = true;
                    if (ice.NewValue != ice.CurrentValue) {
                        foreach (ProjectsListView listView in m_listViews) {
                            if (senderListView != listView) {
                                listView.DoSafeCheck(ice.Index, ice.NewValue);
                            }
                        }
                    }
                    m_disableCheckSynchronization = false;
                }
            }
            if (ItemCheck != null) {
                ItemCheck(sender, ice);
            }
        }

        #endregion // Overriden methods

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AssemblyInfoListViewsControl));
			this.m_panelSelectionButtons = new System.Windows.Forms.Panel();
			this.m_radioButtonAssemblyVersions = new System.Windows.Forms.RadioButton();
			this.m_radioButtonAssemblyFileVersions = new System.Windows.Forms.RadioButton();
			this.m_radioButtonAssemblyInformationalVersions = new System.Windows.Forms.RadioButton();
			this.m_panelListViews = new System.Windows.Forms.Panel();
			this.m_listViewAssemblyVersions = new BuildAutoIncrement.ProjectsListView();
			this.m_imageListProjectIcons = new System.Windows.Forms.ImageList(this.components);
			this.m_listViewFileVersions = new BuildAutoIncrement.ProjectsListView();
			this.m_listViewInformationalVersions = new BuildAutoIncrement.ProjectsListView();
			this.m_panelSelectionButtons.SuspendLayout();
			this.m_panelListViews.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_panelSelectionButtons
			// 
			this.m_panelSelectionButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_panelSelectionButtons.Controls.Add(this.m_radioButtonAssemblyVersions);
			this.m_panelSelectionButtons.Controls.Add(this.m_radioButtonAssemblyFileVersions);
			this.m_panelSelectionButtons.Controls.Add(this.m_radioButtonAssemblyInformationalVersions);
			this.m_panelSelectionButtons.Location = new System.Drawing.Point(0, 0);
			this.m_panelSelectionButtons.Name = "m_panelSelectionButtons";
			this.m_panelSelectionButtons.Size = new System.Drawing.Size(464, 22);
			this.m_panelSelectionButtons.TabIndex = 0;
			// 
			// m_radioButtonAssemblyVersions
			// 
			this.m_radioButtonAssemblyVersions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.m_radioButtonAssemblyVersions.Appearance = System.Windows.Forms.Appearance.Button;
			this.m_radioButtonAssemblyVersions.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.m_radioButtonAssemblyVersions.Location = new System.Drawing.Point(0, 0);
			this.m_radioButtonAssemblyVersions.Name = "m_radioButtonAssemblyVersions";
			this.m_radioButtonAssemblyVersions.Size = new System.Drawing.Size(120, 22);
			this.m_radioButtonAssemblyVersions.TabIndex = 1;
			this.m_radioButtonAssemblyVersions.Text = "Assembly Versions";
			this.m_radioButtonAssemblyVersions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.m_radioButtonAssemblyVersions.CheckedChanged += new System.EventHandler(this.OnSelectionButtonClicked);
			// 
			// m_radioButtonAssemblyFileVersions
			// 
			this.m_radioButtonAssemblyFileVersions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.m_radioButtonAssemblyFileVersions.Appearance = System.Windows.Forms.Appearance.Button;
			this.m_radioButtonAssemblyFileVersions.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.m_radioButtonAssemblyFileVersions.Location = new System.Drawing.Point(124, 0);
			this.m_radioButtonAssemblyFileVersions.Name = "m_radioButtonAssemblyFileVersions";
			this.m_radioButtonAssemblyFileVersions.Size = new System.Drawing.Size(120, 22);
			this.m_radioButtonAssemblyFileVersions.TabIndex = 2;
			this.m_radioButtonAssemblyFileVersions.Text = "File Versions";
			this.m_radioButtonAssemblyFileVersions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.m_radioButtonAssemblyFileVersions.CheckedChanged += new System.EventHandler(this.OnSelectionButtonClicked);
			// 
			// m_radioButtonAssemblyInformationalVersions
			// 
			this.m_radioButtonAssemblyInformationalVersions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.m_radioButtonAssemblyInformationalVersions.Appearance = System.Windows.Forms.Appearance.Button;
			this.m_radioButtonAssemblyInformationalVersions.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.m_radioButtonAssemblyInformationalVersions.Location = new System.Drawing.Point(248, 0);
			this.m_radioButtonAssemblyInformationalVersions.Name = "m_radioButtonAssemblyInformationalVersions";
			this.m_radioButtonAssemblyInformationalVersions.Size = new System.Drawing.Size(120, 22);
			this.m_radioButtonAssemblyInformationalVersions.TabIndex = 3;
			this.m_radioButtonAssemblyInformationalVersions.Text = "Product Versions";
			this.m_radioButtonAssemblyInformationalVersions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.m_radioButtonAssemblyInformationalVersions.CheckedChanged += new System.EventHandler(this.OnSelectionButtonClicked);
			// 
			// m_panelListViews
			// 
			this.m_panelListViews.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_panelListViews.Controls.Add(this.m_listViewAssemblyVersions);
			this.m_panelListViews.Controls.Add(this.m_listViewFileVersions);
			this.m_panelListViews.Controls.Add(this.m_listViewInformationalVersions);
			this.m_panelListViews.Location = new System.Drawing.Point(0, 26);
			this.m_panelListViews.Name = "m_panelListViews";
			this.m_panelListViews.Size = new System.Drawing.Size(464, 302);
			this.m_panelListViews.TabIndex = 1;
			// 
			// m_listViewAssemblyVersions
			// 
			this.m_listViewAssemblyVersions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_listViewAssemblyVersions.Location = new System.Drawing.Point(8, 16);
			this.m_listViewAssemblyVersions.Name = "m_listViewAssemblyVersions";
			this.m_listViewAssemblyVersions.Size = new System.Drawing.Size(416, 168);
			this.m_listViewAssemblyVersions.SmallImageList = this.m_imageListProjectIcons;
			this.m_listViewAssemblyVersions.TabIndex = 0;
			this.m_listViewAssemblyVersions.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
			this.m_listViewAssemblyVersions.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnItemCheck);
			// 
			// m_imageListProjectIcons
			// 
			this.m_imageListProjectIcons.ImageSize = new System.Drawing.Size(16, 16);
			this.m_imageListProjectIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("m_imageListProjectIcons.ImageStream")));
			this.m_imageListProjectIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// m_listViewFileVersions
			// 
			this.m_listViewFileVersions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_listViewFileVersions.Location = new System.Drawing.Point(24, 72);
			this.m_listViewFileVersions.Name = "m_listViewFileVersions";
			this.m_listViewFileVersions.Size = new System.Drawing.Size(416, 168);
			this.m_listViewFileVersions.SmallImageList = this.m_imageListProjectIcons;
			this.m_listViewFileVersions.TabIndex = 1;
			this.m_listViewFileVersions.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
			this.m_listViewFileVersions.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnItemCheck);
			// 
			// m_listViewInformationalVersions
			// 
			this.m_listViewInformationalVersions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.m_listViewInformationalVersions.Location = new System.Drawing.Point(40, 128);
			this.m_listViewInformationalVersions.Name = "m_listViewInformationalVersions";
			this.m_listViewInformationalVersions.Size = new System.Drawing.Size(416, 168);
			this.m_listViewInformationalVersions.SmallImageList = this.m_imageListProjectIcons;
			this.m_listViewInformationalVersions.TabIndex = 2;
			this.m_listViewInformationalVersions.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
			this.m_listViewInformationalVersions.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnItemCheck);
			// 
			// AssemblyInfoListViewsControl
			// 
			this.Controls.Add(this.m_panelListViews);
			this.Controls.Add(this.m_panelSelectionButtons);
			this.Name = "AssemblyInfoListViewsControl";
			this.Size = new System.Drawing.Size(464, 328);
			this.m_panelSelectionButtons.ResumeLayout(false);
			this.m_panelListViews.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

        /// <summary>
        ///   Gets and sets currently selected (i.e. foremost) listview
        /// </summary>
        private ProjectsListView SelectedListView {
            get {
                Debug.Assert(m_panelListViews != null && m_panelListViews.Controls.Count == 3 && m_panelListViews.Controls[0] is ProjectsListView);
                return (ProjectsListView)m_panelListViews.Controls[0];
            }
            set {
                if (SelectedListView != value) {
                    Debug.Assert(m_panelListViews.Controls.Contains(value));
                    SynchronizeColumnWidths();
                    SynchronizeSelections();
                    SynchronizeFocusedItems();
                    ScrollInfo sbInfo = ScrollInfo.GetScrollBarPositions(SelectedListView);
                    ScrollInfo.SetScrollBarPositions(value, sbInfo);
                    value.Visible = true;
                    m_panelListViews.Controls.SetChildIndex(value, 0);
                    m_panelListViews.Controls[1].Visible = false;
                    m_panelListViews.Controls[2].Visible = false;
                    if (SelectedTabIndexChanged != null)
                        SelectedTabIndexChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///   Initializes listviews docking and visibility and assigns a 
        ///   listview to corresponding selection button.
        /// </summary>
        private void SetupListViews() {
            m_listViewAssemblyVersions.Dock = DockStyle.Fill;
            m_listViewAssemblyVersions.AssemblyVersionType = AssemblyVersionType.AssemblyVersion;
            m_listViewFileVersions.Dock = DockStyle.Fill;
            m_listViewFileVersions.AssemblyVersionType = AssemblyVersionType.AssemblyFileVersion;
            m_listViewInformationalVersions.Dock = DockStyle.Fill;
            m_listViewInformationalVersions.AssemblyVersionType = AssemblyVersionType.AssemblyInformationalVersion;

            m_radioButtonAssemblyVersions.Tag = m_listViewAssemblyVersions;
            m_radioButtonAssemblyFileVersions.Tag = m_listViewFileVersions;
            m_radioButtonAssemblyInformationalVersions.Tag = m_listViewInformationalVersions;
        }

        /// <summary>
        ///   Initialize fields.
        /// </summary>
        private void InitializeFields() {
            Debug.Assert(m_panelListViews != null && m_panelListViews.Controls.Count == 3);
            m_listViews = new ProjectsListView[m_panelListViews.Controls.Count];
            for (int i = 0; i < m_panelListViews.Controls.Count; i++) {
                ProjectsListView projectsListView = m_panelListViews.Controls[i] as ProjectsListView;
                Debug.Assert(projectsListView != null);
                m_listViews[i] = projectsListView;
            }
            SynchronizeColumnWidths();
        }

        #region Delegates
        /// <summary>
        ///   Delegate used to browse listviews and call a method with an argument.
        /// </summary>
        private delegate void ProcessProjectsListViewDelegate(ProjectsListView listView, object obj);

        /// <summary>
        ///   Passes through <c>ProjectsListView</c>s and calls passed-in delegate.
        /// </summary>
        /// <param name="processListView">
        ///   Delegate to call.
        /// </param>
        /// <param name="applyToAllListViews">
        ///   Flag indicating if the command should be applied to all listvies. 
        ///   If <c>true</c> all listvies are invoked, else only selected listview.
        /// </param>
        /// <param name="obj">
        ///   <c>Object</c> passed as an argument to the delegate.
        /// </param>
        private void ProcessListViews(ProcessProjectsListViewDelegate processListView, bool applyToAllListViews, object obj) {
            SelectedListView.BeginUpdate();
            processListView(SelectedListView, obj);
            if (applyToAllListViews) {
                foreach (ProjectsListView listView in m_listViews) {
                    if (listView != SelectedListView)
                        processListView(listView, obj);
                }
            }
            SelectedListView.EndUpdate();
        }

        /// <summary>
        ///   Delegate used to browse listviews and call a method with integer 
        ///   as value returned.
        /// </summary>
        private delegate int ProcessProjectsListViewReturningIntDelegate(ProjectsListView listView);

        /// <summary>
        ///   Passes through <c>ProjectsListView</c>s and calls passed-in delegate.
        /// </summary>
        /// <param name="processListView">
        ///   Delegate to call.
        /// </param>
        /// <param name="applyToAllListViews">
        ///   Flag indicating if the command should be applied to all listvies. 
        ///   If <c>true</c> all listvies are invoked, else only selected listview.
        /// </param>
        /// <returns>
        ///   Integer result of the delegate called.
        /// </returns>
        private int ProcessListViewsReturningInt(ProcessProjectsListViewReturningIntDelegate processListView, bool applyToAllListViews) {
            if (applyToAllListViews) {
                int count = 0;
                foreach (ProjectsListView listView in m_listViews) {
                    count += processListView(listView);
                }
                return count;
            }
            else {
                return processListView(SelectedListView);
            }
        }

        /// <summary>
        ///   Delegate used to browse listviews and call a method with an argument 
        ///   and returning <c>ProjectVersion</c>.
        /// </summary>
        private delegate ProjectVersion ProcessProjectsListViewReturningProjectVersionDelegate(ProjectsListView listView, object obj);

        /// <summary>
        ///   Passes through <c>ProjectsListView</c>s and calls passed-in delegate.
        /// </summary>
        /// <param name="processListView">
        ///   Delegate to call.
        /// </param>
        /// <param name="applyToAllListViews">
        ///   Flag indicating if the command should be applied to all listvies. 
        ///   If <c>true</c> all listvies are invoked, else only selected listview.
        /// </param>
        /// <returns>
        ///   Integer result of the delegate called.
        /// </returns>
        private ProjectVersion ProcessListViewsReturningHighestProjectVersion(ProcessProjectsListViewReturningProjectVersionDelegate processListView, bool applyToAllListViews, object obj) {
            if (applyToAllListViews) {
                ProjectVersion highest = ProjectVersion.MinValue;
                foreach (ProjectsListView listView in m_listViews) {
                    ProjectVersion version = processListView(listView, obj);
                    if (version > highest)
                        highest = version;
                }
                return highest;
            }
            else {
                return processListView(SelectedListView, obj);
            }
        }

        #endregion // Delegates

        /// <summary>
        ///   Gets column widths in currently selected listview and apply
        ///   them to all listviews. Called when changing selected listview.
        /// </summary>
        private void SynchronizeColumnWidths() {
            int[] columnWidths = GetSelectedListViewColumnWidths();
            SetAllListViewColumnWidths(columnWidths);
        }

        private int[] GetSelectedListViewColumnWidths() {
            Debug.Assert(SelectedListView != null);
            Debug.Assert(m_listViews != null);
            int[] columnWidths = new int[SelectedListView.Columns.Count];
            for (int i = 0; i < SelectedListView.Columns.Count; i++) {
                columnWidths[i] = SelectedListView.Columns[i].Width;
            }
            return columnWidths;
        }

        private void SetAllListViewColumnWidths(int[] columnWidths) {
            Debug.Assert(m_listViews != null);
            if (columnWidths.Length > 0) {
                foreach (ProjectsListView listView in m_listViews) {
                    for (int i = 0; i < listView.Columns.Count; i++) {
                        listView.Columns[i].Width = columnWidths[i];
                    }
                }
            }
        }

        /// <summary>
        ///   Gets selected indeces in currently selected listview and "clones"
        ///   the selection to all listviews. Called when changing selected
        ///   listview.
        /// </summary>
        private void SynchronizeSelections() {
            Debug.Assert(SelectedListView != null);
            Debug.Assert(m_listViews != null);
            int[] selectedIndices = SelectedListView.SelectedIndices;
            foreach (ProjectsListView listView in m_listViews) {
                listView.SelectedIndices = selectedIndices;
            }
        }

        /// <summary>
        ///   Finds item that has focus in currently selected listview and
        ///   focuses items with the same index in all listviews. 
        /// </summary>
        private void SynchronizeFocusedItems() {
            Debug.Assert(SelectedListView != null);
            Debug.Assert(m_listViews != null);
            if (SelectedListView.Items.Count == 0)
                return;
            int index = 0;
            if (SelectedListView.FocusedItem != null) {
                index = SelectedListView.FocusedItem.Index;
            }
            else if (SelectedListView.SelectedIndices.Length > 0) {
                index = SelectedListView.SelectedIndices[0];
            }
            foreach (ProjectsListView listView in m_listViews) {
                listView.Items[index].Focused = true;
            }
        }

        /// <summary>
        ///   Handler called when selection button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionButtonClicked(object sender, System.EventArgs e) {
            RadioButton senderButton = sender as RadioButton;
            Debug.Assert(senderButton != null);
            ProjectsListView listView = senderButton.Tag as ProjectsListView;
            Debug.Assert(listView != null);
            SelectedListView = listView;
        }

        /// <summary>
        ///   Helper array for quicker listviews access.
        /// </summary>
        private ProjectsListView[] m_listViews = new ProjectsListView[0];

        private bool m_disableCheckSynchronization;
        /// <summary>
        ///   SCROLLINFO structures used to synchronize listview scrollbars
        /// </summary>
        private class ScrollInfo {

            public static ScrollInfo GetScrollBarPositions(ListView listView) {
                ScrollInfo sbInfo = new ScrollInfo();
                sbInfo.HorizontalScrollBarInfo.cbSize  = System.Runtime.InteropServices.Marshal.SizeOf(sbInfo.HorizontalScrollBarInfo);
                sbInfo.HorizontalScrollBarInfo.fMask   = (int)Win32Api.ScrollInfoMask.SIF_POS;
                Win32Api.GetScrollInfo(listView.Handle, (int)Win32Api.ScrollBarDirection.SB_HORZ, ref sbInfo.HorizontalScrollBarInfo);
                sbInfo.VerticalScrollBarInfo.cbSize    = System.Runtime.InteropServices.Marshal.SizeOf(sbInfo.VerticalScrollBarInfo);
                sbInfo.VerticalScrollBarInfo.fMask     = (int)Win32Api.ScrollInfoMask.SIF_POS;
                Win32Api.GetScrollInfo(listView.Handle, (int)Win32Api.ScrollBarDirection.SB_VERT, ref sbInfo.VerticalScrollBarInfo);
                return sbInfo;
            }

            public static void SetScrollBarPositions(ListView listView, ScrollInfo sbInfo) {
                int x = sbInfo.HorizontalScrollBarInfo.nPos;
                int y = 0;
                int itemHeight = 0;
                if (listView.TopItem != null) {
                    itemHeight = listView.TopItem.GetBounds(ItemBoundsPortion.Entire).Height;
                    y = sbInfo.VerticalScrollBarInfo.nPos;
                }
                ScrollInfo currSbInfo = GetScrollBarPositions(listView);
                Win32Api.SendMessage(listView.Handle, (int)Win32Api.LVM.SCROLL, x - currSbInfo.HorizontalScrollBarInfo.nPos, (y - currSbInfo.VerticalScrollBarInfo.nPos) * itemHeight);
            }

            public Win32Api.SCROLLINFO HorizontalScrollBarInfo = new Win32Api.SCROLLINFO();
            public Win32Api.SCROLLINFO VerticalScrollBarInfo = new Win32Api.SCROLLINFO();

        }

    }
}