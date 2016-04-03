/*
 * Filename:    ExceptionForm.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Form displaying exception details.
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
	///   Utility form used to display exception details.
	/// </summary>
	public class ExceptionForm : System.Windows.Forms.Form {

        #region Controls
        private System.Windows.Forms.Panel m_panelMessage;
        private System.Windows.Forms.Button m_buttonOK;
        private System.Windows.Forms.Button m_buttonDetails;
        private System.Windows.Forms.PictureBox m_pictureBoxIcon;
        private System.Windows.Forms.TextBox m_textBoxDetails;
        private System.Windows.Forms.Label m_labelMessage;
        #endregion // Controls

        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        private readonly string m_detailsButtonCaption;
        private          bool   m_detailsVisible;
        private readonly Size   m_fullSize;

        #region Constructors

        /// <summary>
        ///   Creates <c>ExceptionForm</c>.
        /// </summary>
        private ExceptionForm() {
			InitializeComponent();
            m_detailsButtonCaption  = m_buttonDetails.Text;
            m_fullSize              = ClientRectangle.Size;
            HideDetails();
		}

        private ExceptionForm(Exception e, string caption, MessageBoxIcon icon) : this() {
            m_labelMessage.Text     = e.Message;
            m_textBoxDetails.Text   = e.ToString();
            Text                    = caption;
            if (icon == MessageBoxIcon.None) {
                m_pictureBoxIcon.Visible = false;
                m_labelMessage.Left = m_pictureBoxIcon.Left;
            }
            else
                m_pictureBoxIcon.Image  = CreateIcon(icon);
        }

        #endregion // Constructors

        #region Dispose method

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

        #endregion // Dispose method

        #region Private methods

        private Image CreateIcon(MessageBoxIcon icon) {
            switch (icon) {
                case MessageBoxIcon.Asterisk:
                    return SystemIcons.Asterisk.ToBitmap();
                //case MessageBoxIcon.Error:
                //    return SystemIcons.Error.ToBitmap();
                case MessageBoxIcon.Exclamation:
                    return SystemIcons.Exclamation.ToBitmap();
                case MessageBoxIcon.Hand:
                    return SystemIcons.Hand.ToBitmap();
                //case MessageBoxIcon.Information:
                //    return SystemIcons.Information.ToBitmap();
                //case MessageBoxIcon.None:
                //    return new Bitmap(0, 0);
                case MessageBoxIcon.Question:
                    return SystemIcons.Question.ToBitmap();
                //case MessageBoxIcon.Stop:
                //    return SystemIcons.Stop.ToBitmap();
                //case MessageBoxIcon.Warning:
                //    return SystemIcons.Warning.ToBitmap();
                default:
                    Debug.Assert(false, string.Format("Unknown icon type: {0}", icon.ToString()));
                    break;
            }
            return null;
        }

        private void ShowDetails() {
            m_buttonDetails.Text = "<< " + m_detailsButtonCaption;
            SetClientSizeCore(m_panelMessage.Width, m_fullSize.Height);
            m_detailsVisible = true;
        }

        private void HideDetails() {
            m_buttonDetails.Text = m_detailsButtonCaption + " >>";
            SetClientSizeCore(m_panelMessage.Width, m_panelMessage.Height);
            m_detailsVisible = false;
        }

        private void ToggleDetails() {
            if (m_detailsVisible)
                HideDetails();
            else
                ShowDetails();
        }

        #endregion // Private methods
        
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ExceptionForm));
            this.m_buttonOK = new System.Windows.Forms.Button();
            this.m_buttonDetails = new System.Windows.Forms.Button();
            this.m_pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.m_panelMessage = new System.Windows.Forms.Panel();
            this.m_labelMessage = new System.Windows.Forms.Label();
            this.m_textBoxDetails = new System.Windows.Forms.TextBox();
            this.m_panelMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_buttonOK
            // 
            this.m_buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.m_buttonOK.Location = new System.Drawing.Point(400, 17);
            this.m_buttonOK.Name = "m_buttonOK";
            this.m_buttonOK.TabIndex = 0;
            this.m_buttonOK.Text = "OK";
            // 
            // m_buttonDetails
            // 
            this.m_buttonDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonDetails.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.m_buttonDetails.Location = new System.Drawing.Point(400, 57);
            this.m_buttonDetails.Name = "m_buttonDetails";
            this.m_buttonDetails.TabIndex = 1;
            this.m_buttonDetails.Text = "&Details";
            this.m_buttonDetails.Click += new System.EventHandler(this.m_buttonDetails_Click);
            // 
            // m_pictureBoxIcon
            // 
            this.m_pictureBoxIcon.Image = ((System.Drawing.Image)(resources.GetObject("m_pictureBoxIcon.Image")));
            this.m_pictureBoxIcon.Location = new System.Drawing.Point(16, 32);
            this.m_pictureBoxIcon.Name = "m_pictureBoxIcon";
            this.m_pictureBoxIcon.Size = new System.Drawing.Size(32, 32);
            this.m_pictureBoxIcon.TabIndex = 2;
            this.m_pictureBoxIcon.TabStop = false;
            // 
            // m_panelMessage
            // 
            this.m_panelMessage.Controls.Add(this.m_labelMessage);
            this.m_panelMessage.Controls.Add(this.m_pictureBoxIcon);
            this.m_panelMessage.Controls.Add(this.m_buttonOK);
            this.m_panelMessage.Controls.Add(this.m_buttonDetails);
            this.m_panelMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_panelMessage.Location = new System.Drawing.Point(0, 0);
            this.m_panelMessage.Name = "m_panelMessage";
            this.m_panelMessage.Size = new System.Drawing.Size(490, 96);
            this.m_panelMessage.TabIndex = 0;
            // 
            // m_labelMessage
            // 
            this.m_labelMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelMessage.Location = new System.Drawing.Point(64, 23);
            this.m_labelMessage.Name = "m_labelMessage";
            this.m_labelMessage.Size = new System.Drawing.Size(320, 50);
            this.m_labelMessage.TabIndex = 3;
            this.m_labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textBoxDetails
            // 
            this.m_textBoxDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_textBoxDetails.HideSelection = false;
            this.m_textBoxDetails.Location = new System.Drawing.Point(8, 104);
            this.m_textBoxDetails.Multiline = true;
            this.m_textBoxDetails.Name = "m_textBoxDetails";
            this.m_textBoxDetails.ReadOnly = true;
            this.m_textBoxDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.m_textBoxDetails.Size = new System.Drawing.Size(474, 168);
            this.m_textBoxDetails.TabIndex = 1;
            this.m_textBoxDetails.Text = "";
            this.m_textBoxDetails.WordWrap = false;
            // 
            // ExceptionForm
            // 
            this.AcceptButton = this.m_buttonOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(490, 279);
            this.Controls.Add(this.m_textBoxDetails);
            this.Controls.Add(this.m_panelMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExceptionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExceptionForm";
            this.TopMost = true;
            this.m_panelMessage.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private void m_buttonDetails_Click(object sender, System.EventArgs e) {
            ToggleDetails();
        }

        #region Public static methods to invoke the form

        public static DialogResult Show(Exception e) {
            return Show(e, "", MessageBoxIcon.Error);
        }

        public static DialogResult Show(IWin32Window owner, Exception e) {
            return Show(owner, e, "", MessageBoxIcon.Error);
        }

        public static DialogResult Show(Exception e, string caption) {
            return Show(e, caption, MessageBoxIcon.Error);
        }

        public static DialogResult Show(IWin32Window owner, Exception e, string caption) {
            return Show(owner, e, caption, MessageBoxIcon.Error);
        }

        public static DialogResult Show(Exception e, string caption, MessageBoxIcon icon) {
            ExceptionForm ef = new ExceptionForm(e, caption, icon);
            ef.StartPosition = FormStartPosition.CenterScreen;
            return ef.ShowDialog();
        }

        public static DialogResult Show(IWin32Window owner, Exception e, string caption, MessageBoxIcon icon) {
            ExceptionForm ef = new ExceptionForm(e, caption, icon);
            ef.StartPosition = FormStartPosition.CenterParent;
            return ef.ShowDialog(owner);
        }

        #endregion // Public static methods to invoke the form
    }
}