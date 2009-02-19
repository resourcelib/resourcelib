using System;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    public abstract class Kernel32
    {
        public const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        public const uint DONT_RESOLVE_DLL_REFERENCES = 0x00000001;
        public const uint LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;
        public const uint LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct VS_VERSIONINFO
        {
            public UInt16 wLength;
            public UInt16 wValueLength;
            public UInt16 wType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
            public string szKey;
            public UInt16 Padding1;

            public static Int32 PaddingOffset
            {
                get
                {
                    return Marshal.OffsetOf(typeof(Kernel32.VS_VERSIONINFO), "Padding1").ToInt32();
                }
            }
        };

        /// <summary>
        /// This structure depicts the organization of data in a file-version resource. 
        /// http://msdn.microsoft.com/en-us/library/aa909192.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STRING_TABLE_INFO_HEADER
        {
            public UInt16 wLength;
            public UInt16 wValueLength;
            public UInt16 wType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string szKey;
            public UInt16 Padding1;

            public static Int32 PaddingOffset
            {
                get
                {
                    return Marshal.OffsetOf(typeof(Kernel32.STRING_TABLE_INFO_HEADER), "Padding1").ToInt32();
                }
            }
        };
        
        /// <summary>
        /// This structure depicts the organization of data in a file-version resource. It contains a string that 
        /// describes a specific aspect of a file, such as a file's version, its copyright notices, or its trademarks.
        /// http://msdn.microsoft.com/en-us/library/aa909025.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STRING_INFO_HEADER
        {
            public UInt16 wLength;
            public UInt16 wValueLength;
            public UInt16 wType;
        };

        /// <summary>
        /// This structure contains version information about a file. This information is language- and code page–independent.
        /// http://msdn.microsoft.com/en-us/library/aa909176.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct VS_FIXEDFILEINFO
        {
            public UInt32 dwSignature;
            public UInt32 dwStrucVersion;
            public UInt32 dwFileVersionMS;
            public UInt32 dwFileVersionLS;
            public UInt32 dwProductVersionMS;
            public UInt32 dwProductVersionLS;
            public UInt32 dwFileFlagsMask;
            public UInt32 dwFileFlags;
            public UInt32 dwFileOS;
            public UInt32 dwFileType;
            public UInt32 dwFileSubtype;
            public UInt32 dwFileDateMS;
            public UInt32 dwFileDateLS;
        };

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        // from http://blog.kalmbachnet.de/?postid=26

        public const uint RT_CURSOR = 0x00000001;
        public const uint RT_BITMAP = 0x00000002;
        public const uint RT_ICON = 0x00000003;
        public const uint RT_MENU = 0x00000004;
        public const uint RT_DIALOG = 0x00000005;
        public const uint RT_STRING = 0x00000006;
        public const uint RT_FONTDIR = 0x00000007;
        public const uint RT_FONT = 0x00000008;
        public const uint RT_ACCELERATOR = 0x00000009;
        public const uint RT_RCDATA = 0x00000010;
        public const uint RT_MESSAGETABLE = 0x00000011;

        [DllImport("kernel32.dll", EntryPoint = "EnumResourceTypesW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool EnumResourceTypes(IntPtr hModule, EnumResourceTypesDelegate lpEnumFunc, IntPtr lParam);

        public delegate bool EnumResourceTypesDelegate(IntPtr hModule, IntPtr lpszType, IntPtr lParam);

        [DllImport("kernel32.dll", EntryPoint = "EnumResourceNamesW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool EnumResourceNames(IntPtr hModule, IntPtr lpszType, EnumResourceNamesDelegate lpEnumFunc, IntPtr lParam);

        public delegate bool EnumResourceNamesDelegate(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam);

        [DllImport("kernel32.dll", EntryPoint = "EnumResourceLanguagesW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool EnumResourceLanguages(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, EnumResourceLanguagesDelegate lpEnumFunc, IntPtr lParam);

        public delegate bool EnumResourceLanguagesDelegate(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, ushort wIDLanguage, IntPtr lParam);

        [DllImport("kernel32.dll", EntryPoint = "FindResource", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindResource(IntPtr hModule, IntPtr lpszName, IntPtr lpszType);

        [DllImport("kernel32.dll", EntryPoint = "FindResourceExW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindResourceEx(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, long wLanguage);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResData);

        [DllImport("kernel32", SetLastError = true)]
        public static extern int SizeofResource(IntPtr hInstance, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hHandle);
    }
}
