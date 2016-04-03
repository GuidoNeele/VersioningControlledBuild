/*
 * Filename:    FontSelectionUserControl.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: User control for font selection.
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
using System.Data;
using System.Windows.Forms;

namespace BuildAutoIncrement {
	/// <summary>
	///   <c>UserControl</c> used to display and select a font.
	/// </summary>
	public class FontSelectionUserControl : System.Windows.Forms.UserControl {

        #region Controls

        private System.Windows.Forms.Label m_labelCaption;
        private FontTextBox m_textBoxFont;
        private System.Windows.Forms.Button m_buttonSelect;

        #endregion // Controls
        
        /// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FontSelectionUserControl() {
			InitializeComponent();
		}

        #region Public properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public override string Text {
            get {
                return m_labelCaption.Text;
            }
            set {
                m_labelCaption.Text = value;
            }
        }

        public int CaptionWidth {
            get {
                return m_labelCaption.Width;
            }
            set {
                int dx = m_labelCaption.Width - value;
                m_labelCaption.Width = value;
                m_textBoxFont.Left -= dx;
                m_textBoxFont.Width += dx; 
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public FontDescription FontDescription {
            get {
                return m_textBoxFont.FontDescription;
            }
            set {
                m_textBoxFont.FontDescription = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font FontDisplayed {
            get {
                return m_textBoxFont.FontDisplayed;
            }
            set {
                m_textBoxFont.FontDisplayed = value;
            }
        }

        #endregion // Public properties

        #region Overrides

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

        #endregion // Overrides

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.m_labelCaption = new System.Windows.Forms.Label();
            this.m_textBoxFont = new BuildAutoIncrement.FontTextBox();
            this.m_buttonSelect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // m_labelCaption
            // 
            this.m_labelCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left)));
            this.m_labelCaption.Location = new System.Drawing.Point(0, 0);
            this.m_labelCaption.Name = "m_labelCaption";
            this.m_labelCaption.Size = new System.Drawing.Size(72, 20);
            this.m_labelCaption.TabIndex = 0;
            this.m_labelCaption.Text = "label";
            this.m_labelCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_textBoxFont
            // 
            this.m_textBoxFont.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_textBoxFont.Location = new System.Drawing.Point(72, 0);
            this.m_textBoxFont.Name = "m_textBoxFont";
            this.m_textBoxFont.ReadOnly = true;
            this.m_textBoxFont.Size = new System.Drawing.Size(168, 20);
            this.m_textBoxFont.TabIndex = 1;
            // 
            // m_buttonSelect
            // 
            this.m_buttonSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonSelect.Location = new System.Drawing.Point(248, 0);
            this.m_buttonSelect.Name = "m_buttonSelect";
            this.m_buttonSelect.Size = new System.Drawing.Size(24, 20);
            this.m_buttonSelect.TabIndex = 2;
            this.m_buttonSelect.Text = "...";
            this.m_buttonSelect.Click += new System.EventHandler(this.m_buttonSelect_Click);
            // 
            // FontSelectionUserControl
            // 
            this.Controls.Add(this.m_buttonSelect);
            this.Controls.Add(this.m_textBoxFont);
            this.Controls.Add(this.m_labelCaption);
            this.Name = "FontSelectionUserControl";
            this.Size = new System.Drawing.Size(272, 20);
            this.ResumeLayout(false);

        }
		#endregion

        #region Control handlers
        
        private void m_buttonSelect_Click(object sender, System.EventArgs e) {
            /*
            FontForm fd = new FontForm();
            */
            FontDialog fd = new FontDialog();
            fd.AllowScriptChange = false;
            fd.AllowVerticalFonts = false;
            fd.FontMustExist = true;
            fd.ShowEffects = false;
            fd.Font = m_textBoxFont.FontDisplayed;
            if (fd.ShowDialog(this.TopLevelControl) == DialogResult.OK) {
                m_textBoxFont.FontDisplayed = fd.Font;
            }
        }
    
        #endregion // Control handlers
    }
}