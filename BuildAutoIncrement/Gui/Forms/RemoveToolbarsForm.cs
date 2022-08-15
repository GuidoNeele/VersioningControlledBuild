/*
 * Filename:    RemoveToolbarsForm.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Displayed during tool uninstallation asking user if 
 *              toolbars should be removed automatically.
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
using System.Collections.Specialized;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace BuildAutoIncrement {

    public class RemoveToolbarsForm : System.Windows.Forms.Form {

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        private const int HWND_TOPMOST = -1;

        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;

        #region Controls

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button m_buttonNo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel m_panelCheckBoxes;
        private System.Windows.Forms.CheckBox m_checkBoxVisualStudio2003;
        private System.Windows.Forms.CheckBox m_checkBoxVisualStudio2005;
        private System.Windows.Forms.CheckBox m_checkBoxVisualStudio2008;
        private System.Windows.Forms.CheckBox m_checkBoxVisualStudio2010;
        private System.Windows.Forms.CheckBox m_checkBoxVisualStudio2012;
        private System.Windows.Forms.CheckBox m_checkBoxVisualStudio2013;

        #endregion // Controls

        private CheckBox checkBox4;
        private CheckBox checkBox3;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private System.ComponentModel.Container components = null;

        #region Constructors

        private RemoveToolbarsForm() {
			InitializeComponent();
		}

        private RemoveToolbarsForm(StringCollection devEnvironmentNames) : this() {
            foreach (Control control in m_panelCheckBoxes.Controls) {
                Debug.Assert(control is CheckBox);
                control.Enabled = devEnvironmentNames.Contains(control.Tag.ToString());
            }
            m_selectedEnvironments = devEnvironmentNames;
		}

        #endregion // Constructors
        
        /// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

        /// <summary>
        ///   Ensure that form will be shown as the topmost.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            SetWindowPos(Handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        /// <summary>
        ///   Updates selected environments collection.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e) {
            m_selectedEnvironments.Clear();
            foreach (Control control in m_panelCheckBoxes.Controls) {
                CheckBox checkBox = control as CheckBox;
                Debug.Assert(checkBox != null);
                if (checkBox.Checked)
                    m_selectedEnvironments.Add(checkBox.Tag.ToString());
            }
            base.OnClosed(e);
        }

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoveToolbarsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_buttonNo = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.m_checkBoxVisualStudio2003 = new System.Windows.Forms.CheckBox();
            this.m_checkBoxVisualStudio2005 = new System.Windows.Forms.CheckBox();
            this.m_checkBoxVisualStudio2008 = new System.Windows.Forms.CheckBox();
            this.m_checkBoxVisualStudio2010 = new System.Windows.Forms.CheckBox();
            this.m_checkBoxVisualStudio2012 = new System.Windows.Forms.CheckBox();
            this.m_checkBoxVisualStudio2013 = new System.Windows.Forms.CheckBox();
            this.m_panelCheckBoxes = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.m_panelCheckBoxes.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(12, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(452, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Check Visual Studio versions for which VCB toolbars and menus will be removed aut" +
    "omatically.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(12, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(467, 44);
            this.label2.TabIndex = 5;
            this.label2.Text = "Please note that automatic removal may affect Visual Studio customizations and ot" +
    "her add-ins installed.";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Location = new System.Drawing.Point(0, 317);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(491, 4);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(0, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(498, 4);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // m_buttonNo
            // 
            this.m_buttonNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonNo.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_buttonNo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_buttonNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonNo.Location = new System.Drawing.Point(393, 333);
            this.m_buttonNo.Name = "m_buttonNo";
            this.m_buttonNo.Size = new System.Drawing.Size(88, 23);
            this.m_buttonNo.TabIndex = 1;
            this.m_buttonNo.Text = "OK";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(491, 72);
            this.panel1.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(16, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(336, 32);
            this.label3.TabIndex = 0;
            this.label3.Text = "Remove Toolbars and Menus";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_checkBoxVisualStudio2003
            // 
            this.m_checkBoxVisualStudio2003.Checked = true;
            this.m_checkBoxVisualStudio2003.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_checkBoxVisualStudio2003.Location = new System.Drawing.Point(8, 3);
            this.m_checkBoxVisualStudio2003.Name = "m_checkBoxVisualStudio2003";
            this.m_checkBoxVisualStudio2003.Size = new System.Drawing.Size(126, 17);
            this.m_checkBoxVisualStudio2003.TabIndex = 11;
            this.m_checkBoxVisualStudio2003.Tag = "Visual Studio 2003";
            this.m_checkBoxVisualStudio2003.Text = "Visual Studio 200&3  ";
            // 
            // m_checkBoxVisualStudio2005
            // 
            this.m_checkBoxVisualStudio2005.Location = new System.Drawing.Point(8, 26);
            this.m_checkBoxVisualStudio2005.Name = "m_checkBoxVisualStudio2005";
            this.m_checkBoxVisualStudio2005.Size = new System.Drawing.Size(126, 17);
            this.m_checkBoxVisualStudio2005.TabIndex = 12;
            this.m_checkBoxVisualStudio2005.Tag = "Visual Studio 2005";
            this.m_checkBoxVisualStudio2005.Text = "Visual Studio 200&5  ";
            // 
            // m_checkBoxVisualStudio2008
            // 
            this.m_checkBoxVisualStudio2008.Location = new System.Drawing.Point(8, 49);
            this.m_checkBoxVisualStudio2008.Name = "m_checkBoxVisualStudio2008";
            this.m_checkBoxVisualStudio2008.Size = new System.Drawing.Size(126, 17);
            this.m_checkBoxVisualStudio2008.TabIndex = 13;
            this.m_checkBoxVisualStudio2008.Tag = "Visual Studio 2008";
            this.m_checkBoxVisualStudio2008.Text = "Visual Studio 200&8  ";
            // 
            // m_checkBoxVisualStudio2010
            // 
            this.m_checkBoxVisualStudio2010.Location = new System.Drawing.Point(8, 72);
            this.m_checkBoxVisualStudio2010.Name = "m_checkBoxVisualStudio2010";
            this.m_checkBoxVisualStudio2010.Size = new System.Drawing.Size(126, 17);
            this.m_checkBoxVisualStudio2010.TabIndex = 14;
            this.m_checkBoxVisualStudio2010.Tag = "Visual Studio 2010";
            this.m_checkBoxVisualStudio2010.Text = "Visual Studio 201&0  ";
            // 
            // m_checkBoxVisualStudio2012
            // 
            this.m_checkBoxVisualStudio2012.Location = new System.Drawing.Point(8, 95);
            this.m_checkBoxVisualStudio2012.Name = "m_checkBoxVisualStudio2012";
            this.m_checkBoxVisualStudio2012.Size = new System.Drawing.Size(126, 17);
            this.m_checkBoxVisualStudio2012.TabIndex = 15;
            this.m_checkBoxVisualStudio2012.Tag = "Visual Studio 2012";
            this.m_checkBoxVisualStudio2012.Text = "Visual Studio 201&2  ";
            // 
            // m_checkBoxVisualStudio2013
            // 
            this.m_checkBoxVisualStudio2013.Location = new System.Drawing.Point(8, 117);
            this.m_checkBoxVisualStudio2013.Name = "m_checkBoxVisualStudio2013";
            this.m_checkBoxVisualStudio2013.Size = new System.Drawing.Size(126, 17);
            this.m_checkBoxVisualStudio2013.TabIndex = 16;
            this.m_checkBoxVisualStudio2013.Tag = "Visual Studio 2013";
            this.m_checkBoxVisualStudio2013.Text = "Visual Studio 201&3  ";
            // 
            // m_panelCheckBoxes
            // 
            this.m_panelCheckBoxes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_panelCheckBoxes.Controls.Add(this.checkBox4);
            this.m_panelCheckBoxes.Controls.Add(this.checkBox3);
            this.m_panelCheckBoxes.Controls.Add(this.checkBox2);
            this.m_panelCheckBoxes.Controls.Add(this.checkBox1);
            this.m_panelCheckBoxes.Controls.Add(this.m_checkBoxVisualStudio2003);
            this.m_panelCheckBoxes.Controls.Add(this.m_checkBoxVisualStudio2008);
            this.m_panelCheckBoxes.Controls.Add(this.m_checkBoxVisualStudio2005);
            this.m_panelCheckBoxes.Controls.Add(this.m_checkBoxVisualStudio2010);
            this.m_panelCheckBoxes.Controls.Add(this.m_checkBoxVisualStudio2012);
            this.m_panelCheckBoxes.Controls.Add(this.m_checkBoxVisualStudio2013);
            this.m_panelCheckBoxes.Location = new System.Drawing.Point(12, 157);
            this.m_panelCheckBoxes.Name = "m_panelCheckBoxes";
            this.m_panelCheckBoxes.Size = new System.Drawing.Size(469, 154);
            this.m_panelCheckBoxes.TabIndex = 16;
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(153, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(168, 17);
            this.checkBox1.TabIndex = 17;
            this.checkBox1.Tag = "Visual Studio 2015";
            this.checkBox1.Text = "Visual Studio 201&5";
            // 
            // checkBox2
            // 
            this.checkBox2.Location = new System.Drawing.Point(153, 26);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(168, 17);
            this.checkBox2.TabIndex = 18;
            this.checkBox2.Tag = "Visual Studio 2017";
            this.checkBox2.Text = "Visual Studio 201&7";
            // 
            // checkBox3
            // 
            this.checkBox3.Location = new System.Drawing.Point(153, 49);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(168, 17);
            this.checkBox3.TabIndex = 19;
            this.checkBox3.Tag = "Visual Studio 2019";
            this.checkBox3.Text = "Visual Studio 201&9";
            // 
            // checkBox4
            // 
            this.checkBox4.Location = new System.Drawing.Point(153, 72);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(168, 17);
            this.checkBox4.TabIndex = 20;
            this.checkBox4.Tag = "Visual Studio 2022";
            this.checkBox4.Text = "Visual Studio 2022";
            // 
            // RemoveToolbarsForm
            // 
            this.AcceptButton = this.m_buttonNo;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(491, 364);
            this.Controls.Add(this.m_panelCheckBoxes);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.m_buttonNo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "RemoveToolbarsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Versioning Controlled Build";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.m_panelCheckBoxes.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private StringCollection m_selectedEnvironments = null;

        public static DialogResult Show(StringCollection devEnvironmentNames) {
            RemoveToolbarsForm rcf = new RemoveToolbarsForm(devEnvironmentNames);
            rcf.StartPosition = FormStartPosition.CenterScreen;
            return rcf.ShowDialog();
        }
    }
}