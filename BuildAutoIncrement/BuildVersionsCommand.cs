//------------------------------------------------------------------------------
// <copyright file="BuildVersionsCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BuildAutoIncrement
{
  /// <summary>
  /// Command handler
  /// </summary>
  internal sealed class BuildVersionsCommand : BaseCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BuildVersionsCommand"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    private BuildVersionsCommand(Package package) : base(package, 0x0301) { }

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static BuildVersionsCommand Instance
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
      Instance = new BuildVersionsCommand(package);
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

      m_solutionBrowser.CheckOutProjectVersionFiles(m_solutionBrowser.ProjectsToUpdate);
      SetupProjectsLoader spl = new SetupProjectsLoader(m_devEnvApplicationObject);
      spl.UnloadSetupProjects(m_solutionBrowser.ProjectsToUpdate);
      m_solutionBrowser.SaveVersions();
      spl.ReloadSetupProjects();

      bool runCommand = true;
      if (m_solutionBrowser.UpdateSummary.UpdatedItemsCount == 0)
        runCommand = NoUpdateForm.Show(new WindowAdapter(m_devEnvApplicationObject.MainWindow.HWnd.ToInt32())) == DialogResult.Yes;

      if (runCommand)
        m_devEnvApplicationObject.ExecuteCommand("Build.BuildSolution", "");

      if (m_solutionBrowser.UpdateSummary.UpdatedItemsCount > 0 && ConfigurationPersister.Instance.Configuration.DisplayOptions.ShowSuccessDialog)
        OperationSuccesForm.Show(new WindowAdapter(m_devEnvApplicationObject.MainWindow.HWnd.ToInt32()), m_solutionBrowser.UpdateSummary.SummaryItems);

    }
  }
}
