using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace BuildAutoIncrement
{
    public class WoW64RegistryKey : IDisposable
    {
        #region kernel32.dll

        [DllImport("kernel32.dll")]
        private static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        #endregion // kernel32.dll

        #region advapi32.dll

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RegOpenKeyEx(UIntPtr hKey, string subKey, int ulOptions, int samDesired, out UIntPtr hkResult);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
        private static extern int RegQueryValueEx(UIntPtr hKey, string lpValueName, int lpReserved, out uint lpType, System.Text.StringBuilder lpData, ref int lpcbData);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegCloseKey(UIntPtr hKey);

        #endregion // advapi32.dll

        #region Constants

        public static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);
        public static UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001u);

        private enum ProcessorArchitecture : ushort
        {
            Intel = 0,
            IA64 = 6,
            AMD64 = 9,
            Unknown = 0xFFFF
        }

        private enum KeyWow64 : int
        {
            WOW64_64KEY = 0x0100,
            WOW64_32KEY = 0x0200
        }

        private enum RegistryAccess : int
        {
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
            SYNCHRONIZE = 0x00100000,

            STANDARD_RIGHTS_REQUIRED = 0x000F0000,

            STANDARD_RIGHTS_READ = READ_CONTROL,
            STANDARD_RIGHTS_WRITE = READ_CONTROL,
            STANDARD_RIGHTS_EXECUTE = READ_CONTROL,

            STANDARD_RIGHTS_ALL = 0x001F0000,
            SPECIFIC_RIGHTS_ALL = 0x0000FFFF,

            KEY_QUERY_VALUE = 0x0001,
            KEY_SET_VALUE = 0x0002,
            KEY_CREATE_SUB_KEY = 0x0004,
            KEY_ENUMERATE_SUB_KEYS = 0x0008,
            KEY_NOTIFY = 0x0010,
            KEY_CREATE_LINK = 0x0020,

            KEY_READ = ((STANDARD_RIGHTS_READ | KEY_QUERY_VALUE | KEY_ENUMERATE_SUB_KEYS | KEY_NOTIFY) & (~SYNCHRONIZE)),
            KEY_WRITE = ((STANDARD_RIGHTS_WRITE | KEY_SET_VALUE | KEY_CREATE_SUB_KEY) & (~SYNCHRONIZE)),
            KEY_EXECUTE = ((KEY_READ) & (~SYNCHRONIZE)),
            KEY_ALL_ACCESS = ((STANDARD_RIGHTS_ALL | KEY_QUERY_VALUE | KEY_SET_VALUE | KEY_CREATE_SUB_KEY | KEY_ENUMERATE_SUB_KEYS | KEY_NOTIFY | KEY_CREATE_LINK) & (~SYNCHRONIZE)),
        }

        #endregion // Constants

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public UIntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        };

        #endregion // Structures

        ~WoW64RegistryKey() {
            Dispose(false);
        }

        #region IDisposable Members

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!m_disposed) {
                Close();
                m_disposed = true;
            }
        }

        #endregion

        public void Open(UIntPtr rootKey, string keyPath) {
            Debug.Assert(rootKey == HKEY_LOCAL_MACHINE || rootKey == HKEY_CURRENT_USER);

            int samDesired = (int)(RegistryAccess.KEY_READ);
            if (IsWin64())
                samDesired |= (int)(KeyWow64.WOW64_32KEY);

            int errorCode = RegOpenKeyEx(rootKey, keyPath, 0, samDesired, out m_hKey);
            if (errorCode != 0)
                throw new Win32Exception(errorCode);
        }

        public string ReadString(string valueName)
        {
            int size = 0;
            uint type;
            StringBuilder keyBuffer = null;
            RegQueryValueEx(m_hKey, valueName, 0, out type, keyBuffer, ref size);

            keyBuffer = new StringBuilder(size);
            int errorCode = RegQueryValueEx(m_hKey, valueName, 0, out type, keyBuffer, ref size);
            if (errorCode != 0)
                throw new Win32Exception(errorCode);
            return keyBuffer.ToString();
        }

        public void Close() {
            RegCloseKey(m_hKey);
        }

        private bool IsWin64()
        {
            // all versions up to Win2000
            if (System.Environment.OSVersion.Version.Major < 5 || (System.Environment.OSVersion.Version.Major == 5 && System.Environment.OSVersion.Version.Minor == 0))
                return false;

            SYSTEM_INFO sysInfo = new SYSTEM_INFO();
            GetNativeSystemInfo(ref sysInfo);

            switch ((ProcessorArchitecture)sysInfo.wProcessorArchitecture)
            {
                case ProcessorArchitecture.AMD64:
                case ProcessorArchitecture.IA64:
                    return true;
                case ProcessorArchitecture.Intel:
                    return false;
            }
            return false;
        }


        private UIntPtr m_hKey = UIntPtr.Zero;
        private bool m_disposed = false;

    }
}
