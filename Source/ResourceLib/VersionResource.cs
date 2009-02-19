using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// VS_VERSIONINFO
    /// This structure depicts the organization of data in a file-version resource. It is the root structure 
    /// that contains all other file-version information structures.
    /// http://msdn.microsoft.com/en-us/library/aa914916.aspx
    /// </summary>
    public class VersionResource : Resource
    {
        ResourceTable _header = new ResourceTable();
        Kernel32.VS_FIXEDFILEINFO _fixedfileinfo;
        private Dictionary<string, ResourceTable> _resources = null;

        public ResourceTable Header
        {
            get
            {
                return _header;
            }
        }

        public Dictionary<string, ResourceTable> Resources
        {
            get
            {
                return _resources;
            }
        }

        /// <summary>
        /// A version resource
        /// </summary>
        public VersionResource(IntPtr hModule, IntPtr hResource, IntPtr type, IntPtr name, ushort wIDLanguage, int size)
            : base(hModule, hResource, type, name, wIDLanguage, size)
        {
            IntPtr lpRes = Kernel32.LockResource(hResource);

            if (lpRes == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Read(hModule, lpRes);
        }

        public VersionResource()
        {

        }

        public void LoadFrom(string filename)
        {
            base.LoadFrom(filename, Marshal.StringToHGlobalUni("#1"), new IntPtr(16));
        }

        public static byte[] LoadBytesFrom(string filename)
        {
            return Resource.LoadBytesFrom(filename, Marshal.StringToHGlobalUni("#1"), new IntPtr(16));
        }

        public override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            _resources = new Dictionary<string, ResourceTable>();
            IntPtr pFixedFileInfo = _header.Read(lpRes);

            _fixedfileinfo = (Kernel32.VS_FIXEDFILEINFO)Marshal.PtrToStructure(
                pFixedFileInfo, typeof(Kernel32.VS_FIXEDFILEINFO));

            IntPtr pChild = ResourceUtil.Align(pFixedFileInfo.ToInt32() + _header.Header.wValueLength);

            while (pChild.ToInt32() < (lpRes.ToInt32() + _header.Header.wLength))
            {
                ResourceTable rc = new ResourceTable(pChild);
                switch (rc.Key)
                {
                    case "StringFileInfo":
                        rc = new StringFileInfo(pChild);
                        break;
                    default:
                        rc = new VarFileInfo(pChild);
                        break;
                }

                _resources.Add(rc.Key, rc);
                pChild = ResourceUtil.Align(pChild.ToInt32() + rc.Header.wLength);
            }

            return new IntPtr(lpRes.ToInt32() + _header.Header.wLength);
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
            set
            {
                UInt32 major = 0, minor = 0, build = 0, release = 0;
                string[] version_s = value.Split(".".ToCharArray(), 4);
                if (version_s.Length >= 1) major = UInt32.Parse(version_s[0]);
                if (version_s.Length >= 2) minor = UInt32.Parse(version_s[1]);
                if (version_s.Length >= 3) build = UInt32.Parse(version_s[2]);
                if (version_s.Length >= 4) release = UInt32.Parse(version_s[3]);
                _fixedfileinfo.dwFileVersionMS = (major << 16) + minor;
                _fixedfileinfo.dwFileVersionLS = (build << 16) + release;
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
            set
            {
                UInt32 major = 0, minor = 0, build = 0, release = 0;
                string[] version_s = value.Split(".".ToCharArray(), 4);
                if (version_s.Length >= 1) major = UInt32.Parse(version_s[0]);
                if (version_s.Length >= 2) minor = UInt32.Parse(version_s[1]);
                if (version_s.Length >= 3) build = UInt32.Parse(version_s[2]);
                if (version_s.Length >= 4) release = UInt32.Parse(version_s[3]);
                _fixedfileinfo.dwProductVersionMS = (major << 16) + minor;
                _fixedfileinfo.dwProductVersionLS = (build << 16) + release;
            }
        }

        public override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            _header.Write(w);
            
            w.Write(ResourceUtil.GetBytes<Kernel32.VS_FIXEDFILEINFO>(_fixedfileinfo));
            ResourceUtil.PadToDWORD(w);

            Dictionary<string, ResourceTable>.Enumerator resourceEnum = _resources.GetEnumerator();
            while (resourceEnum.MoveNext())
            {
                resourceEnum.Current.Value.Write(w);
            }

            ResourceUtil.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
        }

        public static void SaveTo(string filename, byte[] data)
        {
            Resource.SaveTo(filename, 1, (uint) Kernel32.ResourceTypes.RT_VERSION, data);
        }

        public void SaveTo(string filename)
        {
            base.SaveTo(filename, 1, (uint) Kernel32.ResourceTypes.RT_VERSION);
        }

        public ResourceTable this[string key]
        {
            get
            {
                return Resources[key];
            }
            set
            {
                Resources[key] = value;
            }
        }
    }
}
