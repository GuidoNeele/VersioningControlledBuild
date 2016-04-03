/*
 * Filename:    SetupProjectsLoader.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     AddinImplementation
 * Description: Class that loads/unloads setup projects in Visual Studio 
 *              environment.
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

namespace BuildAutoIncrement {
	/// <summary>
	///   Class responsible for unloading and reloading setup projects.
	///   Unloading is necessary to avoid popup dialogs when version and codes
	///   are modified.
	/// </summary>
	public sealed class SetupProjectsLoader {

        /// <summary>
        ///   Creates loader object.
        /// </summary>
        /// <param name="devEnvApplication">
        ///   Development environment object.
        /// </param>
		public SetupProjectsLoader(DTE devEnvApplication) {
            Debug.Assert(devEnvApplication != null);
            m_devEnvApplication = devEnvApplication;
		}

        /// <summary>
        ///   Unloads projects provided.
        /// </summary>
        /// <param name="projectInfos">
        ///   Array of <c>ProjectInfo</c> objects to unload.
        /// </param>
        public void UnloadSetupProjects(ProjectInfo[] projectInfos) {
            FindAndSelectSetupProjects(projectInfos);
            if (m_selectedSetupProjects.Count > 0) {
                m_devEnvApplication.MainWindow.Activate();
                // Solution Explorer window must be active to allow unloading project
                Window solutionExplorer = m_devEnvApplication.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer);
                solutionExplorer.Activate();
                m_devEnvApplication.ExecuteCommand("Project.UnloadProject", "");
            }
        }

        /// <summary>
        ///   Reloads projects previously unloaded.
        /// </summary>
        public void ReloadSetupProjects() {
            if (m_selectedSetupProjects.Count > 0) {
                SolutionExplorerSelector ses = new SolutionExplorerSelector(m_devEnvApplication);
                foreach (ProjectInfo setupProject in m_selectedSetupProjects) {
                    ses.SelectItem(setupProject, null);
                }
                m_devEnvApplication.MainWindow.Activate();
                // Solution Explorer window must be active to allow reloading project
                Window solutionExplorer = m_devEnvApplication.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer);
                solutionExplorer.Activate();
                m_devEnvApplication.ExecuteCommand("Project.ReloadProject", "");
            }
        }

        /// <summary>
        ///   Selects projects in Solution explorer window.
        /// </summary>
        /// <param name="projectInfos">
        ///   Array of <c>ProjectInfo</c> objects to select.
        /// </param>
        private void FindAndSelectSetupProjects(ProjectInfo[] projectInfos) {
            m_selectedSetupProjects = new ArrayList();
            SolutionExplorerSelector ses = new SolutionExplorerSelector(m_devEnvApplication);
            foreach (ProjectInfo pi in projectInfos) {
                if (pi.ProjectTypeInfo.ProjectType == ProjectType.SetupProject) {
                    ses.SelectItem(pi, null);
                    m_selectedSetupProjects.Add(pi);
                }
            }
        }

        private IList m_selectedSetupProjects;

        private DTE   m_devEnvApplication;
	}
}