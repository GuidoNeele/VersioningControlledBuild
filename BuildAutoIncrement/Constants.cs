/*
 * Filename:    Constants.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Common constants.
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

namespace BuildAutoIncrement
{
  /// <summary>
  ///   Structure with general constants used throughout the application.
  /// </summary>
  public struct Constants
  {

    public const string ProgramTitle = "Versioning Controlled Build";
    public const string CommandBarName = "Versioning Controlled Build";
    public const string MenuName = "&VCB";

    public struct Commands
    {
      public const string GUI = "GUI";
      public const string Build = "Build";
      public const string Rebuild = "Rebuild";
      public const string Save = "Save";
      public const string PrintVersions = "PrintVersions";
      public const string ExportVersions = "ExportVersions";
      public const string Configure = "Configure";
      public const string About = "About";
    }
  }
}