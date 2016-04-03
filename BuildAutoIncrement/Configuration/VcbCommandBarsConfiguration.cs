using System;
using System.Collections;
using System.Xml.Serialization;
using System.Diagnostics;

namespace BuildAutoIncrement {

    /// <summary>
    ///   Contains UI elements configurations (toolbar, menu) for
    ///   all develompment environments.
    /// </summary>
    [Serializable]
    public class VcbCommandBarsConfiguration {

        /// <summary>
        ///   Returns configuration for a given version of Visual Studio.
        /// </summary>
        /// <param name="version">
        ///   Version of Visual Studio for which configuration is requested.
        /// </param>
        /// <param name="addInProgID">
        ///   Addin progId (requred to create full command name).
        /// </param>
        /// <returns>
        ///   <code>VcbCommandBarsSettings</code> control configuration for 
        ///   the version of Visual Studio provided.
        /// </returns>
        public VcbCommandBarsSettings GetConfiguration(string version, string addInProgID) {
            foreach (VcbCommandBarsSettings devEnv in m_developmentEnvironmets) {
                if (devEnv.Version == version)
                    return devEnv;
            }
            VcbCommandBarsSettings newDevEnv = new VcbCommandBarsSettings(version, addInProgID);
            m_developmentEnvironmets.Add(newDevEnv);
            return newDevEnv;
        }

        /// <summary>
        ///   Checks if there is a configuration for version of Visual Studio provided.
        /// </summary>
        /// <param name="version">
        ///   Visual Studio version.
        /// </param>
        /// <returns>
        ///   <code>true</code> if configuration exists, otherwise <code>false</code>:
        /// </returns>
        public bool Contains(string version) {
            foreach (VcbCommandBarsSettings devEnv in m_developmentEnvironmets) {
                if (devEnv.Version == version)
                    return true;
            }
            return false;
        }

        /// <summary>
        ///   Gets or sets the array of settings for each version of Visual Studio.
        /// </summary>
        [XmlElement("VcbCommandBarsSettings")]
        public VcbCommandBarsSettings[] Items {
            get { return (VcbCommandBarsSettings[])m_developmentEnvironmets.ToArray(typeof(VcbCommandBarsSettings)); }
            set { m_developmentEnvironmets = new ArrayList(value); }
        }

        private ArrayList m_developmentEnvironmets = new ArrayList();
    }

    /// <summary>
    ///   VCB menu and toolbar settings for a specific version of Visual Studio.
    /// </summary>
    [Serializable]
    public class VcbCommandBarsSettings {

        public VcbCommandBarsSettings() {
        }

        public VcbCommandBarsSettings(string addInProgId) {
            m_toolbar = new ToolbarSettings(addInProgId);
            m_menuBar = new MenuSettings(addInProgId);
        }

        public VcbCommandBarsSettings(string version, string addInProgId)
            : this(addInProgId) {
            m_version = version;
        }

        /// <summary>
        ///   Gets or sets VCB toolbar settings.
        /// </summary>
        public ToolbarSettings Toolbar {
            get { return m_toolbar; }
            set { m_toolbar = value; }
        }

        /// <summary>
        ///   Gets or sets VCB menu settings.
        /// </summary>
        public MenuSettings Menu {
            get { return m_menuBar; }
            set { m_menuBar = value; }
        }

        /// <summary>
        ///   Gets or sets version of Visual Studio.
        /// </summary>
        [XmlAttribute]
        public string Version {
            get { return m_version; }
            set { m_version = value; }
        }

        private ToolbarSettings m_toolbar = null;

        private MenuSettings m_menuBar = null;

        private string m_version;
    }

    /// <summary>
    ///   Abstract class for menu and toolbar settings.
    /// </summary>
    public abstract class VcbBarSettings
    {
        /// <summary>
        ///   Updates current configuration with a new one provided.
        /// </summary>
        /// <param name="newSettings">
        ///   New settings to apply.
        /// </param>
        public void Update(ArrayList newSettings)
        {
            // remove duplicate controls
            foreach (VcbControl control in newSettings)
            {
                int index = m_vcbControls.IndexOf(control);
                if (index != -1)
                    m_vcbControls.RemoveAt(index);
            }
            // remaining controls are invisible
            foreach (VcbControl control in m_vcbControls)
                control.Visible = false;
            // insert updated visible controls at the beginning
            m_vcbControls.InsertRange(0, newSettings);
        }

        /// <summary>
        ///   Gets or sets the array of controls (menu items or toolbar buttons).
        /// </summary>
        [XmlArray("Controls")]
        [XmlArrayItem(ElementName = "Control")]
        public VcbControl[] VcbControls
        {
            get { return (VcbControl[])m_vcbControls.ToArray(typeof(VcbControl)); }
            set
            {
                if (value != null)
                    m_vcbControls = new ArrayList(value);
            }
        }

        /// <summary>
        ///   Enables or disables the control.
        /// </summary>
        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }

        /// <summary>
        ///   Shows or hides the control.
        /// </summary>
        public bool Visible
        {
            get { return m_visible; }
            set { m_visible = value; }
        }

        protected ArrayList m_vcbControls = new ArrayList();

        private bool m_enabled = true;

        private bool m_visible = true;

    }

    /// <summary>
    ///   VCB toolbar settings.
    /// </summary>
    [Serializable]
    public class ToolbarSettings : VcbBarSettings {

        public ToolbarSettings() {
        }

        public ToolbarSettings(string addInProgId) {
            m_vcbControls.Add(new VcbControl(string.Format("{0}.{1}", addInProgId, Constants.Commands.GUI), false));
            m_vcbControls.Add(new VcbControl(string.Format("{0}.{1}", addInProgId, Constants.Commands.Build), false));
            m_vcbControls.Add(new VcbControl(string.Format("{0}.{1}", addInProgId, Constants.Commands.Rebuild), false));
            m_vcbControls.Add(new VcbControl(string.Format("{0}.{1}", addInProgId, Constants.Commands.Save), false));
        }

        /// <summary>
        ///   Gets or sets the position of toolbar.
        /// </summary>
        public int Position {
            get { return m_position; }
            set { m_position = value; }
        }

        /// <summary>
        ///   Gets or sets <code>Left</code> property of the toolbar.
        /// </summary>
        public int Left {
            get { return m_left; }
            set { m_left = value; }
        }

        /// <summary>
        ///   Gets or sets <code>Top</code> property of the toolbar.
        /// </summary>
        public int Top
        {
            get { return m_top; }
            set { m_top = value; }
        }

        /// <summary>
        ///   Gets or sets row index of the toolbar.
        /// </summary>
        public int RowIndex
        {
            get { return m_rowIndex; }
            set { m_rowIndex = value; }
        }

        private int m_position = 1; // msoBarTop position by default

        private int m_rowIndex = -1;

        private int m_left = 0;

        private int m_top = 0;
    }

    /// <summary>
    ///   VCB menu settings.
    /// </summary>
    [Serializable]
    public class MenuSettings : VcbBarSettings {

        public MenuSettings() {
        }

        public MenuSettings(string addInProgId) {
            m_vcbControls.Add(new VcbControl(string.Format("{0}.{1}", addInProgId, Constants.Commands.GUI), false));
            m_vcbControls.Add(new VcbControl(string.Format("{0}.{1}", addInProgId, Constants.Commands.Build), false));
            m_vcbControls.Add(new VcbControl(string.Format("{0}.{1}", addInProgId, Constants.Commands.Rebuild), false));
            m_vcbControls.Add(new VcbControl(string.Format("{0}.{1}", addInProgId, Constants.Commands.Save), false));
            m_vcbControls.Add(new VcbControl(string.Format("{0}.{1}", addInProgId, Constants.Commands.PrintVersions), false));
            m_vcbControls.Add(new VcbControl(string.Format("{0}.{1}", addInProgId, Constants.Commands.ExportVersions), false));
            m_vcbControls.Add(new VcbControl(string.Format("{0}.{1}", addInProgId, Constants.Commands.Configure), true));
            m_vcbControls.Add(new VcbControl(string.Format("{0}.{1}", addInProgId, Constants.Commands.About), false));
        }

        /// <summary>
        ///   Gets or sets the index of the menu.
        /// </summary>
        public int Index {
            get { return m_index; }
            set { m_index = value; }
        }

        private int m_index = -1;
    }

    /// <summary>
    ///   Settings for a control (menu item or toolbar button).
    /// </summary>
    [Serializable]
    public class VcbControl {

        public VcbControl() {
        }

        public VcbControl(string name) : this(name, false) {
        }

        public VcbControl(string name, bool beginGroup) {
            m_name = name;
            m_beginGroup = beginGroup;
        }

        /// <summary>
        ///   Gets or sets the name of the control.
        /// </summary>
        public string Name {
            get { return m_name; }
            set { m_name = value; }
        }

        /// <summary>
        ///   Overriden to enable <code>Find</code> method in the array of controls.
        ///   Compares controls by their names.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>
        ///   <code>true</code> if controls have the same name, otherwise <code>false</code>.
        /// </returns>
        public override bool Equals(object obj) {
            VcbControl other = obj as VcbControl;
            if (other == null)
                return false;
            return Name == other.Name;
        }

        /// <summary>
        ///   Overriden since <code>Equals</code> have been overriden.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return Name.GetHashCode();
        }

        /// <summary>
        ///   Gets or sets a flag that indicates the control begins a group.
        /// </summary>
        [XmlAttribute]
        public bool BeginGroup {
            get { return m_beginGroup; }
            set { m_beginGroup = value; }
        }

        /// <summary>
        ///   Gets or sets the visibility of control.
        /// </summary>
        [XmlAttribute]
        public bool Visible {
            get { return m_isVisible; }
            set { m_isVisible = value; }
        }

        private string m_name;

        private bool m_beginGroup = false;

        private bool m_isVisible = true;
    }
}
