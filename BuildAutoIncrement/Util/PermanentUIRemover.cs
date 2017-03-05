/*
 * Filename:    ToolbarRemover.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     AddinImplementation
 * Description: Utility structure used to remove add-in UI controls from Visual
 *              Studio.
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

#if !FX1_1
using EnvDTE80;
using DTE = EnvDTE80.DTE2;
#endif

#if FX1_1
using Microsoft.Office.Core;
#else
using Microsoft.VisualStudio.CommandBars;
#endif

using System;
using System.Diagnostics;

namespace BuildAutoIncrement {
	/// <summary>
	///   Utility class that removes add-in commands and toolbar during 
    ///   uninstallation. Called by Installer class.
	/// </summary>
	public class PermanentUIRemover {

        /// <summary>
        ///   Removes VCB toolbar and menu from Visual Studio.
        /// </summary>
        /// <param name="progId">
        ///   ID of the Visual Studio.
        /// </param>
        public static void Remove(string progId) {
            DTE dte = null;
            try {
                Type t = Type.GetTypeFromProgID(progId);
                if (t != null) {
                    dte = (DTE)Activator.CreateInstance(t);
                    Debug.Assert(dte != null);
                    DeleteCommands(dte);
                    DeleteBars(dte);
                }
            }
            finally {
                if (dte != null)
                    dte.Quit();
            }
        }

        /// <summary>
        ///   Delete commands.
        /// </summary>
        /// <param name="dte">
        ///   <c>DTE</c> object (VS environment) to remove from.
        /// </param>
        private static void DeleteCommands(DTE dte) {
            if (dte.Version == "7.10") {
                // hack for VS2003: to remove commands entirely, commands are added
                // to and removed from Tools menu. Otherwise, they remain in
                // the command cache and are displayed even after add-in 
                // is uninstalled.
                // http://support.microsoft.com/default.aspx?scid=kb;en-us;555322
                AddAndRemoveToTools(dte, "BuildAutoIncrement.Connect." + Constants.Commands.GUI);
                AddAndRemoveToTools(dte, "BuildAutoIncrement.Connect." + Constants.Commands.Build);
                AddAndRemoveToTools(dte, "BuildAutoIncrement.Connect." + Constants.Commands.Rebuild);
                AddAndRemoveToTools(dte, "BuildAutoIncrement.Connect." + Constants.Commands.Save);
                AddAndRemoveToTools(dte, "BuildAutoIncrement.Connect." + Constants.Commands.PrintVersions);
                AddAndRemoveToTools(dte, "BuildAutoIncrement.Connect." + Constants.Commands.ExportVersions);
                AddAndRemoveToTools(dte, "BuildAutoIncrement.Connect." + Constants.Commands.Configure);
                AddAndRemoveToTools(dte, "BuildAutoIncrement.Connect." + Constants.Commands.About);
                return;
            }
            foreach (Command command in dte.Commands) {
                if (command.Name.StartsWith("BuildAutoIncrement.Connect.") || command.Name.StartsWith("BuildAutoIncrement.Connect2."))
                    command.Delete();
            }
        }

        /// <summary>
        ///   Removes VCB toolbar and menu from Visual Studio.
        /// </summary>
        /// <param name="dte">
        ///   <c>DTE</c> object (VS environment) to remove from.
        /// </param>
        private static void DeleteBars(DTE dte) {
            try {
                CommandBar toolbar = ((CommandBars)dte.CommandBars)[Constants.CommandBarName];
                if (toolbar != null)
                    dte.Commands.RemoveCommandBar(toolbar);
            }
            catch { 
            }
            // remove menu main entry
            try {
                for (int i = ((CommandBars)dte.CommandBars).ActiveMenuBar.Controls.Count; i > 0 ; i--) {
                    CommandBarControl cbc = (CommandBarControl)((CommandBars)dte.CommandBars).ActiveMenuBar.Controls[i].Control;
                    if (cbc.Caption == Constants.MenuName) {
                        cbc.Delete(false);
                        break;
                    }
                }
            }
            catch {
            }
        }

        /// <summary>
        ///   Adds and remove commands in order to remove them entirely from
        ///   development environment.
        /// </summary>
        /// <param name="dte">
        ///   Visual Studio environment.
        /// </param>
        /// <param name="commandName">
        ///   Name of the command to add and remove.
        /// </param>
        private static void AddAndRemoveToTools(DTE dte, string commandName) {
            Debug.Assert(dte.Version == "7.10");
            try {
                Command command = dte.Commands.Item(commandName, -1);
                if (command != null) {
                    CommandBar toolBar = ((CommandBars)dte.CommandBars)["Tools"];
                    command.AddControl(toolBar, 1);
                    command.Delete();
                }
            }
            catch (Exception e) {
                Trace.WriteLine(string.Format("Command {0} not found.", commandName));
            }
        }
    }
}