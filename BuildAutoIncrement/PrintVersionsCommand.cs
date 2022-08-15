//------------------------------------------------------------------------------
// <copyright file="RebuildVersionsCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BuildAutoIncrement
{
  /// <summary>
  /// Command handler
  /// </summary>
  internal sealed class PrintVersionsCommand : BaseCommand
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintVersionsCommand"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    private PrintVersionsCommand(Package package) : base(package, 0x0305) { }

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static PrintVersionsCommand Instance
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
      Instance = new PrintVersionsCommand(package);
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

      ListPrinter lp = new ListPrinter(new WindowAdapter(m_devEnvApplicationObject.MainWindow.HWnd.ToInt32()));
      lp.Print(string.Empty, m_solutionBrowser.SolutionName, m_solutionBrowser.ProjectInfoList);
    }
  }
}
