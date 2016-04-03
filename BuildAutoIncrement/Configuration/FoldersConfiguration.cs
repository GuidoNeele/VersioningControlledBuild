/*
 * Filename:    FolderConfiguration.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Configuration of folders for SourceSafe and IIS.
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
using System.IO;
using System.Xml.Serialization;

namespace BuildAutoIncrement {
	/// <summary>
	///   Contains configuration of folders for SourceSafe and IIS.
	/// </summary>
    [Serializable]
    public class FolderConfiguration : ICloneable {
        /// <summary>
        ///   Creates default instance.
        /// </summary>
		public FolderConfiguration() {
            m_isAvailable = true;
            m_folder = string.Empty;
		}

        /// <summary>
        ///   Gets/sets a flag if this folder (i.e. component using it) is 
        ///   available at all.
        /// </summary>
        [XmlAttribute]
        public bool IsAvailable {
            get {
                return m_isAvailable;
            }
            set {
                m_isAvailable = value;
            }
        }

        /// <summary>
        ///   Gets/sets a path to the folder.
        /// </summary>
        [XmlText]
        public string Folder {
            get {
                Debug.Assert(m_folder.Length == 0 || Directory.Exists(m_folder));
                return m_folder;
            }
            set {
                Debug.Assert(value.Length == 0 || Directory.Exists(value));
                m_folder = value;
            }
        }

        object ICloneable.Clone() {
            return Clone();
        }

        public FolderConfiguration Clone() {
            return (FolderConfiguration)MemberwiseClone();
        }

        private bool    m_isAvailable;
        private string  m_folder;
	}


    [Serializable, XmlRoot("Folders")]
    public class FoldersConfigurations : ICloneable {
        public FoldersConfigurations() {
            m_sourceSafeFolder = new FolderConfiguration();
            m_iisFolder = new FolderConfiguration();
        }

        [XmlElement("SourceSafe")]
        public FolderConfiguration SourceSafeFolder {
            get {
                return m_sourceSafeFolder;
            }
            set {
                m_sourceSafeFolder = value;
            }
        }

        [XmlElement("IIS")]
        public FolderConfiguration IisFolder {
            get {
                return m_iisFolder;
            }
            set {
                m_iisFolder = value;
            }
        }

        object ICloneable.Clone() {
            return Clone();
        }

        public FoldersConfigurations Clone() {
            FoldersConfigurations clone = new FoldersConfigurations();
            clone.SourceSafeFolder = SourceSafeFolder.Clone();
            clone.IisFolder = IisFolder.Clone();
            return clone;
        }

        private FolderConfiguration m_sourceSafeFolder;
        private FolderConfiguration m_iisFolder;
    }
}