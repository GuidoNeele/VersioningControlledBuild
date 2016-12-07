/*
 * Filename:    FileUtil.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: File utility methods.
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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace BuildAutoIncrement
{

  /// <summary>
  /// A set of file utilities.
  /// </summary>
  public struct FileUtil
  {

    /// <summary>
    ///   Reads text from a file.
    /// </summary>
    /// <param name="fileName">
    ///   Filename.
    /// </param>
    /// <param name="encoding">
    ///   <c>out</c> parameter with the encoding detected.
    /// </param>
    /// <returns>
    ///   Content of the file.
    /// </returns>
    public static string ReadTextFile(string fileName, out Encoding encoding)
    {
      Debug.Assert(fileName != null);
      Debug.Assert(File.Exists(fileName));
      encoding = Encoding.Default;
      using (StreamReader sr = new StreamReader(File.OpenRead(fileName), encoding, true))
      {
        string content = sr.ReadToEnd();
        encoding = sr.CurrentEncoding;
        return content;
      }
    }

    /// <summary>
    ///   Reads text from a file.
    /// </summary>
    /// <param name="fileName">
    ///   Filename.
    /// </param>
    /// <returns>
    ///   Content of the file.
    /// </returns>
    public static string ReadTextFile(string fileName)
    {
      Debug.Assert(fileName != null);
      Debug.Assert(File.Exists(fileName));
      using (StreamReader sr = new StreamReader(File.OpenRead(fileName), true))
      {
        return sr.ReadToEnd();
      }
    }

    /// <summary>
    ///   Saves the text to a file.
    /// </summary>
    /// <param name="fileName">
    ///   Name of the file to save text to.
    /// </param>
    /// <param name="content">
    ///   Text to save.
    /// </param>
    /// <param name="encoding">
    ///   Encoding used to save the file.
    /// </param>
    public static void SaveTextFile(string fileName, string content, Encoding encoding)
    {
      Debug.Assert(fileName != null);
      FileAttributes fileAttribs = File.GetAttributes(fileName);
      if ((fileAttribs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
      {
        fileAttribs ^= FileAttributes.ReadOnly;
        File.SetAttributes(fileName, fileAttribs);
      }
      using (StreamWriter sw = new StreamWriter(fileName, false, encoding))
      {
        sw.Write(content);
      }
    }

    /// <summary>
    ///   Gets last write time for a file.
    /// </summary>
    /// <param name="fileName">
    ///   Filename for which operation is requested.
    /// </param>
    /// <returns>
    ///   <c>DateTime</c> structure.
    /// </returns>
    public static DateTime GetLastWriteTime(string fileName)
    {
      Debug.Assert(fileName != null);
      Debug.Assert(File.Exists(fileName));
      FileInfo fileInfo = new FileInfo(fileName);
      return fileInfo.LastWriteTime;
    }

    /// <summary>
    ///   Checks if name matches pattern with '?' and '*' wildcards.
    /// </summary>
    /// <param name="filename">
    ///   Name to match.
    /// </param>
    /// <param name="pattern">
    ///   Pattern to match to.
    /// </param>
    /// <returns>
    ///   <c>true</c> if name matches pattern, otherwise <c>false</c>.
    /// </returns>
    public static bool FilenameMatchesPattern(string filename, string pattern)
    {
      // prepare the pattern to the form appropriate for Regex class
      StringBuilder sb = new StringBuilder(pattern);
      // remove superflous occurences of  "?*" and "*?"
      while (sb.ToString().IndexOf("?*") != -1)
      {
        sb.Replace("?*", "*");
      }
      while (sb.ToString().IndexOf("*?") != -1)
      {
        sb.Replace("*?", "*");
      }
      // remove superflous occurences of asterisk '*'
      while (sb.ToString().IndexOf("**") != -1)
      {
        sb.Replace("**", "*");
      }
      // if only asterisk '*' is left, the mask is ".*"
      if (sb.ToString().Equals("*"))
        pattern = ".*";
      else {
        // replace '.' with "\."
        sb.Replace(".", "\\.");
        // replaces all occurrences of '*' with ".*" 
        sb.Replace("*", ".*");
        // replaces all occurrences of '?' with '.*' 
        sb.Replace("?", ".");
        // add "\b" to the beginning and end of the pattern
        sb.Insert(0, "\\b");
        sb.Append("\\b");
        pattern = sb.ToString();
      }
      Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
      return regex.IsMatch(filename);
    }

    /// <summary>
    ///   Checks if path provided corresponds to a directory.
    /// </summary>
    /// <param name="pathname">
    ///   Path to check.
    /// </param>
    /// <returns>
    ///   <c>true</c> if path is valid directory; else <c>false</c>.
    /// </returns>
    public static bool IsDirectory(string pathname)
    {
      Debug.Assert(pathname != null);
      FileInfo fileInfo = new FileInfo(pathname);
      return (fileInfo.Attributes & FileAttributes.Directory) != 0;
    }

    /// <summary>
    ///   Extracts a path part of a full path filename, searching for the
    ///   last directory separator or column. If none is found, the entire
    ///   string is returned. Else, the path up to (but not including) 
    ///   separator is returned.
    ///   TODO: check if this is the same as Path.GetDirectoryName method
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string ExtractDirectoryName(string fileName)
    {
      int directoryNameEnd = fileName.LastIndexOfAny(new char[] { ':', '\\', '/' });
      if (directoryNameEnd == -1)
        directoryNameEnd = fileName.Length;
      return fileName.Substring(0, directoryNameEnd);
    }

    /// <summary>
    ///   Compares two rooted paths for equality. Any of the paths provided
    ///   may end with path separator - it will be ignored.
    /// </summary>
    /// <param name="absolutePath1">
    ///   First path.
    /// </param>
    /// <param name="absolutePath2">
    ///   Second path.
    /// </param>
    /// <returns>
    ///   <c>true</c> if both paths correspond to the same folder.
    /// </returns>
    public static bool PathsAreEqual(string absolutePath1, string absolutePath2)
    {
      Debug.Assert(absolutePath1 != null && Path.IsPathRooted(absolutePath1));
      Debug.Assert(absolutePath2 != null && Path.IsPathRooted(absolutePath2));
      // remove any trailing separators
      char endingChar = absolutePath1[absolutePath1.Length - 1];
      if (endingChar == Path.DirectorySeparatorChar || endingChar == Path.AltDirectorySeparatorChar)
        absolutePath1 = Path.GetDirectoryName(absolutePath1);
      else
        absolutePath1 = Path.GetFullPath(absolutePath1);
      endingChar = absolutePath2[absolutePath2.Length - 1];
      if (endingChar == Path.DirectorySeparatorChar || endingChar == Path.AltDirectorySeparatorChar)
        absolutePath2 = Path.GetDirectoryName(absolutePath2);
      else
        absolutePath2 = Path.GetFullPath(absolutePath2);
      return string.Compare(absolutePath1, absolutePath2, true) == 0;
    }

    /// <summary>
    ///   Creates a relative path from absolute pathfilename and a 
    ///   reference path.
    /// </summary>
    /// <param name="pathfilename">
    ///   Absolute pathfilename for which relative pathfilename has to be
    ///   created.
    /// </param>
    /// <param name="referencePath">
    ///   Reference path.
    /// </param>
    /// <returns>
    ///   Returns relative pathfilename. If pathfilename and reference path
    ///   do not have common part (e.g. from different drive), absolute 
    ///   filepathname is returned.
    /// </returns>
    public static string GetRelativePathfilename(string pathfilename, string referencePath)
    {
      Debug.Assert(pathfilename != null && pathfilename.Length > 0);
      Debug.Assert(referencePath != null && referencePath.Length > 0);
      Debug.Assert(Path.IsPathRooted(pathfilename));
      Debug.Assert(Path.IsPathRooted(referencePath));
      // split both pathnames into directory names constituting path name
      string[] pathfilenameSections = pathfilename.Split(DirectorySeparators);
      string[] referencePathSections = referencePath.Split(DirectorySeparators);
      StringBuilder relativePath = new StringBuilder();
      int pathSectionCounter = 0;
      // skip leading backslashes for the case of UNC path name
      if (pathfilename.StartsWith("\\\\") && referencePath.StartsWith("\\\\"))
        pathSectionCounter = 2;
      int filenameSectionCounter = pathSectionCounter;
      bool matchingFound = false;
      // scan for common parts of paths
      while ((pathSectionCounter < referencePathSections.Length) && (referencePathSections[pathSectionCounter].Length > 0))
      {
        if (string.Compare(referencePathSections[pathSectionCounter], pathfilenameSections[pathSectionCounter], true) == 0)
          matchingFound = true;
        else {
          if (matchingFound)
          {
            while ((pathSectionCounter < referencePathSections.Length) && (referencePathSections[pathSectionCounter].Length > 0))
            {
              relativePath.Append("..");
              relativePath.Append(Path.DirectorySeparatorChar);
              pathSectionCounter++;
            }
          }
          break;
        }
        pathSectionCounter++;
        filenameSectionCounter++;
      }
      // no common starting sections - return absolute path
      if (!matchingFound)
        return pathfilename;
      // append filename path
      while (filenameSectionCounter < pathfilenameSections.Length)
      {
        relativePath.Append(pathfilenameSections[filenameSectionCounter]);
        filenameSectionCounter++;
        if (filenameSectionCounter < pathfilenameSections.Length)
          relativePath.Append(Path.DirectorySeparatorChar);
      }
      return relativePath.ToString();
    }

    /// <summary>
    ///   Combines two paths, removing root-folder combinations.
    /// </summary>
    /// <param name="rootPath">
    /// </param>
    /// <param name="relativePath">
    /// </param>
    /// <returns></returns>
    public static string CombinePaths(string rootPath, string relativePath)
    {
      Debug.Assert(Path.IsPathRooted(rootPath));
      Debug.Assert(!Path.IsPathRooted(relativePath));
      string root = Path.GetPathRoot(rootPath);
      // remove any trailing path separators
      if (Path.GetPathRoot(rootPath) != rootPath)
        rootPath = rootPath.Trim(DirectorySeparators);
      int lastSeparator = rootPath.Length;
      string[] relativePathComponents = relativePath.Split(DirectorySeparators);
      int i;
      for (i = 0; i < relativePathComponents.Length; i++)
      {
        if (relativePathComponents[i].Equals(".."))
        {
          lastSeparator = rootPath.LastIndexOfAny(DirectorySeparators, lastSeparator - 1);
          Debug.Assert(lastSeparator > 0);
          rootPath = rootPath.Substring(0, lastSeparator + 1);
        }
        else if (!relativePathComponents[i].Equals("."))
        {
          break;
        }
      }
      Debug.Assert(i < relativePathComponents.Length);
      for (int j = i; j < relativePathComponents.Length; j++)
      {
        rootPath = Path.Combine(rootPath, relativePathComponents[j]);
      }
      return rootPath;
    }

    /// <summary>
    ///   Creates foldername (in user's ApplicationData) where configuration 
    ///   is stored.
    /// </summary>
    /// <returns>
    ///   Path to the folder with configuration file.
    /// </returns>
    public static string GetConfigurationFolder()
    {
      string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      string appName = ((AssemblyProductAttribute)AssemblyProductAttribute.GetCustomAttribute(System.Reflection.Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute))).Product;
      return Path.Combine(appDataPath, appName);
    }

    /// <summary>
    ///   Directory separator characters.
    /// </summary>
    public static readonly char[] DirectorySeparators = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

  }
}