/*
 * Filename:    AddInFileInstaller.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Class responsible for adapting and placing .AddIn file. 
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

namespace BuildAutoIncrement
{
    /// <summary>
    ///   Responsible to preparing and installing/uninstalling .AddIn file.
    /// </summary>
    class AddInFileInstaller
    {
        /// <summary>
        ///   Creates XmlDocument from resource and adapts the target path.
        /// </summary>
        /// <param name="addinFolder">
        ///   Full path to the target directory.
        /// </param>
        public AddInFileInstaller(string addinFolder)
        {
            m_addInFile = LoadFromResource("BuildAutoIncrement.Resources.VCB.AddIn");
            Debug.Assert(m_addInFile != null);
            AddaptAddInFileContent(addinFolder);
        }

        #region Public methods

        /// <summary>
        ///   Modifies the content of the .AddIn file to contain the contain correct
        ///   version of the HostApplication and places the file in corresponding folder
        ///   for all or current user.
        /// </summary>
        /// <param name="allUsers">
        ///   <code>true</code> when addin is installed for all users, <code>false</code> for 
        ///   current user only. 
        /// </param>
        /// <param name="addinFolder">
        ///   Path to folder where add-in is being installed.
        /// </param>
        public string Install(bool allUsers, string devEnvVersion)
        {
            Debug.Assert(m_addInFile != null);
            try
            {
                SetVersionInAddInFile(devEnvVersion);
                string addinFileFolder = GetAddInFileFolder(allUsers, devEnvVersion);
                if (!Directory.Exists(addinFileFolder))
                    Directory.CreateDirectory(addinFileFolder);
                string fullPath = Path.Combine(addinFileFolder, "VCB.AddIn");
                m_addInFile.Save(fullPath);
                return fullPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return string.Empty;
            }
        }

        #endregion // Public methods

        #region Private methods

        /// <summary>
        ///   Evaluates the folder for the .AddIn file considering the version of 
        ///   development environment and installation type (all users/current user only).
        /// </summary>
        /// <param name="allUsers">
        ///   <code>true</code> if addin is installed for all users, otherwise <code>false</code>.
        /// </param>
        /// <param name="devEnvVersion">
        ///   String representing version of development environment.
        /// </param>
        /// <returns>
        ///   Full path to the folder into which .AddIn file must be placed.
        /// </returns>
        private string GetAddInFileFolder(bool allUsers, string devEnvVersion)
        {
            string rootFolder = Environment.GetFolderPath((allUsers) ? Environment.SpecialFolder.CommonApplicationData : Environment.SpecialFolder.ApplicationData);
            string branchfolder = string.Format(@"Microsoft\VisualStudio\{0}\AddIns", devEnvVersion);
            return Path.Combine(rootFolder, branchfolder);
        }

        /// <summary>
        ///   Loads XML file from embeded resource.
        /// </summary>
        /// <param name="resourceName">
        ///   Full name of the resource.
        /// </param>
        /// <returns>
        ///   <code>XmlDocument</code> object.
        /// </returns>
        private XmlDocument LoadFromResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
#if DEBUG
            string[] resourceNames = assembly.GetManifestResourceNames();
#endif
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                return doc;
            }
        }

        /// <summary>
        ///   Adapts .AddIn file:
        ///   Prepends folder to /Extensibility/Addin/Assembly element so that
        ///   .AddIn file will point to the actual location where addin is installed.
        ///   Modifies /Extensibility/Addin/CommandPreload element according to
        ///   toolbar/menu installation type (temporary or permanent).
        /// </summary>
        /// <param name="addinFolder">
        ///   Full path which must be prepended to assembly name.
        /// </param>
        private void AddaptAddInFileContent(string addinFolder)
        {
            string ns = m_addInFile.DocumentElement.NamespaceURI;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(m_addInFile.NameTable);
            nsmgr.AddNamespace("ae", m_addInFile.DocumentElement.NamespaceURI);
            XmlElement assemblyNode = (XmlElement)m_addInFile.SelectSingleNode("/ae:Extensibility/ae:Addin/ae:Assembly", nsmgr);
            assemblyNode.InnerText = Path.Combine(addinFolder, assemblyNode.InnerText);
            // probably CommandPreload should be modified to 0 for temporary bars?
            //XmlElement preloadNode = (XmlElement)m_addInFile.SelectSingleNode("/ae:Extensibility/ae:Addin/ae:CommandPreload", nsmgr);
            //preloadNode.InnerText = "0";
        }

        /// <summary>
        ///   Sets Version element for a HostApplication.
        /// </summary>
        /// <param name="version">
        ///   Version to set.
        /// </param>
        private void SetVersionInAddInFile(string version)
        {
            string ns = m_addInFile.DocumentElement.NamespaceURI;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(m_addInFile.NameTable);
            nsmgr.AddNamespace("ae", m_addInFile.DocumentElement.NamespaceURI);
            XmlElement versionNode = (XmlElement)m_addInFile.SelectSingleNode("/ae:Extensibility/ae:HostApplication/ae:Version", nsmgr);
            versionNode.InnerText = version;
        }

        #endregion // Private methods

        private XmlDocument m_addInFile;
    }
}
