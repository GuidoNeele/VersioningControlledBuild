/*
 * Filename:    SolutionExplorerSelector.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     AddinImplementation
 * Description: Class that selects projects in Solution Explorer window of 
 *              Visual Studio environment.
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

namespace BuildAutoIncrement {
	/// <summary>
    ///     Helper object used to select items via Visual Studio IDE.
    ///     Used primarily for checkout of items under SSC.
    /// </summary>
	public sealed class SolutionExplorerSelector {

        private SolutionExplorerSelector() {
            m_selectedItemsCount = 0;
        }

        /// <summary>
        ///   Creates <c>SolutionExplorerCheckoutHelper</c> object with 
        ///   attached VS IDE object.
        /// </summary>
        /// <param name="environment"></param>
        public SolutionExplorerSelector(DTE environment) : this() {
            UIHierarchy solutionExplorer = (UIHierarchy)environment.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;
            m_rootItem = solutionExplorer.UIHierarchyItems.Item(1);
        }

        /// <summary>
        ///   Selects item in the Solution Explorer that corresponds to the 
        ///   file containing version information.
        /// </summary>
        /// <param name="projectName">
        ///   Name of the project in which item must be searched for.
        /// </param>
        /// <param name="filenames">
        ///   Array of filenames to select. If array is empty or <c>null</c> then
        ///   the root (i.e. node corresponding to the project is selected.
        /// </param>
        public void SelectItem(ProjectInfo projectInfo, string[] filenames) {
            Debug.Assert(m_rootItem != null);
            Debug.Assert(projectInfo.UIPath.Length > 0);
            string[] pathToProjectRoot = projectInfo.UIPath.Split('\\');
            UIHierarchyItem projectRoot = m_rootItem;
            foreach (string pathComponent in pathToProjectRoot) {
                projectRoot = projectRoot.UIHierarchyItems.Item(pathComponent);
                Debug.Assert(projectRoot != null);
            }
            if (filenames == null || filenames.Length == 0) {
                SelectUIHierarchyItem(projectRoot);
                return;
            }
            foreach (string filename in filenames) {
                RecurseProjectTree(projectRoot, filename);
            }
        }

        /// <summary>
        ///   Recurses project tree in Solution explorer, searching for the 
        ///   item for which path is provided. When found, item is selected.
        /// </summary>
        /// <param name="projectInfo">
        ///   <c>ProjectInfo</c> of the project for which AssemblyInfo file has 
        ///   to be selected.
        /// </param>
        /// <param name="parentNode">
        ///   Parent node in the Solution Explorer from which search is 
        ///   started.
        /// </param>
        /// <param name="filename2select">
        ///   Full name of the file to select.
        /// </param>
        /// <returns>
        ///   Returns <c>true</c> if file has been found and selected; used to
        ///   break recursion if file has been found.
        /// </returns>
        private UIHierarchyItem RecurseProjectTree(UIHierarchyItem parentNode, string filename2select) {
            Debug.Assert(parentNode != null);
            foreach (UIHierarchyItem child in parentNode.UIHierarchyItems) {
                ProjectItem projectItem = child.Object as ProjectItem;
                if (projectItem != null) {
                    string fullPath = ProjectItemInfo.GetItemFullPath(projectItem);
                    if (string.Compare(fullPath, filename2select, true) == 0) {
                        SelectUIHierarchyItem(child);
                        return child;
                    }
                    UIHierarchyItem item = RecurseProjectTree(child, filename2select);
                    if (item != null)
                        return item;
                }
            }
            return null;
        }

        /// <summary>
        ///   Selects an item in SolutionExplorer windows.
        /// </summary>
        /// <param name="item">
        ///   Item to select.
        /// </param>
        private void SelectUIHierarchyItem(UIHierarchyItem item) {
            m_selectedItemsCount++;
            if (m_selectedItemsCount > 1)
                item.Select(vsUISelectionType.vsUISelectionTypeToggle);
            else
                item.Select(vsUISelectionType.vsUISelectionTypeSelect);
            Debug.Assert(item.IsSelected);
        }

        private UIHierarchyItem m_rootItem = null;

        private int             m_selectedItemsCount = 0;

    }
}