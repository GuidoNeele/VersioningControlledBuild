/*
 * Filename:    AboutBox.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: About form.
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
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace BuildAutoIncrement {
  /// <summary>
  ///   Displays information related to tool and runtime environment.
  /// </summary>
  public class AboutBox : System.Windows.Forms.Form {

        #region Controls
        private System.Windows.Forms.Label m_labelFrameworkVersion;
        private System.Windows.Forms.Label m_labelCopyright;
        private System.Windows.Forms.Label m_labelApplicationName;
        private System.Windows.Forms.Button m_buttonOK;
        private System.Windows.Forms.Label m_labelVersion;
        private System.Windows.Forms.LinkLabel m_linkLabel;
        #endregion // Controls

        /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

        #region Constructors

        private AboutBox() {
            InitializeComponent();
            
            m_labelFrameworkVersion.Text = String.Format(CultureInfo.CurrentCulture, ".NET Framework: {0}", Environment.Version.ToString());

            Version version = Assembly.LoadFrom(GetImplementationAssembly()).GetName().Version;
            m_labelVersion.Text = version.ToString();
        }

        #endregion // Constructors

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

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.m_labelFrameworkVersion = new System.Windows.Forms.Label();
      this.m_labelCopyright = new System.Windows.Forms.Label();
      this.m_labelApplicationName = new System.Windows.Forms.Label();
      this.m_buttonOK = new System.Windows.Forms.Button();
      this.m_labelVersion = new System.Windows.Forms.Label();
      this.m_linkLabel = new System.Windows.Forms.LinkLabel();
      this.SuspendLayout();
      // 
      // m_labelFrameworkVersion
      // 
      this.m_labelFrameworkVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.m_labelFrameworkVersion.Location = new System.Drawing.Point(80, 159);
      this.m_labelFrameworkVersion.Name = "m_labelFrameworkVersion";
      this.m_labelFrameworkVersion.Size = new System.Drawing.Size(200, 23);
      this.m_labelFrameworkVersion.TabIndex = 13;
      this.m_labelFrameworkVersion.Text = ".NET Framework: ";
      this.m_labelFrameworkVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // m_labelCopyright
      // 
      this.m_labelCopyright.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.m_labelCopyright.Location = new System.Drawing.Point(62, 96);
      this.m_labelCopyright.Name = "m_labelCopyright";
      this.m_labelCopyright.Size = new System.Drawing.Size(237, 55);
      this.m_labelCopyright.TabIndex = 11;
      this.m_labelCopyright.Text = "Copyright © Guido Neele 2016 \n\nBased on the excellent work of Julijan Sribar";
      this.m_labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // m_labelApplicationName
      // 
      this.m_labelApplicationName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.m_labelApplicationName.Location = new System.Drawing.Point(80, 9);
      this.m_labelApplicationName.Name = "m_labelApplicationName";
      this.m_labelApplicationName.Size = new System.Drawing.Size(200, 24);
      this.m_labelApplicationName.TabIndex = 9;
      this.m_labelApplicationName.Text = "Versioning Controlled Build";
      this.m_labelApplicationName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // m_buttonOK
      // 
      this.m_buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_buttonOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.m_buttonOK.Location = new System.Drawing.Point(143, 199);
      this.m_buttonOK.Name = "m_buttonOK";
      this.m_buttonOK.Size = new System.Drawing.Size(75, 23);
      this.m_buttonOK.TabIndex = 8;
      this.m_buttonOK.Text = "OK";
      // 
      // m_labelVersion
      // 
      this.m_labelVersion.Location = new System.Drawing.Point(22, 33);
      this.m_labelVersion.Name = "m_labelVersion";
      this.m_labelVersion.Size = new System.Drawing.Size(317, 32);
      this.m_labelVersion.TabIndex = 14;
      this.m_labelVersion.Text = "ver. ";
      this.m_labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // m_linkLabel
      // 
      this.m_linkLabel.AutoSize = true;
      this.m_linkLabel.Location = new System.Drawing.Point(25, 72);
      this.m_linkLabel.Name = "m_linkLabel";
      this.m_linkLabel.Size = new System.Drawing.Size(311, 13);
      this.m_linkLabel.TabIndex = 15;
      this.m_linkLabel.TabStop = true;
      this.m_linkLabel.Text = "www.codeproject.com/Articles/5851/Versioning-Controlled-Build";
      this.m_linkLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.m_linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.m_linkLabel_LinkClicked);
      // 
      // AboutBox
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.CancelButton = this.m_buttonOK;
      this.ClientSize = new System.Drawing.Size(361, 241);
      this.Controls.Add(this.m_linkLabel);
      this.Controls.Add(this.m_labelVersion);
      this.Controls.Add(this.m_labelFrameworkVersion);
      this.Controls.Add(this.m_labelCopyright);
      this.Controls.Add(this.m_labelApplicationName);
      this.Controls.Add(this.m_buttonOK);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutBox";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "About Versioning Controlled Build";
      this.ResumeLayout(false);
      this.PerformLayout();

        }
    #endregion

        #region Private methods

        /// <summary>
        ///   Opens link in browser.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_linkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e) {
            LinkLabel ll = sender as LinkLabel;
            Debug.Assert(ll != null);
            System.Diagnostics.Process.Start(ll.Text);
        }

        private string GetImplementationAssembly()
        {
            foreach (Assembly currentassembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = currentassembly.GetType("BuildAutoIncrement.VcbCommandPackage");
                if (t != null)
                    return currentassembly.Location;
            }
            Debug.Assert(false, "No implementation file found");
            return string.Empty;
        }

    #endregion // Private methods

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public static DialogResult Show(IWin32Window owner)
    {
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
      AboutBox ab = new AboutBox();
            ab.StartPosition = FormStartPosition.CenterParent;
            return ab.ShowDialog(owner);
        }

    }
}