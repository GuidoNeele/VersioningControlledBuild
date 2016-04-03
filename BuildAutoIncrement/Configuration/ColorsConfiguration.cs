/*
 * Filename:    ColorsConfiguration.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Configuration part used to persist listview colors. 
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
using System.Drawing;
using System.Xml.Serialization;

namespace BuildAutoIncrement {

	/// <summary>
	///   Configuration part used to persist listview colors.
	/// </summary>
	[Serializable]
	public class ProjectsListViewColorsConfiguration : ICloneable {
		public ProjectsListViewColorsConfiguration() {
            m_notModifiedMarked         = SystemColors.WindowText;
            m_notModifiedNotMarked      = SystemColors.GrayText;
            m_modifiedMarked            = Color.Green;
            m_modifiedNotMarked         = Color.LimeGreen;
            m_invalidVersionMarked      = Color.Red;
            m_invalidVersionNotMarked   = Color.LightCoral;
            m_noVersion                 = Color.Orange;
            m_updatedVersion            = Color.Blue;
            m_versionNotChanged         = SystemColors.GrayText;
            m_versionUpdateFailed       = Color.Red;
            m_subProjectRoot            = Color.SlateGray;
		}

        [XmlIgnore]
        public Color NotModifiedMarked {
            get { return m_notModifiedMarked; }
            set { m_notModifiedMarked = value; }
        }

        [XmlElement("NotModifiedProjectMarked")]
        public string NotModifiedMarkedAsString {
            get { return AsString(m_notModifiedMarked); }
            set { m_notModifiedMarked = FromString(value); }
        }

        [XmlIgnore]
        public Color NotModifiedNotMarked {
            get { return m_notModifiedNotMarked; }
            set { m_notModifiedNotMarked = value; }
        }

        [XmlElement("NotModifiedProjectNotMarked")]
        public string NotModifiedNotMarkedAsString {
            get { return AsString(m_notModifiedNotMarked); }
            set { m_notModifiedNotMarked = FromString(value); }
        }

        [XmlIgnore]
        public Color ModifiedMarked {
            get { return m_modifiedMarked; }
            set { m_modifiedMarked = value; }
        }

        [XmlElement("ModifiedProjectMarked")]
        public string ModifiedMarkedAsString {
            get { return AsString(m_modifiedMarked); }
            set { m_modifiedMarked = FromString(value); }
        }

        [XmlIgnore]
        public Color ModifiedNotMarked {
            get { return m_modifiedNotMarked; }
            set { m_modifiedNotMarked = value; }
        }

        [XmlElement("ModifiedProjectNotMarked")]
        public string ModifiedNotMarkedAsString {
            get { return AsString(m_modifiedNotMarked); }
            set { m_modifiedNotMarked = FromString(value); }
        }

        [XmlIgnore]
        public Color InvalidVersionMarked {
            get { return m_invalidVersionMarked; }
            set { m_invalidVersionMarked = value; }
        }

        [XmlElement("ProjectWithInvalidVersionMarked")]
        public string MarkedWithInvalidVersionAsString {
            get { return AsString(m_invalidVersionMarked); }
            set { m_invalidVersionMarked = FromString(value); }
        }

        [XmlIgnore]
        public Color InvalidVersionNotMarked {
            get { return m_invalidVersionNotMarked; }
            set { m_invalidVersionNotMarked = value; }
        }

        [XmlElement("ProjectWithInvalidVersionNotMarked")]
        public string NotMarkedWithInvalidVersionAsString {
            get { return AsString(m_invalidVersionNotMarked); }
            set { m_invalidVersionNotMarked = FromString(value); }
        }

        [XmlIgnore]
        public Color NoVersion {
            get { return m_noVersion; }
            set { m_noVersion = value; }
        }

        [XmlElement("ProjectWithoutVersion")]
        public string WithoutVersionAsString {
            get { return AsString(m_noVersion); }
            set { m_noVersion = FromString(value); }
        }

        [XmlIgnore]
        public Color ReportUpdatedVersion {
            get { return m_updatedVersion; }
            set { m_updatedVersion = value; }
        }

        [XmlElement("ReportUpdatedVersion")]
        public string ReportUpdatedVersionAsString {
            get { return AsString(m_updatedVersion); }
            set { m_updatedVersion = FromString(value); }
        }

        [XmlIgnore]
        public Color ReportVersionNotChanged {
            get { return m_versionNotChanged; }
            set { m_versionNotChanged = value; }
        }

        [XmlElement("ReportVersionNotChanged")]
        public string ReportVersionNotChangedAsString {
            get { return AsString(m_versionNotChanged); }
            set { m_versionNotChanged = FromString(value); }
        }

        [XmlIgnore]
        public Color ReportVersionUpdateFailed {
            get { return m_versionUpdateFailed; }
            set { m_versionUpdateFailed = value; }
        }

        [XmlElement("ReportVersionUpdateFailed")]
        public string ReportVersionUpdateFailedAsString {
            get { return AsString(m_versionUpdateFailed); }
            set { m_versionUpdateFailed = FromString(value); }
        }

        [XmlIgnore]
        public Color SubProjectRoot {
            get { return m_subProjectRoot; }
            set { m_subProjectRoot = value; }
        }

        [XmlElement("SubProjectRoot")]
        public string SubProjectRootAsString {
            get { return AsString(m_subProjectRoot); }
            set { m_subProjectRoot = FromString(value); }
        }

        object ICloneable.Clone() {
            return this.Clone();
        }

        public ProjectsListViewColorsConfiguration Clone() {
            return (ProjectsListViewColorsConfiguration)this.MemberwiseClone();
        }

        public override bool Equals(object other) {
            if (other == null)
                return false;
            if (GetType() != other.GetType())
                return false;
            ProjectsListViewColorsConfiguration oplvcc = (ProjectsListViewColorsConfiguration)other;
            if (NotModifiedMarked != oplvcc.NotModifiedMarked)
                return false;
            if (NotModifiedNotMarked != oplvcc.NotModifiedNotMarked)
                return false;
            if (ModifiedMarked != oplvcc.ModifiedMarked)
                return false;
            if (ModifiedNotMarked != oplvcc.ModifiedNotMarked)
                return false;
            if (InvalidVersionMarked != oplvcc.InvalidVersionMarked)
                return false;
            if (InvalidVersionNotMarked != oplvcc.InvalidVersionNotMarked)
                return false;
            if (NoVersion != oplvcc.NoVersion)
                return false;
            if (ReportUpdatedVersion != oplvcc.ReportUpdatedVersion)
                return false;
            if (ReportVersionNotChanged != oplvcc.ReportVersionNotChanged)
                return false;
            if (ReportVersionUpdateFailed != oplvcc.ReportVersionUpdateFailed)
                return false;
            if (SubProjectRoot != oplvcc.SubProjectRoot)
                return false;
            return true;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        private string AsString(Color c) {
            if (c.IsKnownColor)
                return c.ToKnownColor().ToString(); 
            else
                return c.ToArgb().ToString();
        }

        private Color FromString(string s) {
            if (Enum.IsDefined(typeof(KnownColor), s))
                return Color.FromName(s);
            return Color.FromArgb(int.Parse(s)); 
        }


        private Color m_notModifiedMarked;
        private Color m_notModifiedNotMarked;
        private Color m_modifiedMarked;
        private Color m_modifiedNotMarked;
        private Color m_invalidVersionMarked;
        private Color m_invalidVersionNotMarked;
        private Color m_noVersion;
        private Color m_updatedVersion;
        private Color m_versionNotChanged;
        private Color m_versionUpdateFailed;
        private Color m_subProjectRoot;

        public static bool operator==(ProjectsListViewColorsConfiguration plvcc1, ProjectsListViewColorsConfiguration plvcc2) {
            if (ReferenceEquals(null, plvcc1) || ReferenceEquals(null, plvcc2)) {
                return ReferenceEquals(plvcc1, plvcc2);
            }
            return plvcc1.Equals(plvcc2);  
        }

        public static bool operator!=(ProjectsListViewColorsConfiguration plvcc1, ProjectsListViewColorsConfiguration plvcc2) {
            return !(plvcc1 == plvcc2);  
        }

        public static ProjectsListViewColorsConfiguration Default = new ProjectsListViewColorsConfiguration();
	}
}