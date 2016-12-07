/*
 * Filename:    VssCheckout.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     AddinImplementation
 * Description: Helper class used to checkout items under SourceSafe control 
 *              from Visual Studio environment. 
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
using EnvDTE;

using System;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using EnvDTE80;

namespace BuildAutoIncrement {
  /// <summary>
  ///     Helper object used to checkout items.
  /// </summary>
    public class VSSCheckout : SourceSafeCheckout {

        /// <summary>
        ///   Hides empty constructor.
        /// </summary>
        private VSSCheckout() {
        }

        /// <summary>
        ///   Creates <c>VSSCheckout</c> for the Visual Studio development 
        ///   environment provided.
        /// </summary>
        /// <param name="environment">
        ///   Visual Studio development environment.
        /// </param>
        public VSSCheckout(DTE environment) : base() {
            m_environment = environment;
        }

        /// <summary>
        ///   Checks out version files for the array of <c>ProjectInfo</c> 
        ///   objects provided.
        /// </summary>
        /// <param name="projectsToCheckOut">
        ///   An array of <c>ProjectInfo</c> objects for which version files
        ///   have to be checked out.
        /// </param>
        public override void CheckOut(ProjectInfo[] projectsToCheckOut) {
            Debug.Assert(projectsToCheckOut != null);
            Debug.Assert(m_projectInfosFailedToCheckOut != null && m_projectInfosFailedToCheckOut.Count == 0);
            if (projectsToCheckOut.Length == 0)
                return;
            object[] filesToCheckOut = GetFilesToCheckOut(projectsToCheckOut);
            if (filesToCheckOut.Length == 0)
                return;
            bool checkedOut = CheckOutFilesThroughDTE(filesToCheckOut);
            // in the case that SourceSafe is configured so that each checkout is prompted, 
            // it may not be possible to checkout items through DTE
            if (!checkedOut) {
                CheckOutFilesManually(projectsToCheckOut);
            }
            // now check if all files have successfully been checked out
            ArrayList filesNotCheckedOut = new ArrayList();
            foreach (ProjectInfo pi in projectsToCheckOut) {
                foreach (string filename in pi.VersionFilenames) {
                    if (!m_environment.SourceControl.IsItemCheckedOut(filename)) {
                        filesNotCheckedOut.Add(filename);
                        if (!m_projectInfosFailedToCheckOut.Contains(pi))
                            m_projectInfosFailedToCheckOut.Add(pi);
                    }
                    else {
                        m_filesCheckedOut.Add(filename);
                    }
                }
            }
            if (filesNotCheckedOut.Count > 0) {
                if (CheckOutErrorDialog.Show(new WindowAdapter(m_environment.MainWindow.HWnd), (string[])filesNotCheckedOut.ToArray(typeof(string))) == DialogResult.No) 
                    throw new UserCancelledException();
            }
        }

        /// <summary>
        ///   Checks items out using Visual Studio automation.
        /// </summary>
        /// <param name="projectsToCheckOut">
        ///   An array of <c>ProjectInfo</c> objects for which version files 
        ///   habe to be checked out.
        /// </param>
        private bool CheckOutFilesThroughDTE(object[] filesToCheckOut) {
            Debug.Assert(filesToCheckOut != null && filesToCheckOut.Length > 0);
            return m_environment.SourceControl.CheckOutItems(ref filesToCheckOut);
        }

        /// <summary>
        ///   Checks items out using GUI environment: selects items in 
        ///   SolutionBrowser window and launches checkout command.
        ///   http://groups.google.com/group/microsoft.public.vsnet.vss/msg/b6e12e46cd7140d9?as_umsgid=uVPvlJANDHA.3088@TK2MSFTNGP10.phx.gbl
        /// </summary>
        private void CheckOutFilesManually(ProjectInfo[] projectsToCheckOut) {
            SelectItemsInSolutionExplorer(projectsToCheckOut);
            Window solutionExplorer = m_environment.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer);
            solutionExplorer.Activate();
            // an alternate method would be:
            // object customin = new object();
            // object customout = new object();
            // m_devEnvironment.Commands.Raise("{AA8EB8CD-7A51-11D0-92C3-00A0C9138C45}", 11157, ref customin, ref customout);
            m_environment.ExecuteCommand("File.CheckOut", "");
        }

        /// <summary>
        ///   Selects items to check out in Solution Browser window.
        /// </summary>
        private void SelectItemsInSolutionExplorer(ProjectInfo[] projectsToCheckOut) {
            Debug.Assert(m_environment != null);
            Debug.Assert(projectsToCheckOut != null);
            SolutionExplorerSelector ses = new SolutionExplorerSelector(m_environment);
            foreach (ProjectInfo pi in projectsToCheckOut) {
                if (pi.ProjectTypeInfo.ProjectType == ProjectType.SetupProject)
                    ses.SelectItem(pi, null);
                else
                    ses.SelectItem(pi, pi.VersionFilenames);
            }
            m_environment.MainWindow.Activate();
        }

        /// <summary>
        ///   Returns an array of version files to check out.
        /// </summary>
        /// <param name="projectsToCheckOut">
        ///   Array of <c>ProjectInfo</c> objects corresponding to files to be
        ///   checked out.
        /// </param>
        /// <returns>
        ///   Array of objects containing filenames to check out.
        /// </returns>
        private object[] GetFilesToCheckOut(ProjectInfo[] projectsToCheckOut) {
            Debug.Assert(projectsToCheckOut != null);
            ArrayList filesToCheckout = new ArrayList();
            foreach (ProjectInfo projectInfo in projectsToCheckOut) {
                foreach (string filename in projectInfo.VersionFilenames) {
                    if (m_environment.SourceControl.IsItemUnderSCC(filename) && !m_environment.SourceControl.IsItemCheckedOut(filename)) {
                        filesToCheckout.Add(filename);
                    }
                }
            }
            return (object[])filesToCheckout.ToArray(typeof(object));
        }

        /// <summary>
        ///   Checks out a file provided using Visual Studio automation.
        /// </summary>
        /// <param name="fileName">
        ///   Name of the file to checkout.
        /// </param>
        /// <returns>
        ///   <c>true</c> if checkout operation succeeded.
        /// </returns>
        /*
        protected bool CheckOutFile(string fileName) {
            Debug.Assert(fileName != null);
            Debug.Assert(m_environment != null);
            if (m_environment.SourceControl.IsItemUnderSCC(fileName) && !m_environment.SourceControl.IsItemCheckedOut(fileName)) {
                return m_environment.SourceControl.CheckOutItem(fileName);
            }
            return true;
        }
        */

        private DTE m_environment;
    }
}