using System;
using System.Diagnostics;
using System.Xml.Serialization;

namespace BuildAutoIncrement {

    public enum ExportFileFormat {
        PlainText,
        CSV,
        XML
    }

    #region PrintOptions

    /// <summary>
    ///   Export options related to printing.
    /// </summary>
    public class PrintOptions {

        #region Constructors

        /// <summary>
        ///   Creates <c>PrintOptions</c> object.
        /// </summary>
        public PrintOptions() {
            m_printProjectIcons = true;
            m_itemFont = new FontDescription();
            m_itemFont.Size = 9.0f;
            m_headingFont = new FontDescription();
            m_headingFont.FontStyle = System.Drawing.FontStyle.Bold;
            m_headingFont.Size = 9.0f;
            m_headerFont = new FontDescription();
        }

        #endregion // Constructors

        #region Public properties

        /// <summary>
        ///   Gets or sets flag indicating if project icons are printed.
        /// </summary>
        public bool PrintProjectIcons {
            get {
                return m_printProjectIcons;
            }
            set {
                m_printProjectIcons = value;
            }
        }

        /// <summary>
        ///   Gets or sets <c>FontDescription</c> of the font used for items.
        /// </summary>
        public FontDescription ItemFont {
            get {
                return m_itemFont;
            }
            set {
                m_itemFont = value;
            }
        }

        /// <summary>
        ///   Gets or sets <c>FontDescription</c> of the font used for heading.
        /// </summary>
        public FontDescription HeadingFont {
            get {
                return m_headingFont;
            }
            set {
                m_headingFont = value;
            }
        }

        /// <summary>
        ///   Gets or sets <c>FontDescription</c> of the font used for header.
        /// </summary>
        public FontDescription HeaderFont {
            get {
                return m_headerFont;
            }
            set {
                m_headerFont = value;
            }
        }

        #endregion // Public properties

        #region Private fields

        private bool m_printProjectIcons;
        private FontDescription m_itemFont;
        private FontDescription m_headingFont;
        private FontDescription m_headerFont;

        #endregion // Private fields
    }

    #endregion // PrintOptions

    #region AssemblyVersionTypeSelection

    /// <summary>
    ///   Class with order and selection of assembly version types that are
    ///   exported.
    /// </summary>
    [Serializable]
    public class AssemblyVersionTypeSelection {

        #region Constructors

        /// <summary>
        ///   Creates empty <c>AssemblyVersionTypeSelection</c> object.
        /// </summary>
        public AssemblyVersionTypeSelection() {
            m_isSelected = true;
        }

        /// <summary>
        ///   Creates <c>AssemblyVersionTypeSelection</c> object with 
        ///   <c>AssemblyVersionType</c> provided.
        /// </summary>
        /// <param name="assemblyVersionType">
        ///   <c>AssemblyVersionType</c> to initialize with.
        /// </param>
        public AssemblyVersionTypeSelection(AssemblyVersionType assemblyVersionType) : this() {
            m_assemblyVersionType = assemblyVersionType;
        }

        #endregion // Constructors

        #region Public properties

        /// <summary>
        ///   Gets or sets <c>AssemblyVersionType</c> flag.
        /// </summary>
        [XmlText]
        public AssemblyVersionType AssemblyVersionType {
            get {
                return m_assemblyVersionType;
            }
            set {
                m_assemblyVersionType = value;
            }
        }

        /// <summary>
        ///   Gets or sets flag indicating if current type is selected.
        /// </summary>
        [XmlAttribute]
        public bool IsSelected {
            get {
                return m_isSelected;
            }
            set {
                m_isSelected = value;
            }
        }

        #endregion // Public properties

        private AssemblyVersionType m_assemblyVersionType;

        private bool m_isSelected;

        /// <summary>
        ///   Default array of <c>AssemblyVersionTypeSelection</c> with all types selected.
        /// </summary>
        public static AssemblyVersionTypeSelection[] DefaultSelection 
            = new AssemblyVersionTypeSelection[] { 
                                                    new AssemblyVersionTypeSelection(AssemblyVersionType.AssemblyVersion),
                                                    new AssemblyVersionTypeSelection(AssemblyVersionType.AssemblyFileVersion),
                                                    new AssemblyVersionTypeSelection(AssemblyVersionType.AssemblyInformationalVersion) 
                                                 };

    }

    #endregion // AssemblyVersionTypeSelection

    #region ExportConfiguration

	/// <summary>
	///   Configuration used for exporting.
	/// </summary>
	[Serializable]
	public class ExportConfiguration {
		
        #region Constructors

        public ExportConfiguration() {
            m_assemblyVersionTypes = AssemblyVersionTypeSelection.DefaultSelection;
            m_indentSubItems = true;
            m_indentSubItemsBy = 1;
            m_excludeNonversionableItems = false;
            m_printOptions = new PrintOptions();
            m_exportFileFormat = ExportFileFormat.PlainText;
            m_csvSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            Debug.Assert(m_assemblyVersionTypes != null && m_assemblyVersionTypes.Length == 3);
            Debug.Assert(m_printOptions != null);
		}

        #endregion // Constructors

        #region Public properties

        public AssemblyVersionTypeSelection[] AssemblyVersionTypes {
            get {
                return m_assemblyVersionTypes;
            }
            set {
                m_assemblyVersionTypes = value;
            }
        }

        public bool IndentSubItems {
            get {
                return m_indentSubItems;
            }
            set {
                m_indentSubItems = value;
            }
        }

        public int IndentSubItemsBy {
            get {
                return m_indentSubItemsBy;
            }
            set {
                m_indentSubItemsBy = value;
            }
        }

        public bool ExcludeNonversionableItems {
            get {
                return m_excludeNonversionableItems;
            }
            set {
                m_excludeNonversionableItems = value;
            }
        }

        public PrintOptions PrintOptions {
            get {
                return m_printOptions;
            }
            set {
                m_printOptions = value;
            }
        }

        public ExportFileFormat ExportFileFormat {
            get {
                return m_exportFileFormat;
            }
            set {
                m_exportFileFormat = value;
            }
        }

        public string CsvSeparator {
            get {
                return m_csvSeparator;
            }
            set {
                m_csvSeparator = value;
            }
        }

        #endregion // Public properties

        #region Private fields

        private AssemblyVersionTypeSelection[] m_assemblyVersionTypes;

        private bool m_indentSubItems;

        private int m_indentSubItemsBy;

        private bool m_excludeNonversionableItems;

        private PrintOptions m_printOptions;

        private ExportFileFormat m_exportFileFormat;

        private string m_csvSeparator;

        #endregion // Private fields
    }

    #endregion // ExportConfiguration

}
