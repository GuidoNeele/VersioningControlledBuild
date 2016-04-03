/*
 * Filename:    VcbExportOptionsUserControl.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: User control to configure export settings.
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
	///   <c>UserControl</c> used to select export options for printing 
	///   projects list or exporting it to file.
	/// </summary>
	public class VcbExportOptionsUserControl : System.Windows.Forms.UserControl {

        #region Controls

		private System.Windows.Forms.GroupBox m_groupBoxVersions;
        private BuildAutoIncrement.VersionSelectUserControl m_versionsControl;
        private System.Windows.Forms.CheckBox m_checkBoxIndentSubprojects;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown m_numericUpDownIndentation;
        private System.Windows.Forms.CheckBox m_checkBoxExcludeNonversionable;

        private System.Windows.Forms.GroupBox m_groupBoxFileFormat;
        private System.Windows.Forms.RadioButton m_radioButtonPlainText;
        private System.Windows.Forms.RadioButton m_radioButtonCsv;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox m_textBoxSeparator;

        private BuildAutoIncrement.LabelWithDivider m_labelPrintOptions;
        private System.Windows.Forms.CheckBox m_checkBoxPrintIcons;
        private System.Windows.Forms.GroupBox m_groupBoxFonts;
        private BuildAutoIncrement.FontSelectionUserControl m_userControlItemFont;
        private BuildAutoIncrement.FontSelectionUserControl m_userControlHeaderFont;
        private BuildAutoIncrement.FontSelectionUserControl m_userControlHeadingFont;

        #endregion // Controls

        /// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public VcbExportOptionsUserControl() {
			InitializeComponent();
            m_userControlHeadingFont.FontDescription = new FontDescription("Arial", FontStyle.Bold, 9.75f);
            m_textBoxSeparator.Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
		}

        public ExportConfiguration ExportConfiguration {
            get {
                ExportConfiguration ec = new ExportConfiguration();
                ec.AssemblyVersionTypes = m_versionsControl.AssemblyVersionTypes;
                ec.IndentSubItems = m_checkBoxIndentSubprojects.Checked;
                ec.IndentSubItemsBy = (int)m_numericUpDownIndentation.Value;
                ec.ExcludeNonversionableItems = m_checkBoxExcludeNonversionable.Checked;
                if (m_radioButtonPlainText.Checked)
                    ec.ExportFileFormat = ExportFileFormat.PlainText;
                else if (m_radioButtonCsv.Checked)
                    ec.ExportFileFormat = ExportFileFormat.CSV;
                else
                    Debug.Assert(false, "Not supported ExportFileFormat type");
                ec.CsvSeparator = m_textBoxSeparator.Text;

                PrintOptions po = new PrintOptions();
                po.PrintProjectIcons = m_checkBoxPrintIcons.Checked;
                po.ItemFont = m_userControlItemFont.FontDescription;
                po.HeadingFont = m_userControlHeadingFont.FontDescription;
                po.HeaderFont = m_userControlHeaderFont.FontDescription;
                ec.PrintOptions = po;
                return ec;
            }
            set {
                ExportConfiguration ec = value;
                m_versionsControl.AssemblyVersionTypes = ec.AssemblyVersionTypes;
                m_checkBoxIndentSubprojects.Checked = ec.IndentSubItems;
                m_numericUpDownIndentation.Value = ec.IndentSubItemsBy;
                m_checkBoxExcludeNonversionable.Checked = ec.ExcludeNonversionableItems;
                switch (ec.ExportFileFormat) {
                case (ExportFileFormat.PlainText):
                    m_radioButtonPlainText.Checked = true;
                    break;
                case (ExportFileFormat.CSV):
                    m_radioButtonCsv.Checked = true;
                    break;
                default:
                    Debug.Assert(false, "Not supported ExportFileFormat type");
                    break;
                }
                m_textBoxSeparator.Text = ec.CsvSeparator;

                PrintOptions po = ec.PrintOptions;
                m_checkBoxPrintIcons.Checked = po.PrintProjectIcons;
                m_userControlItemFont.FontDescription = po.ItemFont;
                m_userControlHeadingFont.FontDescription = po.HeadingFont;
                m_userControlHeaderFont.FontDescription = po.HeaderFont;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(VcbExportOptionsUserControl));
            this.m_groupBoxVersions = new System.Windows.Forms.GroupBox();
            this.m_versionsControl = new BuildAutoIncrement.VersionSelectUserControl();
            this.m_checkBoxIndentSubprojects = new System.Windows.Forms.CheckBox();
            this.m_numericUpDownIndentation = new System.Windows.Forms.NumericUpDown();
            this.m_checkBoxExcludeNonversionable = new System.Windows.Forms.CheckBox();
            this.m_groupBoxFonts = new System.Windows.Forms.GroupBox();
            this.m_userControlHeaderFont = new BuildAutoIncrement.FontSelectionUserControl();
            this.m_userControlHeadingFont = new BuildAutoIncrement.FontSelectionUserControl();
            this.m_userControlItemFont = new BuildAutoIncrement.FontSelectionUserControl();
            this.m_checkBoxPrintIcons = new System.Windows.Forms.CheckBox();
            this.m_labelPrintOptions = new BuildAutoIncrement.LabelWithDivider();
            this.label4 = new System.Windows.Forms.Label();
            this.m_groupBoxFileFormat = new System.Windows.Forms.GroupBox();
            this.m_textBoxSeparator = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_radioButtonCsv = new System.Windows.Forms.RadioButton();
            this.m_radioButtonPlainText = new System.Windows.Forms.RadioButton();
            this.m_groupBoxVersions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDownIndentation)).BeginInit();
            this.m_groupBoxFonts.SuspendLayout();
            this.m_groupBoxFileFormat.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_groupBoxVersions
            // 
            this.m_groupBoxVersions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_groupBoxVersions.Controls.Add(this.m_versionsControl);
            this.m_groupBoxVersions.Location = new System.Drawing.Point(0, 0);
            this.m_groupBoxVersions.Name = "m_groupBoxVersions";
            this.m_groupBoxVersions.Size = new System.Drawing.Size(296, 74);
            this.m_groupBoxVersions.TabIndex = 0;
            this.m_groupBoxVersions.TabStop = false;
            this.m_groupBoxVersions.Text = "&Versions to Include";
            // 
            // m_versionsControl
            // 
            this.m_versionsControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_versionsControl.Location = new System.Drawing.Point(8, 16);
            this.m_versionsControl.Name = "m_versionsControl";
            this.m_versionsControl.Size = new System.Drawing.Size(272, 49);
            this.m_versionsControl.TabIndex = 0;
            // 
            // m_checkBoxIndentSubprojects
            // 
            this.m_checkBoxIndentSubprojects.Checked = true;
            this.m_checkBoxIndentSubprojects.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_checkBoxIndentSubprojects.Location = new System.Drawing.Point(8, 82);
            this.m_checkBoxIndentSubprojects.Name = "m_checkBoxIndentSubprojects";
            this.m_checkBoxIndentSubprojects.Size = new System.Drawing.Size(128, 20);
            this.m_checkBoxIndentSubprojects.TabIndex = 1;
            this.m_checkBoxIndentSubprojects.Text = "Indent su&bprojects";
            this.m_checkBoxIndentSubprojects.CheckedChanged += new System.EventHandler(this.m_checkBoxIndentSubprojects_CheckedChanged);
            // 
            // m_numericUpDownIndentation
            // 
            this.m_numericUpDownIndentation.Location = new System.Drawing.Point(208, 82);
            this.m_numericUpDownIndentation.Maximum = new System.Decimal(new int[] {
                                                                                       10,
                                                                                       0,
                                                                                       0,
                                                                                       0});
            this.m_numericUpDownIndentation.Minimum = new System.Decimal(new int[] {
                                                                                       1,
                                                                                       0,
                                                                                       0,
                                                                                       0});
            this.m_numericUpDownIndentation.Name = "m_numericUpDownIndentation";
            this.m_numericUpDownIndentation.Size = new System.Drawing.Size(40, 20);
            this.m_numericUpDownIndentation.TabIndex = 3;
            this.m_numericUpDownIndentation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.m_numericUpDownIndentation.Value = new System.Decimal(new int[] {
                                                                                     1,
                                                                                     0,
                                                                                     0,
                                                                                     0});
            // 
            // m_checkBoxExcludeNonversionable
            // 
            this.m_checkBoxExcludeNonversionable.Checked = true;
            this.m_checkBoxExcludeNonversionable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_checkBoxExcludeNonversionable.Location = new System.Drawing.Point(8, 104);
            this.m_checkBoxExcludeNonversionable.Name = "m_checkBoxExcludeNonversionable";
            this.m_checkBoxExcludeNonversionable.Size = new System.Drawing.Size(240, 20);
            this.m_checkBoxExcludeNonversionable.TabIndex = 4;
            this.m_checkBoxExcludeNonversionable.Text = "Do not export &non-versionable items";
            // 
            // m_groupBoxFonts
            // 
            this.m_groupBoxFonts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_groupBoxFonts.Controls.Add(this.m_userControlHeaderFont);
            this.m_groupBoxFonts.Controls.Add(this.m_userControlHeadingFont);
            this.m_groupBoxFonts.Controls.Add(this.m_userControlItemFont);
            this.m_groupBoxFonts.Location = new System.Drawing.Point(0, 258);
            this.m_groupBoxFonts.Name = "m_groupBoxFonts";
            this.m_groupBoxFonts.Size = new System.Drawing.Size(296, 88);
            this.m_groupBoxFonts.TabIndex = 8;
            this.m_groupBoxFonts.TabStop = false;
            this.m_groupBoxFonts.Text = "Fonts";
            // 
            // m_userControlHeaderFont
            // 
            this.m_userControlHeaderFont.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_userControlHeaderFont.CaptionWidth = 60;
            this.m_userControlHeaderFont.Location = new System.Drawing.Point(8, 60);
            this.m_userControlHeaderFont.Name = "m_userControlHeaderFont";
            this.m_userControlHeaderFont.Size = new System.Drawing.Size(280, 20);
            this.m_userControlHeaderFont.TabIndex = 2;
            this.m_userControlHeaderFont.TabStop = false;
            this.m_userControlHeaderFont.Text = "Heade&r:";
            // 
            // m_userControlHeadingFont
            // 
            this.m_userControlHeadingFont.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_userControlHeadingFont.CaptionWidth = 60;
            this.m_userControlHeadingFont.Location = new System.Drawing.Point(8, 38);
            this.m_userControlHeadingFont.Name = "m_userControlHeadingFont";
            this.m_userControlHeadingFont.Size = new System.Drawing.Size(280, 20);
            this.m_userControlHeadingFont.TabIndex = 1;
            this.m_userControlHeadingFont.TabStop = false;
            this.m_userControlHeadingFont.Text = "Headin&g:";
            // 
            // m_userControlItemFont
            // 
            this.m_userControlItemFont.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_userControlItemFont.CaptionWidth = 60;
            this.m_userControlItemFont.Location = new System.Drawing.Point(8, 16);
            this.m_userControlItemFont.Name = "m_userControlItemFont";
            this.m_userControlItemFont.Size = new System.Drawing.Size(280, 20);
            this.m_userControlItemFont.TabIndex = 0;
            this.m_userControlItemFont.TabStop = false;
            this.m_userControlItemFont.Text = "Ite&m:";
            // 
            // m_checkBoxPrintIcons
            // 
            this.m_checkBoxPrintIcons.Checked = true;
            this.m_checkBoxPrintIcons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.m_checkBoxPrintIcons.Location = new System.Drawing.Point(8, 234);
            this.m_checkBoxPrintIcons.Name = "m_checkBoxPrintIcons";
            this.m_checkBoxPrintIcons.Size = new System.Drawing.Size(240, 20);
            this.m_checkBoxPrintIcons.TabIndex = 7;
            this.m_checkBoxPrintIcons.Text = "Print project ic&ons";
            // 
            // m_labelPrintOptions
            // 
            this.m_labelPrintOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_labelPrintOptions.Location = new System.Drawing.Point(0, 212);
            this.m_labelPrintOptions.Name = "m_labelPrintOptions";
            this.m_labelPrintOptions.Size = new System.Drawing.Size(296, 20);
            this.m_labelPrintOptions.TabIndex = 6;
            this.m_labelPrintOptions.Text = "Print Options";
            this.m_labelPrintOptions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(152, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 20);
            this.label4.TabIndex = 2;
            this.label4.Text = "Indent &by:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_groupBoxFileFormat
            // 
            this.m_groupBoxFileFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_groupBoxFileFormat.Controls.Add(this.m_textBoxSeparator);
            this.m_groupBoxFileFormat.Controls.Add(this.label1);
            this.m_groupBoxFileFormat.Controls.Add(this.m_radioButtonCsv);
            this.m_groupBoxFileFormat.Controls.Add(this.m_radioButtonPlainText);
            this.m_groupBoxFileFormat.Location = new System.Drawing.Point(0, 130);
            this.m_groupBoxFileFormat.Name = "m_groupBoxFileFormat";
            this.m_groupBoxFileFormat.Size = new System.Drawing.Size(296, 72);
            this.m_groupBoxFileFormat.TabIndex = 5;
            this.m_groupBoxFileFormat.TabStop = false;
            this.m_groupBoxFileFormat.Text = "Export File Format";
            // 
            // m_textBoxSeparator
            // 
            this.m_textBoxSeparator.Enabled = false;
            this.m_textBoxSeparator.Location = new System.Drawing.Point(242, 42);
            this.m_textBoxSeparator.Name = "m_textBoxSeparator";
            this.m_textBoxSeparator.Size = new System.Drawing.Size(24, 20);
            this.m_textBoxSeparator.TabIndex = 3;
            this.m_textBoxSeparator.Text = "";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(176, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "&Separator:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // m_radioButtonCsv
            // 
            this.m_radioButtonCsv.Location = new System.Drawing.Point(8, 42);
            this.m_radioButtonCsv.Name = "m_radioButtonCsv";
            this.m_radioButtonCsv.Size = new System.Drawing.Size(168, 20);
            this.m_radioButtonCsv.TabIndex = 1;
            this.m_radioButtonCsv.Text = "&Comma Separated Values";
            this.m_radioButtonCsv.CheckedChanged += new System.EventHandler(this.m_radioButtonCsv_CheckedChanged);
            // 
            // m_radioButtonPlainText
            // 
            this.m_radioButtonPlainText.Checked = true;
            this.m_radioButtonPlainText.Location = new System.Drawing.Point(8, 20);
            this.m_radioButtonPlainText.Name = "m_radioButtonPlainText";
            this.m_radioButtonPlainText.Size = new System.Drawing.Size(168, 20);
            this.m_radioButtonPlainText.TabIndex = 0;
            this.m_radioButtonPlainText.TabStop = true;
            this.m_radioButtonPlainText.Text = "Plain &text";
            // 
            // VcbExportOptionsUserControl
            // 
            this.Controls.Add(this.m_groupBoxFileFormat);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.m_labelPrintOptions);
            this.Controls.Add(this.m_checkBoxExcludeNonversionable);
            this.Controls.Add(this.m_numericUpDownIndentation);
            this.Controls.Add(this.m_checkBoxIndentSubprojects);
            this.Controls.Add(this.m_groupBoxVersions);
            this.Controls.Add(this.m_groupBoxFonts);
            this.Controls.Add(this.m_checkBoxPrintIcons);
            this.Name = "VcbExportOptionsUserControl";
            this.Size = new System.Drawing.Size(296, 348);
            this.m_groupBoxVersions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDownIndentation)).EndInit();
            this.m_groupBoxFonts.ResumeLayout(false);
            this.m_groupBoxFileFormat.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private void m_checkBoxIndentSubprojects_CheckedChanged(object sender, System.EventArgs e) {
            m_numericUpDownIndentation.Enabled = m_checkBoxIndentSubprojects.Checked;
        }

        private void m_radioButtonCsv_CheckedChanged(object sender, System.EventArgs e) {
            m_textBoxSeparator.Enabled = m_radioButtonCsv.Checked;
        }
	}
}
