/*
 * Filename:    FontDescription.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Wrappers around Font class.
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
using System.Drawing;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace BuildAutoIncrement {

    #region FontStyleWrapper
	/// <summary>
	///   Class used for textual presentation of font style.
	/// </summary>
    public class FontStyleWrapper {

        #region Constructors

        private FontStyleWrapper() {
        }

		public FontStyleWrapper(FontStyle fontStyle) {
			m_fontStyle = fontStyle;
		}

        #endregion // Constructors

        #region Overrides

        /// <summary>
        ///   Provides string presentation of <c>FontStyleWrapper</c>.
        /// </summary>
        /// <returns>
        ///   String presentation.
        /// </returns>
		public override string ToString() {
			string result = m_fontStyle.ToString();
			int n = result.IndexOf(',');
			if (n != -1)
				return result.Remove(n, 1);
			return result;
		}

        #endregion // Overrides

        #region Conversion operators

        public static implicit operator FontStyleWrapper(FontStyle fontStyle) {
            switch (fontStyle) {
            case FontStyle.Regular:
                return FontStyleWrapper.Regular;
            case FontStyle.Italic:
                return FontStyleWrapper.Italic;
            case FontStyle.Bold:
                return FontStyleWrapper.Bold;
            case FontStyle.Bold | FontStyle.Italic:
                return FontStyleWrapper.BoldItalic;
            default:
                Debug.Assert(false, "Not supported font style");
                break;
            }
            return new FontStyleWrapper(fontStyle);
        }

        public static explicit operator FontStyle(FontStyleWrapper fontStyleWrapper) {
            return fontStyleWrapper.m_fontStyle;
        }

        #endregion // Conversion operators

        private readonly FontStyle m_fontStyle;

        #region Static values

        public static FontStyleWrapper Regular = new FontStyleWrapper(FontStyle.Regular);
        public static FontStyleWrapper Italic = new FontStyleWrapper(FontStyle.Italic);
        public static FontStyleWrapper Bold = new FontStyleWrapper(FontStyle.Bold);
        public static FontStyleWrapper BoldItalic = new FontStyleWrapper(FontStyle.Bold | FontStyle.Italic);

        #endregion // Static values
    }

    #endregion // FontStyleWrapper

    #region FontDescription

	/// <summary>
	///   Class used for textual presentation of a font.
	/// </summary>
	[Serializable]
	public class FontDescription {

        #region Constructors

        /// <summary>
        ///   Default constructor.
        /// </summary>
        public FontDescription() {
            FontFamily = "Arial";
            m_fontStyleWrapper = FontStyleWrapper.Regular;
            Size = 9.75f;
        }

		public FontDescription(string fontFamily, FontStyle fontStyle, float fontSize) {
			FontFamily = fontFamily;
			m_fontStyleWrapper = fontStyle;
			Size = fontSize;
		}

        #endregion // Constructors

        #region Public properties

        [XmlText]
        public string FontFamily {
            get {
                return m_fontFamily;
            }
            set {
                m_fontFamily = value;
            }
        }

        [XmlAttribute]
        public FontStyle FontStyle {
            get {
                return (FontStyle)m_fontStyleWrapper;
            }
            set {
                m_fontStyleWrapper = value;
            }
        }

        [XmlAttribute]
        public float Size {
            get {
                return m_size;
            }
            set {
                m_size = value;
            }
        }
        
        #endregion // Public properties
        
        #region Conversion operators

        public static implicit operator Font(FontDescription fontDescription) {
            return new Font(fontDescription.FontFamily, fontDescription.Size, fontDescription.FontStyle);
        }

        #endregion // Conversion operators

        #region Overrides

        public override string ToString() {
            if (m_fontStyleWrapper == FontStyleWrapper.Regular)
                return string.Format("{0}, {1} pt", FontFamily, Size);
            return string.Format("{0}, {1}, {2} pt", FontFamily, m_fontStyleWrapper.ToString(), Size);
        }

        #endregion // Overrides

        #region Private fields

		private string m_fontFamily;
		private float m_size;
        private FontStyleWrapper m_fontStyleWrapper;

        #endregion // Private fields
    }

    #endregion // FontDescription
}