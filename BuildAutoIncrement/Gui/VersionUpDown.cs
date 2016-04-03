/*
 * Filename:    VersionUpDown.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: User control consisting of SpinButton and VersionTextBox.
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
using System.Drawing;
using System.Windows.Forms;

namespace BuildAutoIncrement {

    public delegate void ToBeVersionChangedHandler(object sender, string toBeVersionPattern);

	/// <summary>
	/// User control that consists of VersionTextBox and a spin button.
	/// </summary>
	public class VersionUpDown : System.Windows.Forms.UserControl {

        public event ToBeVersionChangedHandler ToBeVersionChanged;
        
        private SpinButton m_spinButton;
        private VersionTextBox m_textBoxVersion;
        
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Creates VersionUpDown control.
        /// </summary>
        public VersionUpDown() {
			InitializeComponent();
            m_textBoxVersion.Maximum = ProjectVersion.MaxVersion - 1;
            m_textBoxVersion.Minimum = -2;
            m_spinButton.Maximum = +1;
            m_spinButton.Minimum = -1;
            AttachEventHandlers();
        }

        override public string Text {
            get { return m_textBoxVersion.Text; }
            set { m_textBoxVersion.Text = value; }
        }

        /// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if (disposing) {
                DetachEventHandlers();
				if (components != null)	{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.m_spinButton = new BuildAutoIncrement.SpinButton();
            this.m_textBoxVersion = new VersionTextBox();
            this.SuspendLayout();
            // 
            // m_spinButton
            // 
            this.m_spinButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.m_spinButton.Enabled = false;
            this.m_spinButton.LargeChange = ((long)(1));
            this.m_spinButton.Location = new System.Drawing.Point(96, 0);
            this.m_spinButton.Maximum = ((long)(0));
            this.m_spinButton.Minimum = ((long)(0));
            this.m_spinButton.Name = "m_spinButton";
            this.m_spinButton.Size = new System.Drawing.Size(16, 20);
            this.m_spinButton.TabIndex = 2;
            this.m_spinButton.Value = ((long)(0));
            this.m_spinButton.Scroll += new System.Windows.Forms.ScrollEventHandler(this.m_spinButton_Scroll);
            // 
            // m_textBoxVersion
            // 
            this.m_textBoxVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_textBoxVersion.Name = "m_textBoxVersion";
            this.m_textBoxVersion.Size = new System.Drawing.Size(96, 20);
            this.m_textBoxVersion.TabIndex = 3;
            this.m_textBoxVersion.Text = "";
            // 
            // VersionUpDown
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.m_textBoxVersion,
                                                                          this.m_spinButton});
            this.Name = "VersionUpDown";
            this.Size = new System.Drawing.Size(112, 20);
            this.ResumeLayout(false);

        }
		#endregion

        private void AttachEventHandlers() {
            m_textBoxVersion.TextChanged        += new System.EventHandler(OnVersionChanged);
            m_textBoxVersion.GotFocus           += new System.EventHandler(OnTextBoxGotFocus);
            m_textBoxVersion.LostFocus          += new System.EventHandler(OnTextBoxLostFocus);
            m_textBoxVersion.ActiveBlockChanged += new System.EventHandler(OnActiveBlockChanged);
        }

        private void DetachEventHandlers() {
            m_textBoxVersion.TextChanged        -= new System.EventHandler(OnVersionChanged);
            m_textBoxVersion.GotFocus           -= new System.EventHandler(OnTextBoxGotFocus);
            m_textBoxVersion.LostFocus          -= new System.EventHandler(OnTextBoxLostFocus);
            m_textBoxVersion.ActiveBlockChanged -= new System.EventHandler(OnActiveBlockChanged);
        }

        #region Event handlers

        private void OnTextBoxGotFocus(object sender, EventArgs e) {
            m_spinButton.Enabled = true;
        }

        private void OnTextBoxLostFocus(object sender, EventArgs e) {
            m_spinButton.Enabled = false;
        }

        private void OnVersionChanged(object sender, EventArgs e) {
            m_spinButton.Value = 0;
            if (ToBeVersionChanged != null)
                ToBeVersionChanged(this, m_textBoxVersion.Text);
        }

        private void OnActiveBlockChanged(object sender, EventArgs e) {
            m_spinButton.Value = 0;
        }

        private void m_spinButton_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) {
            m_textBoxVersion.IncrementVersionNumber((int)m_spinButton.Value);
            m_spinButton.Value = 0;
        }

        #endregion // Event handlers

	}
}