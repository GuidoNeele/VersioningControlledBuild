/*
 * Filename:    NumberingOptions.cs
 * Product:     Versioning Controlled Build
 * Solution:    BuildAutoIncrement
 * Project:     Shared
 * Description: Options used for the next version.
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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace BuildAutoIncrement {

    [Serializable]
    [Flags]
    public enum AssemblyVersionType {
        None                            = 0,
        AssemblyVersion                 = 1,
        AssemblyFileVersion             = 2,
        AssemblyInformationalVersion    = 4,
        All =  AssemblyVersion | AssemblyFileVersion | AssemblyInformationalVersion,
    }

    [Serializable]
    public enum IncrementScheme {
        IncrementMajorVersion,
        IncrementMinorVersion,
        IncrementBuild,
        IncrementRevision
    }

    [Serializable]
    public enum BatchCommandIncrementScheme {
        IncrementModifiedIndependently,
        IncrementAllIndependently,
        IncrementModifiedOnlyAndSynchronize,
        IncrementAllAndSynchronize
    }

    [Serializable]
    [Flags]
    public enum ResetBuildAndRevision {
        ToZero = 0,
        ToOne  = 1
    }

    [Serializable]
    public class NumberingOptions : ICloneable {

        public NumberingOptions() {
            m_saveFilesBeforeRunningAddinCommand        = true;
            m_defaultVersionType                        = AssemblyVersionType.AssemblyVersion;
            m_applyToAllTypes                           = false;
            m_synchronizeAllVersionTypes                = false;
            m_allowArbitraryInformationalVersion        = true;
            m_includeVCppResourceFiles                  = true;
            m_includeSetupProjects                      = true;
            m_generatePackageAndProductCode             = false;

            m_autoBuildAndRevisionNumbering             = false;
            m_incrementScheme                           = IncrementScheme.IncrementRevision;
            m_incrementBy                               = 1;
            m_batchCommandIncrementScheme               = BatchCommandIncrementScheme.IncrementModifiedIndependently;
            m_resetBuildOnMajorIncrement                = true;
            m_resetBuildOnMinorIncrement                = true;
            m_resetRevisionOnMajorIncrement             = true;
            m_resetRevisionOnMinorIncrement             = true;
            m_resetRevisionOnBuildIncrement             = true;
            m_resetBuildAndRevisionTo                   = ResetBuildAndRevision.ToZero;
            m_replaceAsteriskWithVersionComponents      = false;
        }

        public bool SaveModifiedFilesBeforeRunningAddinCommand {
            get { return m_saveFilesBeforeRunningAddinCommand; }
            set { m_saveFilesBeforeRunningAddinCommand = value; }
        }

        public AssemblyVersionType DefaultVersionType {
            get { return m_defaultVersionType; }
            set { m_defaultVersionType = value; }
        }

        public int IncrementBy {
            get { return m_incrementBy; }
            set { m_incrementBy = value; }
        }

        public bool AllowArbitraryInformationalVersion {
            get { return m_allowArbitraryInformationalVersion; }
            set { m_allowArbitraryInformationalVersion = value; }
        }

        public bool IncludeVCppResourceFiles {
            get { return m_includeVCppResourceFiles; }
            set { m_includeVCppResourceFiles = value; }
        }

        public bool IncludeSetupProjects {
            get { return m_includeSetupProjects; }
            set { m_includeSetupProjects = value; }
        }

        public bool GeneratePackageAndProductCodes {
            get { return m_generatePackageAndProductCode; }
            set { m_generatePackageAndProductCode = value; }
        }

        public bool ApplyToAllTypes {
            get { return m_applyToAllTypes; }
            set { m_applyToAllTypes = value; }
        }

        public bool SynchronizeAllVersionTypes {
            get { return m_synchronizeAllVersionTypes; }
            set { m_synchronizeAllVersionTypes = value; }
        }

        public IncrementScheme IncrementScheme {
            get { return m_incrementScheme; }
            set { m_incrementScheme = value; }
        }

        public BatchCommandIncrementScheme BatchCommandIncrementScheme {
            get { return m_batchCommandIncrementScheme; }
            set { m_batchCommandIncrementScheme = value; }
        }

        public bool UseDateTimeBasedBuildAndRevisionNumbering {
            get { return m_autoBuildAndRevisionNumbering; }
            set { m_autoBuildAndRevisionNumbering = value; }
        }

        public bool ResetBuildOnMajorIncrement {
            get { return m_resetBuildOnMajorIncrement; }
            set { m_resetBuildOnMajorIncrement = value; }
        }

        public bool ResetBuildOnMinorIncrement {
            get { return m_resetBuildOnMinorIncrement; }
            set { m_resetBuildOnMinorIncrement = value; }
        }

        public bool ResetRevisionOnMajorIncrement {
            get { return m_resetRevisionOnMajorIncrement; }
            set { m_resetRevisionOnMajorIncrement = value; }
        }

        public bool ResetRevisionOnMinorIncrement {
            get { return m_resetRevisionOnMinorIncrement; }
            set { m_resetRevisionOnMinorIncrement = value; }
        }

        public bool ResetRevisionOnBuildIncrement {
            get { return m_resetRevisionOnBuildIncrement; }
            set { m_resetRevisionOnBuildIncrement = value; }
        }

        public ResetBuildAndRevision ResetBuildAndRevisionTo {
            get { return m_resetBuildAndRevisionTo; }
            set { m_resetBuildAndRevisionTo = value; }
        }

        public bool ReplaceAsteriskWithVersionComponents {
            get { return m_replaceAsteriskWithVersionComponents; }
            set { m_replaceAsteriskWithVersionComponents = value; }
        }

        #region ICloneable implementation

        object ICloneable.Clone() {
            return Clone();
        }

        public NumberingOptions Clone() {
            return (NumberingOptions)this.MemberwiseClone();
        }

        #endregion // ICloneable implementation

        private bool                        m_saveFilesBeforeRunningAddinCommand;
        private AssemblyVersionType         m_defaultVersionType;
        private bool                        m_applyToAllTypes;
        private bool                        m_synchronizeAllVersionTypes;
        private bool                        m_allowArbitraryInformationalVersion;
        private bool                        m_includeVCppResourceFiles;
        private bool                        m_includeSetupProjects;
        private bool                        m_generatePackageAndProductCode;

        private IncrementScheme             m_incrementScheme;
        private int                         m_incrementBy;
        private bool                        m_autoBuildAndRevisionNumbering;
        private bool                        m_resetBuildOnMajorIncrement;
        private bool                        m_resetBuildOnMinorIncrement;
        private bool                        m_resetRevisionOnMajorIncrement;
        private bool                        m_resetRevisionOnMinorIncrement;
        private bool                        m_resetRevisionOnBuildIncrement;
        private bool                        m_replaceAsteriskWithVersionComponents;
        private ResetBuildAndRevision       m_resetBuildAndRevisionTo;

        private BatchCommandIncrementScheme m_batchCommandIncrementScheme;

    }
}