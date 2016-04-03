/*
 * Filename:    NoUpdateForm.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Utility form displaying that no version has been updated.
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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace BuildAutoIncrement {
	/// <summary>
	/// Summary description for NoUpdateForm.
	/// </summary>
	public class NoUpdateForm : System.Windows.Forms.Form {

        #region Controls
        private System.Windows.Forms.Label m_labelMessage;
        private System.Windows.Forms.Button m_buttonYes;
        private System.Windows.Forms.Button m_buttonNo;
        private System.Windows.Forms.Label m_labelRunAnyway;
        private System.Windows.Forms.PictureBox m_pictureBoxIcon;
        #endregion Controls

        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private NoUpdateForm() {
			InitializeComponent();
            m_pictureBoxIcon.Image = SystemIcons.Information.ToBitmap();
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
            this.m_labelMessage = new System.Windows.Forms.Label();
            this.m_buttonYes = new System.Windows.Forms.Button();
            this.m_buttonNo = new System.Windows.Forms.Button();
            this.m_labelRunAnyway = new System.Windows.Forms.Label();
            this.m_pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // m_labelMessage
            // 
            this.m_labelMessage.Location = new System.Drawing.Point(56, 16);
            this.m_labelMessage.Name = "m_labelMessage";
            this.m_labelMessage.Size = new System.Drawing.Size(326, 48);
            this.m_labelMessage.TabIndex = 1;
            this.m_labelMessage.Text = "No project update or no valid version for update has been detected. Run the GUI v" +
                "ersion of the tool to change version(s).";
            // 
            // m_buttonYes
            // 
            this.m_buttonYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.m_buttonYes.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.m_buttonYes.Location = new System.Drawing.Point(119, 104);
            this.m_buttonYes.Name = "m_buttonYes";
            this.m_buttonYes.TabIndex = 3;
            this.m_buttonYes.Text = "Yes";
            // 
            // m_buttonNo
            // 
            this.m_buttonNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.m_buttonNo.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.m_buttonNo.Location = new System.Drawing.Point(200, 104);
            this.m_buttonNo.Name = "m_buttonNo";
            this.m_buttonNo.TabIndex = 0;
            this.m_buttonNo.Text = "No";
            // 
            // m_labelRunAnyway
            // 
            this.m_labelRunAnyway.Location = new System.Drawing.Point(56, 69);
            this.m_labelRunAnyway.Name = "m_labelRunAnyway";
            this.m_labelRunAnyway.Size = new System.Drawing.Size(326, 23);
            this.m_labelRunAnyway.TabIndex = 2;
            this.m_labelRunAnyway.Text = "Do you want to execute the command anyway?";
            // 
            // m_pictureBoxIcon
            // 
            this.m_pictureBoxIcon.Location = new System.Drawing.Point(16, 16);
            this.m_pictureBoxIcon.Name = "m_pictureBoxIcon";
            this.m_pictureBoxIcon.Size = new System.Drawing.Size(32, 32);
            this.m_pictureBoxIcon.TabIndex = 4;
            this.m_pictureBoxIcon.TabStop = false;
            // 
            // NoUpdateForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(394, 144);
            this.Controls.Add(this.m_pictureBoxIcon);
            this.Controls.Add(this.m_labelRunAnyway);
            this.Controls.Add(this.m_buttonNo);
            this.Controls.Add(this.m_buttonYes);
            this.Controls.Add(this.m_labelMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NoUpdateForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Versioning Controlled Build";
            this.ResumeLayout(false);

        }
    #endregion

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public static DialogResult Show(IWin32Window owner)
    {
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
      NoUpdateForm nuf = new NoUpdateForm();
            nuf.StartPosition = FormStartPosition.CenterParent;
            return nuf.ShowDialog(owner);
        }
    }
}
