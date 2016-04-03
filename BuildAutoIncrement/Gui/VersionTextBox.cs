/*
 * Filename:    VersionTextBox.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Text box specialized for version display.
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
using System.Text;
using System.Windows.Forms;

namespace BuildAutoIncrement {

    /// <summary>
    ///   Edit control that restricts entering data to format appropriate for
    ///   versioning: sequence of 4 blocks consisting of integer or asterisk, 
    ///   delimited by dots.
    /// </summary>
    public class VersionTextBox : TextBox {

        public event EventHandler ActiveBlockChanged;

        private System.ComponentModel.Container components = null;

		/// <summary>
		///   Initializes a new instance of the <c>VersionTextBox</c> class.
		/// </summary>
		public VersionTextBox() {
            this.TabStop = false;
		}

        /// <summary>
        ///   Largest possible value in the block.
        /// </summary>
        public long Maximum = ProjectVersion.MaxVersion - 1;
        /// <summary>
        ///   Smallest possible value in the block.
        /// </summary>
        public long Minimum = -2;

        /// <summary>
        ///   Increments currently selected block content.
        /// </summary>
        /// <param name="increment">
        ///   Increment (can be negative as well).
        /// </param>
        public void IncrementVersionNumber(int increment) {
            if (SelectionStart == -1)
                return;
            bool hasPlus = GetCurrentBlockContent().StartsWith("+");
            SetCurrentBlockValue(GetCurrentBlockValue() + increment, hasPlus);
        }

        /// <summary>
        ///   Event handler executed when next control in the form looses focus.
        ///   It positions Selection to the last block in the edit control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNextControlLostFocus(object sender, EventArgs e) {
			SelectionStart = Text.Length - 1;
		}

        /// <summary>
        ///   Event handler executed when next control in the form looses focus.
        ///   It positions selection to the first block in the edit control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviousControlLostFocus(object sender, EventArgs e) {
			SelectionStart = 0;
		}

        protected virtual void OnActiveBlockChanged() {
            if (ActiveBlockChanged != null)
                ActiveBlockChanged(this, EventArgs.Empty);
        }

		#region Overrides

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                RemoveFromFormTabOrder();
                if (components != null)	{
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///   Overrides <c>OnParentVisibleChanged</c> to register <c>LostFocus</c>
        ///   event handlers for the next and previous controls in the main form.
        /// </summary>
        /// <param name="e"></param>
        override protected void OnParentVisibleChanged(EventArgs e) {
            base.OnParentVisibleChanged(e);
            InsertInFormTabOrder();
        }

        /// <summary>
        ///   Overrides <c>OnMouseDown</c> method to position the caret
        ///   by the character clicked.
        /// </summary>
        /// <param name="e">
        ///   A <c>MouseEventArgs</c> that contains the event data.
        /// </param>
        protected override void OnMouseDown(MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                SelectionLength = 0;
                SelectionStart = GetCharIndexFromPosition(e.X);
                m_selectionStart = SelectionStart;
                m_mousePressed = true;
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        ///   Overrides <c>OnDoubleClick</c> method. Selects the block between dots
        ///   where currently selection is.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e) {
            SelectBlock();
        }

        /// <summary>
        /// Overrides <c>OnClick</c> method to notify listeners on possible 
        /// selected block change.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e) {
            base.OnClick(e);
            OnActiveBlockChanged();
        }

        /// <summary>
        /// Overrides <c>OnTextChange</c> method. Prevents entering a negative 
        /// number or a number which is larger than m_maximum.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e) {
            int pos = SelectionStart;
            if (TextLength > 0 && SelectionStart >= 0) {
                string blockContent = GetCurrentBlockContent();
                if (blockContent == null || blockContent.Length == 0) {
                    Text = Text.Substring(0, pos) + "*" + Text.Substring(pos);
                }
                else if ((blockContent != "*") && (blockContent != "+")) {
                    bool hasPlus = blockContent.StartsWith("+");
                    int posOffset = blockContent.Length;
                    try {
                        long number = long.Parse(blockContent);
                        if (number < 0)
                            number = -number;
                        if (number > Maximum) {
                            Undo();
                            pos--;
                        }
                        else {
                            blockContent = number.ToString();
                            posOffset -= blockContent.Length;
                            StringBuilder sb = new StringBuilder(Text.Substring(0, GetBlockStart(pos)));
                            if (hasPlus) {
                                sb.Append('+');
                                posOffset--;
                            }
                            sb.Append(blockContent);
                            sb.Append(Text.Substring(GetBlockEnd(pos)));
                            Text = sb.ToString(); // Text.Substring(0, GetBlockStart(pos)) + blockContent + Text.Substring(GetBlockEnd(pos));
                            pos -= posOffset;
                        }
                    }
                    catch {
                        Text = Text.Substring(0, GetBlockStart(pos)) + "*" + Text.Substring(GetBlockEnd(pos));
                    }
                }
                ClearUndo();
            }
            base.OnTextChanged(e);
            if (pos >= 0)
                SelectionStart = pos;
        }

        /// <summary>
        /// Overrides <c>OnGotFocus</c> message handler to restrict selection to a 
        /// single block.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(EventArgs e) {
			SelectBlock();
            OnActiveBlockChanged();
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Overrides <c>OnKeyPressed</c> event handler to prevent entering characters 
        /// other than digit or asterisk character.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(KeyPressEventArgs e) {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)) {
                e.Handled = false;
            }
            else if (e.KeyChar == '*') {
                SelectBlock();
                e.Handled = false;
            }
            else if (e.KeyChar == '+' && SelectionStart == GetBlockStart(SelectionStart)) {
                e.Handled = false;
            }
            else
                e.Handled = true;
            base.OnKeyPress(e);
		}

        /// <summary>
		/// Overrides standard <c>PreProcessMessage</c> to:
		/// 1. translate Tab character into skip to next/previous block;
		/// 2. prevent crossing dot delimiter on Left, Right, Home and End 
		///    buttons;
		/// 3. prevent deleting accross dot delimiter on Delete and Backspace;
		/// 4. increment/decrement value on Up and Down buttons;
		/// </summary>
		/// <param name="msg">Message to process</param>
		public override bool PreProcessMessage(ref Message msg) {
			if (msg.Msg == (int)Win32Api.WM.KEYDOWN) {
				Keys keyData = ((Keys) (int) msg.WParam) | ModifierKeys;
				Keys keyCode = ((Keys) (int) msg.WParam);

				switch (keyCode) {
				case Keys.Tab:
					if ((ModifierKeys & Keys.Shift) == Keys.Shift) {
						if (IsSelectionInFirstBlock())
							break;
						JumpToPreviousBlock();
					}
					else if (ModifierKeys == 0) {
						if (IsSelectionInLastBlock())
							break;
						JumpToNextBlock();
                    }
                    else
                        break;
					return true;
				case Keys.Left:
                    // if CTRL key pressed
                    if ((ModifierKeys & Keys.Control) != 0) {
                        int posEnd = SelectionStart + SelectionLength;
                        SelectionStart = GetBlockStart(SelectionStart);
                        if ((ModifierKeys & Keys.Shift) != 0) {
                            SelectionLength = posEnd - SelectionStart;
                        }
                        return true;
                    }
                    // if at the start of the block
                    if (SelectionStart == GetBlockStart(SelectionStart)) {
                        if ((ModifierKeys & Keys.Shift) == 0) {
                            if (SelectionLength > 0) {
                                SelectionLength = 0;
                            }
                            else {
                                if (IsSelectionInFirstBlock())
                                    return true;
                                JumpToPreviousBlock();
                            }
                        }
                        return true;
                    }
                    else if (SelectionLength > 0 && (ModifierKeys & Keys.Shift) == 0) {
                        Select(SelectionStart, 0);
                        return true;
                    }
                    break;
				case Keys.Right:
                    // if CTRL key pressed
                    if ((ModifierKeys & Keys.Control) != 0) {
                        int pos = GetBlockEnd(SelectionStart);
                        if ((ModifierKeys & Keys.Shift) != 0) {
                            SelectionLength = SelectionStart + pos;
                        }
                        else
                            SelectionStart = pos;
                        return true;
                    }
                    // if at the end of the block
                    if ((SelectionStart + SelectionLength) == GetBlockEnd(SelectionStart + SelectionLength)) {
                        if ((ModifierKeys & Keys.Shift) == 0) {
                            if (SelectionLength > 0) {
                                SelectionStart += SelectionLength;
                                SelectionLength = 0;
                            }
                            else {
                                if (IsSelectionInLastBlock())
                                    return true;
                                JumpToNextBlock();
                            }
                        }
                        return true;
                    }
                    else if (SelectionLength > 0 && (ModifierKeys & Keys.Shift) == 0) {
                        Select(SelectionStart + SelectionLength, 0);
                        return true;
                    }
                    break;
				case Keys.Up:
                    if ((ModifierKeys & Keys.Control) != 0)
                        SetCurrentBlockValue(Maximum, GetCurrentBlockContent().StartsWith("+"));
                    else
					    IncrementVersionNumber(+1);
					return true;
				case Keys.Down:
                    if ((ModifierKeys & Keys.Control) != 0) 
                        SetCurrentBlockValue(0, false);
                    else
                        IncrementVersionNumber(-1);
					return true;
                case Keys.PageUp:
                    if ((ModifierKeys & Keys.Control) != 0) 
                        SetCurrentBlockValue(Maximum, GetCurrentBlockContent().StartsWith("+"));
                    else
                        IncrementVersionNumber(+1000);
                    return true;
                case Keys.PageDown:
                    if ((ModifierKeys & Keys.Control) != 0) 
                        SetCurrentBlockValue(0, false);
                    else
                        IncrementVersionNumber(-1000);
                    return true;
                case Keys.Delete:
                    // do nothing if caret is positioned just before dot 
                    // separator and selection is empty
                    if (SelectionStart < TextLength && SelectionLength == 0 && Text[SelectionStart] == '.') {
                        return true;
					}
					break;
				case Keys.Back:
                    // do nothing if caret is positioned just after dot 
                    // separator and selection is empty
                    if (SelectionStart > 0 && SelectionLength == 0 && Text[SelectionStart-1] == '.') {
                        return true;
                    }
                    break;
                case Keys.Home:
                    if ((ModifierKeys & Keys.Shift) != 0) {
                        int selectionEnd = SelectionStart + SelectionLength;
                        int newSelectionStart = GetBlockStart(SelectionStart);
                        Select(newSelectionStart, selectionEnd - newSelectionStart);
                    }
                    else {
                        Select(GetBlockStart(SelectionStart), 0);
                    }
                    return true;
                case Keys.End:
                    if ((ModifierKeys & Keys.Shift) != 0) {
                        Select(SelectionStart, GetBlockEnd(SelectionStart) - SelectionStart);
                    }
                    else {
                        Select(GetBlockEnd(SelectionStart), 0);
                    }
                    return true;
                case Keys.A:
                    if ((ModifierKeys & Keys.Control) != 0) {
                        SelectBlock();
                        return true;
                    }
                    break;
                default:
                    if ((keyCode >= Keys.D0 && keyCode <= Keys.D9) || (keyCode >= Keys.NumPad0 && keyCode <= Keys.NumPad9)) {
                        if (GetCurrentBlockContent() == "*") {
                            SelectBlock();
                            break;
                        }
                    }
                    break;
				}
			}
			return base.PreProcessMessage(ref msg);
		}

        /// <summary>
        /// Overrides <c>WndProc</c> procedure to restrict "Select All" to a single 
        /// block and block context menu, copy, cut, paste and undo operations.
        /// </summary>
        /// <param name="msg">Message to process.</param>
        override protected void WndProc(ref Message msg) {
            if (msg.Msg == (int)Win32Api.EM.SETSEL) {
                int startPosition = (int)msg.WParam;
                int endPosition = (int)msg.LParam;
                // restrict "SelectAll" selection to a single block
                if (startPosition == 0 && endPosition == -1) {
                    SelectBlock();
                    return;
                }
            }
            // prevent context menu, clipboard operations and undo
            if (msg.Msg == (int)Win32Api.WM.CONTEXTMENU 
                || msg.Msg == (int)Win32Api.WM.COPY 
                || msg.Msg == (int)Win32Api.WM.CUT 
                || msg.Msg == (int)Win32Api.WM.PASTE 
                || msg.Msg == (int)Win32Api.WM.UNDO) {
                return;
            }
            base.WndProc(ref msg);
        }

        /// <summary>
        /// Overrides <c>OnMouseMove</c> message handler to prevent extending 
        /// selection accross delimiting dot character. 
        /// </summary>
        /// <param name="e"></param>
        override protected void OnMouseMove(MouseEventArgs e) {
            if (m_mousePressed) {
                // prevents extending selection over block boundary 
                int selectionEnd = GetCharIndexFromPosition(e.X);
                RestrictSelectionInsideBlock(m_selectionStart, selectionEnd);
                return;
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Overrides <c>OnMouseUp</c> event handler.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e) {
            m_mousePressed = false;
            base.OnMouseUp(e);
        }

		#endregion // Overrides

        #region Private methods

        /// <summary>
        ///   Inserts this control into main form's tab order.
        /// </summary>
        private void InsertInFormTabOrder() {
            if (TopLevelControl != null) {
                Control nextControl = GetNextTabControl(Parent, true);
                if (nextControl != null)
                    nextControl.LostFocus += new System.EventHandler(OnNextControlLostFocus);
                Control previousControl = GetNextTabControl(Parent, false);
                if (previousControl != null)
                    previousControl.LostFocus += new System.EventHandler(OnPreviousControlLostFocus);
            }
        }

        /// <summary>
        ///   Removes this control into main form's tab order.
        /// </summary>
        private void RemoveFromFormTabOrder() {
            if (TopLevelControl != null) {
                Control nextControl = GetNextTabControl(Parent, true);
                if (nextControl != null)
                    nextControl.LostFocus -= new System.EventHandler(OnNextControlLostFocus);
                Control previousControl = GetNextTabControl(Parent, false);
                if (previousControl != null)
                    previousControl.LostFocus -= new System.EventHandler(OnPreviousControlLostFocus);
            }
        }

        /// <summary>
        ///   Searches for the next control in the tab order of the topmost owner
        ///   form.
        /// </summary>
        /// <param name="ctl">
        ///   <c>Control</c> to start the search from.
        /// </param>
        /// <param name="forward">
        ///   <c>true</c> to search forward in the tab order.
        /// </param>
        /// <returns>
        ///   Next <c>Control</c> object in tab order.
        /// </returns>
        private Control GetNextTabControl(Control ctl, bool forward) {
            do {
                ctl = TopLevelControl.GetNextControl(ctl, forward);
                if (ctl == null || ctl == this)
                    return null;
            } while (ctl.TabStop == false);
            return ctl;
        }

        /// <summary>
        ///   Gets the value of the currently selected block.
        /// </summary>
        /// <returns>
        ///   Value of the block.
        /// </returns>
        private int GetCurrentBlockValue() {
            if (SelectionStart == -1)
                return 0;
            return GetBlockValue(SelectionStart);
        }

        /// <summary>
        ///   Sets the value of a currently selected block.
        /// </summary>
        /// <param name="newValue">
        ///   New value to set.
        /// </param>
        private void SetCurrentBlockValue(long newValue, bool hasPlus) {
            if (SelectionStart >= 0) {
                if (newValue > Maximum)
                    newValue = Maximum;
                if (newValue < Minimum)
                    newValue = Minimum;
                SelectBlock();
                switch (newValue) {
                case -1:
                    SelectedText = "*";
                    break;
                case -2:
                    SelectedText = "+";
                    break;
                default:
                    SelectedText = (hasPlus ? "+" : "") + newValue.ToString();
                    break;
                }
                SelectBlock();
            }
        }

        #endregion // Private methods

		#region Helper functions

		private bool IsSelectionInLastBlock() {
			return Text.IndexOf('.', SelectionStart) == -1;
		}

		private bool IsSelectionInFirstBlock() {
			return (SelectionStart == 0) || (Text.LastIndexOf('.', SelectionStart - 1) == -1);
		}

		private int GetBlockStart(int position) {
            if (TextLength == 0)
                return -1;
            if (position > 0)
				position = Text.LastIndexOf('.', position - 1) + 1;
			return position;
		}

		private int GetBlockEnd(int position) {
            if (TextLength == 0)
                return -1;
			position = Text.IndexOf('.', position);
			if (position < 0)
				position = Text.Length;
			return position;
		}

		private void SelectBlock() {
			int selStart = GetBlockStart(SelectionStart);
			int selEnd = GetBlockEnd(SelectionStart);
			Select(selStart, selEnd - selStart);
		}

        private void RestrictSelectionInsideBlock(int selectionStart, int selectionEnd) {
            if (selectionStart == 0 && selectionEnd == -1)
                SelectBlock();
            else if (selectionEnd > selectionStart) {
                if (GetBlockEnd(selectionStart) < selectionEnd)
                    selectionEnd = GetBlockEnd(selectionStart);
                Select(selectionStart, selectionEnd - selectionStart);
            }
            else if (selectionEnd < selectionStart) {
                if (GetBlockStart(selectionStart) > selectionEnd) {
                    selectionEnd = GetBlockStart(selectionStart);
                }
                Select(selectionEnd, selectionStart - selectionEnd);
            }
        }

		private void JumpToNextBlock() {
			int selStart = Text.IndexOf('.', SelectionStart);
			if (selStart >= 0) {
				Select(selStart + 1, 0);
                SelectBlock();
                OnActiveBlockChanged();
			}
		}

		private void JumpToPreviousBlock() {
			int selStart = Text.LastIndexOf('.', SelectionStart - 1);
			if (selStart >= 0) {
				Select(selStart, 0);
                SelectBlock();
                OnActiveBlockChanged();
            }
		}

        private string GetBlockContent(int position) {
            if (TextLength == 0 && position < 0)
                return null;
            int blockStart = GetBlockStart(position);
            int blockEnd = GetBlockEnd(position);
            return Text.Substring(blockStart, blockEnd - blockStart);
        }

        private string GetCurrentBlockContent() {
            return GetBlockContent(SelectionStart);
        }

        private int GetBlockValue(int position) {
            string blockContent = GetBlockContent(position);
            if (blockContent.Length == 0)
                return 0;
            if (blockContent == "*")
                return -1;
            if (blockContent =="+")
                return -2;
            return Convert.ToInt32(blockContent);
        }

		public int GetCharIndexFromPosition(int X) {
			if (TextLength == 0)
				return 0;
			System.Drawing.Graphics graphics = CreateGraphics();
			float xPrev = 0;
			int index = 0;

			while (index < TextLength) {
				System.Drawing.SizeF sizef = graphics.MeasureString(Text.Substring(0, index + 1), 
															        Font, 
															        System.Drawing.Point.Empty, 
															        System.Drawing.StringFormat.GenericTypographic);
				if (Math.Abs(X - xPrev) < Math.Abs(X - sizef.Width))
                    break;
                xPrev = sizef.Width;
                index++;
			}
			graphics.Dispose();
            return index;
		}

		#endregion

        /// <summary>
        ///   Indicator if the mouse button is pressed. Used by MouseMove event 
        ///   handler to prevent extending the selection across the delimiting 
        ///   dot.
        /// </summary>
        private bool m_mousePressed;
        /// <summary>
        ///   Selection start position.
        /// </summary>
        private int m_selectionStart;
    }
}
