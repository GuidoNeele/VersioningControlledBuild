/*
 * Filename:    VcbConfigurationSerializer.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Serializes and deserializes configuration. 
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
using System.Xml;
using System.Xml.Serialization;

namespace BuildAutoIncrement {

    /// <summary>
    ///   Class responsible to read and save tool configuration. 
    ///   Alhough generic <c>XmlSerializer</c> class can be used as well, this 
    ///   implementation avoids on-the-fly generation of serialization class by 
    ///   .NET Framework and thus makes serialization/deserialization quicker. 
    ///   Moreover, generic <c>XmlSerializer</c> class in some cases throws
    ///   exceptions for no obvious reason that cannot be caught.
    /// </summary>
    public class VcbConfigurationSerializer {

        #region Xml tags and attribute names

        private struct Tag {
            public const string VcbConfiguration                            = "VcbConfiguration";
            public const string MainFormSize                                = "MainFormSize";
            public const string Height                                      = "Height";
            public const string Width                                       = "Width";
            public const string ListViewColumnWidths                        = "ListViewColumnWidths";
            public const string ProjectName                                 = "ProjectName";
            public const string CurrentVersion                              = "CurrentVersion";
            public const string Modified                                    = "Modified";
            public const string ToBeVersion                                 = "ToBeVersion";
            public const string ApplyToAllTabsChecked                       = "ApplyToAllTabsChecked";
            public const string NumberingOptions                            = "NumberingOptions";
            public const string SaveModifiedFilesBeforeRunningAddinCommand  = "SaveModifiedFilesBeforeRunningAddinCommand";
            public const string DefaultVersionType                          = "DefaultVersionType";
            public const string IncrementBy                                 = "IncrementBy";
            public const string AllowArbitraryInformationalVersion          = "AllowArbitraryInformationalVersion";
            public const string IncludeVCppResourceFiles                    = "IncludeVCppResourceFiles";
            public const string IncludeSetupProjects                        = "IncludeSetupProjects";
            public const string GeneratePackageAndProductCodes              = "GeneratePackageAndProductCodes";
            public const string ApplyToAllTypes                             = "ApplyToAllTypes";
            public const string SynchronizeAllVersionTypes                  = "SynchronizeAllVersionTypes";
            public const string IncrementScheme                             = "IncrementScheme";
            public const string BatchCommandIncrementScheme                 = "BatchCommandIncrementScheme";
            public const string UseDateTimeBasedBuildAndRevisionNumbering   = "UseDateTimeBasedBuildAndRevisionNumbering";
            public const string ResetBuildOnMajorIncrement                  = "ResetBuildOnMajorIncrement";
            public const string ResetBuildOnMinorIncrement                  = "ResetBuildOnMinorIncrement";
            public const string ResetRevisionOnMajorIncrement               = "ResetRevisionOnMajorIncrement";
            public const string ResetRevisionOnMinorIncrement               = "ResetRevisionOnMinorIncrement";
            public const string ResetRevisionOnBuildIncrement               = "ResetRevisionOnBuildIncrement";
            public const string ResetBuildAndRevisionTo                     = "ResetBuildAndRevisionTo";
            public const string ReplaceAsteriskWithVersionComponents        = "ReplaceAsteriskWithVersionComponents";
            public const string DisplayOptions                              = "DisplayOptions";
            public const string IndentSubProjectItems                       = "IndentSubProjectItems";
            public const string SubProjectIndentation                       = "SubProjectIndentation";
            public const string ShowSubProjectRoot                          = "ShowSubProjectRoot";
            public const string ShowEnterpriseTemplateProjectRoot           = "ShowEnterpriseTemplateProjectRoot";
            public const string ShowEmptyFolders                            = "ShowEmptyFolders";
            public const string ShowNonVersionableProjects                  = "ShowNonVersionableProjects";
            public const string ShowSuccessDialog                           = "ShowSuccessDialog";
            public const string Colors                                      = "Colors";
            public const string NotModifiedProjectMarked                    = "NotModifiedProjectMarked";
            public const string NotModifiedProjectNotMarked                 = "NotModifiedProjectNotMarked";
            public const string ModifiedProjectMarked                       = "ModifiedProjectMarked";
            public const string ModifiedProjectNotMarked                    = "ModifiedProjectNotMarked";
            public const string ProjectWithInvalidVersionMarked             = "ProjectWithInvalidVersionMarked";
            public const string ProjectWithInvalidVersionNotMarked          = "ProjectWithInvalidVersionNotMarked";
            public const string ProjectWithoutVersion                       = "ProjectWithoutVersion";
            public const string ReportUpdatedVersion                        = "ReportUpdatedVersion";
            public const string ReportVersionNotChanged                     = "ReportVersionNotChanged";
            public const string ReportVersionUpdateFailed                   = "ReportVersionUpdateFailed";
            public const string SubProjectRoot                              = "SubProjectRoot";
            public const string FoldersConfigurations                       = "FoldersConfigurations";
            public const string SourceSafe                                  = "SourceSafe";
            public const string IIS                                         = "IIS";
            public const string ExportConfiguration                         = "ExportConfiguration";
            public const string AssemblyVersionTypes                        = "AssemblyVersionTypes";
            public const string AssemblyVersionType                         = "AssemblyVersionType";
            public const string IndentSubItems                              = "IndentSubItems";
            public const string IndentSubItemsBy                            = "IndentSubItemsBy";
            public const string ExcludeNonversionableItems                  = "ExcludeNonversionableItems";
            public const string ExportFileFormat                            = "ExportFileFormat";
            public const string CsvSeparator                                = "CSVSeparator";
            public const string PrintOptions                                = "PrintOptions";
            public const string PrintProjectIcons                           = "PrintProjectIcons";
            public const string ItemFont                                    = "ItemFont";
            public const string HeadingFont                                 = "HeadingFont";
            public const string HeaderFont                                  = "HeaderFont";
        }

        private struct Attr {
            public const string IsAvailable                                 = "IsAvailable";
            public const string IsSelected                                  = "IsSelected";
            public const string FontFamily                                  = "FontFamily";
            public const string FontStyle                                   = "FontStyle";
            public const string FontSize                                    = "Size";
        }

        #endregion // Xml tags and attribute names

        #region Writer class
        /// <summary>
        ///   Writer class.
        /// </summary>
        private class VcbConfigurationXmlWriter : IDisposable {

            #region Constructor, finalize and dispose related methods

            public VcbConfigurationXmlWriter(string filename) {
                m_writer = new XmlTextWriter(filename, null);
                m_writer.Namespaces = false;
                m_writer.Formatting = System.Xml.Formatting.Indented;
            }

            ~VcbConfigurationXmlWriter() {
                Dispose(false);
            }

            public void Dispose() {
                GC.SuppressFinalize(this);
                Dispose(true);
            }

            protected virtual void Dispose(bool disposing) {
                if (!m_disposed) {
                    m_writer.Close();
                    m_disposed = true;
                }
            }

            #endregion // Constructor, finalize and dispose related methods

            #region Public method

            /// <summary>
            ///   Writes the configuration.
            /// </summary>
            /// <param name="configuration">
            ///   Configuration to write.
            /// </param>
            public void WriteConfiguration(VcbConfiguration configuration) {
                Debug.Assert(configuration != null);
                m_writer.WriteStartDocument();
                WriteVcbConfiguration(configuration);
                m_writer.Flush();
            }

            #endregion // Public method

            #region Private methods

            private void WriteVcbConfiguration(VcbConfiguration configuration) {
                Debug.Assert(configuration != null);
                m_writer.WriteStartElement(Tag.VcbConfiguration);
                    WriteFormSizeSize(configuration.MainFormSize);
                    WriteListViewColumnWidths(configuration.ListViewColumnWidths);
                    m_writer.WriteElementString(Tag.ApplyToAllTabsChecked, configuration.ApplyToAllTabsChecked.ToString().ToLower());
                    WriteNumberingOptions(configuration.NumberingOptions);
                    WriteDisplayOptions(configuration.DisplayOptions);
                    WriteFoldersConfigurations(configuration.FoldersConfigurations);
                    WriteExportConfigurations(configuration.ExportConfiguration);
                m_writer.WriteEndElement();
            }

            void WriteFormSizeSize(System.Drawing.Size size) {
                m_writer.WriteStartElement(Tag.MainFormSize);
                    m_writer.WriteElementString(Tag.Width,  size.Width.ToString());
                    m_writer.WriteElementString(Tag.Height, size.Height.ToString());
                m_writer.WriteEndElement();
            }

            void WriteListViewColumnWidths(ListViewColumnWidths cw) {
                Debug.Assert(cw != null);
                m_writer.WriteStartElement(Tag.ListViewColumnWidths);
                    m_writer.WriteElementString(Tag.ProjectName,        cw.ProjectName.ToString());
                    m_writer.WriteElementString(Tag.CurrentVersion,     cw.CurrentVersion.ToString());
                    m_writer.WriteElementString(Tag.Modified,           cw.Modified.ToString());
                    m_writer.WriteElementString(Tag.ToBeVersion,        cw.ToBeVersion.ToString());
                m_writer.WriteEndElement();
            }

            void WriteNumberingOptions(NumberingOptions no) {
                Debug.Assert(no != null);
                m_writer.WriteStartElement(Tag.NumberingOptions);
                    m_writer.WriteElementString(Tag.SaveModifiedFilesBeforeRunningAddinCommand, no.SaveModifiedFilesBeforeRunningAddinCommand.ToString().ToLower());
                    m_writer.WriteElementString(Tag.DefaultVersionType,                         no.DefaultVersionType.ToString());
                    m_writer.WriteElementString(Tag.IncrementBy,                                no.IncrementBy.ToString());
                    m_writer.WriteElementString(Tag.AllowArbitraryInformationalVersion,         no.AllowArbitraryInformationalVersion.ToString().ToLower());
                    m_writer.WriteElementString(Tag.IncludeVCppResourceFiles,                   no.IncludeVCppResourceFiles.ToString().ToLower());
                    m_writer.WriteElementString(Tag.IncludeSetupProjects,                       no.IncludeSetupProjects.ToString().ToLower());
                    m_writer.WriteElementString(Tag.GeneratePackageAndProductCodes,             no.GeneratePackageAndProductCodes.ToString().ToLower());
                    m_writer.WriteElementString(Tag.ApplyToAllTypes,                            no.ApplyToAllTypes.ToString().ToLower());
                    m_writer.WriteElementString(Tag.SynchronizeAllVersionTypes,                 no.SynchronizeAllVersionTypes.ToString().ToLower());
                    m_writer.WriteElementString(Tag.IncrementScheme,                            no.IncrementScheme.ToString());
                    m_writer.WriteElementString(Tag.BatchCommandIncrementScheme,                no.BatchCommandIncrementScheme.ToString());
                    m_writer.WriteElementString(Tag.UseDateTimeBasedBuildAndRevisionNumbering,  no.UseDateTimeBasedBuildAndRevisionNumbering.ToString().ToLower());
                    m_writer.WriteElementString(Tag.ResetBuildOnMajorIncrement,                 no.ResetBuildOnMajorIncrement.ToString().ToLower());
                    m_writer.WriteElementString(Tag.ResetBuildOnMinorIncrement,                 no.ResetBuildOnMinorIncrement.ToString().ToLower());
                    m_writer.WriteElementString(Tag.ResetRevisionOnMajorIncrement,              no.ResetRevisionOnMajorIncrement.ToString().ToLower());
                    m_writer.WriteElementString(Tag.ResetRevisionOnMinorIncrement,              no.ResetRevisionOnMinorIncrement.ToString().ToLower());
                    m_writer.WriteElementString(Tag.ResetRevisionOnBuildIncrement,              no.ResetRevisionOnBuildIncrement.ToString().ToLower());
                    m_writer.WriteElementString(Tag.ResetBuildAndRevisionTo,                    no.ResetBuildAndRevisionTo.ToString());
                    m_writer.WriteElementString(Tag.ReplaceAsteriskWithVersionComponents,       no.ReplaceAsteriskWithVersionComponents.ToString().ToLower());
                m_writer.WriteEndElement();
            }

            void WriteDisplayOptions(DisplayOptions o) {
                Debug.Assert(o != null);
                m_writer.WriteStartElement(Tag.DisplayOptions);
                    WriteProjectsListViewColorsConfiguration(o.Colors);
                    m_writer.WriteElementString(Tag.IndentSubProjectItems,              o.IndentSubProjectItems.ToString().ToLower());
                    m_writer.WriteElementString(Tag.SubProjectIndentation,              o.SubProjectIndentation.ToString());
                    m_writer.WriteElementString(Tag.ShowSubProjectRoot,                 o.ShowSubProjectRoot.ToString().ToLower());
                    m_writer.WriteElementString(Tag.ShowEnterpriseTemplateProjectRoot,  o.ShowEnterpriseTemplateProjectRoot.ToString().ToLower());
                    m_writer.WriteElementString(Tag.ShowEmptyFolders,                   o.ShowEmptyFolders.ToString().ToLower());
                    m_writer.WriteElementString(Tag.ShowNonVersionableProjects,         o.ShowNonVersionableProjects.ToString().ToLower());
                    m_writer.WriteElementString(Tag.ShowSuccessDialog,                  o.ShowSuccessDialog.ToString().ToLower());
                m_writer.WriteEndElement();
            }

            void WriteProjectsListViewColorsConfiguration(ProjectsListViewColorsConfiguration o) {
                Debug.Assert((object)o != null);
                m_writer.WriteStartElement(Tag.Colors);
                    m_writer.WriteElementString(Tag.NotModifiedProjectMarked,           o.NotModifiedMarkedAsString);
                    m_writer.WriteElementString(Tag.NotModifiedProjectNotMarked,        o.NotModifiedNotMarkedAsString);
                    m_writer.WriteElementString(Tag.ModifiedProjectMarked,              o.ModifiedMarkedAsString);
                    m_writer.WriteElementString(Tag.ModifiedProjectNotMarked,           o.ModifiedNotMarkedAsString);
                    m_writer.WriteElementString(Tag.ProjectWithInvalidVersionMarked,    o.MarkedWithInvalidVersionAsString);
                    m_writer.WriteElementString(Tag.ProjectWithInvalidVersionNotMarked, o.NotMarkedWithInvalidVersionAsString);
                    m_writer.WriteElementString(Tag.ProjectWithoutVersion,              o.WithoutVersionAsString);
                    m_writer.WriteElementString(Tag.ReportUpdatedVersion,               o.ReportUpdatedVersionAsString);
                    m_writer.WriteElementString(Tag.ReportVersionNotChanged,            o.ReportVersionNotChangedAsString);
                    m_writer.WriteElementString(Tag.ReportVersionUpdateFailed,          o.ReportVersionUpdateFailedAsString);
                    m_writer.WriteElementString(Tag.SubProjectRoot,                     o.SubProjectRootAsString);
                m_writer.WriteEndElement();
            }

            void WriteFoldersConfigurations(FoldersConfigurations o) {
                Debug.Assert(o != null);
                m_writer.WriteStartElement(Tag.FoldersConfigurations);
                    WriteFolderConfiguration(Tag.SourceSafe, o.SourceSafeFolder);
                    WriteFolderConfiguration(Tag.IIS,        o.IisFolder);
                m_writer.WriteEndElement();
            }

            void WriteFolderConfiguration(string name, FolderConfiguration o) {
                Debug.Assert(name != null && name.Length > 0);
                Debug.Assert(o != null);
                m_writer.WriteStartElement(name);
                    m_writer.WriteAttributeString(Attr.IsAvailable, o.IsAvailable.ToString().ToLower()); 
                    m_writer.WriteString(o.Folder);
                m_writer.WriteEndElement();
            }

            void WriteExportConfigurations(ExportConfiguration o) {
                Debug.Assert(o != null);
                m_writer.WriteStartElement(Tag.ExportConfiguration);
                    WriteAssemblyVersionTypes(o.AssemblyVersionTypes);
                    m_writer.WriteElementString(Tag.IndentSubItems, o.IndentSubItems.ToString().ToLower());
                    m_writer.WriteElementString(Tag.IndentSubItemsBy, o.IndentSubItemsBy.ToString());
                    m_writer.WriteElementString(Tag.ExcludeNonversionableItems, o.ExcludeNonversionableItems.ToString().ToLower());
                    m_writer.WriteElementString(Tag.ExportFileFormat, o.ExportFileFormat.ToString());
                    m_writer.WriteElementString(Tag.CsvSeparator, o.CsvSeparator);
                    WritePrintOptions(o.PrintOptions);
                m_writer.WriteEndElement();
            }

            void WriteAssemblyVersionTypes(AssemblyVersionTypeSelection[] o) {
                Debug.Assert(o != null && o.Length == 3);
                m_writer.WriteStartElement(Tag.AssemblyVersionTypes);
                for (int i = 0; i < 3; i++) {
                    m_writer.WriteStartElement(Tag.AssemblyVersionType);
                        m_writer.WriteAttributeString(Attr.IsSelected, o[i].IsSelected.ToString().ToLower());
                        m_writer.WriteString(o[i].AssemblyVersionType.ToString());
                    m_writer.WriteEndElement();
                }
                m_writer.WriteEndElement();
            }

            void WritePrintOptions(PrintOptions o) {
                Debug.Assert(o != null);
                m_writer.WriteStartElement(Tag.PrintOptions);
                    m_writer.WriteElementString(Tag.PrintProjectIcons, o.PrintProjectIcons.ToString().ToLower());
                    WriteFontDescription(Tag.ItemFont, o.ItemFont);
                    WriteFontDescription(Tag.HeadingFont, o.HeadingFont);
                    WriteFontDescription(Tag.HeaderFont, o.HeaderFont);
                m_writer.WriteEndElement();
            }

            void WriteFontDescription(string elementName, FontDescription o) {
                m_writer.WriteStartElement(elementName);
                    m_writer.WriteAttributeString(Attr.FontStyle, o.FontStyle.ToString());
                    m_writer.WriteAttributeString(Attr.FontSize, o.Size.ToString());
                    m_writer.WriteString(o.FontFamily);
                m_writer.WriteEndElement();
            }

            #endregion // Private methods

            #region Private fields

            private XmlTextWriter m_writer = null;
            private bool m_disposed = false;

            #endregion // Private fields
        }

        #endregion // Writer class

        #region Reader class
        /// <summary>
        ///   Reader class.
        /// </summary>
        private class VcbConfigurationXmlReader : IDisposable {

            #region Constructor, finalize and dispose related methods

            public VcbConfigurationXmlReader(string filename) {
                m_reader = new System.Xml.XmlTextReader(filename);
                //InitIDs();
            }

            ~VcbConfigurationXmlReader() {
                Dispose(false);
            }

            public void Dispose() {
                GC.SuppressFinalize(this);
                Dispose(true);
            }

            protected virtual void Dispose(bool disposing) {
                if (!m_disposed) {
                    m_reader.Close();
                    m_disposed = true;
                }
            }

            #endregion // Constructor, finalize and dispose related methods

            #region Private fields

            private XmlTextReader m_reader = null;
            private bool m_disposed = false;

            #endregion // Private fields

            #region Public method

            public VcbConfiguration ReadConfiguration() {
                m_reader.Read();
                m_reader.MoveToContent();
                if (m_reader.NodeType == System.Xml.XmlNodeType.Element) {
                    if (m_reader.LocalName == Tag.VcbConfiguration) {
                        return ReadVcbConfiguration();
                    }
                    else {
                        throw InvalidElementName();
                    }
                }
                else {
                    throw InvalidNodeType();
                }
            }

            #endregion // Public method

            #region Private method

            XmlException InvalidElementName() {
                return new XmlException(string.Format("Invalid element name {0} at line: {1} position: {2}", m_reader.LocalName, m_reader.LineNumber, m_reader.LinePosition));
            }

            XmlException InvalidNodeType() {
                return new XmlException(string.Format("{0} node {1} not valid at line: {2} position: {3}", m_reader.NodeType, m_reader.LocalName, m_reader.LineNumber, m_reader.LinePosition));
            }

            VcbConfiguration ReadVcbConfiguration() {
                VcbConfiguration o = new VcbConfiguration();
                m_reader.MoveToElement();
                if (m_reader.IsEmptyElement) {
                    m_reader.Skip();
                    return o;
                }
                bool[] paramsRead = new bool[7];
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != XmlNodeType.EndElement) {
                    if (m_reader.NodeType == XmlNodeType.Element) {
                        if (!paramsRead[0] && (m_reader.LocalName == Tag.MainFormSize)) {
                            o.MainFormSize = ReadSize();
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && (m_reader.LocalName == Tag.ListViewColumnWidths)) {
                            o.ListViewColumnWidths = ReadListViewColumnWidths();
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && (m_reader.LocalName == Tag.ApplyToAllTabsChecked)) {
                            o.ApplyToAllTabsChecked = bool.Parse(m_reader.ReadElementString());
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && (m_reader.LocalName == Tag.NumberingOptions)) {
                            o.NumberingOptions = ReadNumberingOptions();
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && (m_reader.LocalName == Tag.DisplayOptions)) {
                            o.DisplayOptions = ReadDisplayOptions();
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && (m_reader.LocalName == Tag.FoldersConfigurations)) {
                            o.FoldersConfigurations = ReadFoldersConfigurations();
                            paramsRead[5] = true;
                        }
                        else if (!paramsRead[6] && (m_reader.LocalName == Tag.ExportConfiguration)) {
                            o.ExportConfiguration = ReadExportConfiguration();
                            paramsRead[6] = true;
                        }
                        else {
                            throw InvalidElementName();
                        }
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return o;
            }

            System.Drawing.Size ReadSize() {
                System.Drawing.Size size = new System.Drawing.Size(0, 0);
                m_reader.MoveToElement();
                if (m_reader.IsEmptyElement) {
                    m_reader.Skip();
                    return size;
                }
                bool[] paramsRead = new bool[2];
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != XmlNodeType.EndElement) {
                    if (m_reader.NodeType == XmlNodeType.Element) {
                        if (!paramsRead[0] && (m_reader.LocalName == Tag.Width)) {
                            size.Width = int.Parse(m_reader.ReadElementString());
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && (m_reader.LocalName == Tag.Height)) {
                            size.Height = int.Parse(m_reader.ReadElementString());
                            paramsRead[1] = true;
                        }
                        else {
                            throw InvalidElementName();
                        }
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return size;
            }

            ListViewColumnWidths ReadListViewColumnWidths() {
                ListViewColumnWidths lvcw = new ListViewColumnWidths();
                m_reader.MoveToElement();
                if (m_reader.IsEmptyElement) {
                    m_reader.Skip();
                    return lvcw;
                }
                bool[] paramsRead = new bool[4];
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != XmlNodeType.EndElement) {
                    if (m_reader.NodeType == XmlNodeType.Element) {
                        if (!paramsRead[0] && (m_reader.LocalName == Tag.ProjectName)) {
                            lvcw.ProjectName = int.Parse(m_reader.ReadElementString());
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && (m_reader.LocalName == Tag.CurrentVersion)) {
                            lvcw.CurrentVersion = int.Parse(m_reader.ReadElementString());
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && (m_reader.LocalName == Tag.Modified)) {
                            lvcw.Modified = int.Parse(m_reader.ReadElementString());
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && (m_reader.LocalName == Tag.ToBeVersion)) {
                            lvcw.ToBeVersion = int.Parse(m_reader.ReadElementString());
                            paramsRead[3] = true;
                        }
                        else {
                            throw InvalidElementName();
                        }
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return lvcw;
            }

            NumberingOptions ReadNumberingOptions() {
                NumberingOptions no = new NumberingOptions();
                m_reader.MoveToElement();
                if (m_reader.IsEmptyElement) {
                    m_reader.Skip();
                    return no;
                }
                bool[] paramsRead = new bool[19];
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != XmlNodeType.EndElement) {
                    if (m_reader.NodeType == XmlNodeType.Element) {
                        if (!paramsRead[0] && (m_reader.LocalName == Tag.SaveModifiedFilesBeforeRunningAddinCommand)) {
                            no.SaveModifiedFilesBeforeRunningAddinCommand = bool.Parse(m_reader.ReadElementString());
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && (m_reader.LocalName == Tag.DefaultVersionType)) {
                            no.DefaultVersionType = ReadAssemblyVersionType(m_reader.ReadElementString());
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && (m_reader.LocalName == Tag.IncrementBy)) {
                            no.IncrementBy = int.Parse(m_reader.ReadElementString());
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && (m_reader.LocalName == Tag.AllowArbitraryInformationalVersion)) {
                            no.AllowArbitraryInformationalVersion = bool.Parse(m_reader.ReadElementString());
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && (m_reader.LocalName == Tag.IncludeVCppResourceFiles)) {
                            no.IncludeVCppResourceFiles = bool.Parse(m_reader.ReadElementString());
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && (m_reader.LocalName == Tag.IncludeSetupProjects)) {
                            no.IncludeSetupProjects = bool.Parse(m_reader.ReadElementString());
                            paramsRead[5] = true;
                        }
                        else if (!paramsRead[6] && (m_reader.LocalName == Tag.GeneratePackageAndProductCodes)) {
                            no.GeneratePackageAndProductCodes = bool.Parse(m_reader.ReadElementString());
                            paramsRead[6] = true;
                        }
                        else if (!paramsRead[7] && (m_reader.LocalName == Tag.ApplyToAllTypes)) {
                            no.ApplyToAllTypes = bool.Parse(m_reader.ReadElementString());
                            paramsRead[7] = true;
                        }
                        else if (!paramsRead[8] && (m_reader.LocalName == Tag.SynchronizeAllVersionTypes)) {
                            no.SynchronizeAllVersionTypes = bool.Parse(m_reader.ReadElementString());
                            paramsRead[8] = true;
                        }
                        else if (!paramsRead[9] && (m_reader.LocalName == Tag.IncrementScheme)) {
                            no.IncrementScheme = ReadIncrementScheme(m_reader.ReadElementString());
                            paramsRead[9] = true;
                        }
                        else if (!paramsRead[10] && (m_reader.LocalName == Tag.BatchCommandIncrementScheme)) {
                            no.BatchCommandIncrementScheme = ReadBatchCommandIncrementScheme(m_reader.ReadElementString());
                            paramsRead[10] = true;
                        }
                        else if (!paramsRead[11] && (m_reader.LocalName == Tag.UseDateTimeBasedBuildAndRevisionNumbering)) {
                            no.UseDateTimeBasedBuildAndRevisionNumbering = bool.Parse(m_reader.ReadElementString());
                            paramsRead[11] = true;
                        }
                        else if (!paramsRead[12] && (m_reader.LocalName == Tag.ResetBuildOnMajorIncrement)) {
                            no.ResetBuildOnMajorIncrement = bool.Parse(m_reader.ReadElementString());
                            paramsRead[12] = true;
                        }
                        else if (!paramsRead[13] && (m_reader.LocalName == Tag.ResetBuildOnMinorIncrement)) {
                            no.ResetBuildOnMinorIncrement = bool.Parse(m_reader.ReadElementString());
                            paramsRead[13] = true;
                        }
                        else if (!paramsRead[14] && (m_reader.LocalName == Tag.ResetRevisionOnMajorIncrement)) {
                            no.ResetRevisionOnMajorIncrement = bool.Parse(m_reader.ReadElementString());
                            paramsRead[14] = true;
                        }
                        else if (!paramsRead[15] && (m_reader.LocalName == Tag.ResetRevisionOnMinorIncrement)) {
                            no.ResetRevisionOnMinorIncrement = bool.Parse(m_reader.ReadElementString());
                            paramsRead[15] = true;
                        }
                        else if (!paramsRead[16] && (m_reader.LocalName == Tag.ResetRevisionOnBuildIncrement)) {
                            no.ResetRevisionOnBuildIncrement = bool.Parse(m_reader.ReadElementString());
                            paramsRead[16] = true;
                        }
                        else if (!paramsRead[17] && (m_reader.LocalName == Tag.ResetBuildAndRevisionTo)) {
                            no.ResetBuildAndRevisionTo = ReadResetBuildAndRevision(m_reader.ReadElementString());
                            paramsRead[17] = true;
                        }
                        else if (!paramsRead[18] && (m_reader.LocalName == Tag.ReplaceAsteriskWithVersionComponents)) {
                            no.ReplaceAsteriskWithVersionComponents = bool.Parse(m_reader.ReadElementString());
                            paramsRead[18] = true;
                        }
                        else {
                            throw InvalidElementName();
                        }
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return no;
            }

            AssemblyVersionType ReadAssemblyVersionType(string s) {
                return (AssemblyVersionType)Enum.Parse(typeof(AssemblyVersionType), s, true);
            }

            IncrementScheme ReadIncrementScheme(string s) {
                return (IncrementScheme)Enum.Parse(typeof(IncrementScheme), s, true);
            }

            BatchCommandIncrementScheme ReadBatchCommandIncrementScheme(string s) {
                return (BatchCommandIncrementScheme)Enum.Parse(typeof(BatchCommandIncrementScheme), s, true);
            }

            BuildAutoIncrement.ResetBuildAndRevision ReadResetBuildAndRevision(string s) {
                return (BuildAutoIncrement.ResetBuildAndRevision)Enum.Parse(typeof(BuildAutoIncrement.ResetBuildAndRevision), s, true);
            }

            DisplayOptions ReadDisplayOptions() {
                DisplayOptions o = new DisplayOptions();
                m_reader.MoveToElement();
                if (m_reader.IsEmptyElement) {
                    m_reader.Skip();
                    return o;
                }
                bool[] paramsRead = new bool[8];
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != XmlNodeType.EndElement) {
                    if (m_reader.NodeType == XmlNodeType.Element) {
                        if (!paramsRead[0] && (m_reader.LocalName == Tag.Colors)) {
                            o.Colors = ReadProjectsListViewColorsConfiguration();
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && (m_reader.LocalName == Tag.IndentSubProjectItems)) {
                            o.IndentSubProjectItems = bool.Parse(m_reader.ReadElementString());
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && (m_reader.LocalName == Tag.SubProjectIndentation)) {
                            o.SubProjectIndentation = int.Parse(m_reader.ReadElementString());
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && (m_reader.LocalName == Tag.ShowSubProjectRoot)) {
                            o.ShowSubProjectRoot = bool.Parse(m_reader.ReadElementString());
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && (m_reader.LocalName == Tag.ShowEnterpriseTemplateProjectRoot)) {
                            o.ShowEnterpriseTemplateProjectRoot = bool.Parse(m_reader.ReadElementString());
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && (m_reader.LocalName == Tag.ShowEmptyFolders)) {
                            o.ShowEmptyFolders = bool.Parse(m_reader.ReadElementString());
                            paramsRead[5] = true;
                        }
                        else if (!paramsRead[6] && (m_reader.LocalName == Tag.ShowNonVersionableProjects)) {
                            o.ShowNonVersionableProjects = bool.Parse(m_reader.ReadElementString());
                            paramsRead[6] = true;
                        }
                        else if (!paramsRead[7] && (m_reader.LocalName == Tag.ShowSuccessDialog)) {
                            o.ShowSuccessDialog = bool.Parse(m_reader.ReadElementString());
                            paramsRead[7] = true;
                        }
                        else {
                            throw InvalidElementName();
                        }
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return o;
            }

            ProjectsListViewColorsConfiguration ReadProjectsListViewColorsConfiguration() {
                ProjectsListViewColorsConfiguration o = new ProjectsListViewColorsConfiguration();
                m_reader.MoveToElement();
                if (m_reader.IsEmptyElement) {
                    m_reader.Skip();
                    return o;
                }
                bool[] paramsRead = new bool[11];
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != XmlNodeType.EndElement) {
                    if (m_reader.NodeType == XmlNodeType.Element) {
                        if (!paramsRead[0] && (m_reader.LocalName == Tag.NotModifiedProjectMarked)) {
                            o.NotModifiedMarkedAsString = m_reader.ReadElementString();
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && (m_reader.LocalName == Tag.NotModifiedProjectNotMarked)) {
                            o.NotModifiedNotMarkedAsString = m_reader.ReadElementString();
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && (m_reader.LocalName == Tag.ModifiedProjectMarked)) {
                            o.ModifiedMarkedAsString = m_reader.ReadElementString();
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && (m_reader.LocalName == Tag.ModifiedProjectNotMarked)) {
                            o.ModifiedNotMarkedAsString = m_reader.ReadElementString();
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && (m_reader.LocalName == Tag.ProjectWithInvalidVersionMarked)) {
                            o.MarkedWithInvalidVersionAsString = m_reader.ReadElementString();
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && (m_reader.LocalName == Tag.ProjectWithInvalidVersionNotMarked)) {
                            o.NotMarkedWithInvalidVersionAsString = m_reader.ReadElementString();
                            paramsRead[5] = true;
                        }
                        else if (!paramsRead[6] && (m_reader.LocalName == Tag.ProjectWithoutVersion)) {
                            o.WithoutVersionAsString = m_reader.ReadElementString();
                            paramsRead[6] = true;
                        }
                        else if (!paramsRead[7] && (m_reader.LocalName == Tag.ReportUpdatedVersion)) {
                            o.ReportUpdatedVersionAsString = m_reader.ReadElementString();
                            paramsRead[7] = true;
                        }
                        else if (!paramsRead[8] && (m_reader.LocalName == Tag.ReportVersionNotChanged)) {
                            o.ReportVersionNotChangedAsString = m_reader.ReadElementString();
                            paramsRead[8] = true;
                        }
                        else if (!paramsRead[9] && (m_reader.LocalName == Tag.ReportVersionUpdateFailed)) {
                            o.ReportVersionUpdateFailedAsString = m_reader.ReadElementString();
                            paramsRead[9] = true;
                        }
                        else if (!paramsRead[10] && (m_reader.LocalName == Tag.SubProjectRoot)) {
                            o.SubProjectRootAsString = m_reader.ReadElementString();
                            paramsRead[10] = true;
                        }
                        else {
                            throw InvalidElementName();
                        }
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return o;
            }

            FoldersConfigurations ReadFoldersConfigurations() {
                BuildAutoIncrement.FoldersConfigurations o = new BuildAutoIncrement.FoldersConfigurations();
                m_reader.MoveToElement();
                if (m_reader.IsEmptyElement) {
                    m_reader.Skip();
                    return o;
                }
                bool[] paramsRead = new bool[2];
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != XmlNodeType.EndElement) {
                    if (m_reader.NodeType == XmlNodeType.Element) {
                        if (!paramsRead[0] && (m_reader.LocalName == Tag.SourceSafe)) {
                            o.SourceSafeFolder = ReadFolderConfiguration();
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && (m_reader.LocalName == Tag.IIS)) {
                            o.IisFolder = ReadFolderConfiguration();
                            paramsRead[1] = true;
                        }
                        else {
                            throw InvalidElementName();
                        }
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return o;
            }

            FolderConfiguration ReadFolderConfiguration() {
                FolderConfiguration o = new FolderConfiguration();
                bool[] paramsRead = new bool[1];
                while (m_reader.MoveToNextAttribute()) {
                    if (!paramsRead[0] && (m_reader.LocalName == Attr.IsAvailable)) {
                        o.IsAvailable = bool.Parse(m_reader.Value);
                        paramsRead[0] = true;
                    }
                    else
                        InvalidNodeType();
                }
                m_reader.MoveToElement();
                if (m_reader.IsEmptyElement) {
                    m_reader.Skip();
                    return o;
                }
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != XmlNodeType.EndElement) {
                    if (m_reader.NodeType == XmlNodeType.Text || 
                        m_reader.NodeType == XmlNodeType.CDATA || 
                        m_reader.NodeType == XmlNodeType.Whitespace || 
                        m_reader.NodeType == XmlNodeType.SignificantWhitespace) 
                    {
                        o.Folder = m_reader.ReadString();
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return o;
            }

            ExportConfiguration ReadExportConfiguration() {
                BuildAutoIncrement.ExportConfiguration o = new BuildAutoIncrement.ExportConfiguration();
                m_reader.MoveToElement();
                if (m_reader.IsEmptyElement) {
                    m_reader.Skip();
                    return o;
                }
                bool[] paramsRead = new bool[7];
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != XmlNodeType.EndElement) {
                    if (m_reader.NodeType == XmlNodeType.Element) {
                        if (!paramsRead[0] && m_reader.LocalName == Tag.AssemblyVersionTypes) {
                            AssemblyVersionTypeSelection[] avts = null;
                            if (!m_reader.IsEmptyElement) {
                                avts = ReadAssemblyVersionTypes();
                            }
                            else {
                                m_reader.Skip();
                            }
                            Debug.Assert(avts != null && avts.Length == 3);
                            o.AssemblyVersionTypes = avts;
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && (m_reader.LocalName == Tag.IndentSubItems)) {
                            o.IndentSubItems = bool.Parse(m_reader.ReadElementString());
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && (m_reader.LocalName == Tag.IndentSubItemsBy)) {
                            o.IndentSubItemsBy = int.Parse(m_reader.ReadElementString());
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && (m_reader.LocalName == Tag.ExcludeNonversionableItems)) {
                            o.ExcludeNonversionableItems = bool.Parse(m_reader.ReadElementString());
                            paramsRead[3] = true;
                        }
                        else if (!paramsRead[4] && (m_reader.LocalName == Tag.ExportFileFormat)) {
                            o.ExportFileFormat = (ExportFileFormat)Enum.Parse(typeof(ExportFileFormat), m_reader.ReadElementString());
                            paramsRead[4] = true;
                        }
                        else if (!paramsRead[5] && (m_reader.LocalName == Tag.CsvSeparator)) {
                            o.CsvSeparator = m_reader.ReadElementString();
                            paramsRead[5] = true;
                        }
                        else if (!paramsRead[6] && (m_reader.LocalName == Tag.PrintOptions)) {
                            o.PrintOptions = ReadPrintOptions();
                            paramsRead[6] = true;
                        }
                        else {
                            throw InvalidElementName();
                        }
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return o;
            }

            AssemblyVersionTypeSelection[] ReadAssemblyVersionTypes() {
                AssemblyVersionTypeSelection[] o = AssemblyVersionTypeSelection.DefaultSelection;
                int i = 0;
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != XmlNodeType.EndElement) {
                    if (m_reader.NodeType == XmlNodeType.Element && m_reader.LocalName == Tag.AssemblyVersionType && i < 3) {
                        o[i] = ReadAssemblyVersionTypeSelection();
                        i++;
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return o;
            }

            AssemblyVersionTypeSelection ReadAssemblyVersionTypeSelection() {
                AssemblyVersionTypeSelection o = new AssemblyVersionTypeSelection();
                bool[] paramsRead = new bool[1];
                while (m_reader.MoveToNextAttribute()) {
                    if (!paramsRead[0] && (m_reader.LocalName == Attr.IsSelected)) {
                        o.IsSelected = bool.Parse(m_reader.Value);
                        paramsRead[0] = true;
                    }
                    else {
                        throw InvalidNodeType();
                    }
                }
                m_reader.MoveToElement();
                if (m_reader.IsEmptyElement) {
                    m_reader.Skip();
                    return o;
                }
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != XmlNodeType.EndElement) {
                    if (m_reader.NodeType == XmlNodeType.Text || 
                        m_reader.NodeType == XmlNodeType.CDATA || 
                        m_reader.NodeType == XmlNodeType.Whitespace || 
                        m_reader.NodeType == XmlNodeType.SignificantWhitespace) {
                        o.AssemblyVersionType = ReadAssemblyVersionType(m_reader.ReadString());
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return o;
            }


            PrintOptions ReadPrintOptions() {
                PrintOptions o = new PrintOptions();
                m_reader.MoveToElement();
                if (m_reader.IsEmptyElement) {
                    m_reader.Skip();
                    return o;
                }
                bool[] paramsRead = new bool[4];
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != XmlNodeType.EndElement) {
                    if (m_reader.NodeType == XmlNodeType.Element) {
                        if (!paramsRead[0] && (m_reader.LocalName == Tag.PrintProjectIcons)) {
                            o.PrintProjectIcons = bool.Parse(m_reader.ReadElementString());
                            paramsRead[0] = true;
                        }
                        else if (!paramsRead[1] && (m_reader.LocalName == Tag.ItemFont)) {
                            o.ItemFont = ReadFontDescription();
                            paramsRead[1] = true;
                        }
                        else if (!paramsRead[2] && (m_reader.LocalName == Tag.HeadingFont)) {
                            o.HeadingFont = ReadFontDescription();
                            paramsRead[2] = true;
                        }
                        else if (!paramsRead[3] && (m_reader.LocalName == Tag.HeaderFont)) {
                            o.HeaderFont = ReadFontDescription();
                            paramsRead[3] = true;
                        }
                        else {
                            throw InvalidElementName();
                        }
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return o;
            }

            FontDescription ReadFontDescription() {
                FontDescription o = new FontDescription();
                bool[] paramsRead = new bool[2];
                while (m_reader.MoveToNextAttribute()) {
                    if (!paramsRead[0] && m_reader.LocalName == Attr.FontStyle) {
                        o.FontStyle = ReadFontStyle(m_reader.Value);
                        paramsRead[0] = true;
                    }
                    else if (!paramsRead[1] && m_reader.LocalName == Attr.FontSize) {
                        o.Size = float.Parse(m_reader.Value);
                        paramsRead[1] = true;
                    }
                    else {
                        throw InvalidNodeType();
                    }
                }
                m_reader.MoveToElement();
                if (m_reader.IsEmptyElement) {
                    m_reader.Skip();
                    Debug.Assert(false, "FontFamily not provided");
                    return o;
                }
                m_reader.ReadStartElement();
                m_reader.MoveToContent();
                while (m_reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                    if (m_reader.NodeType == System.Xml.XmlNodeType.Text || 
                        m_reader.NodeType == System.Xml.XmlNodeType.CDATA || 
                        m_reader.NodeType == System.Xml.XmlNodeType.Whitespace || 
                        m_reader.NodeType == System.Xml.XmlNodeType.SignificantWhitespace) {
                        o.FontFamily = m_reader.ReadString();
                    }
                    else {
                        throw InvalidNodeType();
                    }
                    m_reader.MoveToContent();
                }
                m_reader.ReadEndElement();
                return o;
            }

            System.Drawing.FontStyle ReadFontStyle(string s) {
                return (System.Drawing.FontStyle)Enum.Parse(typeof(System.Drawing.FontStyle), s);
            }

            #endregion // Private method

        }

        #endregion // Reader class

        #region Public methods

        /// <summary>
        ///   Serializes configuration to the file.
        /// </summary>
        /// <param name="filename">
        ///   Name of the file where to serialize.
        /// </param>
        /// <param name="configuration">
        ///   Configuration to serialize.
        /// </param>
        public void Serialize(string filename, VcbConfiguration configuration) {
            using (VcbConfigurationXmlWriter writer = new VcbConfigurationXmlWriter(filename)) {
                writer.WriteConfiguration(configuration);
            }
        }

        /// <summary>
        ///   Deserializes the configuration.
        /// </summary>
        /// <param name="filename">
        ///   Name of the file to deserialize from.
        /// </param>
        /// <returns>
        ///   Configuration.
        /// </returns>
        public VcbConfiguration Deserialize(string filename) {
            using (VcbConfigurationXmlReader reader = new VcbConfigurationXmlReader(filename)) {
                return reader.ReadConfiguration();
            }
        }

        #endregion // Public methods
    }
}