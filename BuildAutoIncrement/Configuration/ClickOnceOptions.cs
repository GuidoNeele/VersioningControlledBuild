/*
 * Filename:    ClickOnceOptions.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Options for ClickOnce support.
 * Copyright:   Julijan Šribar, 2004-2009
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

namespace BuildAutoIncrement {

    [Serializable]
    public class ClickOnceOptions {

        public ClickOnceOptions() {
            m_applyVersionToPublishVersion = false;
            m_applyVersionToMinimumRequiredVersion = false;
        }

        public bool ApplyVersionToPublishVersion {
            get { return m_applyVersionToPublishVersion; }
            set { m_applyVersionToPublishVersion = value; }
        }

        public bool ApplyVersionToMinimumRequiredVersion {
            get { return m_applyVersionToMinimumRequiredVersion; }
            set { m_applyVersionToMinimumRequiredVersion = value; }
        }

        private bool m_applyVersionToPublishVersion;
        private bool m_applyVersionToMinimumRequiredVersion;
    }
}
