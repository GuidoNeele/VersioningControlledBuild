/*
 * Filename:    FontTextBox.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Text box used to display font description.
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace BuildAutoIncrement {

    /// <summary>
    ///   Displays font description.
    /// </summary>
    public class FontTextBox : System.Windows.Forms.TextBox {

        /// <summary>
        ///   Initializes <c>FontTextBox</c> control.
        /// </summary>
        public FontTextBox() : base() {
            ReadOnly = true;
            FontDescription = new FontDescription("Arial", FontStyle.Regular, 10f);
        }

        #region Public properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FontDescription FontDescription {
            get {
                return m_fontDescription;
            }
            set {
                m_fontDescription = value;
                base.Text = m_fontDescription.ToString();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font FontDisplayed {
            get {
                return new Font(m_fontDescription.FontFamily, m_fontDescription.Size, m_fontDescription.FontStyle);
            }
            set {
                FontDescription = new FontDescription(value.FontFamily.Name, value.Style, value.Size);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new string Text {
            get {
                return base.Text;
            }
            set {
            }
        }

        #endregion // Public properties

        #region Overrides

        protected override void InitLayout() {
            base.InitLayout ();
            Text = m_fontDescription.ToString();
        }

        #endregion // Overrides

        FontDescription m_fontDescription;		
    }
}