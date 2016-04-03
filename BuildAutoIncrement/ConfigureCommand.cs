//------------------------------------------------------------------------------
// <copyright file="RebuildVersionsCommand.cs" company="Company">
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
  internal sealed class ConfigureCommand : BaseCommand
  {

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureCommand"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    private ConfigureCommand(Package package) : base(package, 0x0306, false) { }

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static ConfigureCommand Instance
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
      Instance = new ConfigureCommand(package);
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

      try
      {
        ConfigurationForm configurationForm = new ConfigurationForm();
        if (configurationForm.ShowDialog(new WindowAdapter(m_devEnvApplicationObject.MainWindow.HWnd)) == DialogResult.OK)
        {
          ConfigurationPersister.Instance.Configuration = configurationForm.GetConfiguration();
          ConfigurationPersister.Instance.StoreConfiguration();
        }
      }
      catch (Exception ex)
      {
        ExceptionForm.Show(new WindowAdapter(m_devEnvApplicationObject.MainWindow.HWnd), ex, "Error starting Versioning Controlled Build add-in");
      }
    }
  }
}
