//------------------------------------------------------------------------------
// <copyright file="GuiCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BuildAutoIncrement
{
  /// <summary>
  /// Command handler
  /// </summary>
  internal sealed class GuiCommand : BaseCommand
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="GuiCommand"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    private GuiCommand(Package package) : base(package, 0x0300)    {    }

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static GuiCommand Instance
    {
      get;
      private set;
    }

    /// <summary>
    /// Initializes the singleton instance of the command.
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    public static void Initialize(Package package)
    {
      Instance = new GuiCommand(package);
    }

    /// <summary>
    /// This function is the callback used to execute the command when the menu item is clicked.
    /// See the constructor to see how the menu item is associated with this function using
    /// OleMenuCommandService service and MenuCommand class.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event args.</param>
    protected override void MenuItemCallback(object sender, EventArgs e)
    {
      base.PreMenuItemCallback();

      using (MainForm mainForm = new MainForm(m_solutionBrowser))
      {
#if DEBUG
        mainForm.ShowInTaskbar = true;
#endif
        WindowAdapter wa = new WindowAdapter(m_devEnvApplicationObject.MainWindow.HWnd.ToInt32());
        if (mainForm.ShowDialog(wa) != DialogResult.Cancel)
        {
          ProjectInfo[] toCheckout = mainForm.MarkedProjects;
          m_solutionBrowser.CheckOutProjectVersionFiles(toCheckout);
          // setup projects have to be unloaded in order to avoid popup dialogs
          SetupProjectsLoader spl = new SetupProjectsLoader(m_devEnvApplicationObject);
          spl.UnloadSetupProjects(mainForm.MarkedProjects);
          mainForm.SaveVersions();
          spl.ReloadSetupProjects();
          if (mainForm.CommandToPerform.Length != 0)
          {
            m_devEnvApplicationObject.ExecuteCommand(mainForm.CommandToPerform, "");
          }
        }
      }
    }

  }
}
