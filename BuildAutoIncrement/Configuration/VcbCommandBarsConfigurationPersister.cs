using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Specialized;

namespace BuildAutoIncrement {
    /// <summary>
    ///   Class responsible to load and save add-in GUI configuration file.
    /// </summary>
    public sealed class VcbCommandBarsConfigurationPersister {

        private static readonly VcbCommandBarsConfigurationPersister m_instance = new VcbCommandBarsConfigurationPersister();
   
        /// <summary>
        ///   Creates <c>VcbCommandBarsConfigurationPersister</c> object.
        /// </summary>
        private VcbCommandBarsConfigurationPersister() {
            ConfigurationFolder = FileUtil.GetConfigurationFolder();
            ConfigurationFilename = Path.Combine(ConfigurationFolder, Filename);

            // read current configuration or create empty one if file not found
            m_uiElementsConfigurations = LoadConfiguration();
        }

        public static VcbCommandBarsConfigurationPersister Instance {
            get {
                return m_instance;
            }
        }

        /// <summary>
        ///   Gets configuration.
        /// </summary>
        public VcbCommandBarsConfiguration Configurations {
            get {
                Debug.Assert(m_uiElementsConfigurations != null);
                return m_uiElementsConfigurations;
            }
        }

        /// <summary>
        ///   Reads configuration from the file. If file does not exists,
        ///   creates default configuration.
        /// </summary>
        /// <returns></returns>
        private VcbCommandBarsConfiguration LoadConfiguration() {
            if (Directory.Exists(ConfigurationFolder)) {
                try {
                    using (FileStream reader = new FileStream(ConfigurationFilename, FileMode.Open)) {
                        XmlSerializer serializer = new XmlSerializer(typeof(VcbCommandBarsConfiguration));
                        return (VcbCommandBarsConfiguration)serializer.Deserialize(reader);
                    }
                }
                catch (FileNotFoundException) {
                }
                catch (Exception exception) {
                    ExceptionForm.Show(exception);
                }
            }
            return new VcbCommandBarsConfiguration();
        }

        /// <summary>
        ///   Saves configuration to the file.
        /// </summary>
        public void StoreConfiguration() {
            try {
                if (!Directory.Exists(ConfigurationFolder)) {
                    // create directory where configuration will be saved
                    Directory.CreateDirectory(ConfigurationFolder);
                }
                using (FileStream writer = new FileStream(ConfigurationFilename, FileMode.Create)) {
                    XmlSerializer serializer = new XmlSerializer(typeof(VcbCommandBarsConfiguration));
                    serializer.Serialize(writer, m_uiElementsConfigurations);
                }
            }
            catch (Exception exception) {
                ExceptionForm.Show(exception);
            }
        }

        private readonly VcbCommandBarsConfiguration m_uiElementsConfigurations;

        private readonly string ConfigurationFolder;
        private readonly string ConfigurationFilename;

        public const string Filename = "UiSettings.xml";
    }
}
