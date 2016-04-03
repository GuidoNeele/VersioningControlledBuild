/*
 * Filename:    RemoveConfigurationsForm.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Displayed during tool uninstallation asking user if 
 *              configuration file should be deleted.
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
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace BuildAutoIncrement {

    public class RemoveConfigurationsForm : System.Windows.Forms.Form {

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
        private System.Windows.Forms.Button m_buttonYes;
        private System.Windows.Forms.Button m_buttonNo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;

        #endregion // Controls

		private System.ComponentModel.Container components = null;

		private RemoveConfigurationsForm() {
			InitializeComponent();
		}

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

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            SetWindowPos(Handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoveConfigurationsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_buttonYes = new System.Windows.Forms.Button();
            this.m_buttonNo = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(16, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(472, 24);
            this.label1.TabIndex = 4;
            this.label1.Text = "Do you want all configuration files to be removed?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(16, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(472, 40);
            this.label2.TabIndex = 5;
            this.label2.Text = "If you select \"Yes\", you\'ll have to configure the application again when you inst" +
                "all it next time.";
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(0, 336);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(498, 4);
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
            // m_buttonYes
            // 
            this.m_buttonYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.m_buttonYes.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_buttonYes.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonYes.Location = new System.Drawing.Point(306, 352);
            this.m_buttonYes.Name = "m_buttonYes";
            this.m_buttonYes.Size = new System.Drawing.Size(88, 23);
            this.m_buttonYes.TabIndex = 0;
            this.m_buttonYes.Text = "&Yes";
            // 
            // m_buttonNo
            // 
            this.m_buttonNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.m_buttonNo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.m_buttonNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_buttonNo.Location = new System.Drawing.Point(400, 352);
            this.m_buttonNo.Name = "m_buttonNo";
            this.m_buttonNo.Size = new System.Drawing.Size(88, 23);
            this.m_buttonNo.TabIndex = 1;
            this.m_buttonNo.Text = "&No";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(498, 72);
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
            this.label3.Text = "Remove Previous Configurations";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RemoveConfigurationsForm
            // 
            this.AcceptButton = this.m_buttonNo;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(498, 383);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.m_buttonNo);
            this.Controls.Add(this.m_buttonYes);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "RemoveConfigurationsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Versioning Controlled Build";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        public static new DialogResult Show() {
            RemoveConfigurationsForm rcf = new RemoveConfigurationsForm();
            rcf.StartPosition = FormStartPosition.CenterScreen;
            return rcf.ShowDialog();
        }
    }
}