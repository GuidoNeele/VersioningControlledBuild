/*
 * Filename:    ListPrinter.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Prints the projects list.
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
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace BuildAutoIncrement {
	/// <summary>
	///   Provides formatted output to a printer.
	/// </summary>
    public class ListPrinter : ListExporter {

        /// <summary>
        ///   Initializes <c>ListPrinter</c> object using settings from 
        ///   <c>ExportConfiguration</c>.
        /// </summary>
		public ListPrinter(IWin32Window owner) {
            m_owner = owner;
            m_printDocument = new PrintDocument();
            PrintOptions po = ConfigurationPersister.Instance.Configuration.ExportConfiguration.PrintOptions;
            m_printFont     = po.ItemFont;
            m_headingFont   = po.HeadingFont;
            m_headerFont    = po.HeaderFont;
            m_drawIcons     = po.PrintProjectIcons;
        }

        /// <summary>
        ///   Sets the font used to print items.
        /// </summary>
        public Font Font {
            set { 
                m_printFont = value; 
            }
        }

        /// <summary>
        ///   Sets the font used to print page header.
        /// </summary>
        public Font HeaderFont {
            set { 
                m_headerFont = value;
            }
        }

        /// <summary>
        ///   Sets the font used to print list heading.
        /// </summary>
        public Font HeadingFont {
            set { 
                m_headingFont = value;
            }
        }

        /// <summary>
        ///   Sets the flag if icons are drawn in front of project name.
        /// </summary>
        public bool DrawIcons {
            set {
                m_drawIcons = value;
            }
        }

        /// <summary>
        ///   Starts the printing on default printer.
        /// </summary>
        /// <param name="solutionName">
        ///   Name of the solution, printed in the header
        /// </param>
        /// <param name="projectInfoList">
        ///   List of items to print.
        /// </param>
        public void Print(string solutionName, ProjectInfoList projectInfoList) {
            Debug.Assert(m_printDocument.PrinterSettings.IsValid);
            m_solutionName = solutionName;
            m_projectInfoList = projectInfoList;
            m_exportDateTime = DateTime.Now;
            m_indent = m_printFont.Height * m_indentBy;
            CollectColumnWidths();
            DoPrint();
        }

        /// <summary>
        ///   Starts the printing on selected printer.
        /// </summary>
        /// <param name="printerName">
        ///   Name of the printer to print on.
        /// </param>
        /// <param name="solutionName">
        ///   Name of the solution, printed in the header
        /// </param>
        /// <param name="projectInfoList">
        ///   List of items to print.
        /// </param>
        public void Print(string printerName, string solutionName, ProjectInfoList projectInfoList) {
            m_printDocument.PrinterSettings.PrinterName = printerName;
            if (printerName == string.Empty || !m_printDocument.PrinterSettings.IsValid) {
                PrintDialog pd = new PrintDialog();
                pd.Document = m_printDocument;
                if (pd.ShowDialog(m_owner) == DialogResult.OK) {
                    // to update main form immediately
                    Win32Api.UpdateWindow(m_owner.Handle);
                    m_printDocument.PrinterSettings.PrinterName = pd.PrinterSettings.PrinterName;
                    Debug.Assert(m_printDocument.PrinterSettings.IsValid);
                }
                else
                    return;
            }
            Print(solutionName, projectInfoList);
        }

        /// <summary>
        ///   Passes through the entire list and evaluates required column widths.
        /// </summary>
        private void CollectColumnWidths() {
            m_columnWidths = new float[Enum.GetValues(typeof(ColumnName)).Length];
            using (Graphics gr = m_printDocument.PrinterSettings.CreateMeasurementGraphics()) {
                m_columnWidths[(int)ColumnName.ProjectName] = gr.MeasureString(HeaderProjectName, m_headingFont).Width;
                // find largest version column header
                m_columnWidths[(int)ColumnName.Version] = 0;
                foreach (AssemblyVersionTypeSelection avts in m_assemblyVersionTypes) {
                    if (avts.IsSelected) {
                        float width = gr.MeasureString((string)m_headings[avts.AssemblyVersionType], m_headingFont).Width;
                        if (width > m_columnWidths[(int)ColumnName.Version])
                            m_columnWidths[(int)ColumnName.Version] = width;
                    }
                }
                foreach (ProjectInfo pi in m_projectInfoList) {
                    if (pi.IsVersionable || !m_dontExportNonversionable) {
                        float width = gr.MeasureString(pi.ProjectName, m_printFont).Width + pi.Level * m_indent;
                        if (width > m_columnWidths[(int)ColumnName.ProjectName])
                            m_columnWidths[(int)ColumnName.ProjectName] = width;
                        // go through all selected version types
                        foreach (AssemblyVersionTypeSelection avts in m_assemblyVersionTypes) {
                            if (avts.IsSelected) {
                                width = gr.MeasureString(pi.CurrentAssemblyVersions[avts.AssemblyVersionType].ToString(), m_printFont).Width;
                                if (width > m_columnWidths[(int)ColumnName.Version])
                                    m_columnWidths[(int)ColumnName.Version] = width;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   Initializes printing process.
        /// </summary>
        private void DoPrint() {
            Debug.Assert(m_projectInfoList != null && m_projectInfoList.ProjectInfos != null);
            m_pageNumber = 1;
            try {
                m_printDocument.PrintPage += new PrintPageEventHandler(PrintPage);
                // replace PrintControllerWithStatusDialog (e.g. for Acrobat Destiller) which
                // may cause problems when cancel button pressed
                m_printDocument.PrintController = new StandardPrintController();
                m_projectInfoListEnumerator = m_projectInfoList.ProjectInfos.GetEnumerator();
                m_printDocument.Print();
            }
            catch (Exception exception) {
                Trace.WriteLine(exception.Message);
            }
            finally {
                m_printDocument.PrintPage -= new PrintPageEventHandler(PrintPage);
            }
        }

        /// <summary>
        ///   PrintPage event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        private void PrintPage(object sender, PrintPageEventArgs ev) {
            // to update main form if a dialog was open (e.g. file dialog when printing to file)
            Win32Api.UpdateWindow(m_owner.Handle);
            m_iconWidth = m_printFont.GetHeight(ev.Graphics);
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            float bottomLine = ev.MarginBounds.Bottom - m_printFont.GetHeight(ev.Graphics);
            float yPos = topMargin;

            PrintHeader(ev);
            float height = PrintHeading(ev.Graphics, leftMargin, yPos);
            yPos += height * 1.25f;
            // print lines
            while (yPos < bottomLine && m_projectInfoListEnumerator.MoveNext()) {
                ProjectInfo pi = (ProjectInfo)m_projectInfoListEnumerator.Current;
                if (pi.IsVersionable || !m_dontExportNonversionable) {
                    // draw project name
                    float offset = leftMargin;
                    if (m_drawIcons) {
                        DrawProjectIcon(ev.Graphics, pi.ProjectTypeInfo.IconIndex, offset + pi.Level * m_indent, yPos);
                        offset += m_iconWidth;
                    }
                    ev.Graphics.DrawString(pi.ProjectName, m_printFont, Brushes.Black, offset + pi.Level * m_indent, yPos);
                    offset += m_columnWidths[(int)ColumnName.ProjectName] + 15;
                    // go through all selected version types
                    foreach (AssemblyVersionTypeSelection avts in m_assemblyVersionTypes) {
                        if (avts.IsSelected) {
                            string version = pi.CurrentAssemblyVersions[avts.AssemblyVersionType].ToString();
                            ev.Graphics.DrawString(version, m_printFont, Brushes.Black, offset, yPos);
                            offset += m_columnWidths[(int)ColumnName.Version] + 10;
                        }
                    }
                    yPos += m_printFont.GetHeight(ev.Graphics);
                }
            }
            if (m_projectInfoListEnumerator.MoveNext()) {
                ev.HasMorePages = true;
                m_pageNumber++;
            }
            else
                ev.HasMorePages = false;
        }

        /// <summary>
        ///   Draws project icon in front of project name.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="iconIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void DrawProjectIcon(Graphics graphics, int iconIndex, float x, float y) {
            iconIndex /= 2;
            Debug.Assert(iconIndex < m_bitmaps.Images.Count);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(m_bitmaps.Images[iconIndex], x, y + m_iconWidth * 0.1f, m_iconWidth * 0.8f, m_iconWidth * 0.8f);
        }

        /// <summary>
        ///   Prints table heading.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        private float PrintHeading(Graphics graphics, float left, float top) {
            float offset = left;
            if (m_drawIcons)
                offset += m_iconWidth;
            graphics.DrawString(HeaderProjectName, m_headingFont, Brushes.Black, offset, top);
            offset += m_columnWidths[(int)ColumnName.ProjectName] + 15;
            foreach (AssemblyVersionTypeSelection avts in m_assemblyVersionTypes) {
                if (avts.IsSelected) {
                    graphics.DrawString((string)m_headings[avts.AssemblyVersionType], m_headingFont, Brushes.Black, offset, top);
                    offset += m_columnWidths[(int)ColumnName.Version] + 10;
                }
            }
            return m_headingFont.GetHeight(graphics);
        }

        /// <summary>
        ///   Prints page header.
        /// </summary>
        /// <param name="ev"></param>
        private void PrintHeader(PrintPageEventArgs ev) {
            float top = ev.MarginBounds.Top - m_headerFont.GetHeight(ev.Graphics) * 2f;
            ev.Graphics.DrawString(m_solutionName, m_headerFont, Brushes.Black, ev.MarginBounds.Left, top);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            ev.Graphics.DrawString(m_exportDateTime.ToString("g"), m_headerFont, Brushes.Black, (ev.MarginBounds.Left + ev.MarginBounds.Right) / 2, top, sf);
            sf.Alignment = StringAlignment.Far;
            ev.Graphics.DrawString(m_pageNumber.ToString(), m_headerFont, Brushes.Black, ev.MarginBounds.Right, top, sf);
            top += m_headerFont.GetHeight(ev.Graphics);
            using (Pen pen = new Pen(Brushes.Black, 0.5f)) {
                ev.Graphics.DrawLine(pen, ev.MarginBounds.Left, top, ev.MarginBounds.Right, top);
            }
        }

        /// <summary>
        ///   Static constructor.
        /// </summary>
        static ListPrinter() {
            // load bitmap resources
            m_bitmaps = new ImageList();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            Debug.Assert(assembly != null);
            Bitmap bmp = new Bitmap(assembly.GetManifestResourceStream("BuildAutoIncrement.Resources.CSharpProject.bmp"));
            Debug.Assert(bmp != null);
            m_bitmaps.Images.Add(bmp);
            bmp = new Bitmap(assembly.GetManifestResourceStream("BuildAutoIncrement.Resources.eCSharpProject.bmp"));
            Debug.Assert(bmp != null);
            m_bitmaps.Images.Add(bmp);
            bmp = new Bitmap(assembly.GetManifestResourceStream("BuildAutoIncrement.Resources.VBProject.bmp"));
            Debug.Assert(bmp != null);
            m_bitmaps.Images.Add(bmp);
            bmp = new Bitmap(assembly.GetManifestResourceStream("BuildAutoIncrement.Resources.eVBProject.bmp"));
            Debug.Assert(bmp != null);
            m_bitmaps.Images.Add(bmp);
            bmp = new Bitmap(assembly.GetManifestResourceStream("BuildAutoIncrement.Resources.VJSharpProject.bmp"));
            Debug.Assert(bmp != null);
            m_bitmaps.Images.Add(bmp);
            bmp = new Bitmap(assembly.GetManifestResourceStream("BuildAutoIncrement.Resources.VCProject.bmp"));
            Debug.Assert(bmp != null);
            m_bitmaps.Images.Add(bmp);
            bmp = new Bitmap(assembly.GetManifestResourceStream("BuildAutoIncrement.Resources.SetupProject.bmp"));
            Debug.Assert(bmp != null);
            m_bitmaps.Images.Add(bmp);
            bmp = new Bitmap(assembly.GetManifestResourceStream("BuildAutoIncrement.Resources.EnterpriseTemplate.bmp"));
            Debug.Assert(bmp != null);
            m_bitmaps.Images.Add(bmp);
            bmp = new Bitmap(assembly.GetManifestResourceStream("BuildAutoIncrement.Resources.folder.bmp"));
            Debug.Assert(bmp != null);
            m_bitmaps.Images.Add(bmp);
            bmp = new Bitmap(assembly.GetManifestResourceStream("BuildAutoIncrement.Resources.webproject.bmp"));
            Debug.Assert(bmp != null);
            m_bitmaps.Images.Add(bmp);
            bmp = new Bitmap(assembly.GetManifestResourceStream("BuildAutoIncrement.Resources.DbProject.bmp"));
            Debug.Assert(bmp != null);
            m_bitmaps.Images.Add(bmp);
        }

        private float[] m_columnWidths;

        private bool m_drawIcons = true;

        private float m_iconWidth;

        private float m_indent;

        private Font m_printFont;

        private Font m_headingFont;

        private Font m_headerFont;

        private PrintDocument m_printDocument;

        System.Collections.IEnumerator m_projectInfoListEnumerator;

        private int m_pageNumber;

        IWin32Window m_owner;

        private static readonly ImageList m_bitmaps;

    }
}