/*
 * Filename:    SpinButton.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Spin button.
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
using System.Windows.Forms;

namespace BuildAutoIncrement {
    /// <summary>
    ///   Spin button control. Extends the range to long type range in order
    ///   to cope with largest values of version without overflow problem.
    /// </summary>
    public class SpinButton : VScrollBar {

        public SpinButton() : base() {
        }

        public new long Value {
            get {
                return m_value;
            }
            set {
                m_value = value;
            }
        }

        public new long Maximum {
            get {
                return m_maximum;
            }
            set {
                m_maximum = value;
            }
        }

        public new long Minimum {
            get {
                return m_minimum;
            }
            set {
                m_minimum = value;
            }
        }

        public new long LargeChange {
            get {
                return m_largeChange;
            }
            set {
                m_largeChange = value;
            }
        }

        protected override void OnScroll(ScrollEventArgs se) {
            switch (se.Type) {
            case ScrollEventType.LargeDecrement:
                m_value += LargeChange;
                break;
            case ScrollEventType.LargeIncrement:
                m_value -= LargeChange;
                break;
            case ScrollEventType.SmallDecrement:
                m_value += SmallChange;
                break;
            case ScrollEventType.SmallIncrement:
                m_value -= SmallChange;
                break;
            }
            if (m_value > Maximum)
                m_value = Maximum;
            if (m_value < Minimum)
                m_value = Minimum;
            base.OnScroll(se);
        }


        private long m_value;
        private long m_maximum;
        private long m_minimum;
        private long m_largeChange;
    }
}