/*
 * Filename:    VSSolutionBrowser.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     AddinImplementation
 * Description: Class that collects information for all projects in Visual 
 *              Studio solution. 
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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BuildAutoIncrement
{

  /// <summary>
  ///   Object responsible for collecting project properties.
  /// </summary>
  public class VSSolutionBrowser : SolutionBrowser, IDisposable
  {
    /// <summary>
    ///   Enumeration for OnRunOrPreview setting.
    /// </summary>
    private enum OnRunOrPreviewSetting
    {
      Undefined = -1,
      SaveChanges,
      PromptToSaveChanges,
      DontSaveChanges
    };

    #region Constructor and Dispose methods

    /// <summary>
    ///   Initializes an empty <c>VSSolutionBrowser</c> object.
    /// </summary>
    /*
    private VSSolutionBrowser()
    {
        MessageFilter.Register();
    }
    */

    /// <summary>
    ///   Initializes ProjectBrowser object.
    /// </summary>
    /// <param name="devEnvApplicationObject">
    ///   Development environment from which application is started.
    /// </param>
    public VSSolutionBrowser(DTE devEnvApplicationObject, VcbConfiguration configuration)
        : base(configuration)
    {
      MessageFilter.Register();
      Debug.Assert(devEnvApplicationObject != null);
      m_devEnvApplicationObject = devEnvApplicationObject;
      PreProcess();
      m_sourceSafeCheckOut = new VSSCheckout(m_devEnvApplicationObject);
      LoadProjectsInfo();
    }

    protected override void Dispose(bool disposing)
    {
      if (!m_disposed)
      {
        try
        {
          PostProcess();
          MessageFilter.Revoke();
          m_disposed = true;
        }
        finally
        {
          base.Dispose(disposing);
        }
      }
    }

    #endregion // Constructor and Dispose method

    #region Public properties

    /// <summary>
    ///   Gets the current solution name. 
    /// </summary>
    public override string SolutionName
    {
      get { return Path.GetFileNameWithoutExtension(m_devEnvApplicationObject.Solution.FileName); }
    }

    /// <summary>
    ///   Gets the current solution filename. 
    /// </summary>
    public override string SolutionFilename
    {
      get { return m_devEnvApplicationObject.Solution.FileName; }
    }

    #endregion // Public properties

    #region Public methods

    /// <summary>
    ///   Checks out project version files for projects provided.
    /// </summary>
    /// <param name="projectsToCheckOut">
    ///   Array of <c>ProjectInfo</c> objects which must be checked out.
    /// </param>
    public override void CheckOutProjectVersionFiles(ProjectInfo[] projectsToCheckOut)
    {
      if (m_sourceSafeCheckOut != null)
        m_sourceSafeCheckOut.CheckOut(projectsToCheckOut);
    }

    #endregion // Public methods

    #region Protected methods

    /// <summary>
    ///   Basic preparation before displaying the main form.
    /// </summary>
    protected override void PreProcess()
    {
      Debug.Assert(m_devEnvApplicationObject != null);
      m_initialOnRunOrPreview = OnRunOrPreview;
      m_initialAutoloadExternalChanges = AutoloadExternalChanges;
      if (!m_initialAutoloadExternalChanges)
        AutoloadExternalChanges = true;
      if (m_numberingOptions.SaveModifiedFilesBeforeRunningAddinCommand)
        SaveAllUnsavedDocuments();
      else
        CheckForUnsavedFiles();
    }

    /// <summary>
    ///   Restores environment settings.
    /// </summary>
    protected override void PostProcess()
    {
      OnRunOrPreview = m_initialOnRunOrPreview;
      AutoloadExternalChanges = m_initialAutoloadExternalChanges;
    }

    #endregion // Protected methods

    #region Private methods

    /// <summary>
    ///   Browses the solution for all projects, creates corresponding 
    ///   <c>ProjectInfo</c> and and adds them to <c>m_projects</c> list.
    /// </summary>
    private void LoadProjectsInfo()
    {
      Debug.Assert(m_devEnvApplicationObject != null && m_devEnvApplicationObject.Solution != null);
      Debug.Assert(m_allProjects != null);
      m_allProjects.Clear();
      int level = 0;
      foreach (Project project in m_devEnvApplicationObject.Solution.Projects)
      {
        try
        {
          ProjectTypeInfo projectTypeInfo = ProjectTypeInfo.ProjectTypeInfos[project.Kind];
          if (projectTypeInfo != null)
          {
            Debug.Assert(projectTypeInfo != null);
            // setup projects are treated differently since the version is stored inside project file!
            if (projectTypeInfo == ProjectTypeInfo.SetupProject || projectTypeInfo == ProjectTypeInfo.InstallShieldLEProject)
              m_allProjects.Add(GetSetupInfo(project, "", projectTypeInfo, 0));
            else if (projectTypeInfo == ProjectTypeInfo.EnterpriseProject)
            {
              // it is enterprise template project with subprojects
              ProjectInfo enterpriseTemplateProjectInfo = new ProjectInfo(project.Name, project.FullName, project.Name, projectTypeInfo, level);
              RecurseSubProjectTree(project.ProjectItems, enterpriseTemplateProjectInfo, level);
              m_allProjects.Add(enterpriseTemplateProjectInfo);
            }
            else if (projectTypeInfo == ProjectTypeInfo.SolutionFolder)
            {
              ProjectInfo solutionFolder = new ProjectInfo(project.Name, project.Name, project.Name, projectTypeInfo, level);
              RecurseSubProjectTree(project.ProjectItems, solutionFolder, level);
              m_allProjects.Add(solutionFolder);
            }
            else
            {
              m_allProjects.Add(GetProjectInfo(project, "", projectTypeInfo, level));
            }
          }
          else
            Trace.WriteLine(string.Format("Not supported project kind: {0}", project.Kind));
        }
        catch (Exception e)
        {
          Console.WriteLine(e.ToString());
#if DEBUG
                    ExceptionForm.Show(new WindowAdapter(m_devEnvApplicationObject.MainWindow.HWnd), e, "VCB Error");
#endif
        }
      }
    }

    /// <summary>
    ///   Recurses sub project tree (e.g. for Enterprise template projects)
    ///   searching for the largest version.
    /// </summary>
    /// <param name="projectItems">
    ///   Collection of <c>ProjectItems</c> to recurse.
    /// </param>
    private void RecurseSubProjectTree(ProjectItems parentProjectItems, ProjectInfo parentProjectInfo, int level)
    {
      level++;
      Debug.Assert(parentProjectItems != null);
      Debug.Assert(parentProjectInfo != null);
      try
      {
        foreach (ProjectItem projectItem in parentProjectItems)
        {
          try
          {
            Project subProject = projectItem.SubProject;
            if (subProject != null)
            {
              ProjectTypeInfo subProjectTypeInfo = ProjectTypeInfo.ProjectTypeInfos[subProject.Kind];
              if (subProjectTypeInfo == ProjectTypeInfo.EnterpriseProject || subProjectTypeInfo == ProjectTypeInfo.SolutionFolder)
              {
                ProjectInfo subProjectInfo = new ProjectInfo(subProject.Name, subProject.Name, AppendBranchToPath(parentProjectInfo.UIPath, subProject.Name), subProjectTypeInfo, level);
                RecurseSubProjectTree(subProject.ProjectItems, subProjectInfo, level);
                parentProjectInfo.SubProjects.Add(subProjectInfo);
              }
              else if (subProjectTypeInfo == ProjectTypeInfo.SetupProject)
              {
                ProjectInfo pi = GetSetupInfo(subProject, parentProjectInfo.UIPath, subProjectTypeInfo, level);
                Debug.Assert(pi != null);
                Debug.Assert(m_allProjects != null);
                parentProjectInfo.SubProjects.Add(pi);
              }
              else if (subProjectTypeInfo != null)
              {
                ProjectInfo pi = GetProjectInfo(subProject, parentProjectInfo.UIPath, subProjectTypeInfo, level);
                Debug.Assert(pi != null);
                Debug.Assert(m_allProjects != null);
                parentProjectInfo.SubProjects.Add(pi);
              }
            }
            else
            {
              // folders do not have subprojects but only project items
              ProjectTypeInfo projectTypeInfo = ProjectTypeInfo.ProjectTypeInfos[projectItem.Kind];
              if (projectTypeInfo == ProjectTypeInfo.VirtualFolder)
              {
                ProjectInfo subProjectInfo = new ProjectInfo(projectItem.Name, projectItem.Name, AppendBranchToPath(parentProjectInfo.UIPath, projectItem.Name), projectTypeInfo, level);
                RecurseSubProjectTree(projectItem.ProjectItems, subProjectInfo, level);
                if (subProjectInfo.SubProjects.Count > 0)
                {
                  parentProjectInfo.SubProjects.Add(subProjectInfo);
                }
              }
            }
          }
          catch (NotImplementedException e)
          {
            Debug.WriteLine(e.ToString());
          }
        }
      }
      catch (Exception e)
      {
        ExceptionForm.Show(new WindowAdapter(m_devEnvApplicationObject.MainWindow.HWnd), e, "Browsing Solution Error");
      }
    }

    /// <summary>
    ///   Creates <c>ProjectInfo</c> for project provided.
    /// </summary>
    /// <param name="project">
    ///   Project examined.
    /// </param>
    /// <param name="projectTypeInfo">
    ///   Project type info for the project.
    /// </param>
    /// <returns>
    ///   <c>ProjectInfo</c> for the project provided.
    /// </returns>
    private ProjectInfo GetProjectInfo(Project project, string parentUiPath, ProjectTypeInfo projectTypeInfo, int level)
    {
      if (!projectTypeInfo.IsVersionable)
      {
        return new ProjectInfo(project.Name, project.FullName, AppendBranchToPath(parentUiPath, project.Name), projectTypeInfo, level);
      }
      // array of version streams that will be provided to ProjectInfo constructor
      ArrayList versionStreams = new ArrayList();
      // AssemblyVersions used to find the largest one
      AssemblyVersions assemblyVersions = AssemblyVersions.Empty;
      // for a VC++ project search for resource file (which may contain version) if configured so
      if (projectTypeInfo == ProjectTypeInfo.VCppProject && m_numberingOptions.IncludeVCppResourceFiles)
      {
        string[] resourceFilenames = GetProjectVersionFile(project, "*.rc");
        // if VC++ project contains both AssemblyInfo file and resource 
        // file with version, compare them and get the larger value
        foreach (string resourceFilename in resourceFilenames)
        {
          VersionStream resourceFileStream = new ResourceFileStream(resourceFilename);
          AssemblyVersions resourceVersion = resourceFileStream.GetVersions();
          versionStreams.Add(resourceFileStream);
          assemblyVersions = AssemblyVersions.Max(assemblyVersions, resourceVersion);
        }
      }
      string[] assemblyInfoFilenames = GetProjectVersionFile(project, projectTypeInfo.AssemblyInfoFilename);
      Debug.Assert(assemblyInfoFilenames.Length <= 1);
      if (assemblyInfoFilenames.Length > 0)
      {
        VersionStream assemblyInfoStream = new AssemblyInfoStream(assemblyInfoFilenames[0]);
        AssemblyVersions assemblyInfoVersions = assemblyInfoStream.GetVersions();
        versionStreams.Add(assemblyInfoStream);
        if (assemblyVersions == AssemblyVersions.Empty)
          assemblyVersions = assemblyInfoVersions;
        else
          assemblyVersions = AssemblyVersions.Max(assemblyVersions, assemblyInfoVersions);
      }
      VersionStream[] vs = (VersionStream[])versionStreams.ToArray(typeof(VersionStream));
      bool isProjectModified = IsProjectModified(project, vs);
      return new ProjectInfo(project.Name, project.FullName, AppendBranchToPath(parentUiPath, project.Name), projectTypeInfo, isProjectModified, level, assemblyVersions, vs);
    }

    /// <summary>
    ///   Merges parent path with a new branch.
    /// </summary>
    /// <param name="parentUiPath"></param>
    /// <param name="newBranch"></param>
    /// <returns></returns>
    private string AppendBranchToPath(string parentUiPath, string newBranch)
    {
      Debug.Assert(parentUiPath != null);
      Debug.Assert(newBranch != null && newBranch.Length > 0);
      if (parentUiPath.Length == 0)
        return newBranch;
      return string.Format("{0}{1}{2}", parentUiPath, Path.DirectorySeparatorChar, newBranch);
    }

    /// <summary>
    ///   Creates <c>ProjectInfo</c> for setup project provided.
    /// </summary>
    /// <param name="project">
    ///   Project examined.
    /// </param>
    /// <param name="projectTypeInfo">
    /// </param>
    /// <returns>
    ///   <c>ProjectInfo</c> for the project provided.
    /// </returns>
    private ProjectInfo GetSetupInfo(Project project, string parentUiPath, ProjectTypeInfo projectTypeInfo, int level)
    {
      Debug.Assert(project != null);
      Debug.Assert(projectTypeInfo != null && (projectTypeInfo.ProjectType == ProjectType.SetupProject || projectTypeInfo.ProjectType == ProjectType.InstallShieldLEProject));
      string directoryName = Path.GetDirectoryName(project.FileName);
      string versionFileExtension = Path.GetExtension(projectTypeInfo.AssemblyInfoFilename);
      string setupFilename = Path.ChangeExtension(Path.GetFileName(project.UniqueName), versionFileExtension);
      string filename = Path.Combine(directoryName, setupFilename);
      Debug.Assert(File.Exists(filename));
      VersionStream versionStream = null;
      switch (projectTypeInfo.ProjectType)
      {
        case ProjectType.SetupProject:
          versionStream = new SetupVersionStream(filename);
          break;
        case ProjectType.InstallShieldLEProject:
          versionStream = new InstallShieldLEVersionStream(filename);
          break;
        default:
          Debug.Assert(false);
          break;
      }
      Debug.Assert(versionStream != null);
      AssemblyVersions setupVersion = versionStream.GetVersions();
      // setup projects are always at solution root
      return new ProjectInfo(project.Name, project.FullName, AppendBranchToPath(parentUiPath, project.Name), projectTypeInfo, false, level, setupVersion, new VersionStream[] { versionStream });
    }


    /// <summary>
    ///   Recurses the project to see if any file has been modified more
    ///   recently than version file and sets <c>Modified</c> flag.
    /// </summary>
    private bool IsProjectModified(Project project, VersionStream[] versionStreams)
    {
      DateTime lastWriteTime = DateTime.MinValue;
      string referenceVersionFile = null;
      foreach (VersionStream vs in versionStreams)
      {
        DateTime fileWriteTime = FileUtil.GetLastWriteTime(vs.Filename);
        if (fileWriteTime > lastWriteTime)
        {
          lastWriteTime = fileWriteTime;
          referenceVersionFile = vs.Filename;
        }
      }
      // check if the any file in the project has been modified later than reference file
      if (referenceVersionFile != null)
        return IsAnyFileInProjectNewer(project, lastWriteTime, referenceVersionFile);
      return false;
    }

    /// <summary>
    ///   Recurses a project and checks if there is a file that has been 
    ///   modified later than a reference time. Used to check if any file 
    ///   has been modified later than corresponding AssemblyInfo file.
    /// </summary>
    /// <param name="dateTimeToCompare">
    ///   Reference date&time.
    /// </param>
    /// <param name="referenceFile">
    ///   Reference file to which comparison is done.
    /// </param>
    /// <returns>
    ///   <c>true</c> if there is a file modified after the reference date
    ///   & time.
    /// </returns>
    private bool IsAnyFileInProjectNewer(Project project, DateTime dateTimeToCompare, string referenceFile)
    {
      Debug.Assert(project != null);
      Debug.Assert(referenceFile != null);
      if (project.ProjectItems != null)
      {
        foreach (ProjectItem projectItem in project.ProjectItems)
        {
          if (IsProjectItemNewer(projectItem, dateTimeToCompare, referenceFile))
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    ///   Checks if a project item has been modified after the reference 
    ///   date&time.
    /// </summary>
    /// <param name="projectItem">
    ///   <c>ProjectItem</c> for which check is done.
    /// </param>
    /// <param name="path">
    ///   Path to the project root.
    /// </param>
    /// <param name="dateTimeToCompare">
    ///   Reference date & time.
    /// </param>
    /// <param name="referenceFile">
    ///   Reference file to which comparison is done.
    /// </param>
    /// <returns>
    ///   <c>true</c> if the item has been modified after or at the 
    ///   same time as the reference date & time.
    /// </returns>
    private bool IsProjectItemNewer(ProjectItem projectItem, DateTime dateTimeToCompare, string referenceFile)
    {
      string pathFileName = ProjectItemInfo.GetItemFullPath(projectItem);
      if (pathFileName != null && !FileUtil.IsDirectory(pathFileName) && !FileUtil.PathsAreEqual(pathFileName, referenceFile))
      {
        int comp = dateTimeToCompare.CompareTo((object)FileUtil.GetLastWriteTime(pathFileName));
        if (comp <= 0)
          return true;
      }
      if (projectItem != null && projectItem.ProjectItems != null)
      {
        foreach (ProjectItem projectSubItem in projectItem.ProjectItems)
        {
          if (IsProjectItemNewer(projectSubItem, dateTimeToCompare, referenceFile))
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    ///   Helper function that fetches a development environment property.
    /// </summary>
    /// <param name="category">
    ///   Category in the Tools - Options dialog.
    /// </param>
    /// <param name="page">
    ///   Page inside a category.
    /// </param>
    /// <param name="propertyName">
    ///   Name of the property to fetch.
    /// </param>
    /// <returns>
    ///   Value of the property.
    /// </returns>
    private object GetEnvProperty(string category, string page, string propertyName)
    {
      EnvDTE.Properties properties = m_devEnvApplicationObject.get_Properties(category, page);
      Property property = properties.Item(propertyName);
      Debug.Assert(property != null, "Property " + propertyName + " not found", "Category: " + category + " / Page: " + page);
      return property.Value;
    }

    /// <summary>
    ///   Helper function to set a development environment property.
    /// </summary>
    /// <param name="category">
    ///   Category in the Tools - Options dialog.
    /// </param>
    /// <param name="page">
    ///   Page inside a category.
    /// </param>
    /// <param name="propertyName">
    ///   Name of the property to set.
    /// </param>
    /// <param name="newValue">
    ///   A new value of the property.
    /// </param>
    private void SetEnvProperty(string category, string page, string propertyName, object newValue)
    {
      int retryCount = 20;
      int count = 0;
      while (count < retryCount)
      {
        try
        {
          EnvDTE.Properties properties = m_devEnvApplicationObject.get_Properties(category, page);
          Property property = properties.Item(propertyName);
          Debug.Assert(property != null, "Property " + propertyName + " not found", "Category: " + category + " / Page: " + page);
          property.Value = newValue;
          Trace.WriteLine(string.Format("'SetEnvProperty({0}, {1}, {2}, {3})' executed in {4} attempts", category, page, propertyName, newValue, count + 1));
          return;
        }
        catch (System.Runtime.InteropServices.COMException)
        {
        }
        catch (System.Runtime.InteropServices.InvalidComObjectException)
        {
        }
        System.Threading.Thread.Sleep(100);
        count++;
      }
      Trace.WriteLine(string.Format("Failed to execute 'SetEnvProperty({0}, {1}, {2}, {3})' in {4} attempts.", category, page, propertyName, newValue, retryCount));
    }

    /// <summary>
    ///   Gets or sets OnRunOrPreview property (Tools - Options - 
    ///   Environment category - Projects and Solutions page - Build 
    ///   and Run radiobutton group).
    /// </summary>
    private OnRunOrPreviewSetting OnRunOrPreview
    {
      get
      {
        return (OnRunOrPreviewSetting)GetEnvProperty("Environment", "ProjectsAndSolution", "OnRunOrPreview");
      }
      set
      {
        SetEnvProperty("Environment", "ProjectsAndSolution", "OnRunOrPreview", value);
        Debug.Assert(OnRunOrPreview == value);
      }
    }

    /// <summary>
    ///   Gets or sets AutoloadExternalChanges (Tools - Options -
    ///   Environment category - Documents page - Auto-load changes if 
    ///   saved checkbox).
    /// </summary>
    private bool AutoloadExternalChanges
    {
      get
      {
        return (bool)GetEnvProperty("Environment", "Documents", "AutoloadExternalChanges");
      }
      set
      {
        SetEnvProperty("Environment", "Documents", "AutoloadExternalChanges", value);
        Debug.Assert(AutoloadExternalChanges == value);
      }
    }

    /// <summary>
    ///   Checks for unsaved documents and saves documents according to 
    ///   environment settings, then turns saving option off in order to 
    ///   prevent message pop-ups during build process.
    /// </summary>
    private void CheckForUnsavedFiles()
    {
      Debug.Assert(m_initialOnRunOrPreview != OnRunOrPreviewSetting.Undefined);
      switch (m_initialOnRunOrPreview)
      {
        case OnRunOrPreviewSetting.SaveChanges:
          SaveAllUnsavedDocuments();
          break;
        case OnRunOrPreviewSetting.PromptToSaveChanges:
          PromptForUnsavedDocuments();
          // turn saving temporarily off to prevent pop-up dialogs
          OnRunOrPreview = OnRunOrPreviewSetting.DontSaveChanges;
          break;
        default:
          break;
      }
    }

    /// <summary>
    ///   Saves all unsaved documents.
    /// </summary>
    private void SaveAllUnsavedDocuments()
    {
      Documents documents = m_devEnvApplicationObject.Documents;
      foreach (Document document in documents)
      {
        if (!document.Saved)
          document.Save(document.FullName);
      }
    }

    /// <summary>
    ///   Checks for unsaved documents and if any exists, a confirmation 
    ///   dialog is shown.
    /// </summary>
    private void PromptForUnsavedDocuments()
    {
      ArrayList unsavedDocuments = new ArrayList();
      Documents documents = m_devEnvApplicationObject.Documents;
      foreach (Document document in documents)
      {
        if (!document.Saved)
          unsavedDocuments.Add(document);
      }
      if (unsavedDocuments.Count > 0)
      {
        if (PromptUnsavedDocumentsDialog.Show(new WindowAdapter(m_devEnvApplicationObject.MainWindow.HWnd), unsavedDocuments) == System.Windows.Forms.DialogResult.Cancel)
        {
          throw new UserCancelledException();
        }
      }
    }

    /// <summary>
    ///   Starts searching the project directories to find file with 
    ///   version information. Filename pattern argument passed contains 
    ///   pattern for the filename which is searched for. It can be a plain
    ///   filename or wildcard with a certain extension.
    /// </summary>
    /// <param name="project">
    ///   Project to recurse.
    /// </param>
    /// <param name="filenamePattern">
    ///   Name of the file to search for. May contain wildcard characters.
    /// </param>
    /// <returns>
    ///   Full path to the file with version.
    /// </returns>
    private string[] GetProjectVersionFile(Project project, string filenamePattern)
    {
      ArrayList versionFiles = new ArrayList();
      if (project.ProjectItems != null)
      {
        foreach (ProjectItem projectItem in project.ProjectItems)
        {
          versionFiles.AddRange(RecurseProjectForVersionInfoFile(projectItem, project.Name, filenamePattern));
        }
      }
      return (string[])versionFiles.ToArray(typeof(string));
    }

    /// <summary>
    ///   Recurses project item searching for a version file with a given 
    ///   name pattern.
    /// </summary>
    /// <param name="projectItem">
    ///   <c>ProjectItem</c> to recurse.
    /// </param>
    /// <param name="path">
    ///   Path to start search from.
    /// </param>
    /// <param name="filenamePattern">
    ///   Name of the file to search for. May contain wildcard characters.
    /// </param>
    /// <returns>
    ///   Array of full filenames (including path) if version file has been 
    ///   found, otherwise, empty array.
    /// </returns>
    private string[] RecurseProjectForVersionInfoFile(ProjectItem projectItem, string path, string filenamePattern)
    {
      ArrayList versionFiles = new ArrayList();
      if (projectItem.Object != null)
      {
        string pathFileName = AppendBranchToPath(path, projectItem.Name);
        if (FileUtil.FilenameMatchesPattern(projectItem.Name, filenamePattern))
        {
          string filename = ProjectItemInfo.GetItemFullPath(projectItem);
          // it is not RC file (i.e. it is AssemblyInfo file), 
          // then it is the only version file so we can return
          if (!filenamePattern.Equals("*.rc"))
            return new string[] { filename };
          // there could be more .rc files, so check if this one contains version info
          ResourceFileStream rfs = new ResourceFileStream(filename);
          if (!rfs.GetVersions().Equals(AssemblyVersions.Empty))
            versionFiles.Add(filename);
        }
        if (projectItem.ProjectItems != null)
        {
          foreach (ProjectItem projectSubItem in projectItem.ProjectItems)
          {
            versionFiles.AddRange(RecurseProjectForVersionInfoFile(projectSubItem, pathFileName, filenamePattern));
          }
        }
      }
      return (string[])versionFiles.ToArray(typeof(string));
    }

    #endregion // Private methods

    #region Private fields

    /// <summary>
    ///   Enclosing development environment object.
    /// </summary>
    private DTE m_devEnvApplicationObject;
    /// <summary>
    ///   Initial OnRunOrPreview setting.
    /// </summary>
    private OnRunOrPreviewSetting m_initialOnRunOrPreview = OnRunOrPreviewSetting.Undefined;

    private bool m_initialAutoloadExternalChanges;

    private bool m_disposed = false;

    #endregion // Private fields

  }
}