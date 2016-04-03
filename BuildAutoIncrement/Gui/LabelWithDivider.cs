/*
 * Filename:    LabelWithDivider.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Divider with a caption.
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

namespace BuildAutoIncrement {

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(LabelWithDivider))]
    public class LabelWithDivider : System.Windows.Forms.Label {

        /// <summary>
        ///   Gets or sets the gap (in pixels) between label and line.
        /// </summary>
        [Category("Appearance")]
        [Description("Gap between text and divider line.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(0)]
        public int Gap {
            get { return m_gap; }
            set { 
                m_gap = value; 
                Invalidate();
            }
        }

        /// <summary>
        ///   Overrides <c>Label.OnPaint</c> method.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e) {
            PlaceLine(e.Graphics);
            base.OnPaint(e);
        }

        /// <summary>
        ///   Calculates points for 3D horizontal divider and places it.
        /// </summary>
        /// <param name="g">
        ///   <c>Graphics</c> object.
        /// </param>
        protected void PlaceLine(Graphics g) {
            // evaluates text size
            SizeF textSize = g.MeasureString(this.Text, this.Font);
            int x0 = 0;           // first point x-coordinate
            int x1 = this.Width;  // second point x-coordinate
            // for different horizontal alignments recalculates x-coordinates
            switch (GetHorizontalAlignment()) {
            case HorizontalAlignment.Left:
                x0 = (int)textSize.Width + m_gap;
                break;
            case HorizontalAlignment.Right:
                x1 = this.Width - (int)textSize.Width - m_gap;
                break;
            case HorizontalAlignment.Center:
                x1 = (this.Width - (int)textSize.Width) / 2 - m_gap;
                break;
            }
            int y = (int)textSize.Height / 2;  
            // for different vertical alignments recalculates y-coordinate           
            if (TextAlign == ContentAlignment.MiddleLeft 
                || TextAlign == ContentAlignment.MiddleCenter 
                || TextAlign == ContentAlignment.MiddleRight)
                y = this.Height / 2;
            else if (TextAlign == ContentAlignment.BottomLeft 
                || TextAlign == ContentAlignment.BottomCenter 
                || TextAlign == ContentAlignment.BottomRight)
                y = this.Height - (int)(textSize.Height / 2) - 2;

            Draw3DLine(g, x0, y, x1, y);
            // for centered text, two line sections have to be drawn
            if (TextAlign == ContentAlignment.TopCenter 
                || TextAlign == ContentAlignment.MiddleCenter 
                || TextAlign == ContentAlignment.BottomCenter) {
                x0 = (this.Width + (int)textSize.Width) / 2 + m_gap;
                x1 = this.Width;
                Draw3DLine(g, x0, y, x1, y);
            }
        }

        /// <summary>
        ///   Evaluates horizontal alignment depending on <c>TextAlign</c> and
        ///   <c>RightToLeft</c> settings.
        /// </summary>
        /// <returns>
        ///   One of the <c>HorizontalAlignment</c> values.
        /// </returns>
        protected HorizontalAlignment GetHorizontalAlignment() {
            if (TextAlign == ContentAlignment.TopLeft 
                || TextAlign == ContentAlignment.MiddleLeft 
                || TextAlign == ContentAlignment.BottomLeft) {
                if (RightToLeft == RightToLeft.Yes)
                    return HorizontalAlignment.Right;
                else
                    return HorizontalAlignment.Left;
            }
            if (TextAlign == ContentAlignment.TopRight 
                || TextAlign == ContentAlignment.MiddleRight 
                || TextAlign == ContentAlignment.BottomRight) {
                if (RightToLeft == RightToLeft.Yes)
                    return HorizontalAlignment.Left;
                else
                    return HorizontalAlignment.Right;
            }
            return HorizontalAlignment.Center;
        }

        /// <summary>
        ///   Draws 3D horizontal divider line
        /// </summary>
        /// <param name="g">
        ///   <c>Graphics</c> object.
        /// </param>
        /// <param name="x1">
        ///   x-coordinate of the first point.
        /// </param>
        /// <param name="y1">
        ///   y-coordinate of the first point.
        /// </param>
        /// <param name="x2">
        ///   x-coordinate of the second point. 
        /// </param>
        /// <param name="y2">
        ///   y-coordinate of the second point.
        /// </param>
        protected void Draw3DLine(Graphics g, int x1, int y1, int x2, int y2) {
            g.DrawLine(SystemPens.ControlDark, x1, y1, x2, y2);
            g.DrawLine(SystemPens.ControlLightLight, x1, y1+1, x2, y2+1);
        }

        private int m_gap;
    }
}