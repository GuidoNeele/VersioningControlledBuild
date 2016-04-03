/*
 * Filename:    ProjectVersion.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Wrapper arround version.
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
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;

namespace BuildAutoIncrement {

	/// <summary>
	///   <c>ProjectVersion</c> is a wrapper arround <c>Version</c> class 
	///   adding increment functionality and supporting <c>Empty</c> (i.e.
	///   not defined) version.
	/// </summary>
	public class ProjectVersion : IComparable, ICloneable {
        
        public enum VersionComponent {
            Major,
            Minor,
            Build,
            Revision
        }

        #region Constructors

        /// <summary>
		///   Initializes a new instance of the <c>ProjectVersion</c> class 
		///   with a non-defined (empty) version.
		/// </summary>
		private ProjectVersion() {
            m_version = new ListDictionary();
            Valid = true;
		}

        private ProjectVersion(bool valid) : this() {
            Valid = valid;
        }

        /// <summary>
        ///   Initializes a new instance of the <c>ProjectVersion</c> class using 
        ///   the value represented by the specified <c>String</c>.
        /// </summary>
        /// <param name="version">
        ///   Version string of the form: [Major].[Minor].[Build].[Revision]. 
        ///   Major and Minor components are obligatory, while Build and 
        ///   Revison may be omitted or substituted by a single or two 
        ///   asterisks.
        /// </param>
        protected ProjectVersion(string version) : this() {
            m_originalString = version;
            SplitComponents(version);
        }

        public ProjectVersion(string version, AssemblyVersionType versionType) : this() {
            m_originalString = version;
            if (version.IndexOf(',') != -1) {
                version = version.Replace(',', '.');
            }
            Valid = IsValidVersionString(version, versionType, ProjectType.CSharpProject);
            if (Valid)
                SplitComponents(version);
        }

        /// <summary>
		///   Initializes a new instance of the <c>ProjectVersion</c> class 
		///   with the specified major, minor, build, and revision numbers.
		/// </summary>
		/// <param name="major">
		///   The major version number.
		/// </param>
		/// <param name="minor">
		///   The minor version number.
		/// </param>
		/// <param name="build">
		///   The build number.
		/// </param>
		/// <param name="revision">
		///   The revision number.
		/// </param>
		private ProjectVersion(int major, int minor, int build, int revision) : this(major, minor, build) {
            m_version[VersionComponent.Revision] = revision;
		}

        /// <summary>
        ///   Initializes a new instance of the <c>ProjectVersion</c> class 
        ///   with the specified major, minor and build numbers.
        /// </summary>
        /// <param name="major">
        ///   The major version number.
        /// </param>
        /// <param name="minor">
        ///   The minor version number.
        /// </param>
        /// <param name="build">
        ///   The build number.
        /// </param>
        private ProjectVersion(int major, int minor, int build) : this(major, minor) {
            m_version[VersionComponent.Build] = build;
        }
        
        /// <summary>
        ///   Initializes a new instance of the <c>ProjectVersion</c> class 
        ///   with the specified major and minor numbers.
        /// </summary>
        /// <param name="major">
        ///   The major version number.
        /// </param>
        /// <param name="minor">
        ///   The minor version number.
        /// </param>
        private ProjectVersion(int major, int minor) : this() {
            m_version[VersionComponent.Major] = major;
            m_version[VersionComponent.Minor] = minor;
        }
        
        /// <summary>
		///   Copy constructor.
		/// </summary>
		/// <param name="version">
		///   <c>ProjectVersion</c> to copy.
		/// </param>
		private ProjectVersion(ProjectVersion version) : this() {
            foreach (VersionComponent vc in version.Version.Keys) {
                m_version[vc] = (int)version.Version[vc];
            }
            Valid = version.Valid;
            m_originalString = version.m_originalString;
		}

        #endregion // Constructors

        #region IComparable interface implementation

        /// <summary>
        ///   IComparable interface implementation.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int IComparable.CompareTo(object obj) {
            return CompareTo((ProjectVersion)obj);
        }

        /// <summary>
        ///   Compares this instance with another one.
        /// </summary>
        /// <param name="pattern">
        ///   <c>ProjectVersion</c> to compare to.
        /// </param>
        /// <returns>
        ///   0 if versions are equal, negative number if this instance 
        ///   is a lower version or positive number if this instance is a 
        ///   higher version than the version provided.
        /// </returns>
        public int CompareTo(ProjectVersion other) {
            int[] vc1 = ComparableComponents;
            int[] vc2 = other.ComparableComponents;
            for (int i = 0; i < 4; i++) {
                if (vc1[i] < vc2[i]) {
                    return -1;
                }
                if (vc1[i] > vc2[i]) {
                    return +1;
                }
            }
            return 0;
        }

        /// <summary>
        ///   Compares this version with a string presentation of another
        ///   instance.
        /// </summary>
        /// <param name="other">
        ///   String presentation of the <c>ProjectVersion</c> to compare to.
        /// </param>
        /// <returns>
        ///   0 if versions are equal, negative number if this instance 
        ///   is a lower version or positive number if this instance is a 
        ///   higher version than the version provided.
        /// </returns>
        public int CompareTo(string other) {
            ProjectVersion otherVersion = new ProjectVersion(other);
            return CompareTo(otherVersion);
        }

        
        /// <summary>
        ///   Checks if version provided is higher than the crrent instance. In
        ///   contrast to <c>CompareTo</c> methods, for a version component 
        ///   containing asterisk, the pattern provided is assumed to be higher.
        /// </summary>
        /// <param name="pattern">
        ///   String presentation of a version.
        /// </param>
        /// <returns>
        ///   <c>true</c> if pattern provided is higher version.
        /// </returns>
        public bool IsStringPatternHigher(string pattern) {
            ProjectVersion other = new ProjectVersion(pattern);
            int[] vc1 = ComparableComponents;
            int[] vc2 = other.ComparableComponents;
            for (int i = 0; i < 4; i++) {
                if (vc1[i] != vc2[i])
                    return vc2[i] > vc1[i];
                // if component is asterisk, then string pattern will be higher
                if (vc1[i] == MaxVersion && vc2[i] == MaxVersion) {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        ///   Compares this version with a pattern provided.
        /// </summary>
        /// <param name="pattern">
        ///   String pattern to compare to. Pattern may contain asterisk ('*') 
        ///   and '+' characters which are treated as wildcards. For '*' 
        ///   wildcard corresponding version component is assumed to be equal.
        ///   For '+' version component is assumed to be lower.
        /// </param>
        /// <returns>
        ///   0 if versions are equal, negative non-zero integer if this version 
        ///   is lower, positive non-zero integer if this version is higher.
        /// </returns>
        public int CompareToPattern(string pattern) {
            Debug.Assert(pattern != null && pattern.Length > 0);
            string[] splitPattern = pattern.Split('.');
            Debug.Assert(splitPattern.Length == 4);
            int[] components = ComparableComponents;
            for (int i = 0; i < 4; i++) {
                if (splitPattern[i] == "+")
                    return -1;
                if (splitPattern[i] != "*") {
                    int patternComponent = int.Parse(splitPattern[i]);
                    if (components[i] < patternComponent)
                        return -1;
                    if (components[i] > patternComponent)
                        return +1;
                }
            }
            return 0;
        }

        #endregion // IComparable interface implementation

        #region ICloneable interface implementation

        object ICloneable.Clone() {
            return Clone();
        }

        public ProjectVersion Clone() {
            return new ProjectVersion(this);
        }

        /// <summary>
        ///   Clones this version assigning provided Build and Revision values.
        /// </summary>
        /// <param name="build">
        ///   Build to assign.
        /// </param>
        /// <param name="revision">
        ///   Revision to assign.
        /// </param>
        /// <returns>
        ///   New <c>ProjectVersion</c> object.
        /// </returns>
        public ProjectVersion Clone(int build, int revision, bool createRevision) {
            Debug.Assert(m_version.Count >= 2);
            if (m_version.Count == 2) 
                return new ProjectVersion(this[VersionComponent.Major], this[VersionComponent.Minor]);
            if (m_version.Count == 4 || (this[VersionComponent.Build] == MaxVersion && createRevision))
                return new ProjectVersion(this[VersionComponent.Major], this[VersionComponent.Minor], build, revision);
            return new ProjectVersion(this[VersionComponent.Major], this[VersionComponent.Minor], build);
        }

        #endregion // ICloneable interface implementation

        /// <summary>
        ///   Maximum version for a component.
        /// </summary>
        public const int MaxVersion = UInt16.MaxValue;

        #region Public methods

        /// <summary>
        ///   Increments the version according to numbering scheme defined in
        ///   configuration.
        /// </summary>
        /// <param name="numberingOptions">
        ///   <c>NumberingOptions</c> that define which component of the version
        ///   should be incremented.
        /// </param>
        public void Increment(NumberingOptions numberingOptions) {
            switch (numberingOptions.IncrementScheme) {
            case IncrementScheme.IncrementMajorVersion:
                IncrementComponent(VersionComponent.Major, numberingOptions);
                break;
            case IncrementScheme.IncrementMinorVersion:
                IncrementComponent(VersionComponent.Minor, numberingOptions);
                break;
            case IncrementScheme.IncrementBuild:
                IncrementComponent(VersionComponent.Build, numberingOptions);
                break;
            case IncrementScheme.IncrementRevision:
                IncrementComponent(VersionComponent.Revision, numberingOptions);
                break;
            }
        }
        
        /// <summary>
        ///   Increments the version using configuration settings. Numerical 
        ///   overflow increments the higher component.
        /// </summary>
        /// <param name="toIncrement">
        ///   <c>VersionComponent</c> to increment.
        /// </param>
        /// <param name="numberingOptions">
        ///   <c>NumberingOptions</c> which defines the scheme for increment.
        /// </param>
        public void IncrementComponent(VersionComponent toIncrement, NumberingOptions numberingOptions) {
            if (numberingOptions.UseDateTimeBasedBuildAndRevisionNumbering) 
                IncrementUsingDateTimeBasedBuildAndRevisionNumbering(toIncrement, numberingOptions);
            else
                IncrementVersionStandard(toIncrement, numberingOptions);
        }

        /// <summary>
        ///   Checks if version contains a wildcard ('*') character.
        /// </summary>
        /// <returns>
        ///   Returns <c>true</c> if there is a wildcard character.
        /// </returns>
        public bool ContainsWildCard() {
            if (Version[VersionComponent.Build] != null && this[VersionComponent.Build] == MaxVersion)
                return true;
            if (Version[VersionComponent.Revision] != null && this[VersionComponent.Revision] == MaxVersion)
                return true;
            return false;
        }

        #endregion // Public methods

        #region Public overrides

        /// <summary>
        ///   Converts the value of this instance to its equivalent <c>String</c> 
        ///   representation.
        /// </summary>
        /// <returns>
        ///   The string representation of the values of the major, minor, 
        ///   build, and revision components of this instance.
        /// </returns>
        public override string ToString() {
            return ToString(false);
        }

        /// <summary>
        ///   Converts the value of this instance to its equivalent <c>String</c> 
        ///   representation.
        /// </summary>
        /// <param name="displayAllComponents">
        ///   Flag indicating if all versions (including Build and Revision) 
        ///   should be included. If this flag is set to <c>true</c> and Build
        ///   and/or Revision are missing, they are substituted by asterisk.
        /// </param>
        /// <returns>
        ///   The string representation of the values of the major, minor, 
        ///   build, and revision components of this instance.
        /// </returns>
        public string ToString(bool displayAllComponents) {
            if (this == ProjectVersion.Empty)
                return m_originalString;
            StringBuilder result = new StringBuilder();
            result.AppendFormat("{0}.{1}", (int)Version[VersionComponent.Major], (int)Version[VersionComponent.Minor]);
            bool asteriskInput = false;
            if (Version[VersionComponent.Build] != null) {
                int build = (int)Version[VersionComponent.Build];
                if (build < MaxVersion) {
                    Debug.Assert(build >= 0);
                    result.AppendFormat(".{0}", build);
                }
                else {
                    result.Append(".*");
                    asteriskInput = true;
                }
            }
            else if (displayAllComponents) {
                result.Append(".*");
            }
            if (Version[VersionComponent.Revision] != null) {
                int revision = (int)Version[VersionComponent.Revision];
                if (revision < MaxVersion) {
                    Debug.Assert(revision >= 0);
                    result.AppendFormat(".{0}", revision);
                }
                else if (!asteriskInput) {
                    result.Append(".*");
                }
            }
            else if (displayAllComponents) {
                result.Append(".*");
            }
            return result.ToString();
        }

        /// <summary>
        ///   Returns a value indicating whether this instance is equal to a 
        ///   specified object. 
        /// </summary>
        /// <param name="obj">
        ///   An object to compare with this instance, or a null reference.
        /// </param>
        /// <returns>
        ///   <c>true</c> if this instance and <c>obj</c> are both 
        ///   <c>ProjectVersion</c> objects, and every component of this 
        ///   instance matches the corresponding component of <c>obj</c>; 
        ///   otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) 
                return false;
            ProjectVersion otherPv = (ProjectVersion)obj;
            return CompareTo(otherPv) == 0;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode() {
            return 0;
        }

        #endregion Public overrides

        #region Private properties

        /// <summary>
        ///   Gets the internal <c>Version</c> list.
        /// </summary>
        private ListDictionary Version {
            get { return m_version; }
        }

        private int this[VersionComponent component] {
            get {
                return (int)Version[component];
            }
        }

        #endregion // Private properties

        #region Private methods

        /// <summary>
        ///   Splits version components.
        /// </summary>
        /// <param name="version"></param>
        private void SplitComponents(string version) {
            string[] splitVersion = version.Split('.');
            Debug.Assert(splitVersion.Length > 1);
            m_version[VersionComponent.Major] = int.Parse(splitVersion[0]);
            if (splitVersion.Length > 1)
                m_version[VersionComponent.Minor] = int.Parse(splitVersion[1]);
            if (splitVersion.Length > 2)
                m_version[VersionComponent.Build] = splitVersion[2] == "*"    ? MaxVersion : int.Parse(splitVersion[2]);
            if (splitVersion.Length > 3)
                m_version[VersionComponent.Revision] = splitVersion[3] == "*" ? MaxVersion : int.Parse(splitVersion[3]);
        }

        /// <summary>
        ///   Increments version.
        /// </summary>
        /// <param name="toIncrement">
        ///   Version component to increment.
        /// </param>
        /// <param name="numberingOptions">
        ///   Numbering options from the configuration.
        /// </param>
        private void IncrementVersionStandard(VersionComponent toIncrement, NumberingOptions numberingOptions) {
            Debug.Assert(!numberingOptions.UseDateTimeBasedBuildAndRevisionNumbering);
            CreateMissingBuildAndRevision(numberingOptions);
            int incrementStep = numberingOptions.IncrementBy;
            bool overflow = false;
            if (toIncrement == VersionComponent.Revision) {
                // if Revision exists and is not asterisk then increment it
                if (Version[VersionComponent.Revision] != null && (int)Version[VersionComponent.Revision] != MaxVersion) {
                    overflow = true;
                    Version[VersionComponent.Revision] = IncrementIntWithOverflow(this[VersionComponent.Revision], incrementStep, ref overflow, (int)numberingOptions.ResetBuildAndRevisionTo);
                }
            }
            if (toIncrement == VersionComponent.Build) {
                overflow = true;
                if (numberingOptions.ResetRevisionOnBuildIncrement && Version[VersionComponent.Revision] != null)
                    Version[VersionComponent.Revision] = (int)numberingOptions.ResetBuildAndRevisionTo;
            }
            if (overflow) {
                // if Build exists and is not asterisk, then increment it
                if ((Version[VersionComponent.Build] != null) && (int)Version[VersionComponent.Build] != MaxVersion) {
                    Version[VersionComponent.Build] = IncrementIntWithOverflow((int)Version[VersionComponent.Build], incrementStep, ref overflow, (int)numberingOptions.ResetBuildAndRevisionTo);
                }
                else {
                    overflow = false;
                }
            }
            if (toIncrement == VersionComponent.Minor) {
                overflow = true;
                if (numberingOptions.ResetBuildOnMinorIncrement && Version[VersionComponent.Build] != null)
                    Version[VersionComponent.Build] = (int)numberingOptions.ResetBuildAndRevisionTo;
                if (numberingOptions.ResetRevisionOnMinorIncrement && Version[VersionComponent.Revision] != null)
                    Version[VersionComponent.Revision] = (int)numberingOptions.ResetBuildAndRevisionTo;
            }
            Version[VersionComponent.Minor] = IncrementIntWithOverflow((int)Version[VersionComponent.Minor], incrementStep, ref overflow, 0);
            if (toIncrement == VersionComponent.Major) {
                overflow = true;
                // incrementing Major will automatically reset Minor
                Version[VersionComponent.Minor] = 0;
                if (numberingOptions.ResetBuildOnMinorIncrement && Version[VersionComponent.Build] != null)
                    Version[VersionComponent.Build] = (int)numberingOptions.ResetBuildAndRevisionTo;
                if (numberingOptions.ResetRevisionOnMinorIncrement && Version[VersionComponent.Revision] != null)
                    Version[VersionComponent.Revision] = (int)numberingOptions.ResetBuildAndRevisionTo;
            }
            Version[VersionComponent.Major] = IncrementIntWithOverflow((int)Version[VersionComponent.Major], incrementStep, ref overflow, 0);
			if (overflow) {
				throw new VersionOverflowException(VersionComponent.Major);
			}
		}

        /// <summary>
        ///   Creates missing Build and Revision components. Called before
        ///   version increment in order to supply missing components.
        /// </summary>
        /// <param name="toIncrement">
        ///   <c>VersionComponent</c> that is going to be incremented.
        /// </param>
        /// <param name="numberingOptions">
        ///   Numbering options.
        /// </param>
        private void CreateMissingBuildAndRevision(NumberingOptions numberingOptions) {
            if (numberingOptions.ReplaceAsteriskWithVersionComponents) {
                if ((Version[VersionComponent.Revision] == null && Version[VersionComponent.Build] != null && (int)Version[VersionComponent.Build] == MaxVersion) 
                    || (Version[VersionComponent.Revision] != null && (int)Version[VersionComponent.Revision] == MaxVersion)) {
                    Version[VersionComponent.Revision] = (int)numberingOptions.ResetBuildAndRevisionTo;
                }
                if (Version[VersionComponent.Build] != null && (int)Version[VersionComponent.Build] == MaxVersion) {
                    Version[VersionComponent.Build] = (int)numberingOptions.ResetBuildAndRevisionTo;
                }
            }
        }

        /// <summary>
        ///   Increments version using date &amp; time based build/revision 
        ///   schema.
        /// </summary>
        /// <param name="toIncrement">
        ///   Version component to increment.
        /// </param>
        /// <param name="numberingOptions">
        ///   Numbering options from the configuration.
        /// </param>
        private void IncrementUsingDateTimeBasedBuildAndRevisionNumbering(VersionComponent toIncrement, NumberingOptions numberingOptions) {
            Debug.Assert(numberingOptions.UseDateTimeBasedBuildAndRevisionNumbering);
            int incrementStep = numberingOptions.IncrementBy;
            bool overflow = false;
            if (toIncrement == VersionComponent.Minor) 
                overflow = true;
            Version[VersionComponent.Minor] = IncrementIntWithOverflow((int)Version[VersionComponent.Minor], incrementStep, ref overflow, 0);
            if (toIncrement == VersionComponent.Major) {
                overflow = true;
                // incrementing Major automatically resets Minor
                Version[VersionComponent.Minor] = 0;
            }
            Version[VersionComponent.Major] = IncrementIntWithOverflow((int)Version[VersionComponent.Major], incrementStep, ref overflow, 0);
            if (overflow) {
                throw new VersionOverflowException(VersionComponent.Major);
            }
        }

        /// <summary>
        ///   Increments an integer if overflow flag is set. If incremented 
        ///   integer exceeds largest integer value, resulting value is reset 
        ///   to <c>resetValue</c> and overflow flag is set. If integer is 
        ///   negative, it is set to <c>resetValue</c>.
        /// </summary>
        /// <param name="toIncrement">
        ///   Integer to increment.
        /// </param>
        /// <param name="overflow">
        ///   Overflow flag.
        /// </param>
        /// <param name="resetValue">
        ///   Value to start numbering from. Usually 0, but for Build and 
        ///   Revision it may be 1, depending on configuration settings.
        /// </param>
        /// <returns>
        ///   Incremented value.
        /// </returns>
        private int IncrementIntWithOverflow(int toIncrement, int incrementStep, ref bool overflow, int resetValue) {
			if (overflow) {
                if (toIncrement < 0) {
                    overflow = false;
                    return resetValue;
                }
                toIncrement += incrementStep;
                // if value to increment has the largest value, then it 
                // should be reset and overflow is forwarded to higher version
                if (toIncrement >= (MaxVersion)) {
					return resetValue;
				}
			    overflow = false;
			}
			return toIncrement;
		}

        /// <summary>
        ///   Gets an array of integers representing components, that may be 
        ///   used for version comparisons.
        /// </summary>
        private int[] ComparableComponents {
            get {
                if (Version.Count == 0)
                    return new int[] { -1, -1, -1, -1 };
                int[] components = { 0, 0, 0, 0 };
                Debug.Assert(Version.Count >= 2 && Version.Count <= 4);
                components[0] = (int)Version[VersionComponent.Major];
                components[1] = (int)Version[VersionComponent.Minor];
                if (Version.Count > 2) {
                    components[2] = (int)Version[VersionComponent.Build];
                    if (Version.Count > 3) {
                        components[3] = (int)Version[VersionComponent.Revision];
                    }
                    else if (components[2] == MaxVersion) {
                        components[3] = MaxVersion;
                    }
                    else
                        components[3] = 0;
                }
                return components;
            }
        }

        #endregion // Private methods

        #region Private fields

		private ListDictionary m_version;

        private string m_originalString = "";

        private static readonly string s_txtInvalidVersionString;
        private static readonly string s_txtInvalidVersionPattern;
        private static readonly string s_txtVersionMustConsistOfAtLeastNComponents;
        private static readonly string s_txtVersionMustConsistOfAtMostNComponents;
        private static readonly string s_txtMustNotContainNegativeIntegers;
        private static readonly string s_txtMustBeNonNegativeIntegersOrAsterisk;
        private static readonly string s_txtMustBeNonNegativeIntegers;
        private static readonly string s_txtAsteriskMustBeLast;
        private static readonly string s_txtNoAsteriskAllowed;

        private static readonly string s_txtMustBeIntegerSmallerThanMaxValue;

        private static readonly string s_txtVersionMustNotEndWithDot;

        #endregion Private fields

        #region Type constructor

        static ProjectVersion() {
            ResourceManager resources = new System.Resources.ResourceManager("BuildAutoIncrement.Resources.Shared", typeof(ResourceAccessor).Assembly);
            Debug.Assert(resources != null);
            
            s_txtInvalidVersionString                                = resources.GetString("Invalid version string");
            s_txtInvalidVersionPattern                               = resources.GetString("Invalid version pattern");
            s_txtVersionMustConsistOfAtLeastNComponents              = resources.GetString("Version must consist of at least N components");
            s_txtVersionMustConsistOfAtMostNComponents               = resources.GetString("Version must consist of at most N components");
            s_txtMustNotContainNegativeIntegers                      = resources.GetString("Version must not contain negative integers");
            s_txtMustBeNonNegativeIntegersOrAsterisk                 = resources.GetString("Version may consist of non-negative integers or a single asterisk character");
            s_txtMustBeNonNegativeIntegers                           = resources.GetString("Version may consist of non-negative integers");
            s_txtAsteriskMustBeLast                                  = resources.GetString("Asterisk may appear only at the end of version string");
            s_txtVersionMustNotEndWithDot                            = resources.GetString("Version must not end with dot");
            s_txtNoAsteriskAllowed                                   = resources.GetString("Asterisk not allowed");
            s_txtMustBeIntegerSmallerThanMaxValue                    = string.Format(resources.GetString("Version must be smaller than"), MaxVersion + 1);

            Debug.Assert(s_txtInvalidVersionString != null);
            Debug.Assert(s_txtInvalidVersionPattern != null);
            Debug.Assert(s_txtVersionMustConsistOfAtLeastNComponents != null);
            Debug.Assert(s_txtVersionMustConsistOfAtMostNComponents != null);
            Debug.Assert(s_txtMustNotContainNegativeIntegers != null);
            Debug.Assert(s_txtMustBeNonNegativeIntegersOrAsterisk != null);
            Debug.Assert(s_txtMustBeNonNegativeIntegers != null);
            Debug.Assert(s_txtAsteriskMustBeLast != null);
            Debug.Assert(s_txtVersionMustNotEndWithDot != null);
            Debug.Assert(s_txtNoAsteriskAllowed != null);
            Debug.Assert(s_txtMustBeIntegerSmallerThanMaxValue != null);
        }

        #endregion // Type constructor

        #region Public static properties

        public readonly bool Valid;

        public static readonly ProjectVersion Empty    = new ProjectVersion();

        public static readonly ProjectVersion Invalid  = new ProjectVersion(false);

        public static readonly ProjectVersion MinValue = new ProjectVersion(0, 0, 0, 0);

        #endregion // Public static properties

        #region Public static methods

        /// <summary>
        ///   Applies a pattern to the version.
        /// </summary>
        /// <param name="pattern">
        ///   Pattern to apply. Pattern may contain '*' and '+' wildcards.
        /// </param>
        /// <param name="version">
        ///   Version onto which pattern has to be applied.
        /// </param>
        /// <param name="buildAndRevisionResetValue">
        ///   Reset value of Build and Revision.
        /// </param>
        /// <returns>
        ///   String with new version.
        /// </returns>
        public static string ApplyVersionPattern(string pattern, string version, int buildAndRevisionResetValue) {
            Debug.Assert(IsValidPattern(pattern));
            string[] patternSections = pattern.Split('.');
            // replace "+" with "+1" for consistency
            for (int l = 0; l < patternSections.Length; l++) {
                if (patternSections[l] == "+")
                    patternSections[l] = "+1";
            }
            string[] versionSections = version.Split('.');
            Debug.Assert(patternSections.Length >= versionSections.Length);
            StringCollection result = new StringCollection();
            int versionLength = versionSections.Length;
            int i = 0;
            bool moreNumbersInPattern = MoreNumbersInPattern(patternSections, 0);
            // first apply pattern sections to existent version sections
            while (i < versionLength) {
                if (patternSections[i].StartsWith("+")) {
                    int increment = int.Parse(patternSections[i]);
                    try {
                        result.Add(IncrementStringInteger(versionSections[i], increment));
                    }
                    catch (OverflowException) {
                        VersionComponent vc = (VersionComponent)Enum.GetValues(typeof(VersionComponent)).GetValue(i);
                        throw new VersionOverflowException(vc);
                    }
                }
                else if (patternSections[i] != "*") {
                    result.Add(patternSections[i]);
                }
                else {
                    // update moreNumbersInPatern (only if true)
                    if (moreNumbersInPattern) {
                        Debug.Assert(patternSections[i] == "*");
                        moreNumbersInPattern = MoreNumbersInPattern(patternSections, i + 1);
                    }
                    // if this character is asterisk and pattern contains more numbers, skip to next loop
                    if (moreNumbersInPattern && versionSections[i] == "*") {
                        int resetValue = i >= (int)VersionComponent.Build ? buildAndRevisionResetValue : 0;
                        result.Add(resetValue.ToString());
                        i++;
                        break;
                    }
                    result.Add(versionSections[i]);
                }
                i++;
            }
            // extend version if ends with asterisk and pattern contains more numerical versions
            if (versionSections[i - 1] == "*" && moreNumbersInPattern) {
                int patternLength = patternSections.Length;
                while (i < patternLength) {
                    if (!MoreNumbersInPattern(patternSections, i))
                        break;
                    int resetValue = i >= (int)VersionComponent.Build ? buildAndRevisionResetValue : 0;
                    if (patternSections[i].StartsWith("+")) {
                        int newValue = int.Parse(patternSections[i]) + resetValue;
                        result.Add(newValue.ToString());
                    }
                    else if (patternSections[i] == "*") {
                        result.Add(resetValue.ToString());
                    }
                    else { 
                        result.Add(patternSections[i]);
                    }
                    i++;
                }
            }
            // setup the output string
            StringBuilder sb = new StringBuilder();
            foreach (string s in result) {
                sb.Append(s);
                sb.Append(".");
            }
            return sb.ToString(0, sb.Length - 1);
        }


        /// <summary>
        ///   Converts comma delimited string into dot delimited.
        /// </summary>
        /// <param name="version">String to convert</param>
        /// <returns>Converted string.</returns>
        public static string ConvertFromCommaDelimited(string version) {
            string[] versionSections = version.Split(',');
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < versionSections.Length; i++) {
                result.Append(versionSections[i].Trim() + ".");
            }
            return result.ToString(0, result.Length - 1);
        }

        /// <summary>
        ///   Class used for checking version formats.
        /// </summary>
        private class AssemblyVersionFormatProvider {
            public AssemblyVersionFormatProvider(AssemblyVersionType assemblyVersionType, ProjectType projectType) {
                m_assemblyVersionType = assemblyVersionType;
                m_projectType         = projectType;
            }

            public int MinLength {
                get {
                    switch (m_assemblyVersionType) {
                    case AssemblyVersionType.AssemblyVersion:
                        return 2;
                    case AssemblyVersionType.AssemblyFileVersion:
                        return 2;
                    case AssemblyVersionType.AssemblyInformationalVersion:
                        return 2;
                    }
                    return 2;
                }
            }

            public int MaxLength {
                get {
                    if (m_projectType == ProjectType.SetupProject)
                        return 3;
                    return 4;
                }
            }

            public bool IsWildcardAllowed {
                get {
                    switch (m_assemblyVersionType) {
                    case AssemblyVersionType.AssemblyVersion:
                        return true;
                    case AssemblyVersionType.AssemblyFileVersion:
                        return false;
                    case AssemblyVersionType.AssemblyInformationalVersion:
                        return false;
                    }
                    return false;
                }
            }

            AssemblyVersionType m_assemblyVersionType;
            ProjectType         m_projectType;
        }

        /// <summary>
        ///   Validates version string and returns description string in the
        ///   case of invalid format. If version string is valid, returns empty
        ///   string.
        /// </summary>
        /// <param name="version">
        ///   Version string to validate.
        /// </param>
        /// <param name="assemblyVersionType">
        ///   <c>AssemblyVersionType</c> to which this string corresponds.
        /// </param>
        /// <param name="projectType">
        ///   <c>ProjectType</c> to which this string corresponds.
        /// </param>
        /// <returns>
        ///   String with error description or empty string if version is valid.
        /// </returns>
        public static string ValidateVersionString(string version, AssemblyVersionType assemblyVersionType, ProjectType projectType) {
            Debug.Assert(version != null);
            AssemblyVersionFormatProvider avfp = new AssemblyVersionFormatProvider(assemblyVersionType, projectType);
            if (version.EndsWith("."))
                return s_txtVersionMustNotEndWithDot;
            string[] versionParts = version.Split('.');
            if (versionParts.Length < avfp.MinLength)
                return string.Format(s_txtVersionMustConsistOfAtLeastNComponents, avfp.MinLength);
            if (versionParts.Length > avfp.MaxLength)
                return string.Format(s_txtVersionMustConsistOfAtMostNComponents, avfp.MaxLength);
            for (int i = 0; i < avfp.MinLength; i++) {
                try {
                    int val = int.Parse(versionParts[i]);
                    if (val < 0)
                        return s_txtMustNotContainNegativeIntegers;
                }
                catch {
                    return s_txtMustBeNonNegativeIntegers;
                }
            }
            // if there is asterisk in Build or Revision it must be the last component in version 
            if (avfp.IsWildcardAllowed) {
                if (versionParts.Length > 3 && versionParts[2] == "*")
                    return s_txtAsteriskMustBeLast;
            }
            // flag controlling if number appears after an asterisk (asterisk 
            // must not be followed by integer version components)
            for (int i = avfp.MinLength; i < versionParts.Length; i++) {
                if (versionParts[i] == "*") {
                    if (!avfp.IsWildcardAllowed)
                        return s_txtNoAsteriskAllowed;
                }
                else {
                    int val = 0;
                    try {
                        val = int.Parse(versionParts[i]);
                    }
                    catch (FormatException) {
                        return avfp.IsWildcardAllowed ? s_txtMustBeNonNegativeIntegersOrAsterisk : s_txtMustBeNonNegativeIntegers;
                    }
                    catch (OverflowException) {
                        return s_txtMustBeIntegerSmallerThanMaxValue;
                    }
                    if (val < 0)
                        return s_txtMustNotContainNegativeIntegers;
                    if (val > MaxVersion) 
                        return s_txtMustBeIntegerSmallerThanMaxValue;
                }
            }
            return string.Empty;
        }

        /// <summary>
        ///   Returns higher of two <c>ProjectVersion</c> objects provided.
        /// </summary>
        /// <param name="v1">
        ///   First <c>ProjectVersion</c> to compare.
        /// </param>
        /// <param name="v2">
        ///   Second <c>ProjectVersion</c> to compare.
        /// </param>
        /// <returns>
        ///   Reference two higher <c>ProjectVersion</c> objects.
        /// </returns>
        public static ProjectVersion Max(ProjectVersion v1, ProjectVersion v2) {
            int[] vc1 = v1.ComparableComponents;
            int[] vc2 = v2.ComparableComponents;
            for (int i = 0; i < 4 ; i++) {
                if (vc1[i] != vc2[i]) {
                    // if both aren't wildcards, return the larger
                    if (vc1[i] != MaxVersion && vc2[i] != MaxVersion) {
                        return vc1[i] > vc2[i] ? v1 : v2;
                    }
                    else {
                        return vc1[i] != MaxVersion ? v1 : v2;
                    }
                }
            }
            return v1;
        }

        /// <summary>
        ///   Validates version string.
        /// </summary>
        /// <param name="version">
        ///   Version string to validate.
        /// </param>
        /// <param name="assemblyVersionType">
        ///   <c>AssemblyVersionType</c> to which this string corresponds.
        /// </param>
        /// <param name="projectType">
        ///   <c>ProjectType</c> to which this string corresponds.
        /// </param>
        /// <returns>
        ///   <c>true</c> if version is valid, else returns <c>false</c>.
        /// </returns>
        public static bool IsValidVersionString(string version, AssemblyVersionType assemblyVersionType, ProjectType projectType) {
            return ValidateVersionString(version, assemblyVersionType, projectType) == string.Empty;
        }

        /// <summary>
        ///   Checks if pattern to be applied is valid. Pattern must consist of
        ///   exactly four dot separated sections (Major, Minor, Build and 
        ///   Revision). Each section may consist of an integer, an integer
        ///   preceeded by '+' character, '*' or '+' character.
        /// </summary>
        /// <param name="pattern">
        ///   Pattern to validate.
        /// </param>
        /// <returns>
        ///   <c>true</c> if pattern is a valid one, else returns <c>false</c>.
        /// </returns>
        public static bool IsValidPattern(string pattern) {
            string[] patternSections = pattern.Split('.');
            if (patternSections.Length != 4)
                return false;
            foreach (string section in patternSections) {
                switch (section) {
                case "*":
                case "+":
                    break;
                default:
                    try {
                        long val = long.Parse(section);
                        if (val < 0 || val >= MaxVersion)
                            return false;
                    }
                    catch (Exception) {
                        return false;
                    }
                    break;
                }
            }
            return true;
        }

        #endregion // Public static methods
 
        #region Private static methods

        private static string IncrementStringInteger(string integerValue, int increment) {
            if (integerValue == "*")
                return integerValue;
            // although Parse method may throw an exception, in this case 
            // integerValue should be a valid string representation of integer
            long n = long.Parse(integerValue);
            n += increment;
            if (n >= MaxVersion)
                throw new OverflowException();
            return n.ToString();
        }

        private static bool MoreNumbersInPattern(string[] sections, int index) {
            for (int i = index; i < sections.Length; i++) {
                if (sections[i] != "*" && !sections[i].StartsWith("+")) {
                    return true;
                }
            }
            return false;
        }

        #endregion Private static methods

        #region Operators

        /// <summary>
        ///   Determines whether two specified instances of <c>ProjectVersion</c> 
        ///   are equal.
        /// </summary>
        /// <param name="v1">
        ///   The first instance of <c>ProjectVersion</c>.
        /// </param>
        /// <param name="v2">
        ///   The second instance of <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///   <c>true</c> if v1 equals v2; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ProjectVersion v1, ProjectVersion v2) {
            if ((object)v1 == null)
                return (object)v1 == (object)v2;
            return v1.Equals(v2);
        }

        /// <summary>
        ///   Determines whether two specified instances of <c>ProjectVersion</c> 
        ///   are not equal.
        /// </summary>
        /// <param name="v1">
        ///   The first instance of <c>ProjectVersion</c>.
        /// </param>
        /// <param name="v2">
        ///   The second instance of <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///   <c>true</c> if v1 does not equal v2; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ProjectVersion v1, ProjectVersion v2) {
            return !(v1 == v2);
        }

        /// <summary>
        ///   Determines whether the first specified instance of <c>ProjectVersion</c> 
        ///   is greater than the second specified instance of <c>ProjectVersion</c>.
        /// </summary>
        /// <param name="v1">
        ///   The first instance of <c>ProjectVersion</c>.
        /// </param>
        /// <param name="v2">
        ///   The second instance of <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///   <c>true</c> if v1 is greater than v2; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(ProjectVersion v1, ProjectVersion v2) {
            Debug.Assert(v1 != null);
            Debug.Assert(v2 != null);
            int[] vc1 = v1.ComparableComponents;
            int[] vc2 = v2.ComparableComponents;
            for (int i = 0; i < 4; i++) {
                if (vc1[i] != vc2[i])
                    return vc1[i] > vc2[i];
            }
            return false;
        }

        /// <summary>
        ///   Determines whether the first specified instance of <c>ProjectVersion</c> 
        ///   is greater than or equal to the second specified instance of 
        ///   <c>ProjectVersion</c>.
        /// </summary>
        /// <param name="v1">
        ///   The first instance of <c>ProjectVersion</c>.
        /// </param>
        /// <param name="v2">
        ///   The second instance of <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///   <c>true</c> if v1 is greater than or equal to v2; 
        ///   otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >=(ProjectVersion v1, ProjectVersion v2) {
            Debug.Assert(v1 != null);
            Debug.Assert(v2 != null);
            int[] vc1 = v1.ComparableComponents;
            int[] vc2 = v2.ComparableComponents;
            for (int i = 0; i < 4; i++) {
                if (vc1[i] != vc2[i])
                    return vc1[i] >= vc2[i];
            }
            return true;
        }

        /// <summary>
        ///   Determines whether the first specified instance of <c>ProjectVersion</c> 
        ///   is less than the second specified instance of 
        ///   <c>ProjectVersion</c>.
        /// </summary>
        /// <param name="v1">
        ///   The first instance of <c>ProjectVersion</c>.
        /// </param>
        /// <param name="v2">
        ///   The second instance of <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///   <c>true</c> if v1 is less than v2; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(ProjectVersion v1, ProjectVersion v2) {
            Debug.Assert(v1 != null);
            Debug.Assert(v2 != null);
            int[] vc1 = v1.ComparableComponents;
            int[] vc2 = v2.ComparableComponents;
            for (int i = 0; i < 4; i++) {
                if (vc1[i] != vc2[i])
                    return vc1[i] < vc2[i];
            }
            return false;
        }

        /// <summary>
        ///    Determines whether the first specified instance of <c>ProjectVersion</c> 
        ///    is less than or equal to the second specified instance of 
        ///    <c>ProjectVersion</c>.
        /// </summary>
        /// <param name="v1">
        ///   The first instance of <c>ProjectVersion</c>.
        /// </param>
        /// <param name="v2">
        ///   The second instance of <c>ProjectVersion</c>.
        /// </param>
        /// <returns>
        ///   <c>true</c> if v1 is less than or equal to v2; 
        ///   otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <=(ProjectVersion v1, ProjectVersion v2) {
            Debug.Assert(v1 != null);
            Debug.Assert(v2 != null);
            int[] vc1 = v1.ComparableComponents;
            int[] vc2 = v2.ComparableComponents;
            for (int i = 0; i < 4; i++) {
                if (vc1[i] != vc2[i])
                    return vc1[i] <= vc2[i];
            }
            return true;
        }
        #endregion // Operators

    }
}