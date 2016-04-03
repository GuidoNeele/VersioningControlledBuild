/*
 * Filename:    VersionSelectUserControl.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Control to select version types to export.
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
using System.Data;
using System.Windows.Forms;

namespace BuildAutoIncrement {
	/// <summary>
	///   <c>UserControl</c> to select version types and their order.
	/// </summary>
	public class VersionSelectUserControl : System.Windows.Forms.UserControl {

        #region Controls

		private System.Windows.Forms.Button m_buttonMoveDown;
		private System.Windows.Forms.Button m_buttonMoveUp;
		private System.Windows.Forms.CheckedListBox m_checkedListBoxVersions;

        #endregion // Controls

        /// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        /// <summary>
        ///   Direction flag.
        /// </summary>
		enum MoveDirection {
			Up,
			Down
		}

		public VersionSelectUserControl() {
			InitializeComponent();
            m_checkedListBoxVersions.Items.Clear();
            m_checkedListBoxVersions.Items.Add(AssemblyVersionType.AssemblyVersion, true);
            m_checkedListBoxVersions.Items.Add(AssemblyVersionType.AssemblyFileVersion, true);
            m_checkedListBoxVersions.Items.Add(AssemblyVersionType.AssemblyInformationalVersion, true);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public BuildAutoIncrement.AssemblyVersionTypeSelection[] AssemblyVersionTypes
        {
            get {
                Debug.Assert(m_checkedListBoxVersions.Items.Count == 3);
                BuildAutoIncrement.AssemblyVersionTypeSelection[] avts = new BuildAutoIncrement.AssemblyVersionTypeSelection[3];
                for (int i = 0; i < 3; i++) {
                    AssemblyVersionType avt = (AssemblyVersionType)m_checkedListBoxVersions.Items[i];
                    avts[i] = new BuildAutoIncrement.AssemblyVersionTypeSelection(avt);
                    avts[i].IsSelected = m_checkedListBoxVersions.GetItemChecked(i);
                }
                return avts;
            }
            set {
                m_checkedListBoxVersions.Items.Clear();
                Debug.Assert(value.Length == 3);
                for (int i = 0; i < 3; i++) {
                    m_checkedListBoxVersions.Items.Add(value[i].AssemblyVersionType, value[i].IsSelected);
                }
            }
        }

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
        
        /// <summary>
        ///   Overrides <c>OnSize</c> method to accommodate control size and 
        ///   layout.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if (Height != m_checkedListBoxVersions.Height) {
				Height = m_checkedListBoxVersions.Height;
			}
			int buttonHeight = m_buttonMoveUp.Width;
			if (Height < (m_buttonMoveUp.Height + m_buttonMoveDown.Height)) {
				buttonHeight = Height / 2;
			}
			m_buttonMoveUp.Height = m_buttonMoveDown.Height = buttonHeight;
		}

        /// <summary>
        ///   Overrides <c>InitLayout</c> method to check all version types by 
        ///   default.
        /// </summary>
		protected override void InitLayout() {
			base.InitLayout ();
			for (int i = 0; i < m_checkedListBoxVersions.Items.Count; ++i) {
				m_checkedListBoxVersions.SetItemChecked(i, true);
			}
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_buttonMoveDown = new System.Windows.Forms.Button();
			this.m_buttonMoveUp = new System.Windows.Forms.Button();
			this.m_checkedListBoxVersions = new System.Windows.Forms.CheckedListBox();
			this.SuspendLayout();
			// 
			// m_buttonMoveDown
			// 
			this.m_buttonMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_buttonMoveDown.Enabled = false;
			this.m_buttonMoveDown.Font = new System.Drawing.Font("Marlett", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.m_buttonMoveDown.Location = new System.Drawing.Point(129, 26);
			this.m_buttonMoveDown.Name = "m_buttonMoveDown";
			this.m_buttonMoveDown.Size = new System.Drawing.Size(23, 23);
			this.m_buttonMoveDown.TabIndex = 5;
			this.m_buttonMoveDown.Text = "u";
			this.m_buttonMoveDown.Click += new System.EventHandler(this.m_buttonMoveDown_Click);
			// 
			// m_buttonMoveUp
			// 
			this.m_buttonMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_buttonMoveUp.Enabled = false;
			this.m_buttonMoveUp.Font = new System.Drawing.Font("Marlett", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.m_buttonMoveUp.Location = new System.Drawing.Point(129, 0);
			this.m_buttonMoveUp.Name = "m_buttonMoveUp";
			this.m_buttonMoveUp.Size = new System.Drawing.Size(23, 23);
			this.m_buttonMoveUp.TabIndex = 4;
			this.m_buttonMoveUp.Text = "5";
			this.m_buttonMoveUp.Click += new System.EventHandler(this.m_buttonMoveUp_Click);
			// 
			// m_checkedListBoxVersions
			// 
			this.m_checkedListBoxVersions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.m_checkedListBoxVersions.Items.AddRange(new object[] {
																		  "Assembly version",
																		  "File version",
																		  "Product version"});
			this.m_checkedListBoxVersions.Location = new System.Drawing.Point(0, 0);
			this.m_checkedListBoxVersions.Name = "m_checkedListBoxVersions";
			this.m_checkedListBoxVersions.Size = new System.Drawing.Size(120, 49);
			this.m_checkedListBoxVersions.TabIndex = 3;
			this.m_checkedListBoxVersions.SelectedIndexChanged += new System.EventHandler(this.m_checkedListBoxVersions_SelectedIndexChanged);
			// 
			// VersionSelectUserControl
			// 
			this.Controls.Add(this.m_buttonMoveDown);
			this.Controls.Add(this.m_buttonMoveUp);
			this.Controls.Add(this.m_checkedListBoxVersions);
			this.Name = "VersionSelectUserControl";
			this.Size = new System.Drawing.Size(152, 49);
			this.ResumeLayout(false);

		}
		#endregion

        #region Control handlers

        /// <summary>
        ///   Disables or enables arrow buttons depending on item currently 
        ///   being selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_checkedListBoxVersions_SelectedIndexChanged(object sender, System.EventArgs e) {
			Debug.Assert(m_checkedListBoxVersions.SelectedItems.Count <= 1);
			bool enableMove = m_checkedListBoxVersions.SelectedItems.Count > 0;
			m_buttonMoveDown.Enabled = enableMove && (m_checkedListBoxVersions.SelectedIndex < m_checkedListBoxVersions.Items.Count - 1);
			m_buttonMoveUp.Enabled = enableMove && (m_checkedListBoxVersions.SelectedIndex > 0);
		}

        /// <summary>
        ///   Moves selected item up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void m_buttonMoveUp_Click(object sender, System.EventArgs e) {
			MoveSelectedItem(MoveDirection.Up);
			m_buttonMoveUp.Focus();
		}

        /// <summary>
        ///   Moves selected item down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void m_buttonMoveDown_Click(object sender, System.EventArgs e) {
			MoveSelectedItem(MoveDirection.Down);
			m_buttonMoveDown.Focus();
		}

        #endregion // Control handlers

        /// <summary>
        ///   Moves selected item up or down.
        /// </summary>
        /// <param name="direction"></param>
		private void MoveSelectedItem(MoveDirection direction) {
			Debug.Assert(m_checkedListBoxVersions.SelectedItems.Count == 1);
			int index = m_checkedListBoxVersions.SelectedIndex;
			bool isChecked = m_checkedListBoxVersions.GetItemChecked(index);
			AssemblyVersionType avt = (AssemblyVersionType)m_checkedListBoxVersions.Items[index];
			m_checkedListBoxVersions.Items.RemoveAt(index);
			int newIndex = index;
			switch (direction) {
            case MoveDirection.Down:
                newIndex++;
                break;
            case MoveDirection.Up:
                newIndex--;
                break;
            default:
                Debug.Assert(false, "Not supported MoveDirection");
                break;
            }
			m_checkedListBoxVersions.Items.Insert(newIndex, avt);
			m_checkedListBoxVersions.SetItemChecked(newIndex, isChecked);
			m_checkedListBoxVersions.SelectedIndex = newIndex;
		}
	}
}