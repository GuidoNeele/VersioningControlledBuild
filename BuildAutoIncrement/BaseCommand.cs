using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BuildAutoIncrement
{
  abstract public class BaseCommand
  {

    /// <summary>
    /// Command menu group (command set GUID).
    /// </summary>
    public static readonly Guid CommandSet = new Guid("2291da24-92e5-4ea4-bdb7-72a9b5ac7d59");

    /// <summary>
    /// VS Package that provides this command, not null.
    /// </summary>
    private readonly Package package;

    public BaseCommand(Package package, int commandId, bool isDynamic = true)
    {
      if (package == null)
      {
        throw new ArgumentNullException("package");
      }

      this.package = package;

      OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
      if (commandService != null)
      {
        var menuCommandID = new CommandID(CommandSet, commandId);
        var menuItem = new OleMenuCommand(this.MenuItemCallback, menuCommandID);
        if (isDynamic)
        {
          menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
        }
        commandService.AddCommand(menuItem);
      }
    }

    private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
    {
      // get the menu that fired the event
      var menuCommand = sender as OleMenuCommand;
      if (menuCommand != null)
      {
        // start by assuming that the menu will not be shown
        menuCommand.Visible = false;
        menuCommand.Enabled = false;

        IVsHierarchy hierarchy = null;
        uint itemid = VSConstants.VSITEMID_NIL;

        if (!IsSingleProjectItemSelection(out hierarchy, out itemid)) return;

        menuCommand.Visible = true;
        menuCommand.Enabled = true;
      }
    }

    public static bool IsSingleProjectItemSelection(out IVsHierarchy hierarchy, out uint itemid)
    {
      hierarchy = null;
      itemid = VSConstants.VSITEMID_NIL;
      int hr = VSConstants.S_OK;

      var monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
      var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
      if (monitorSelection == null || solution == null)
      {
        return false;
      }

      IVsMultiItemSelect multiItemSelect = null;
      IntPtr hierarchyPtr = IntPtr.Zero;
      IntPtr selectionContainerPtr = IntPtr.Zero;

      try
      {
        hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out multiItemSelect, out selectionContainerPtr);

        if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
        {
          // there is no selection
          return false;
        }

        // multiple items are selected
        if (multiItemSelect != null) return false;

        // there is a hierarchy root node selected, thus it is not a single item inside a project

        if (itemid == VSConstants.VSITEMID_ROOT) return false;

        hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
        if (hierarchy == null) return false;

        Guid guidProjectID = Guid.Empty;

        if (ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out guidProjectID)))
        {
          return false; // hierarchy is not a project inside the Solution if it does not have a ProjectID Guid
        }

        // if we got this far then there is a single project item selected
        return true;
      }
      finally
      {
        if (selectionContainerPtr != IntPtr.Zero)
        {
          Marshal.Release(selectionContainerPtr);
        }

        if (hierarchyPtr != IntPtr.Zero)
        {
          Marshal.Release(hierarchyPtr);
        }
      }
    }

    /// <summary>
    /// Gets the service provider from the owner package.
    /// </summary>
    private IServiceProvider ServiceProvider
    {
      get
      {
        return this.package;
      }
    }

    protected void PreMenuItemCallback()
    {
      m_devEnvApplicationObject = (DTE)this.ServiceProvider.GetService(typeof(DTE));
      configuration = ConfigurationPersister.Instance.Configuration;

      Debug.Assert(m_devEnvApplicationObject != null);
      Debug.Assert(configuration != null);

      m_solutionBrowser = new VSSolutionBrowser(m_devEnvApplicationObject, configuration, this.ServiceProvider);
      IProjectFilter projectFilter = new ProjectFilterByType(configuration.NumberingOptions.IncludeSetupProjects, configuration.DisplayOptions.ShowNonVersionableProjects, configuration.DisplayOptions.ShowSubProjectRoot, configuration.DisplayOptions.ShowEnterpriseTemplateProjectRoot);
      m_solutionBrowser.ApplyFilter(projectFilter);
      Debug.Assert(m_solutionBrowser != null);
    }

    abstract protected void MenuItemCallback(object sender, EventArgs e);

    protected DTE m_devEnvApplicationObject;
    protected VSSolutionBrowser m_solutionBrowser;
    protected VcbConfiguration configuration;
  }
}
