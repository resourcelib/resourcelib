using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A version resource, RT_RCDATA
    /// </summary>
    public class VersionResource : Resource
    {
        Kernel32.VS_VERSIONINFO _versioninfo;
        Kernel32.VS_FIXEDFILEINFO _fixedfileinfo;
        private Dictionary<string, Resource> _resources = null;

        public Dictionary<string, Resource> Resources
        {
            get
            {
                return _resources;
            }
        }

        /// <summary>
        /// A version resource
        /// </summary>
        public VersionResource(IntPtr hResource, IntPtr type, IntPtr name, ushort wIDLanguage, int size)
            : base(hResource, type, name, wIDLanguage, size)
        {
            IntPtr lpRes = Kernel32.LockResource(hResource);

            if (lpRes == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Load(lpRes);
        }

        public VersionResource(string filename)
        {
            IntPtr hModule = IntPtr.Zero;

            try
            {
                // load DLL
                hModule = Kernel32.LoadLibraryEx(filename, IntPtr.Zero,
                    Kernel32.DONT_RESOLVE_DLL_REFERENCES | Kernel32.LOAD_LIBRARY_AS_DATAFILE);

                if (IntPtr.Zero == hModule)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                IntPtr hRes = Kernel32.FindResource(hModule, Marshal.StringToHGlobalUni("#1"), new IntPtr(Kernel32.RT_RCDATA));
                if (IntPtr.Zero == hRes)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                IntPtr hGlobal = Kernel32.LoadResource(hModule, hRes);
                if (IntPtr.Zero == hGlobal)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                IntPtr lpRes = Kernel32.LockResource(hGlobal);

                if (lpRes == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                _size = Kernel32.SizeofResource(hModule, hRes);
                if (_size <= 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                Load(lpRes);
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                    Kernel32.FreeLibrary(hModule);
            }
        }

        /// <summary>
        /// Load a version resource, heavily inspired from http://www.codeproject.com/KB/dotnet/FastFileVersion.aspx
        /// </summary>
        /// <param name="lpRes"></param>
        public void Load(IntPtr lpRes)
        {
            _resources = new Dictionary<string, Resource>();
            _versioninfo = (Kernel32.VS_VERSIONINFO)Marshal.PtrToStructure(lpRes, typeof(Kernel32.VS_VERSIONINFO));

            IntPtr pFI = ResourceUtil.Align(lpRes.ToInt32() + Kernel32.VS_VERSIONINFO.PaddingOffset + 1);

            _fixedfileinfo = (Kernel32.VS_FIXEDFILEINFO) Marshal.PtrToStructure(
                pFI, typeof(Kernel32.VS_FIXEDFILEINFO));

            IntPtr pChild = ResourceUtil.Align(pFI.ToInt32() + _versioninfo.wValueLength);

            Kernel32.STRING_OR_VAR_INFO_HEADER pChildInfo = (Kernel32.STRING_OR_VAR_INFO_HEADER) Marshal.PtrToStructure(
                pChild, typeof(Kernel32.STRING_OR_VAR_INFO_HEADER));

            while (pChild.ToInt32() < (lpRes.ToInt32() + _versioninfo.wLength))
            {
                Resource rc = null;
                IntPtr pKey = new IntPtr(pChild.ToInt32() + Marshal.SizeOf(pChildInfo));
                string key = Marshal.PtrToStringUni(pKey);
                IntPtr pData = ResourceUtil.Align(pKey.ToInt32() + (key.Length + 1) * 2);
                switch (key)
                {
                    case "StringFileInfo":
                        rc = new StringTableResource(pData, pChildInfo.wType, key, Language, pChildInfo.wLength);
                        break;
                    default:
                        rc = new VariableTableResource(pData, pChildInfo.wType, key, Language, pChildInfo.wLength);
                        break;
                }

                _resources.Add(key, rc);

                pChild = ResourceUtil.Align(pChild.ToInt32() + pChildInfo.wLength);

                pChildInfo = (Kernel32.STRING_OR_VAR_INFO_HEADER)Marshal.PtrToStructure(
                    pChild, typeof(Kernel32.STRING_OR_VAR_INFO_HEADER));
            }
        }

        /// <summary>
        /// File version
        /// </summary>
        public string FileVersion
        {
            get
            {
                return string.Format("{0}.{1}.{2}.{3}",
                    (_fixedfileinfo.dwFileVersionMS & 0xffff0000) >> 16,
                    _fixedfileinfo.dwFileVersionMS & 0x0000ffff,
                    (_fixedfileinfo.dwFileVersionLS & 0xffff0000) >> 16,
                    _fixedfileinfo.dwFileVersionLS & 0x0000ffff);
            }
        }

        /// <summary>
        /// Product binary version
        /// </summary>
        public string ProductVersion
        {
            get
            {
                return string.Format("{0}.{1}.{2}.{3}",
                    (_fixedfileinfo.dwProductVersionMS & 0xffff0000) >> 16,
                    _fixedfileinfo.dwProductVersionMS & 0x0000ffff,
                    (_fixedfileinfo.dwProductVersionLS & 0xffff0000) >> 16,
                    _fixedfileinfo.dwProductVersionLS & 0x0000ffff);
            }
        }
    }
}
