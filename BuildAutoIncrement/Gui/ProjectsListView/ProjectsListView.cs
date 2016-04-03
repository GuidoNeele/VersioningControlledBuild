/*
 * Filename:    ProjectsListView.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: ListView derived control to display project details.
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
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BuildAutoIncrement {

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ListView))]
    public class ProjectsListView : System.Windows.Forms.ListView {

        #region CheckBoxRenderer
        /// <summary>
        ///   Class renders checkboxes in <c>ProjectsListView</c> in accordance
        ///   with visual style. If visual styles are not supported or disabled
        ///   then regular <c>ControlPaint.DrawCheckBox</c> method is used.
        /// </summary>
        private class CheckBoxRenderer : IDisposable {

            /// <summary>
            ///   <c>Button</c> parts for <c>DrawThemeBackground</c> method.
            /// </summary>
            private enum ButtonParts {
                PushButton = 1,
                RadioButton = 2,
                CheckBox = 3,
                GroupBox = 4,
                UserBox = 5
            }

            /// <summary>
            ///   CheckBox states for <c>DrawThemeBackground</c> method.
            /// </summary>
            private enum CheckBoxStates {
                UncheckedNormal = 1,
                UncheckedHot = 2,
                UncheckedPressed = 3,
                UncheckedDisabled = 4,
                CheckedNormal = 5,
                CheckedHot = 6,
                CheckedPressed = 7,
                CheckedDisabled = 8,
                MixedNormal = 9,
                MixedHot = 10,
                MixedPressed = 11
            }

            /// <summary>
            ///   Creates an instance.
            /// </summary>
            /// <param name="parentHandle">
            ///   Handle of the parent listview.
            /// </param>
            public CheckBoxRenderer(IntPtr parentHandle) {
                Debug.Assert(parentHandle != IntPtr.Zero);
                if (VisualStyles.Supported) {
                    m_hTheme = Win32Api.OpenThemeData(parentHandle, "Button");
                    Debug.Assert(m_hTheme != IntPtr.Zero);
                }
                else
                    m_hTheme = IntPtr.Zero;
            }

            /// <summary>
            ///   Finalize method.
            /// </summary>
            ~CheckBoxRenderer() {
                Dispose(false);
            }

            /// <summary>
            ///   Dispose method.
            /// </summary>
            public void Dispose() {
                GC.SuppressFinalize(this);
                Dispose(true);
            }

            /// <summary>
            ///   Disposes used resources. Calls <c>CloseThemeData</c> method
            ///   if necessary.
            /// </summary>
            /// <param name="disposing"></param>
            private void Dispose(bool disposing) {
                if (!m_disposed) {
                    if (m_hTheme != IntPtr.Zero)
                        Win32Api.CloseThemeData(m_hTheme);
                    m_disposed = true;
                }
            }

            /// <summary>
            ///   Draws checkbox.
            /// </summary>
            /// <param name="graphics"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="bs"></param>
            public void DrawCheckBox(Graphics graphics, int x, int y, int width, int height, ButtonState bs) {
                if (m_hTheme != IntPtr.Zero && VisualStyles.Enabled) {
                    CheckBoxStates state = CheckBoxStates.UncheckedNormal;
                    if ((bs & ButtonState.Checked) == ButtonState.Checked)
                        state = CheckBoxStates.CheckedNormal;
                    else if ((bs & ButtonState.Inactive) == ButtonState.Inactive)
                        state = CheckBoxStates.UncheckedDisabled;
                    Win32Api.RECT rect = new Win32Api.RECT();
                    rect.left = x;
                    rect.top = y;
                    rect.right = x + width;
                    rect.bottom = y + height;
                    IntPtr hdc = graphics.GetHdc();
                    Win32Api.DrawThemeBackground(m_hTheme, hdc, (int)ButtonParts.CheckBox, (int)state, ref rect, ref rect);
                    graphics.ReleaseHdc(hdc);
                }
                else {
                    ControlPaint.DrawCheckBox(graphics, x, y, width, height, bs);
                }
            }

            private IntPtr m_hTheme;

            private bool m_disposed;
        }

        #endregion // CheckBoxRenderer

        #region Embedded TextBox

        /// <summary>
        ///   Text box used to manually edit "to be version".
        /// </summary>
        private class EmbeddedTextBox : System.Windows.Forms.TextBox {

            public EmbeddedTextBox() : base() {
                // multiline to allow smaller size than for single line
                // so that control fits into listview item row
                Multiline = true;
                WordWrap = false;
            }

            public void Activate(Rectangle bounds, string text) {
                Text = text;
                Bounds = bounds;
                Visible = true;
                BringToFront();
                Focus();
                SelectAll();
            }

            public void Deactivate() {
                Visible = false;
                Parent.Focus();
            }

            /// <summary>
            ///   Overriden to prevent entering new line character (control is 
            ///   set to multiline to allow smaller height than for a single
            ///   line control).
            /// </summary>
            /// <param name="e"></param>
            protected override void OnKeyPress(KeyPressEventArgs e) {
                if (e.KeyChar == (char)(int)Keys.Enter)
                    e.Handled = true;
                /*
                else if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar) || e.KeyChar == '*' || e.KeyChar == '.')
                    e.Handled = false;
                */
                else
                    e.Handled = false;
                base.OnKeyPress(e);
            }

            /// <summary>
            ///   Overriden to prevent pasting multiline text.
            /// </summary>
            /// <param name="msg"></param>
            protected override void WndProc(ref Message msg) {
                // prevents pasting multiline text
                if (msg.Msg == (int)Win32Api.WM.PASTE) {
                    if (!Clipboard.GetDataObject().GetDataPresent(typeof(string)))
                        return;
                    object clipBoardContent = Clipboard.GetDataObject().GetData(typeof(string));
                    string textToPaste = (string)Clipboard.GetDataObject().GetData(typeof(string));
                    int endOfLine = textToPaste.IndexOf(Environment.NewLine, 0);
                    if (endOfLine > -1) {
                        int toRemove = textToPaste.Length - endOfLine;
                        textToPaste = textToPaste.Remove(endOfLine, toRemove);
                    }
                    Clipboard.SetDataObject(textToPaste);
                    base.WndProc(ref msg);
                    Clipboard.SetDataObject(clipBoardContent, true);
                    return;
                }
                base.WndProc(ref msg);
            }

            /// <summary>
            ///   Overridden to handle Ctrl + A key combination to select
            ///   entire text.
            /// </summary>
            /// <param name="msg"></param>
            /// <returns></returns>
            public override bool PreProcessMessage(ref Message msg) {
                if (msg.Msg == (int)Win32Api.WM.KEYDOWN) {
                    Keys keyData = ((Keys) (int) msg.WParam) | ModifierKeys;
                    Keys keyCode = ((Keys) (int) msg.WParam);
    
                    switch (keyCode) {
                    case Keys.A:
                        if ((ModifierKeys & Keys.Control) != 0) {
                            SelectAll();
                            return true;
                        }
                        break;
                    }
                }
                return base.PreProcessMessage (ref msg);
            }
        }

        #endregion // Embedded TextBox

        #region ProjectsListViewItem

        /// <summary>
        ///   Internal class substituting <c>ListViewItem</c> in order to 
        ///   provide "safe" checking (i.e. allows checking only if an item 
        ///   has a valid assembly version). This is accomplished through 
        ///   new <c>Checked</c> property.
        /// </summary>
        private class ProjectsListViewItem : ListViewItem {

            protected ProjectsListViewItem() : base() {
            }

            /// <summary>
            ///   Creates <c>ProjectsListViewItem</c> for a given <c>ProjectInfo</c>
            ///   and <c>AssemblyVersionType</c>.
            /// </summary>
            /// <param name="projectInfo">
            ///   <c>ProjectInfo</c> object that is associated with this
            ///   ListView item.
            /// </param>
            /// <param name="assemblyVersionType">
            ///   <c>AssemblyVersionType</c> for which this item contains data.
            /// </param>
            public ProjectsListViewItem(ProjectInfo projectInfo, AssemblyVersionType assemblyVersionType) : base(projectInfo.ProjectName) {
                Debug.Assert(projectInfo != null);
                Debug.Assert(assemblyVersionType != AssemblyVersionType.All);

                ProjectInfo = projectInfo;
                CurrentVersion = ProjectInfo.CurrentAssemblyVersions[assemblyVersionType];
                string[] subItems = CreateSubItems(assemblyVersionType);
                Debug.Assert(subItems.Length == 3);
                SubItems.AddRange(subItems);
                Checked = IsAssemblyVersionDefined && projectInfo.IsMarkedForUpdate(assemblyVersionType);
                SetImageIndex(Checked);

                Debug.Assert(ProjectInfo != null);
                Debug.Assert(CurrentVersion != null);
            }

            /// <summary>
            ///   Gets or sets the new checked state. Item can be checked only 
            ///   if it contains a valid assembly version.
            /// </summary>
            public new bool Checked {
                get {
                    return base.Checked;
                }
                set {
                    if (IsAssemblyVersionDefined && value != base.Checked) {
                        base.Checked = value;
                        SetImageIndex(value);
                    }
                }
            }

            /// <summary>
            ///   Flag indicating that proposed "to be version" is lower than 
            ///   current version. Used to draw "to be version" in different color.
            /// </summary>
            public bool PossiblyInvalidToBeVersion = false;

            /// <summary>
            ///   <c>ProjectInfo</c> object assigned to the listview item.
            /// </summary>
            public readonly ProjectInfo ProjectInfo;

            /// <summary>
            ///   Current version of the assembly version.
            /// </summary>
            public readonly ProjectVersion CurrentVersion = null;

            /// <summary>
            ///   Gets a flag indicating if current version is assigned (i.e. 
            ///   assembly version does exist).
            /// </summary>
            public bool IsAssemblyVersionDefined {
                get {
                    Debug.Assert(CurrentVersion != null);
                    return CurrentVersion != ProjectVersion.Empty;
                }
            }

            /// <summary>
            ///   Creates subitem strings for the item.
            /// </summary>
            /// <param name="assemblyVersionType">
            ///   <c>AssemblyVersionType</c> for which subitems should be 
            ///   created.
            /// </param>
            /// <returns>
            ///   Array of strings.
            /// </returns>
            private string[] CreateSubItems(AssemblyVersionType assemblyVersionType) {
                if (IsAssemblyVersionDefined) {
                    return new string[] { 
                                            ProjectInfo.CurrentAssemblyVersions[assemblyVersionType].ToString(), 
                                            ProjectInfo.VersionFileWrite.ToString(),
                                            ProjectInfo.ToBecomeAssemblyVersions[assemblyVersionType].ToString() 
                                        };
                }
                Debug.Assert(!IsAssemblyVersionDefined);
                if (ProjectInfo.AssemblyFileExists) {
                    return new string[] { 
                                            ProjectInfo.CurrentAssemblyVersions[assemblyVersionType].Valid ? s_txtVersionNotFound : ProjectInfo.CurrentAssemblyVersions[assemblyVersionType].ToString(),
                                            ProjectInfo.VersionFileWrite.ToString(),
                                            "" 
                                        };
                }
                return new string[] { 
                                        "",
                                        s_txtFileNotFound, 
                                        "" 
                                    };
            }

            /// <summary>
            ///   Adjusts image index associated with the project type and
            ///   check state.
            /// </summary>
            public void SetImageIndex(bool isChecked) {
                if (isChecked)
                    ImageIndex = ProjectInfo.ProjectTypeInfo.IconIndex;
                else
                    ImageIndex = ProjectInfo.ProjectTypeInfo.IconIndex + 1;
            }
        }

        #endregion // ProjectsListViewItem

        #region ListView column headers

        private System.Windows.Forms.ColumnHeader m_columnHeaderName;
        private System.Windows.Forms.ColumnHeader m_columnHeaderCurrentVersion;
        private System.Windows.Forms.ColumnHeader m_columnHeaderToBecomeVersion;
        private System.Windows.Forms.ColumnHeader m_columnHeaderLastModified;

        #endregion // ListView column headers

        private System.ComponentModel.IContainer components;

        /// <summary>
        ///   Creates empty <c>ProjectsListView</c> control.
        /// </summary>
        public ProjectsListView() : base() {
            InitializeComponent();
            // sets ListView properties
            View                        = View.Details;
            CheckBoxes                  = true;
            FullRowSelect               = true;
            HideSelection               = false;
            LabelEdit                   = false;
            // column indices
            ProjectNameColumnIndex      = Columns.IndexOf(m_columnHeaderName);
            ModifiedColumnIndex         = Columns.IndexOf(m_columnHeaderLastModified);
            ToBecomeVersionColumnIndex  = Columns.IndexOf(m_columnHeaderToBecomeVersion);
            CurrentVersionColumnIndex   = Columns.IndexOf(m_columnHeaderCurrentVersion);
            // initializes embedded textbox for in-line editing
            m_editControl = new EmbeddedTextBox();
            m_editControl.Visible = false;
            m_editControl.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(m_editControl);
        }

        #region Public properties and methods

        /// <summary>
        ///   Gets and sets assembly version type displayed in the listview.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public AssemblyVersionType AssemblyVersionType { 
            get {
                Debug.Assert(Tag != null);
                return (AssemblyVersionType)Tag;
            }
            set {
                Debug.Assert(value != AssemblyVersionType.All);
                Tag = value;
            }
        }

        /// <summary>
        ///   Gets a flag indicating if selection contains only one item and if
        ///   it contains a valid version. Used from the main form to enable or 
        ///   disable "Get selected" button.
        /// </summary>
        public bool IsSingleSelectionWithValidAssemblyVersion {
            get {
                return SelectedItems.Count == 1 && ((ProjectsListViewItem)SelectedItems[0]).IsAssemblyVersionDefined;
            }
        }

        /// <summary>
        ///   Gets an array of all marked <c>ProjectInfo</c> items.
        /// </summary>
        public ProjectInfo[] MarkedProjectInfos {
            get {
                ProjectInfo[] markedItems = new ProjectInfo[CheckedItemsCount];
                int i = 0;
                foreach (ProjectsListViewItem plvi in CheckedItems) {
                    markedItems[i] = plvi.ProjectInfo;
                    i++;
                }
                return markedItems;
            }
        }

        public int CheckedItemsCount {
            get { 
                int checkedItemsCount = 0;
                foreach (ListViewItem item in Items) {
                    if (item.Checked)
                        checkedItemsCount++;
                }
                return checkedItemsCount; }
        }

        /// <summary>
        ///   Checks/unchecks an item. Item is checked only if it has a valid 
        ///   assembly version.
        /// </summary>
        /// <param name="index">
        ///   Index of the item to check or uncheck.
        /// </param>
        /// <param name="newCheckState">
        ///   New <c>CheckState</c>.
        /// </param>
        public void DoSafeCheck(int index, CheckState newCheckState) {
            ProjectsListViewItem lvi = (ProjectsListViewItem)Items[index];
            lvi.Checked = newCheckState == CheckState.Checked;
        }

        /// <summary>
        ///   Checks if all items have a lower version than one provided.
        /// </summary>
        /// <param name="includeNotMarked">
        ///   Flag indicating if items not checked should be included too.
        ///   If <c>true</c> all items are compared. Otherwise, only checked
        ///   items are compared.
        /// </param>
        /// <param name="versionToCompareTo">
        ///   Version string to which versions in the listview should be
        ///   compared.
        /// </param>
        /// <returns>
        ///   <c>true</c> if all items scanned have lower version.
        /// </returns>
        public bool HaveAllListViewItemsLowerCurrentVersion(bool includeNotMarked, string versionToCompareTo) {
            foreach (ProjectsListViewItem lvi in Items) {
                if (lvi.Checked || includeNotMarked) {
                    ProjectVersion version = lvi.ProjectInfo.CurrentAssemblyVersions[AssemblyVersionType];
                    if (version.CompareToPattern(versionToCompareTo) > 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Gets selected assembly version.
        /// </summary>
        /// <returns>
        ///   If selected item is checked (i.e. marked for update) "to be 
        ///   version" is returned. Otherwise, the current version is returned.
        /// </returns>
        public ProjectVersion GetSelectedVersion() {
            ProjectsListViewItem firstSelectedItem = (ProjectsListViewItem)SelectedItems[0];
            if (firstSelectedItem.Checked)
                return new ProjectVersion(firstSelectedItem.SubItems[ToBecomeVersionColumnIndex].Text, this.AssemblyVersionType);
            return firstSelectedItem.CurrentVersion;
        }

        #endregion // Public properties and methods

        #region Overriden public properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new System.Windows.Forms.View View {
            get { return View.Details; }
            set { base.View = View.Details; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new bool CheckBoxes {
            get { return true; }
            set { base.CheckBoxes = true; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new bool FullRowSelect {
            get { return true; }
            set { base.FullRowSelect = true; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new bool HideSelection {
            get { return false; }
            set { base.HideSelection = false; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new bool LabelEdit {
            get { return false; }
            set { base.LabelEdit = false; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new int[] SelectedIndices {
            get {
                int[] selectedIndices = new int[SelectedItems.Count];
                base.SelectedIndices.CopyTo(selectedIndices, 0);
                return selectedIndices;
            }
            set {
                for (int i = 0; i < Items.Count; i++) {
                    Items[i].Selected = Array.IndexOf(value, i) != -1;
                }
            }
        }

        #endregion // Overriden public properties

        #region Overriden methods

        public new void BeginUpdate() {
            m_batchUpdate = true;
            base.BeginUpdate();
        }

        public new void EndUpdate() {
            m_batchUpdate = false;
            base.EndUpdate();
        }

        /// <summary>
        ///   Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///   Override to create <c>CheckBoxRender</c> member.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            m_checkBoxRenderer = new CheckBoxRenderer(this.Handle);
            Debug.Assert(m_checkBoxRenderer != null);
        }

        /// <summary>
        ///   Override to dispose <c>CheckBoxRender</c> member.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleDestroyed(EventArgs e) {
            m_checkBoxRenderer.Dispose();
            base.OnHandleDestroyed(e);
        }

        /// <summary>
        ///   Prevents checking items with no valid version. Also, changes
        ///   the color of the item when check state changes.
        /// </summary>
        /// <param name="ice"></param>
        protected override void OnItemCheck(ItemCheckEventArgs ice) {
            ProjectsListViewItem lvi = null;
            if (ice.Index < Items.Count) {
                lvi = (ProjectsListViewItem)Items[ice.Index];
                if (lvi.SubItems[ToBecomeVersionColumnIndex].Text.Length == 0) {
                    ice.NewValue = CheckState.Unchecked;
                    if (SelectedItems.Count == 0)
                        lvi.Focused = true;
                    return;
                }
                else {
                    bool newChecked = ice.NewValue == CheckState.Checked;
                    if (lvi.Checked != newChecked) {
                        ColorItem(lvi, newChecked);
                        lvi.SetImageIndex(newChecked);
                    }
                }
            }
            base.OnItemCheck(ice);
            if (lvi != null) {
                if (SelectedItems.Count == 0)
                    lvi.Focused = true;
            }
        }

        /// <summary>
        ///   Overriden to implement custom drawing. Also prevents automatic 
        ///   setting focus on an item when ListView receives focus (and no 
        ///   item is actually selected). m_updating flag filters drawing to
        ///   only single item marked for update, in order to reduce flicker
        ///   when item fore color is changed.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
            case (int)Win32Api.WM.ERASEBKGND:
                // reduces flickering
                if (m_updating)
                    m.Msg = (int)Win32Api.WM.NULL;
                break;
            case (int)Win32Api.WM.PAINT:
                // reduces flickering
                if (m_updating) {
                    Debug.Assert(m_listViewItemToUpdate != null);
                    Win32Api.RECT vrect = new Win32Api.RECT();
                    Win32Api.GetWindowRect(this.Handle, ref vrect);
                    // validate the entire window                
                    Win32Api.ValidateRect(this.Handle, ref vrect);
                    //Invalidate only the new item
                    Invalidate(m_listViewItemToUpdate.Bounds);
                }
                break;
            // WM_VSCROLL, WM_HSCROLL or WM_SIZE messages should stop in-line editing
            case (int)Win32Api.WM.VSCROLL:
            case (int)Win32Api.WM.HSCROLL:
            case (int)Win32Api.WM.SIZE:
                EndEditingToBeVersion(false);
                break;
            case (int)Win32Api.WM.NOTIFY:
                // Look for WM_NOTIFY of events that might also change the
                // editor's position/size: Column reordering or resizing
                Win32Api.NMHDR h = (Win32Api.NMHDR)Marshal.PtrToStructure(m.LParam, typeof(Win32Api.NMHDR));
                if (h.code == (int)Win32Api.HDN.ITEMCHANGING)
                    EndEditingToBeVersion(false);
                break;
            }
            base.WndProc(ref m);
            switch (m.Msg) {
            case (int)(Win32Api.WM.REFLECT | Win32Api.WM.NOTIFY): 
                Win32Api.NMHDR nmhdr = (Win32Api.NMHDR)m.GetLParam(typeof(Win32Api.NMHDR)); 
                switch(nmhdr.code) { 
                case (int)Win32Api.NM.CUSTOMDRAW: 
                    Win32Api.NMCUSTOMDRAW nmcd = (Win32Api.NMCUSTOMDRAW)m.GetLParam(typeof(Win32Api.NMCUSTOMDRAW)); 
                    switch(nmcd.dwDrawStage) { 
                    case (int)Win32Api.CDDS.PREPAINT:
                        m.Result = (IntPtr)(Win32Api.CDRF.NOTIFYITEMDRAW);
                        break; 
                    case (int)Win32Api.CDDS.ITEMPREPAINT: 
                        using (Graphics graphics = Graphics.FromHdc(nmcd.hdc)) {
                            int iRow = (int)nmcd.dwItemSpec; 
                            if (iRow < Items.Count) {
                                ProjectsListViewItem item = (ProjectsListViewItem)Items[iRow];
                                DrawProjectListViewItem(graphics, item);
                            }
                        }
                        m.Result = (IntPtr)(Win32Api.CDRF.SKIPDEFAULT);
                        break; 
                    }
                    break;
                } 
                break; 
            }
        }

        protected override void OnSelectedIndexChanged(EventArgs e) {
            base.OnSelectedIndexChanged(e);
            if (SelectedItems.Count == 0) {
                // clears the list of previously selected items
                m_previouslySelectedItems = new ProjectsListViewItem[0];
            }
        }

        /// <summary>
        ///   Overriden to ensure focus rectangle is drawn.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(EventArgs e) {
            base.OnGotFocus(e);
            // make sure to draw focus rectangle when control receives focus
            if (SelectedItems.Count == 0 && FocusedItem == null && Items.Count > 0) {
                Items[0].Focused = true;
            }
        }

        /// <summary>
        ///   Overriden to activate in-line editing of "to be version"
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            ProjectsListViewItem item = (ProjectsListViewItem)GetItemAt(e.X, e.Y);
            if (item != null) {
                if (Array.IndexOf(m_previouslySelectedItems, item) != -1) {
                    if (GetSubItemIndex(item.Bounds.Left, e.X) == ToBecomeVersionColumnIndex) {
                        StartEditingToBeVersion(item);
                    }
                }
                else {
                    m_previouslySelectedItems = new ProjectsListViewItem[SelectedItems.Count];
                    SelectedItems.CopyTo(m_previouslySelectedItems, 0);
                }
            }
        }

        /// <summary>
        ///   Overriden to activate in-line editing of "to be version" when F2 
        ///   key pressed.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.F2) {
                if (SelectedItems.Count > 0) {
                    StartEditingToBeVersion((ProjectsListViewItem)SelectedItems[0]);
                    m_previouslySelectedItems = new ProjectsListViewItem[] { (ProjectsListViewItem)SelectedItems[0] };
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion // Overriden methods

        #region Methods accessed through delegates

        public static void FillListView(ProjectsListView listView, object obj) {
            Debug.Assert(listView != null);
            Debug.Assert(obj != null && obj is ICollection);
            listView.FillListView((ICollection)obj);
        }

        /// <summary>
        ///   Fills the list view with project informations.
        /// </summary>
        /// <param name="projectInfos">
        ///   <c>ICollection</c> with information for each individiual project.
        /// </param>
        private void FillListView(ICollection projectInfos) {
            Debug.Assert(projectInfos != null);
            m_projectInfos = new ArrayList(projectInfos);
            FillListView();
            Debug.Assert(m_projectInfos != null);
        }

        public static int GetValidVersionsCount(ProjectsListView listView) {
            Debug.Assert(listView != null);
            return listView.m_validVersionsCount;
        }

        public static int GetMarkedItemsCount(ProjectsListView listView) {
            Debug.Assert(listView != null);
            return listView.CheckedItemsCount;
        }

        public static void ProposeToBeVersion(ProjectsListView listView, object obj) {
            Debug.Assert(listView != null);
            Debug.Assert(obj is string);
            listView.ProposeToBeVersion((string)obj);
        }

        /// <summary>
        ///   Proposes new to be version updating colors of items.
        /// </summary>
        private void ProposeToBeVersion(string toBeVersionPattern) {
            Debug.Assert(toBeVersionPattern != null && toBeVersionPattern.Length > 0);
            m_proposedToBeVersionPattern = toBeVersionPattern;
            UpdateColors();
        }

        public static void MarkAllProjects(ProjectsListView listView, object obj) {
            Debug.Assert(listView != null);
            Debug.Assert(obj == null);
            listView.MarkAllProjects();
        }

        private void MarkAllProjects() {
            foreach (ProjectsListViewItem lvi in Items) {
                lvi.Checked = true;
            }
        }

        public static void UnmarkAllProjects(ProjectsListView listView, object obj) {
            Debug.Assert(listView != null);
            Debug.Assert(obj == null);
            foreach (ProjectsListViewItem lvi in listView.Items) {
                lvi.Checked = false;
            }
        }

        public static void InvertProjectMarks(ProjectsListView listView, object obj) {
            Debug.Assert(listView != null);
            Debug.Assert(obj == null);
            listView.InvertProjectMarks();
        }

        private void InvertProjectMarks() {
            foreach (ProjectsListViewItem lvi in Items) 
                lvi.Checked = !lvi.Checked;
        }

        public static void ResetVersions(ProjectsListView listView, object obj) {
            Debug.Assert(listView != null);
            Debug.Assert(obj == null);
            listView.ResetVersions();
        }

        private void ResetVersions() {
            Debug.Assert(m_projectInfos != null);
            foreach (ProjectsListViewItem lvi in CheckedItems) {
                ProjectInfo projectInfo = lvi.ProjectInfo;
                string toBecomeAssemblyVersion = projectInfo.ToBecomeAssemblyVersions[AssemblyVersionType].ToString();
                lvi.SubItems[ToBecomeVersionColumnIndex].Text = toBecomeAssemblyVersion;
                ColorItem(lvi, lvi.Checked);
            }
        }

        public static void ResetListView(ProjectsListView listView, object obj) {
            Debug.Assert(listView != null);
            Debug.Assert(obj == null);
            listView.ResetListView();
        }

        private void ResetListView() {
            ArrayList selectedIndices = new ArrayList(SelectedIndices);
            FillListView();
            foreach (int index in selectedIndices) {
                Items[index].Selected = true;
            }
            UpdateColors();
        }

        public static void ResetMarks(ProjectsListView listView, object obj) {
            Debug.Assert(listView != null);
            Debug.Assert(obj == null);
            listView.ResetMarks();
        }

        private void ResetMarks() {
            Debug.Assert(m_projectInfos != null);
            foreach (ProjectsListViewItem lvi in Items) {
                ProjectInfo projectInfo = lvi.ProjectInfo;
                lvi.Checked = projectInfo.IsMarkedForUpdate(AssemblyVersionType);
            }
        }

        public static void SaveVersions(ProjectsListView listView, object obj) {
            Debug.Assert(listView != null);
            Debug.Assert(obj == null);
            listView.SaveVersions();
        }

        private void SaveVersions() {
            foreach (ProjectsListViewItem lvi in CheckedItems) {
                ProjectInfo projectInfo = lvi.ProjectInfo;
                string newVersion = lvi.SubItems[ToBecomeVersionColumnIndex].Text;
                Debug.Assert(newVersion != null && newVersion.Length > 0);
                projectInfo.Save(AssemblyVersionType, newVersion);
            }
        }

        public static void IncrementVersions(ProjectsListView listView, object componentToIncrement) {
            Debug.Assert(listView != null);
            Debug.Assert(componentToIncrement != null && componentToIncrement is ProjectVersion.VersionComponent);
            listView.IncrementVersions((ProjectVersion.VersionComponent)componentToIncrement);
        }

        private void IncrementVersions(ProjectVersion.VersionComponent toIncrement) {
            NumberingOptions numberingOptions = ConfigurationPersister.Instance.Configuration.NumberingOptions;
            foreach (ProjectsListViewItem lvi in CheckedItems) {
                ProjectInfo projectInfo = lvi.ProjectInfo;
                ProjectVersion newVersion = IncrementVersion(projectInfo, toIncrement, numberingOptions);
                lvi.SubItems[ToBecomeVersionColumnIndex].Text = newVersion.ToString();
            }
        }

        public static void ApplyVersion(ProjectsListView listView, object versionToApply) {
            Debug.Assert(listView != null);
            Debug.Assert(versionToApply != null && versionToApply is string);
            listView.ApplyVersion((string)versionToApply);
        }

        private void ApplyVersion(string versionToApply) {
            ArrayList failedProjects = new ArrayList();
            foreach (ProjectsListViewItem lvi in CheckedItems) {
                try {
                    lvi.SubItems[ToBecomeVersionColumnIndex].Text = ProjectVersion.ApplyVersionPattern(versionToApply, lvi.ProjectInfo.CurrentAssemblyVersions[AssemblyVersionType].ToString(), ResetBuildAndRevisionTo);
                    ColorItem(lvi, lvi.Checked);
                }
                catch (VersionOverflowException) {
                    failedProjects.Add(lvi.Text);
                }
            }
            if (failedProjects.Count > 0) {
                StringBuilder sb = new StringBuilder();
                foreach (string failedProject in failedProjects) {
                    sb.AppendFormat(Environment.NewLine + "    ");
                    sb.Append(failedProject);
                }
                string message = string.Format(s_txtVersionOverflowError, AssemblyVersionType.ToString(), sb.ToString());
                MessageBox.Show(TopLevelControl, message, s_txtVersionFormatError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static ProjectVersion GetHighestMarkedVersion(ProjectsListView listView, object obj) {
            Debug.Assert(listView != null);
            Debug.Assert(obj == null);
            return listView.GetHighestMarkedVersion();
        }

        private ProjectVersion GetHighestMarkedVersion() {
            ProjectVersion highestVersion = ProjectVersion.MinValue;
            foreach (ProjectsListViewItem lvi in CheckedItems) {
                if (lvi.SubItems[ToBecomeVersionColumnIndex].Text.Length > 0) {
                    highestVersion = ProjectVersion.Max(highestVersion, new ProjectVersion(lvi.SubItems[ToBecomeVersionColumnIndex].Text, this.AssemblyVersionType));
                }
            }
            return highestVersion;
        }

        public static ProjectVersion GetHighestToBecomeVersion(ProjectsListView listView, object obj) {
            Debug.Assert(listView != null);
            Debug.Assert(obj == null);
            return listView.GetHighestToBecomeVersion();
        }

        private ProjectVersion GetHighestToBecomeVersion() {
            ProjectVersion highestVersion = ProjectVersion.MinValue;
            foreach (ProjectsListViewItem lvi in Items) {
                if (lvi.SubItems[ToBecomeVersionColumnIndex].Text.Length > 0) {
                    highestVersion = ProjectVersion.Max(highestVersion, new ProjectVersion(lvi.SubItems[ToBecomeVersionColumnIndex].Text, this.AssemblyVersionType));
                }
            }
            return highestVersion;
        }

        public static ProjectVersion GetHighestToUpdateVersion(ProjectsListView listView, object includeProjectsNotForUpdate) {
            Debug.Assert(listView != null);
            Debug.Assert(includeProjectsNotForUpdate != null && includeProjectsNotForUpdate is bool);
            return listView.GetHighestToUpdateVersion((bool)includeProjectsNotForUpdate);
        }

        private ProjectVersion GetHighestToUpdateVersion(bool includeProjectsNotForUpdate) {
            ProjectVersion highestVersion = ProjectVersion.MinValue;
            foreach (ProjectsListViewItem lvi in Items) {
                if (lvi.SubItems[ToBecomeVersionColumnIndex].Text.Length > 0) {
                    ProjectInfo projectInfo = lvi.ProjectInfo;
                    ProjectVersion itemVersion = null;
                    if (projectInfo.IsMarkedForUpdate(AssemblyVersionType) || includeProjectsNotForUpdate) {
                        itemVersion = projectInfo.ToBecomeAssemblyVersions[AssemblyVersionType];
                    }
                    else {
                        itemVersion = projectInfo.CurrentAssemblyVersions[AssemblyVersionType];
                    }
                    if (itemVersion > highestVersion) {
                        highestVersion = itemVersion;
                    }
                }
            }
            return highestVersion;
        }

        public static void MarkProjectsWithLowerVersion(ProjectsListView listView, object version) {
            Debug.Assert(listView != null);
            Debug.Assert(version != null && version is ProjectVersion);
            listView.MarkProjectsWithLowerVersion((ProjectVersion)version);
        }

        private void MarkProjectsWithLowerVersion(ProjectVersion version) {
            foreach (ProjectsListViewItem lvi in Items) {
                ProjectInfo projectInfo = lvi.ProjectInfo;
                ProjectVersion currentVersion = projectInfo.CurrentAssemblyVersions[AssemblyVersionType];
                lvi.Checked = currentVersion < version;
            }
        }

        #endregion // Methods accessed through delegates

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ProjectsListView));

            this.m_columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.m_columnHeaderCurrentVersion = new System.Windows.Forms.ColumnHeader();
            this.m_columnHeaderLastModified = new System.Windows.Forms.ColumnHeader();
            this.m_columnHeaderToBecomeVersion = new System.Windows.Forms.ColumnHeader();

            this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                            this.m_columnHeaderName,
                                                                            this.m_columnHeaderCurrentVersion,
                                                                            this.m_columnHeaderLastModified,
                                                                            this.m_columnHeaderToBecomeVersion});
            // 
            // m_columnHeaderName
            // 
            this.m_columnHeaderName.Text = "Project name";
            this.m_columnHeaderName.Width = 174;
            // 
            // m_columnHeaderCurrentVersion
            // 
            this.m_columnHeaderCurrentVersion.Text = "Current version";
            this.m_columnHeaderCurrentVersion.Width = 83;
            // 
            // m_columnHeaderLastModified
            // 
            this.m_columnHeaderLastModified.Text = "Modified";
            this.m_columnHeaderLastModified.Width = 120;
            // 
            // m_columnHeaderToBecomeVersion
            // 
            this.m_columnHeaderToBecomeVersion.Text = "To be version";
            this.m_columnHeaderToBecomeVersion.Width = 82;

            this.Name = "ProjectsListView";

            this.ResumeLayout(false);
        }
		#endregion

        #region Private properites and methods

        private ProjectsListViewColorsConfiguration ColorsConfiguration {
            get { return ConfigurationPersister.Instance.Configuration.DisplayOptions.Colors; }
        }

        private int ResetBuildAndRevisionTo {
            get {
                return (int)ConfigurationPersister.Instance.Configuration.NumberingOptions.ResetBuildAndRevisionTo;
            }
        }

        private void FillListView() {
            Debug.Assert(m_projectInfos != null);
            BeginUpdate();
            Items.Clear();
            ProjectsListViewItem lvi = null;
            m_validVersionsCount = m_projectInfos.Count;
            foreach (ProjectInfo projectInfo in m_projectInfos) {
                lvi = new ProjectsListViewItem(projectInfo, AssemblyVersionType);
                Items.Add(lvi);
                lvi.Checked = projectInfo.Modified;
                UpdateItem(lvi);
            }
            EndUpdate();
        }

        /// <summary>
        ///   Draws the <c>LisviewItem</c>.
        /// </summary>
        /// <param name="graphics">
        /// </param>
        /// <param name="item">
        ///   <c>ListViewItem</c> to draw.
        /// </param>
        private void DrawProjectListViewItem(Graphics graphics, ProjectsListViewItem item) {
            Rectangle rectEntire = item.GetBounds(ItemBoundsPortion.Entire);
            Rectangle rect = rectEntire;
            int indent = ConfigurationPersister.Instance.Configuration.DisplayOptions.IndentSubProjectItems ? item.ProjectInfo.Level * ConfigurationPersister.Instance.Configuration.DisplayOptions.SubProjectIndentation : 0;
            if (item.ProjectInfo.IsVersionable) {
                ButtonState bs = item.Checked ? ButtonState.Checked : ButtonState.Normal;
                if (item.SubItems[ToBecomeVersionColumnIndex].Text.Length == 0)
                    bs = bs | ButtonState.Inactive;
                m_checkBoxRenderer.DrawCheckBox(graphics, rectEntire.X + 3, rectEntire.Top + 1, 13, 13, bs);
            }
            if (item.ImageIndex >= 0) {
                rect = item.GetBounds(ItemBoundsPortion.Icon);
                graphics.DrawImage(item.ImageList.Images[item.ImageIndex], rect.X + indent, rect.Y);
            }
            rect = item.GetBounds(ItemBoundsPortion.Label);
            rect.X += indent;
            rect.Width -= indent;
            Rectangle rectHighlight = new Rectangle(rect.X, rect.Y, rectEntire.Width - rect.Left, rectEntire.Height);
            if (item.Selected) {
                if (Focused)
                    graphics.FillRectangle(SystemBrushes.Highlight, rectHighlight);
                else 
                    graphics.FillRectangle(SystemBrushes.Control, rectHighlight);
            }
            else {
                using (Brush brush = new SolidBrush(this.BackColor)) {
                    graphics.FillRectangle(brush, rectHighlight);
                }
            }
            if (Focused && item.Focused) 
                ControlPaint.DrawFocusRectangle(graphics, rectHighlight, BackColor, SystemColors.Highlight);
            DrawSubItemStrings(graphics, item);
        }

        /// <summary>
        ///   Draws subitems of a listview item.
        /// </summary>
        /// <param name="graphics">
        /// </param>
        /// <param name="item">
        ///   <c>ListViewItem</c> for which subitems are drawn.
        /// </param>
        private void DrawSubItemStrings(Graphics graphics, ProjectsListViewItem item) {
            StringFormat sf = StringFormat.GenericDefault;
            sf.FormatFlags |= StringFormatFlags.NoWrap;
            sf.LineAlignment = StringAlignment.Center;
            Rectangle rect = item.GetBounds(ItemBoundsPortion.Label);
            int indent = ConfigurationPersister.Instance.Configuration.DisplayOptions.IndentSubProjectItems ? item.ProjectInfo.Level * ConfigurationPersister.Instance.Configuration.DisplayOptions.SubProjectIndentation : 0;
            rect.X += indent;
            rect.Width -= indent;
            using (SolidBrush textColor = new SolidBrush(item.ForeColor)) {
                if (Focused && item.Selected)
                    textColor.Color = SystemColors.HighlightText;
                string text = TrimText(item.Text, rect.Width, graphics); 
                graphics.DrawString(text, this.Font, textColor, rect, sf);
                if (item.ProjectInfo.IsVersionable) {
                    for (int i = 1; i < item.SubItems.Count; i++) {
                        rect.X = rect.Right + 4;
                        rect.Width = Columns[i].Width - 4;
                        ListViewItem.ListViewSubItem subItem = item.SubItems[i];
                        text = TrimText(subItem.Text, rect.Width, graphics);
                        if ((i == item.SubItems.Count - 1) && item.PossiblyInvalidToBeVersion) {
                            if (Focused && item.Selected)
                                graphics.DrawString(text, this.Font, textColor, rect, sf);
                            else {
                                using (SolidBrush errorHighlightColor = new SolidBrush(item.Checked ? ConfigurationPersister.Instance.Configuration.DisplayOptions.Colors.InvalidVersionMarked : ConfigurationPersister.Instance.Configuration.DisplayOptions.Colors.InvalidVersionNotMarked)) {
                                    graphics.DrawString(text, this.Font, errorHighlightColor, rect, sf);
                                }
                            }
                        }
                        else
                            graphics.DrawString(text, this.Font, textColor, rect, sf);
                    }
                }
            }
        }

        /// <summary>
        ///   Trims the text to a given width appending ellipses (...) to the
        ///   trimmed end.
        /// </summary>
        /// <param name="text">
        ///   Text to trim.
        /// </param>
        /// <param name="widthToTrim">
        ///   Width to which text should be trimmed.
        /// </param>
        /// <param name="graphics">
        /// </param>
        /// <returns>
        ///   Trimmed string.
        /// </returns>
        private string TrimText(string text, int widthToTrim, Graphics graphics) {
            if (graphics.MeasureString(text, Font).Width < widthToTrim)
                return text;
            StringBuilder sb = new StringBuilder(text);
            sb.Append("...");
            while (graphics.MeasureString(sb.ToString(), Font).Width > widthToTrim && sb.Length > 4)
                sb.Remove(sb.Length - 4, 1);
            return sb.ToString();
        }

        /// <summary>
        ///   Updates colors for items to reflect their state.
        /// </summary>
        public void UpdateColors() {
            foreach (ProjectsListViewItem lvi in Items) {
                ColorItem(lvi, lvi.Checked);
            }
        }

        /// <summary>
        ///   Sets the color of an item in the listview to reflect its 
        ///   properties.
        /// </summary>
        /// <param name="lvi">
        ///   <c>ListViewItem</c> to color.
        /// </param>
        /// <param name="itemChecked">
        ///   Flag indicating if item is selected. Unchecked items will have
        ///   lighter color.
        /// </param>
        /// <param name="toBeVersionPattern">
        ///   To be version used to compare with actual version. If actual 
        ///   version is less, item is displayed in red.
        /// </param>
        private void ColorItem(ProjectsListViewItem lvi, bool itemChecked) {
            ProjectInfo projectInfo = lvi.ProjectInfo;
            Color oldColor = lvi.ForeColor;
            bool oldPossiblyInvalidToBeVersion = lvi.PossiblyInvalidToBeVersion;
            if (!lvi.ProjectInfo.IsVersionable)
                lvi.ForeColor = ColorsConfiguration.SubProjectRoot;
            else if (!lvi.IsAssemblyVersionDefined) {
                lvi.ForeColor = ColorsConfiguration.NoVersion;
            }
            else {
                string toBeVersion = lvi.SubItems[ToBecomeVersionColumnIndex].Text;
                bool isRegularVersion = ProjectVersion.IsValidVersionString(toBeVersion, AssemblyVersionType, lvi.ProjectInfo.ProjectTypeInfo.ProjectType);
                // check if "to be version" is higher than current version
                Debug.Assert(isRegularVersion || AssemblyVersionType == AssemblyVersionType.AssemblyInformationalVersion);
                if (!isRegularVersion || lvi.CurrentVersion.CompareTo(toBeVersion) <= 0) {
                    if (projectInfo.Modified) {
                        if (itemChecked)
                            lvi.ForeColor = ColorsConfiguration.ModifiedMarked;
                        else
                            lvi.ForeColor = ColorsConfiguration.ModifiedNotMarked;
                    }
                    else {
                        if (itemChecked)
                            lvi.ForeColor = ColorsConfiguration.NotModifiedMarked;
                        else
                            lvi.ForeColor = ColorsConfiguration.NotModifiedNotMarked;
                    }
                }
                else {
                    if (itemChecked)
                        lvi.ForeColor = ColorsConfiguration.InvalidVersionMarked;
                    else
                        lvi.ForeColor = ColorsConfiguration.InvalidVersionNotMarked;
                }
                if (isRegularVersion) {
                    try {
                        string s = ProjectVersion.ApplyVersionPattern(m_proposedToBeVersionPattern, lvi.ProjectInfo.CurrentAssemblyVersions[AssemblyVersionType].ToString(), ResetBuildAndRevisionTo);
                        ProjectVersion curr = lvi.ProjectInfo.CurrentAssemblyVersions[AssemblyVersionType];
                        lvi.PossiblyInvalidToBeVersion = !curr.IsStringPatternHigher(s) || !curr.IsStringPatternHigher(toBeVersion);
                    }
                    catch (VersionOverflowException) {
                        lvi.PossiblyInvalidToBeVersion = true;
                    }
                }
            }
            if (oldColor != lvi.ForeColor || oldPossiblyInvalidToBeVersion != lvi.PossiblyInvalidToBeVersion)
                UpdateItem(lvi);
        }

        private void UpdateItem(ProjectsListViewItem lvi) {
            if (!m_batchUpdate && Visible) {
                m_updating = true;
                m_listViewItemToUpdate = lvi;
                this.Update();
                m_updating = false;
            }
        }

        private ProjectVersion IncrementVersion(ProjectInfo projectInfo, ProjectVersion.VersionComponent toIncrement, NumberingOptions numberingOptions) {
            ProjectVersion newVersion = projectInfo.CurrentAssemblyVersions[AssemblyVersionType].Clone();
            newVersion.IncrementComponent(toIncrement, numberingOptions);
            return newVersion;
        }

        #endregion // Private properites and methods

        #region Inline editing methods

        private void StartEditingToBeVersion(ProjectsListViewItem item) {
            if (!IsVersionEditable(item))
                return;
            item.EnsureVisible();
            Rectangle subItemBounds = GetToBeVersionSubItemBounds(item);
            subItemBounds.Intersect(ClientRectangle);
            m_editControl.Activate(subItemBounds, item.SubItems[ToBecomeVersionColumnIndex].Text);
            m_editControl.Leave     += new EventHandler(m_editControl_Leave);
            m_editControl.KeyPress  += new KeyPressEventHandler(m_editControl_KeyPress);
            m_itemEdited = item;
        }

        private void EndEditingToBeVersion(bool acceptChanges) {
            if (m_itemEdited == null)
                return;
            if (acceptChanges) {
                if (AssemblyVersionType != AssemblyVersionType.AssemblyInformationalVersion 
                    || m_itemEdited.ProjectInfo.ProjectTypeInfo == ProjectTypeInfo.VCppProject
                    || m_itemEdited.ProjectInfo.ProjectTypeInfo == ProjectTypeInfo.SetupProject
                    || !ConfigurationPersister.Instance.Configuration.NumberingOptions.AllowArbitraryInformationalVersion) {
                    string message = ProjectVersion.ValidateVersionString(m_editControl.Text, AssemblyVersionType, m_itemEdited.ProjectInfo.ProjectTypeInfo.ProjectType);
                    if (message != string.Empty) {
                        MessageBox.Show(TopLevelControl, message, s_txtVersionFormatError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        m_editControl.Focus();
                        return;
                    }
                }
                m_itemEdited.SubItems[ToBecomeVersionColumnIndex].Text = m_editControl.Text;
                ColorItem(m_itemEdited, m_itemEdited.Checked);
            }
            m_editControl.Leave     -= new EventHandler(m_editControl_Leave);
            m_editControl.KeyPress  -= new KeyPressEventHandler(m_editControl_KeyPress);
            m_editControl.Deactivate();
            m_itemEdited = null;

            m_previouslySelectedItems = new ProjectsListViewItem[SelectedItems.Count];
            SelectedItems.CopyTo(m_previouslySelectedItems, 0);
        }

        private bool IsVersionEditable(ProjectsListViewItem item) {
            return item.IsAssemblyVersionDefined || !item.CurrentVersion.Valid;
        }

        private void m_editControl_Leave(object sender, EventArgs e) {
            EndEditingToBeVersion(true);
        }

        private void m_editControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case (char)(int)Keys.Escape:
                    EndEditingToBeVersion(false);
                    break;
                case (char)(int)Keys.Enter:
                    EndEditingToBeVersion(true);
                    break;
            }
        }

        private int GetSubItemIndex(int itemLeft, int x) {
            for (int i = 0; i < Columns.Count; i++) {
                ColumnHeader h = Columns[i];
                if (x <	itemLeft + h.Width) {
                    return h.Index;
                }
                itemLeft += h.Width;
            }
            return -1;
        }

        private Rectangle GetToBeVersionSubItemBounds(ProjectsListViewItem Item) {
            Rectangle subItemRect = Rectangle.Empty;
            Rectangle lviBounds = Item.GetBounds(ItemBoundsPortion.Entire);
            int	subItemX = lviBounds.Left;
            int i;
            for (i = 0; i < Columns.Count; i++) {
                ColumnHeader col = Columns[i];
                if (col.Index == ToBecomeVersionColumnIndex)
                    break;
                subItemX += col.Width;
            } 
            subItemRect	= new Rectangle(subItemX + 4, lviBounds.Top, Columns[i].Width - 4, lviBounds.Height);
            return subItemRect;
        }

        #endregion // Inline editing methods

        #region Private fields
        /// <summary>
        ///   
        /// </summary>
        private IList           m_projectInfos;
        /// <summary>
        ///   Number of valid versions in the listview.
        /// </summary>
        private int             m_validVersionsCount;
        /// <summary>
        ///   Proposed "to be version" pattern taken from main form text box.
        /// </summary>
        private string          m_proposedToBeVersionPattern;
        /// <summary>
        ///   <c>ListViewItem</c> currently being updated; used to avoid 
        ///   listview flickering.
        /// </summary>
        private ProjectsListViewItem    m_listViewItemToUpdate;
        /// <summary>
        ///   Flag that controls listview updating; used to avoid listview 
        ///   flickering.
        /// </summary>
        private bool                    m_updating = false;
        /// <summary>
        ///   Flag that prevents update of individual items for multiple 
        ///   commands.
        /// </summary>
        private bool                    m_batchUpdate = false;
        /// <summary>
        ///   Embedded textbox control used to manually edit version.
        /// </summary>
        private EmbeddedTextBox         m_editControl;
        /// <summary>
        ///   ListViewItem being manually edited.
        /// </summary>
        private ProjectsListViewItem    m_itemEdited;
        /// <summary>
        ///   Array of <c>ListViewItem</c>s being selected. Used when manually 
        ///   editing version.
        /// </summary>
        private ProjectsListViewItem[]  m_previouslySelectedItems = new ProjectsListViewItem[0];
        /// <summary>
        ///   Object used to render checkboxes in the listview.
        /// </summary>
        CheckBoxRenderer m_checkBoxRenderer = null;

        #endregion // Private fields

        #region Static methods and fields

        static ProjectsListView() {
            ResourceManager resources = new System.Resources.ResourceManager("BuildAutoIncrement.Resources.Shared", typeof(ResourceAccessor).Assembly);
            Debug.Assert(resources != null);

            s_txtVersionNotFound    = resources.GetString("Version not found");
            s_txtVersionInvalid     = resources.GetString("Invalid version");
            s_txtFileNotFound       = resources.GetString("AssemblyInfo file not found in project");
            s_txtVersionFormatError = resources.GetString("Version format error");
            s_txtVersionOverflowError = resources.GetString("Version overflow occured for projects");

            Debug.Assert(s_txtVersionNotFound != null);
            Debug.Assert(s_txtVersionInvalid != null);
            Debug.Assert(s_txtFileNotFound != null);
            Debug.Assert(s_txtVersionFormatError != null);
        }
            
        private static readonly string s_txtVersionNotFound;
        private static readonly string s_txtVersionInvalid;
        private static readonly string s_txtFileNotFound;
        private static readonly string s_txtVersionFormatError;
        private static readonly string s_txtVersionOverflowError;

        private static int ProjectNameColumnIndex;
        private static int ToBecomeVersionColumnIndex;
        private static int ModifiedColumnIndex;
        private static int CurrentVersionColumnIndex;
        
        #endregion // Static methods and fields

        private const int Indentation = 10;
    }
}