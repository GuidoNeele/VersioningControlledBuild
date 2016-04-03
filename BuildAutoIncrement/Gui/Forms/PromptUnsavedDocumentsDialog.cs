/*
 * Filename:    PromptUnsavedDocumentsDialog.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     AddinImplementation
 * Description: Dialog that displays the list of unsaved files before starting 
 *              the add-in.
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
using EnvDTE;

#if !FX1_1
using EnvDTE80;
using DTE = EnvDTE80.DTE2;
#endif

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace BuildAutoIncrement {
	/// <summary>
	/// Dialog displayed before application is actually run, hirarchically 
	/// displaying the list of unsaved documents and their corresponding projects.
	/// </summary>
	public class PromptUnsavedDocumentsDialog : System.Windows.Forms.Form {

        #region Controls
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button m_buttonYes;
        private System.Windows.Forms.Button m_buttonNo;
        private System.Windows.Forms.Button m_buttonCancel;
        private System.Windows.Forms.ListBox m_listBoxUnsavedDocuments;
        #endregion // Controls
        
        /// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        #region Constructors
        /// <summary>
        ///   Hides empty constructor.
        /// </summary>
        private PromptUnsavedDocumentsDialog(){
            InitializeComponent();
        }
        
        /// <summary>
        ///   Creates a dialog.
        /// </summary>
        /// <param name="unsavedDocuments">
        ///   Array of unsaved <c>Document</c> objects.
        /// </param>
        private PromptUnsavedDocumentsDialog(IList unsavedDocuments) : this() {
            FillListBox(unsavedDocuments);
        }

        #endregion // Constructors

        #region Dispose method
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
        #endregion // Dispose method

        #region Private methods
        /// <summary>
        /// Fills lisbox with unsaved documents preceeded with corresponding
        /// projects and parent solution names.
        /// </summary>
        /// <param name="unsavedDocuments"></param>
        private void FillListBox(IList unsavedDocuments) {
            m_listBoxUnsavedDocuments.BeginUpdate();
            foreach (Document document in unsavedDocuments) {
                AddDocumentItem(document);
            }
            m_listBoxUnsavedDocuments.EndUpdate();
        }

        /// <summary>
        /// Adds a document to the listbox. Document name is indented and 
        /// preceeded with hierarchically outlined corresponding project name 
        /// and solution name.
        /// </summary>
        /// <param name="document"></param>
        private void AddDocumentItem(Document document) {
            Project project = document.ProjectItem.ContainingProject;
            string projectName = project.Name;
            Solution solution = project.DTE.Solution;
            SolutionLBItem newSolutionLBItem = new SolutionLBItem(solution);
            if (m_listBoxUnsavedDocuments.FindStringExact(newSolutionLBItem.ToString()) == -1) {
                m_listBoxUnsavedDocuments.Items.Add(newSolutionLBItem);
            }
            ProjectLBItem newProjectLBItem = new ProjectLBItem(project);
            int index = m_listBoxUnsavedDocuments.FindStringExact(newProjectLBItem.ToString());
            if (index == -1) {
                index = m_listBoxUnsavedDocuments.Items.Add(newProjectLBItem);
            }
            m_listBoxUnsavedDocuments.Items.Insert(index+1, new DocumentLBItem(document));
        }

        #endregion // Private methods

        #region Message handlers

        /// <summary>
        /// Yes button message handler. Saves all selected documents.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonYes_Click(object sender, System.EventArgs e) {
            foreach (object item in m_listBoxUnsavedDocuments.SelectedItems) {
                if (item is DocumentLBItem) {
                    DocumentLBItem docItem = (DocumentLBItem)item;
                    docItem.Value.Save(docItem.Value.FullName);
                }
                    // this is probably superflous
                else if (item is ProjectLBItem) {
                    ProjectLBItem projectItem = (ProjectLBItem)item;
                    projectItem.Value.Save(projectItem.Value.FullName);
                }
            }
            this.Close();
        }

        /// <summary>
        /// Load event handler. Selects all items in the list box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PromptUnsavedDocumentsDialog_Load(object sender, System.EventArgs e) {
            // selects all items in the listbox
            for (int i = 0; i < m_listBoxUnsavedDocuments.Items.Count; i++) {
                m_listBoxUnsavedDocuments.SetSelected(i, true);
            }
        }

        /// <summary>
        /// No button click handler. Just closes the dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonNo_Click(object sender, System.EventArgs e) {
            this.Close();
        }

        #endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.m_listBoxUnsavedDocuments = new System.Windows.Forms.ListBox();
            this.m_buttonYes = new System.Windows.Forms.Button();
            this.m_buttonNo = new System.Windows.Forms.Button();
            this.m_buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Save changes to the following items?";
            // 
            // m_listBoxUnsavedDocuments
            // 
            this.m_listBoxUnsavedDocuments.Location = new System.Drawing.Point(8, 24);
            this.m_listBoxUnsavedDocuments.Name = "m_listBoxUnsavedDocuments";
            this.m_listBoxUnsavedDocuments.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.m_listBoxUnsavedDocuments.Size = new System.Drawing.Size(360, 160);
            this.m_listBoxUnsavedDocuments.TabIndex = 1;
            // 
            // m_buttonYes
            // 
            this.m_buttonYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.m_buttonYes.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.m_buttonYes.Location = new System.Drawing.Point(128, 208);
            this.m_buttonYes.Name = "m_buttonYes";
            this.m_buttonYes.TabIndex = 2;
            this.m_buttonYes.Text = "&Yes";
            this.m_buttonYes.Click += new System.EventHandler(this.m_buttonYes_Click);
            // 
            // m_buttonNo
            // 
            this.m_buttonNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.m_buttonNo.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.m_buttonNo.Location = new System.Drawing.Point(208, 208);
            this.m_buttonNo.Name = "m_buttonNo";
            this.m_buttonNo.TabIndex = 3;
            this.m_buttonNo.Text = "&No";
            this.m_buttonNo.Click += new System.EventHandler(this.m_buttonNo_Click);
            // 
            // m_buttonCancel
            // 
            this.m_buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.m_buttonCancel.Location = new System.Drawing.Point(288, 208);
            this.m_buttonCancel.Name = "m_buttonCancel";
            this.m_buttonCancel.TabIndex = 4;
            this.m_buttonCancel.Text = "Cancel";
            // 
            // PromptUnsavedDocumentsDialog
            // 
            this.AcceptButton = this.m_buttonYes;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.m_buttonCancel;
            this.ClientSize = new System.Drawing.Size(378, 247);
            this.Controls.Add(this.m_buttonCancel);
            this.Controls.Add(this.m_buttonNo);
            this.Controls.Add(this.m_buttonYes);
            this.Controls.Add(this.m_listBoxUnsavedDocuments);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PromptUnsavedDocumentsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Microsoft Development Environment";
            this.Load += new System.EventHandler(this.PromptUnsavedDocumentsDialog_Load);
            this.ResumeLayout(false);

        }
		#endregion

        #region Helper classes

        private class DocumentLBItem {
            public DocumentLBItem(Document document) {
                m_document = document;
            }

            public override string ToString() {
                return "    " + m_document.Name;
            }

            public Document Value {
                get { return m_document; }
            }

            private Document m_document;
        }

        private class ProjectLBItem {
            public ProjectLBItem(Project project) {
                m_project = project;
            }

            public override string ToString() {
                return "  " + m_project.Name;
            }

            public Project Value {
                get { return m_project; }
            }

            private Project m_project;
        }

        private class SolutionLBItem {
            public SolutionLBItem(Solution solution) {
                m_solution = solution;
            }

            public override string ToString() {
                return Path.GetFileName(m_solution.FullName);
            }

            public Solution Value {
                get { return m_solution; }
            }

            private Solution m_solution;
        }
        
        #endregion

        public static DialogResult Show(IWin32Window owner, IList unsavedDocuments) {
            PromptUnsavedDocumentsDialog pudd = new PromptUnsavedDocumentsDialog(unsavedDocuments);
            pudd.StartPosition = FormStartPosition.CenterParent;
            return pudd.ShowDialog(owner);
        }
    }
}