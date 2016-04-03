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
  internal sealed class SaveVersionsCommand : BaseCommand
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveVersionsCommand"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    private SaveVersionsCommand(Package package) : base(package, 0x0304) { }

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static SaveVersionsCommand Instance
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
      Instance = new SaveVersionsCommand(package);
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

      ListExporterToFile lef = null;
      switch (ConfigurationPersister.Instance.Configuration.ExportConfiguration.ExportFileFormat)
      {
        case ExportFileFormat.PlainText:
          lef = new ListExporterToPlainTextFile();
          break;
        case ExportFileFormat.CSV:
          lef = new ListExporterToCsvFile();
          lef.Separator = ConfigurationPersister.Instance.Configuration.ExportConfiguration.CsvSeparator;
          break;
        default:
          Debug.Assert(false, "Not supported ExportFileFormat");
          break;
      }
      lef.Export(m_solutionBrowser.SolutionName, m_solutionBrowser.SolutionFilename, m_solutionBrowser.ProjectInfoList);
    }
  }
}
