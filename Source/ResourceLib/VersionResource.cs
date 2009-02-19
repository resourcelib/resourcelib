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
        private Dictionary<string, StringTableResource> _stringtableresources = null;

        public Dictionary<string, StringTableResource> StringTableResources
        {
            get
            {
                return _stringtableresources;
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

        /// <summary>
        /// Load a version resource, heavily inspired from http://www.codeproject.com/KB/dotnet/FastFileVersion.aspx
        /// </summary>
        /// <param name="lpRes"></param>
        public void Load(IntPtr lpRes)
        {
            _stringtableresources = new Dictionary<string, StringTableResource>();

            _versioninfo = (Kernel32.VS_VERSIONINFO)Marshal.PtrToStructure(lpRes, typeof(Kernel32.VS_VERSIONINFO));

            IntPtr pFI = ResourceUtil.Align(lpRes.ToInt32() + Kernel32.VS_VERSIONINFO.PaddingOffset + 1);

            _fixedfileinfo = (Kernel32.VS_FIXEDFILEINFO) Marshal.PtrToStructure(
                pFI, typeof(Kernel32.VS_FIXEDFILEINFO));

            IntPtr pChild = ResourceUtil.Align(pFI.ToInt32() + _versioninfo.wValueLength);
            Kernel32.VS_VERSIONINFO pChildInfo = (Kernel32.VS_VERSIONINFO) Marshal.PtrToStructure(
                pChild, typeof(Kernel32.VS_VERSIONINFO));

            while (pChild.ToInt32() < (lpRes.ToInt32() + _versioninfo.wLength))
            {
                switch (pChildInfo.szKey)
                {
                    case "StringFileInfo":
                        StringTableResource s = new StringTableResource(
                            pChild, pChildInfo.wType, pChildInfo.szKey, Language, pChildInfo.wLength);
                        _stringtableresources.Add(pChildInfo.szKey, s);
                        break;
                    case "VarFileInfo":
                        // \todo: implement this
                        break;
                }

                pChild = ResourceUtil.Align(pChild.ToInt32() + pChildInfo.wLength);
                
                pChildInfo = (Kernel32.VS_VERSIONINFO)Marshal.PtrToStructure(
                    pChild, typeof(Kernel32.VS_VERSIONINFO));
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
