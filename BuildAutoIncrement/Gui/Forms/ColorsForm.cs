/*
 * Filename:    ColorsForm.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Pop-up form used to select a color.
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

using System.Windows.Forms.ColorPicker;

namespace BuildAutoIncrement {
	/// <summary>
	/// Summary description for FormColors.
	/// </summary>
	public class ColorsForm : System.Windows.Forms.Form {

        public event ColorSelectedEventHandler ColorSelected;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ColorsForm() {
			InitializeComponent();
            m_colorSelector.ColorSelected += new System.Windows.Forms.ColorPicker.ColorSelectedEventHandler(this.OnColorSelected);
		}

        private System.Windows.Forms.Panel m_panelBorder;

        private System.Windows.Forms.ColorPicker.MultiTabColorPicker m_colorSelector;

        public Color Color {
            set {
                m_colorSelector.Color = value;
            }
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                m_colorSelector.ColorSelected -= new ColorSelectedEventHandler(this.OnColorSelected);
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///   Resizes the window to fit the ColorTabControl
        /// </summary>
        /// <param name="levent"></param>
        protected override void OnLayout(LayoutEventArgs levent) {
            base.OnLayout(levent);
            int dHeight = this.m_colorSelector.Height - m_panelBorder.ClientRectangle.Height;
            int dWidth = this.m_colorSelector.Width - m_panelBorder.ClientRectangle.Width;
            this.Height += dHeight;
            this.Width += dWidth;
        }

        /// <summary>
        ///   Window is closed automatically if it loses focus.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDeactivate(EventArgs e) {
            base.OnDeactivate(e);
            this.Hide();
        }

        /// <summary>
        ///   Hides the window when ESC key is pressed.
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData) {
            if (keyData == Keys.Escape) {
                this.Hide();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }


        /// <summary>
        ///   Closes window and forwards color selection event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnColorSelected(object sender, ColorSelectedEventArgs e) {
            this.Hide();
            if (ColorSelected != null)
                ColorSelected(sender, e);
        }

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.m_colorSelector = new System.Windows.Forms.ColorPicker.MultiTabColorPicker();
            this.m_panelBorder = new System.Windows.Forms.Panel();
            this.m_panelBorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_colorSelector
            // 
            this.m_colorSelector.Name = "m_colorSelector";
            this.m_colorSelector.Size = new System.Drawing.Size(198, 198);
            this.m_colorSelector.TabIndex = 0;
            // 
            // m_panelBorder
            // 
            this.m_panelBorder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_panelBorder.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                        this.m_colorSelector});
            this.m_panelBorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_panelBorder.Name = "m_panelBorder";
            this.m_panelBorder.Size = new System.Drawing.Size(200, 200);
            this.m_panelBorder.TabIndex = 1;
            // 
            // FormColors
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(200, 200);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.m_panelBorder});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormColors";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Select a color";
            this.m_panelBorder.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

	}
}