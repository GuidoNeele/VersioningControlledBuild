/*
 * Filename:    ListViewColumnWidths.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: GUI form listview column widths.
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
using System.Xml.Serialization;

namespace BuildAutoIncrement {

    [Serializable]
    public class ListViewColumnWidths {

        public ListViewColumnWidths() {
            m_projectName    = 175;
            m_currentVersion = 85;
            m_modified       = 120;
            m_toBeVersion    = 85;
        }

        public int ProjectName {
            get { return m_projectName; }
            set { m_projectName = value; }
        }

        public int CurrentVersion {
            get { return m_currentVersion; }
            set { m_currentVersion = value; }
        }

        public int Modified {
            get { return m_modified; }
            set { m_modified = value; }
        }

        public int ToBeVersion {
            get { return m_toBeVersion; }
            set { m_toBeVersion = value; }
        }
    
        private int m_projectName;
        private int m_currentVersion;
        private int m_modified;
        private int m_toBeVersion;
    }
}