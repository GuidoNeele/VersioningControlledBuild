/*
 * Filename:    Installer.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Implements additional installation/uninstallation clean-up 
 *              tasks.
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
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BuildAutoIncrement
{

  [System.ComponentModel.RunInstallerAttribute(true)]
  public partial class BuilAutoIncrementInstaller : System.Configuration.Install.Installer
  {

    #region Overriden methods

    /// <summary>
    ///   Overrides <c>Install</c> method to save information on 
    ///   environments for which add-in has been installed.
    /// </summary>
    /// <param name="mySavedState"></param>
    public override void Install(IDictionary savedState)
    {
#if DEBUG_SETUP
            System.Diagnostics.Debugger.Launch();
#endif
      base.Install(savedState);

      try
      {
        string targetDir = GetTargetDir();
        Debug.Assert(targetDir != null && targetDir.Length > 0);
        AddInFileInstaller afi = new AddInFileInstaller(targetDir);

        bool permanentBars = (string)Context.Parameters["permanentbars"] == "1";
        savedState.Add("permanentbars", permanentBars);

        bool allUsers = (string)Context.Parameters["allusers"] == "1";
        bool installAddinFiles = (string)Context.Parameters["installaddinfiles"] == "1";

        foreach (DevEnvironmentVersion devEnvironmentVersion in DevEnvironmentVersions)
        {
          if (IsDevEnvironmentInstalled(devEnvironmentVersion))
          {
            savedState.Add(devEnvironmentVersion.LaunchCondition, true);
            // for XML registered addins, copy corresponding file and save their names 
            // to delete them on uninstallation
            if (installAddinFiles && !devEnvironmentVersion.AddInIsComRegistered)
            {
              string addInFile = afi.Install(allUsers, devEnvironmentVersion.Version);
              savedState.Add(string.Format("{0}.AddInFile", devEnvironmentVersion.Version), addInFile);
            }
          }
        }
      }
      catch (Exception e)
      {
        ExceptionForm.Show(e, InstallErrorCaption);
      }
    }

    /// <summary>
    ///   Overrides <c>Uninstall</c> method to remove toolbar and menubar, 
    ///   delete PreloadAddinState in registry and optionally clear 
    ///   configuration folders for all user profiles.
    /// </summary>
    /// <param name="mySavedState"></param>
    [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
    public override void Uninstall(IDictionary mySavedState)
    {
#if DEBUG_SETUP
            System.Diagnostics.Debugger.Launch();
#endif
      if (mySavedState != null)
      {
        try
        {
          // for permanent commandbars, toolbar and menu must be removed explicitely
          if (mySavedState.Contains("permanentbars") && (bool)mySavedState["permanentbars"] == true)
          {
            // collect a list of all IDEs for which add-in has been installed
            StringCollection devEnvironmentNames = new StringCollection();
            foreach (DevEnvironmentVersion devEnvironmentVersion in DevEnvironmentVersions)
            {
              // check if toolbar was originally installed for the enviromnment, 
              // if environment was started at all since add-in was installed and
              // if this environment still exists
              if (mySavedState.Contains(devEnvironmentVersion.LaunchCondition) && IsDevEnvironmentInstalled(devEnvironmentVersion))
                devEnvironmentNames.Add(devEnvironmentVersion.Name);
            }
            if (devEnvironmentNames.Count > 0)
            {
              RemoveToolbarsForm.Show(devEnvironmentNames);
              foreach (DevEnvironmentVersion devEnvironmentVersion in DevEnvironmentVersions)
              {
                // remove permanent UI entries for selected environments only
                if (devEnvironmentNames.Contains(devEnvironmentVersion.Name))
                  RemovePermamanentUI(devEnvironmentVersion);
              }
            }
          }

          foreach (DevEnvironmentVersion devEnvironmentVersion in DevEnvironmentVersions)
          {
            // clean-up PreloadAddinState registry entries 
            if (mySavedState.Contains(devEnvironmentVersion.LaunchCondition) && devEnvironmentVersion.AddInIsComRegistered && PreloadAddinStateExists(devEnvironmentVersion) && IsDevEnvironmentInstalled(devEnvironmentVersion))
            {
              DeletePreloadAddinState(devEnvironmentVersion);
              mySavedState.Remove(devEnvironmentVersion.LaunchCondition);
            }
          }
          // delete AddIn files
          foreach (DevEnvironmentVersion devEnvironmentVersion in DevEnvironmentVersions)
          {
            string addinFileKey = string.Format("{0}.AddInFile", devEnvironmentVersion.Version);
            if (mySavedState.Contains(addinFileKey))
            {
              string addInFilename = (string)mySavedState[addinFileKey];
              File.Delete(addInFilename);
            }
          }
        }
        catch (Exception e)
        {
          ExceptionForm.Show(e, UnistallErrorCaption);
        }
        DeleteUiSettings();
        if (RemoveConfigurationsForm.Show() == DialogResult.Yes)
        {
          DeleteConfigurations();
        }
      }
      base.Uninstall(mySavedState);
    }

    #endregion // Overriden methods

    #region Private methods

    /// <summary>
    ///   Gets target directory.
    /// </summary>
    /// <returns>
    ///   Full path to the folder where add-in files will be placed.
    /// </returns>
    private string GetTargetDir()
    {
      string targetDir = (string)Context.Parameters["targetdir"];
      // fetching [TARGETDIR] requires trailing backslash:
      // http://msdn.microsoft.com/en-us/library/2w2fhwzz%28VS.80%29.aspx
      // followed by a space:
      // http://social.msdn.microsoft.com/Forums/en-US/winformssetup/thread/4c71d619-5322-4734-89f3-5c1997e42b93/
      // Therefore, obtained target dir must be trimmed off
      return targetDir.TrimEnd(new char[] { ' ', '\\' });
    }

    /// <summary>
    ///   Checks if a version of Visual Studio is installed by reading
    ///   the registry.
    /// </summary>
    /// <param name="devEnvironmentVersion">
    ///   Version of Visual Studio to check for.
    /// </param>
    /// <returns>
    ///   <code>true</code> if Visual Studio is installed, otherwise
    ///   <code>false</code>.
    /// </returns>
    private bool IsDevEnvironmentInstalled(DevEnvironmentVersion devEnvironmentVersion)
    {
      using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(string.Format(VisualStudioRegistryPath, devEnvironmentVersion.Version)))
      {
        if (rk == null)
          return false;
        return rk.GetValue("InstallDir") != null;
      }
    }

    /// <summary>
    ///   Identifies configuration folder for each user and UI
    ///   settings file.
    /// </summary>
    private void DeleteUiSettings()
    {
      using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(ProfilesSubKey))
      {
        string[] profilesSubKeys = rk.GetSubKeyNames();
        foreach (string profile in profilesSubKeys)
        {
          using (RegistryKey rkSub = rk.OpenSubKey(profile))
          {
            string configurationFolder = (string)rkSub.GetValue("ProfileImagePath") + "\\Application Data\\" + ((AssemblyProductAttribute)AssemblyProductAttribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute))).Product;
            if (Directory.Exists(configurationFolder))
            {
              string configurationFile = Path.Combine(configurationFolder, VcbCommandBarsConfigurationPersister.Filename);
              if (File.Exists(configurationFile))
                File.Delete(configurationFile);
            }
          }
        }
      }
    }

    /// <summary>
    ///   Identifies configuration folder for each user and deletes it if
    ///   exists.
    /// </summary>
    private void DeleteConfigurations()
    {
      using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(ProfilesSubKey))
      {
        string[] profilesSubKeys = rk.GetSubKeyNames();
        foreach (string profile in profilesSubKeys)
        {
          using (RegistryKey rkSub = rk.OpenSubKey(profile))
          {
            string configurationFolder = (string)rkSub.GetValue("ProfileImagePath") + "\\Application Data\\" + ((AssemblyProductAttribute)AssemblyProductAttribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute))).Product;
            if (Directory.Exists(configurationFolder))
            {
              Directory.Delete(configurationFolder, true);
            }
          }
        }
      }
    }

    /// <summary>
    ///   Deletes add-in entry in PreloadAddinState key of the current user.
    /// </summary>
    private void DeletePreloadAddinState(DevEnvironmentVersion devEnvironmentVersion)
    {
      using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(PreloadAddinStateKey(devEnvironmentVersion), true))
      {
        if (rk != null && devEnvironmentVersion.AddinProgId.Length > 0)
          rk.DeleteValue(devEnvironmentVersion.AddinProgId, false);
      }
    }

    /// <summary>
    ///   Check if PreloadAddinState key exists (i.e. if environment has been 
    ///   started after add-in installation).
    /// </summary>
    /// <param name="devEnvironmentVersion"></param>
    /// <returns></returns>
    private bool PreloadAddinStateExists(DevEnvironmentVersion devEnvironmentVersion)
    {
      using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(PreloadAddinStateKey(devEnvironmentVersion), true))
      {
        return rk != null && devEnvironmentVersion.AddinProgId.Length > 0 && rk.GetValue(devEnvironmentVersion.AddinProgId) != null;
      }
    }

    /// <summary>
    ///   Gets the PreloadAddinState key.
    /// </summary>
    /// <param name="devEnvironmentVersion"></param>
    /// <returns>
    ///   Full path to the registry key.
    /// </returns>
    private string PreloadAddinStateKey(DevEnvironmentVersion devEnvironmentVersion)
    {
      return string.Format(VisualStudioRegistryPath + "\\{1}", devEnvironmentVersion.Version, PreloadAddinStateSubKey);
    }

    /// <summary>
    ///   Workarround for toolbar not being removed when add-in is 
    ///   uninstalled.
    /// </summary>
    private void RemovePermamanentUI(DevEnvironmentVersion devEnvironmentVersion)
    {
      try
      {
        // since command removal is dealing with CommandBar objects that are in 
        // different namespaces and assemblies in .NET 1.1 and later versions,
        // removal process has been placed into implementation assemblies to
        // cope with these differences. Therefore corresponding method is called 
        // using reflection mechanism.
        Assembly assembly = ImplementationAssemblyLoader.LoadMainAssembly(devEnvironmentVersion.RuntimeVersion);
        Type type = assembly.GetType("BuildAutoIncrement.PermanentUIRemover");
        Debug.Assert(type != null);
        MethodInfo mi = type.GetMethod("Remove");
        Debug.Assert(mi != null && mi.IsStatic);
        mi.Invoke(null, new object[] { devEnvironmentVersion.ProgId });
      }
      catch (Exception e)
      {
#if DEBUG_SETUP
                ExceptionForm.Show(e, devEnvironmentVersion.ProgId);
#else
        string message = string.Format(ToolbarRemovalFailed, devEnvironmentVersion.Name) + Environment.NewLine + RemoveToolbarManually;
        MessageBox.Show(message, MessageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
      }
    }

    #endregion // Private methods

    #region Constants

    /// <summary>
    ///   Registry key with a list of all user profiles.
    /// </summary>
    private const string ProfilesSubKey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\ProfileList";

    private const string VisualStudioRegistryPath = "SOFTWARE\\Microsoft\\VisualStudio\\{0}";

    private const string PreloadAddinStateSubKey = "PreloadAddinState";

    /// <summary>
    ///   Messages.
    /// </summary>
    private const string MessageBoxCaption = "Versioning Controlled Build Setup";
    private const string ToolbarRemovalFailed = "Failed to remove VCB toolbar from {0}.";
    private const string UnistallErrorCaption = "Uninstall Error";
    private const string InstallErrorCaption = "Install Error";
    private const string RemoveToolbarManually = "You'll have to remove the toolbar manually.";

    #endregion // Constants

    #region Embedded classes

    private sealed class DevEnvironmentVersion
    {
      public DevEnvironmentVersion(string version, int runtimeVersion, string name, string progId, string launchCondition, bool addInIsComRegistered)
      {
        Version = version;
        RuntimeVersion = runtimeVersion;
        Name = name;
        ProgId = progId;
        LaunchCondition = launchCondition;
        AddInIsComRegistered = addInIsComRegistered;
      }

      public string AddinProgId
      {
        get
        {
          return AddInIsComRegistered ? "BuildAutoIncrement.Connect" : "BuildAutoIncrement.Connect2";
        }
      }

      public readonly string Version;
      public readonly int RuntimeVersion;
      public readonly string Name;
      public readonly string ProgId;
      public readonly string LaunchCondition;
      public readonly bool AddInIsComRegistered;
    }

    #endregion // Embedded classes

    #region Private fields

    private static readonly DevEnvironmentVersion[] DevEnvironmentVersions = new DevEnvironmentVersion[] {
        new DevEnvironmentVersion("7.1", 1, "Visual Studio 2003", "VisualStudio.DTE.7.1", "VS_7_1_INSTALLDIR", true),
        new DevEnvironmentVersion("8.0", 2, "Visual Studio 2005", "VisualStudio.DTE.8.0", "VS_8_0_INSTALLDIR", true),
                new DevEnvironmentVersion("9.0", 2, "Visual Studio 2008", "VisualStudio.DTE.9.0", "VS_9_0_INSTALLDIR", false),
                new DevEnvironmentVersion("10.0", 3, "Visual Studio 2010", "VisualStudio.DTE.10.0", "VS_10_0_INSTALLDIR", false),
                new DevEnvironmentVersion("11.0", 4, "Visual Studio 2012", "VisualStudio.DTE.11.0", "VS_11_0_INSTALLDIR", false),
                new DevEnvironmentVersion("12.0", 4, "Visual Studio 2013", "VisualStudio.DTE.12.0", "VS_12_0_INSTALLDIR", false),
                                                                                                            };

    #endregion // Private fields
  }
}