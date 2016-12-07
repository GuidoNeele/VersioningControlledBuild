/*
 * Filename:    SourceSafeCheckout.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Interface and base abstract class for SourceSafe check-out
 *              utility classes.
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
using System.Diagnostics;

namespace BuildAutoIncrement
{
  /// <summary>
  ///     Defines interface for classes responsible to check out version 
  ///     files.
  /// </summary>
  public interface ISourceSafeCheckout
  {

    /// <summary>
    ///   Checks out version files for an array of <c>ProjectInfo</c> 
    ///   objects provided.
    /// </summary>
    /// <param name="projectInfos">
    ///   An array of <c>ProjectInfo</c> objects to check out.
    /// </param>
    void CheckOut(ProjectInfo[] projectInfos);

    /// <summary>
    ///   Gets array of filenames checked out succesfully.
    /// </summary>
    string[] FilesCheckedOut { get; }

    /// <summary>
    ///   Gets array of <c>ProjectInfo</c> objects failed to check out.
    /// </summary>
    ProjectInfo[] ProjectInfosFailedToCheckOut { get; }

    /// <summary>
    ///   Gets array of filenames failed to check out.
    /// </summary>
    string[] FilesFailedToCheckOut { get; }

  }

  /// <summary>
  ///   Abstract base class for classes responsible to check out version 
  ///   files.
  /// </summary>
  public abstract class SourceSafeCheckout : ISourceSafeCheckout
  {

    /// <summary>
    ///   Creates <c>SourceSafeControl</c> object.
    /// </summary>
    protected SourceSafeCheckout()
    {
      m_filesCheckedOut = new ArrayList();
      m_projectInfosFailedToCheckOut = new ArrayList();
      m_filesFailedToCheckOut = new ArrayList();
    }

    #region ISourceSafeCheckout implementation

    public abstract void CheckOut(ProjectInfo[] projectInfos);

    /// <summary>
    ///   Gets an array of filenames checked out succesfully.
    /// </summary>
    public string[] FilesCheckedOut
    {
      get
      {
        Debug.Assert(m_filesCheckedOut != null);
        return (string[])m_filesCheckedOut.ToArray(typeof(string));
      }
    }

    /// <summary>
    ///   Gets an array of <c>ProjectInfo</c> objects that failed to check 
    ///   out.
    /// </summary>
    public ProjectInfo[] ProjectInfosFailedToCheckOut
    {
      get
      {
        Debug.Assert(m_projectInfosFailedToCheckOut != null);
        return (ProjectInfo[])m_projectInfosFailedToCheckOut.ToArray(typeof(ProjectInfo));
      }
    }

    /// <summary>
    ///   Gets an array of filenames that failed to check out.
    /// </summary>
    public string[] FilesFailedToCheckOut
    {
      get
      {
        Debug.Assert(m_filesFailedToCheckOut != null);
        return (string[])m_filesFailedToCheckOut.ToArray(typeof(string));
      }
    }

    #endregion // ISourceSafeCheckout implementation

    protected ArrayList m_filesCheckedOut;

    protected ArrayList m_projectInfosFailedToCheckOut;

    protected ArrayList m_filesFailedToCheckOut;
  }
}