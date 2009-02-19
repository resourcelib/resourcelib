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

        /// <summary>
        /// A resource header.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct RESOURCE_HEADER
        {
            public UInt16 wLength;
            public UInt16 wValueLength;
            public UInt16 wType;
        };

        /// <summary>
        /// Language and code page combinations.
        /// The low-order word of each DWORD must contain a Microsoft language identifier, 
        /// and the high-order word must contain the IBM code page number. 
        /// Either high-order or low-order word can be zero, indicating that the file is language 
        /// or code page independent.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct VAR_HEADER
        {
            public UInt16 wLanguageIDMS;
            public UInt16 wCodePageIBM;
        };

        /// <summary>
        /// This structure contains version information about a file. 
        /// This information is language- and code page–independent.
        /// http://msdn.microsoft.com/en-us/library/ms647001.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
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

        /// <summary>
        /// A group icon resource header.
        /// http://msdn.microsoft.com/en-us/library/ms997538.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct GRPICONDIR
        {
            public UInt16 wReserved; // reserved
            public UInt16 wType; // type, 1 = icon, 2 = cursor
            public UInt16 wImageCount; // image count
        };

        /// <summary>
        /// An icon directory entry
        /// http://msdn.microsoft.com/en-us/library/ms997538.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct GRPICONDIRENTRY
        {
            public Byte bWidth; // width
            public Byte bHeight; // height
            public Byte bColors; // 0 means 256 or more
            public Byte bReserved; // reserved
            public UInt16 wPlanes; // number of planes
            public UInt16 wBitsPerPixel; // bits per pixel
            public UInt32 dwImageSize; // size of image data
            public UInt16 nID; // icon ID
        };

        /// <summary>
        /// Icon group directory entry in an .ico file
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct FILEGRPICONDIRENTRY
        {
            public Byte bWidth;
            public Byte bHeight;
            public Byte bColors;
            public Byte bReserved;
            public UInt16 wPlanes;
            public UInt16 wBitsPerPixel;
            public UInt32 dwImageSize;
            public UInt32 dwFileOffset;
        };

        /// <summary>
        /// Icon group structure in an .ico file
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct FILEGRPICONDIR
        {
            public UInt16 wReserved;
            public UInt16 wType;
            public UInt16 wCount;
        };

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        public enum ResourceTypes
        {
            RT_CURSOR = 1,
            RT_BITMAP = 2,
            RT_ICON = 3,
            RT_MENU = 4,
            RT_DIALOG = 5,
            RT_STRING = 6,
            RT_FONTDIR = 7,
            RT_FONT = 8,
            RT_ACCELERATOR = 9,
            RT_RCDATA = 10,
            RT_MESSAGETABLE = 11,
            RT_GROUP_CURSOR = 12,
            RT_GROUP_ICON = 14,
            RT_VERSION = 16,
            RT_DLGINCLUDE = 17,
            RT_PLUGPLAY = 19,
            RT_VXD = 20,
            RT_ANICURSOR = 21,
            RT_ANIICON = 22,
            RT_HTML = 23,
        };

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
        public static extern IntPtr FindResourceEx(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, ushort wLanguage);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int SizeofResource(IntPtr hInstance, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll", EntryPoint = "BeginUpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

        [DllImport("kernel32.dll", EntryPoint = "UpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, IntPtr lpName, UInt16 wLanguage, byte[] lpData, UInt32 cbData);

        [DllImport("kernel32.dll", EntryPoint = "EndUpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        public const ushort LANG_NEUTRAL = 0;
        public const ushort LANG_ENGLISH = 9;

        public const ushort SUBLANG_NEUTRAL = 0;
        public const ushort SUBLANG_ENGLISH_US = 1;
    }
}
