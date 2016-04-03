/*
 * Filename:    ConfigurationPersister.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Loads and saves configuration file. 
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
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace BuildAutoIncrement {

    /// <summary>
    ///   Class responsible to load and save configuration file. Implemented
    ///   as a singleton to be accessible from entire code.
    /// </summary>
    public sealed class ConfigurationPersister {

        private static readonly ConfigurationPersister m_instance = new ConfigurationPersister();

        /// <summary>
        ///   Creates <c>ConfigurationPersister</c> object. Tries to read 
        ///   the configuration file. If reading fails, default configuration
        ///   is used.
        /// </summary>
        private ConfigurationPersister() {
            ConfigurationFolder = FileUtil.GetConfigurationFolder();
            ConfigurationFilename = Path.Combine(ConfigurationFolder, Filename);
            // first create a default configuration
            m_configuration = new VcbConfiguration();
            // custom class is used becase <c>XmlSerializer</c> is throwing <c>NullReferenceException</c>s
            //m_xmlSerializer = new XmlSerializer(typeof(VcbConfiguration));
            m_xmlSerializer = new VcbConfigurationSerializer();
            ReadConfiguration();
            // gets path to SourceSafe executable (required for command-line util)
            GetSourceSafePath();
            // get root directory of the IIS
            GetIisRoot();
            Debug.Assert(ConfigurationFolder != null && ConfigurationFolder.Length > 0);
            Debug.Assert(ConfigurationFilename != null && ConfigurationFilename.Length > 0);
            Debug.Assert(m_xmlSerializer != null);
            Debug.Assert(m_configuration != null);
        }

        /// <summary>
        ///   Accessor to <c>ConfigurationPesister</c> instance.
        /// </summary>
        public static ConfigurationPersister Instance {
            get {
                m_instance.ReadConfiguration();
                return m_instance; 
            }
        }

        /// <summary>
        ///   Gets current configuration.
        /// </summary>
        public VcbConfiguration Configuration {
            get { return m_configuration; }
            set { m_configuration = value; }
        }

        /// <summary>
        ///   Stores configuration.
        /// </summary>
        public void StoreConfiguration() {
            Debug.Assert(m_xmlSerializer != null);
            try {
                if (!Directory.Exists(ConfigurationFolder)) {
                    // create directory where configuration will be saved
                    Directory.CreateDirectory(ConfigurationFolder);
                }
                m_xmlSerializer.Serialize(ConfigurationFilename, m_configuration);
            }
            catch (Exception exception) {
                Trace.WriteLine(exception.ToString());
                try {
                    if (File.Exists(ConfigurationFilename))
                        File.Delete(ConfigurationFilename);
                }
                catch {
                }
                throw;
            }
        }

        /// <summary>
        ///   Reads configuration file if it has been updated meanwhile.
        /// </summary>
        private void ReadConfiguration() {
            Debug.Assert(m_xmlSerializer != null);
            if (File.Exists(ConfigurationFilename)) {
                DateTime lastWrite = File.GetLastWriteTime(ConfigurationFilename);
                if (lastWrite > m_lastConfigurationDateTime) {
                    try {
                        m_configuration = m_xmlSerializer.Deserialize(ConfigurationFilename);
                        m_configuration.ConfigurationFileRead = true;
                        m_lastConfigurationDateTime = File.GetLastWriteTime(ConfigurationFilename);
                    }
                    catch (Exception exception) {
                        Trace.WriteLine(exception.ToString());
                        // file seems to be corrupted so delete it
                        try {
                            File.Delete(ConfigurationFilename);
                        }
                        catch {
                        }
                    }
                }
            }
            Debug.Assert(m_configuration != null);
        }

        /// <summary>
        ///   Gets the IIS root, reading it from registry.
        /// </summary>
        private void GetIisRoot() {
            Debug.Assert(m_configuration != null && m_configuration.FoldersConfigurations != null);
            FolderConfiguration iis = m_configuration.FoldersConfigurations.IisFolder;
            Debug.Assert(iis != null);
            // if no folder set but is available (e.g. when starting new version
            // with old configuration)
            if (iis.IsAvailable && iis.Folder.Length == 0) {
                if (InetRootLocator.Instance.IsIisAvailable)
                    iis.Folder = InetRootLocator.Instance.PathWwwRoot;
                else
                    iis.IsAvailable = false;
            }
        }

        /// <summary>
        ///   Gets the SourceSafe executable path, reading from registry.
        /// </summary>
        private void GetSourceSafePath() {
            Debug.Assert(m_configuration != null && m_configuration.FoldersConfigurations != null);
            FolderConfiguration ss = m_configuration.FoldersConfigurations.SourceSafeFolder;
            Debug.Assert(ss != null);
            // if no folder set but is available (e.g. when starting new version
            // with old configuration)
            if (ss.IsAvailable && ss.Folder.Length == 0) {
                if (SourceSafeLocator.Instance.IsSourceSafeAvailable)
                    ss.Folder = SourceSafeLocator.Instance.SourceSafeRoot;
                else
                    ss.IsAvailable = false;
            }
        }

        /// <summary>
        ///   Configuration data object.
        /// </summary>
        private VcbConfiguration m_configuration = null;
        /// <summary>
        ///   <c>XmlSerializer</c> object used to store/load configuration.
        /// </summary>
        //private XmlSerializer m_xmlSerializer = null;
        // custom class is used becase <c>XmlSerializer</c> is throwing <c>NullReferenceException</c>s
        private VcbConfigurationSerializer m_xmlSerializer = null;

        private DateTime m_lastConfigurationDateTime = DateTime.MinValue;

        private readonly string ConfigurationFolder;
        private readonly string ConfigurationFilename;

        private const string Filename = "Configuration.xml";
	}
}