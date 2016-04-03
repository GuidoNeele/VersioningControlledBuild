/*
 * Filename:    CheckOutErrorDialog.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Displays the list of files failed to check-out.
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
	///   Form displaying the list of files failed to check-out.
	/// </summary>
	public class CheckOutErrorDialog : System.Windows.Forms.Form {
        
        #region Controls
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox m_textBoxNotCheckedOut;
        private System.Windows.Forms.Button m_buttonYes;
        private System.Windows.Forms.Button m_buttonNo;
        private System.Windows.Forms.PictureBox m_pictureBoxIcon;
        #endregion // Controls
        
        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        private CheckOutErrorDialog() {
            InitializeComponent();
            m_pictureBoxIcon.Image = Bitmap.FromHicon(SystemIcons.Warning.Handle);
        }

		private CheckOutErrorDialog(string[] notCheckedOutFiles) : this() {
            Debug.Assert(notCheckedOutFiles != null && notCheckedOutFiles.Length > 0);
            m_textBoxNotCheckedOut.Lines = notCheckedOutFiles;
            m_textBoxNotCheckedOut.SelectionLength = 0;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.m_textBoxNotCheckedOut = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.m_buttonYes = new System.Windows.Forms.Button();
            this.m_buttonNo = new System.Windows.Forms.Button();
            this.m_pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(56, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(360, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Following files could not be checked out automatically:";
            // 
            // m_textBoxNotCheckedOut
            // 
            this.m_textBoxNotCheckedOut.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.m_textBoxNotCheckedOut.Cursor = System.Windows.Forms.Cursors.Default;
            this.m_textBoxNotCheckedOut.ForeColor = System.Drawing.SystemColors.WindowText;
            this.m_textBoxNotCheckedOut.Location = new System.Drawing.Point(56, 32);
            this.m_textBoxNotCheckedOut.Multiline = true;
            this.m_textBoxNotCheckedOut.Name = "m_textBoxNotCheckedOut";
            this.m_textBoxNotCheckedOut.ReadOnly = true;
            this.m_textBoxNotCheckedOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_textBoxNotCheckedOut.Size = new System.Drawing.Size(384, 72);
            this.m_textBoxNotCheckedOut.TabIndex = 1;
            this.m_textBoxNotCheckedOut.Text = "";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(56, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(384, 32);
            this.label2.TabIndex = 2;
            this.label2.Text = "You should check them out manually first to complete the procedure successfully, " +
                "or they will be out of sync with SourceSafe database.";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(56, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(384, 23);
            this.label3.TabIndex = 3;
            this.label3.Text = "Do you want to continue anyway?";
            // 
            // m_buttonYes
            // 
            this.m_buttonYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.m_buttonYes.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.m_buttonYes.Location = new System.Drawing.Point(144, 184);
            this.m_buttonYes.Name = "m_buttonYes";
            this.m_buttonYes.TabIndex = 4;
            this.m_buttonYes.Text = "Yes";
            // 
            // m_buttonNo
            // 
            this.m_buttonNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.m_buttonNo.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.m_buttonNo.Location = new System.Drawing.Point(232, 184);
            this.m_buttonNo.Name = "m_buttonNo";
            this.m_buttonNo.TabIndex = 5;
            this.m_buttonNo.Text = "No";
            // 
            // m_pictureBoxIcon
            // 
            this.m_pictureBoxIcon.Location = new System.Drawing.Point(12, 38);
            this.m_pictureBoxIcon.Name = "m_pictureBoxIcon";
            this.m_pictureBoxIcon.Size = new System.Drawing.Size(36, 36);
            this.m_pictureBoxIcon.TabIndex = 6;
            this.m_pictureBoxIcon.TabStop = false;
            // 
            // CheckOutErrorDialog
            // 
            this.AcceptButton = this.m_buttonNo;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_buttonNo;
            this.ClientSize = new System.Drawing.Size(450, 223);
            this.Controls.Add(this.m_pictureBoxIcon);
            this.Controls.Add(this.m_buttonNo);
            this.Controls.Add(this.m_buttonYes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.m_textBoxNotCheckedOut);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckOutErrorDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Checkout Error";
            this.ResumeLayout(false);

        }
		#endregion

        public static DialogResult Show(IWin32Window owner, string[] notCheckedOutFiles) {
            CheckOutErrorDialog coed = new CheckOutErrorDialog(notCheckedOutFiles);
            coed.StartPosition = FormStartPosition.CenterParent;
            return coed.ShowDialog(owner);
        }
    }
}