/*
 * Filename:    OperationSuccesForm.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Utility form displaying update summary.
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
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace BuildAutoIncrement {
	/// <summary>
	///   Displays update summary.
	/// </summary>
	public class OperationSuccesForm : System.Windows.Forms.Form {

        #region Controls

        private System.Windows.Forms.ColumnHeader m_columnHeaderProjectName;
        private System.Windows.Forms.ColumnHeader m_columnHeaderAssemblyVersion;
        private System.Windows.Forms.ColumnHeader m_columnHeaderFileVersion;
        private System.Windows.Forms.ColumnHeader m_columnHeaderProductVersion;
        private System.Windows.Forms.Button m_buttonOK;
        private System.Windows.Forms.Label m_labelOperationSuccess;
        private System.Windows.Forms.ListView m_listViewDetails;
        private System.Windows.Forms.PictureBox m_pictureBox;
        private BuildAutoIncrement.LabelWithDivider m_labelListViewDivider;

        #endregion // Controls

        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        #region Constructor

        private OperationSuccesForm() {
            InitializeComponent();
            m_pictureBox.Image = SystemIcons.Information.ToBitmap();
        }
        
        private OperationSuccesForm(UpdateSummary.UpdateSummaryItem[] updatedSummary) : this() {
            Debug.Assert(updatedSummary != null && updatedSummary.Length > 0);
            FillDetailsListView(updatedSummary);
		}

        #endregion // Constructor
        
        #region Dispose method

        /// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

        #endregion // Dispose method

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(OperationSuccesForm));
            this.m_labelOperationSuccess = new System.Windows.Forms.Label();
            this.m_buttonOK = new System.Windows.Forms.Button();
            this.m_pictureBox = new System.Windows.Forms.PictureBox();
            this.m_listViewDetails = new System.Windows.Forms.ListView();
            this.m_columnHeaderProjectName = new System.Windows.Forms.ColumnHeader();
            this.m_columnHeaderAssemblyVersion = new System.Windows.Forms.ColumnHeader();
            this.m_columnHeaderFileVersion = new System.Windows.Forms.ColumnHeader();
            this.m_columnHeaderProductVersion = new System.Windows.Forms.ColumnHeader();
            this.m_labelListViewDivider = new BuildAutoIncrement.LabelWithDivider();
            this.SuspendLayout();
            // 
            // m_labelOperationSuccess
            // 
            this.m_labelOperationSuccess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelOperationSuccess.Location = new System.Drawing.Point(56, 20);
            this.m_labelOperationSuccess.Name = "m_labelOperationSuccess";
            this.m_labelOperationSuccess.Size = new System.Drawing.Size(320, 24);
            this.m_labelOperationSuccess.TabIndex = 1;
            this.m_labelOperationSuccess.Text = "Version(s) updated successfully.";
            this.m_labelOperationSuccess.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_buttonOK
            // 
            this.m_buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.m_buttonOK.Location = new System.Drawing.Point(390, 21);
            this.m_buttonOK.Name = "m_buttonOK";
            this.m_buttonOK.TabIndex = 2;
            this.m_buttonOK.Text = "OK";
            // 
            // m_pictureBox
            // 
            this.m_pictureBox.Location = new System.Drawing.Point(16, 16);
            this.m_pictureBox.Name = "m_pictureBox";
            this.m_pictureBox.Size = new System.Drawing.Size(32, 32);
            this.m_pictureBox.TabIndex = 1;
            this.m_pictureBox.TabStop = false;
            // 
            // m_listViewDetails
            // 
            this.m_listViewDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_listViewDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                                this.m_columnHeaderProjectName,
                                                                                                this.m_columnHeaderAssemblyVersion,
                                                                                                this.m_columnHeaderFileVersion,
                                                                                                this.m_columnHeaderProductVersion});
            this.m_listViewDetails.FullRowSelect = true;
            this.m_listViewDetails.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.m_listViewDetails.Location = new System.Drawing.Point(8, 88);
            this.m_listViewDetails.Name = "m_listViewDetails";
            this.m_listViewDetails.Size = new System.Drawing.Size(464, 184);
            this.m_listViewDetails.TabIndex = 0;
            this.m_listViewDetails.View = System.Windows.Forms.View.Details;
            // 
            // m_columnHeaderProjectName
            // 
            this.m_columnHeaderProjectName.Text = "Project name";
            this.m_columnHeaderProjectName.Width = 150;
            // 
            // m_columnHeaderAssemblyVersion
            // 
            this.m_columnHeaderAssemblyVersion.Text = "Assembly version";
            this.m_columnHeaderAssemblyVersion.Width = 100;
            // 
            // m_columnHeaderFileVersion
            // 
            this.m_columnHeaderFileVersion.Text = "File version";
            this.m_columnHeaderFileVersion.Width = 100;
            // 
            // m_columnHeaderProductVersion
            // 
            this.m_columnHeaderProductVersion.Text = "Product version";
            this.m_columnHeaderProductVersion.Width = 100;
            // 
            // m_labelListViewDivider
            // 
            this.m_labelListViewDivider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelListViewDivider.Location = new System.Drawing.Point(8, 64);
            this.m_labelListViewDivider.Name = "m_labelListViewDivider";
            this.m_labelListViewDivider.Size = new System.Drawing.Size(464, 20);
            this.m_labelListViewDivider.TabIndex = 3;
            this.m_labelListViewDivider.Text = "&Projects updated";
            this.m_labelListViewDivider.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OperationSuccesForm
            // 
            this.AcceptButton = this.m_buttonOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_buttonOK;
            this.ClientSize = new System.Drawing.Size(480, 283);
            this.Controls.Add(this.m_labelListViewDivider);
            this.Controls.Add(this.m_buttonOK);
            this.Controls.Add(this.m_labelOperationSuccess);
            this.Controls.Add(this.m_pictureBox);
            this.Controls.Add(this.m_listViewDetails);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(488, 310);
            this.Name = "OperationSuccesForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Versioning Controlled Build";
            this.ResumeLayout(false);

        }
		#endregion

        #region Private methods

        private void FillDetailsListView(UpdateSummary.UpdateSummaryItem[] updateInfos) {
            m_listViewDetails.Items.Clear();
            foreach (UpdateSummary.UpdateSummaryItem updateInfo in updateInfos) {
                ListViewItem lvi = new ListViewItem(updateInfo.ProjectName);
                lvi.UseItemStyleForSubItems = false;
                lvi.ForeColor = SelectStatusColor(updateInfo.UpdateState);
                foreach (AssemblyVersionType versionType in AssemblyVersions.AssemblyVersionTypes) {
                    ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
                    lvsi.Text = updateInfo[versionType].Version;
                    lvsi.ForeColor = SelectStatusColor(updateInfo[versionType].UpdateState);
                    lvi.SubItems.Add(lvsi);
                }
                m_listViewDetails.Items.Add(lvi);
            }
        }

        private Color SelectStatusColor(UpdateSummary.UpdateState updateState) {
            switch (updateState) {
            case UpdateSummary.UpdateState.Updated:
                return ConfigurationPersister.Instance.Configuration.DisplayOptions.Colors.ReportUpdatedVersion;
            case UpdateSummary.UpdateState.NotUpdated:
                return ConfigurationPersister.Instance.Configuration.DisplayOptions.Colors.ReportVersionNotChanged;
            case UpdateSummary.UpdateState.Failed:
                return ConfigurationPersister.Instance.Configuration.DisplayOptions.Colors.ReportVersionUpdateFailed;
            }
            Debug.Assert(false, "Not configured UpdateState color");
            return SystemColors.ControlText;
        }

        #endregion // Private methods
    
        public static DialogResult Show(IWin32Window owner, UpdateSummary.UpdateSummaryItem[] updatedSummary) {
            OperationSuccesForm osf = new OperationSuccesForm(updatedSummary);
            osf.StartPosition = FormStartPosition.CenterParent;
            return osf.ShowDialog(owner);
        }
    }
}