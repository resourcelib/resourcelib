using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a group icon resource.
    /// </summary>
    public class GroupIconResource : Resource
    {
        public enum GroupType
        {
            Icon = 1,
            Cursor = 2
        };

        Kernel32.GRPICONDIR _header;
        List<IconResource> _icons = new List<IconResource>();

        /// <summary>
        /// Type of the group icon resource.
        /// </summary>
        public GroupType Type
        {
            get
            {
                return (GroupType) _header.wType;
            }
            set
            {
                _header.wType = (byte) value;
            }
        }

        public List<IconResource> Icons
        {
            get
            {
                return _icons;
            }
            set
            {
                _icons = value;
            }
        }

        /// <summary>
        /// An icon resource
        /// </summary>
        public GroupIconResource(IntPtr hModule, IntPtr hResource, IntPtr type, IntPtr name, ushort wIDLanguage, int size)
            : base(hModule, hResource, type, name, wIDLanguage, size)
        {
            IntPtr lpRes = Kernel32.LockResource(hResource);

            if (lpRes == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Read(hModule, lpRes);
        }

        public GroupIconResource()
        {

        }

        public override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            _icons.Clear();

            _header = (Kernel32.GRPICONDIR) Marshal.PtrToStructure(
                lpRes, typeof(Kernel32.GRPICONDIR));

            IntPtr pEntry = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header));

            for (int i = 0; i < _header.wImageCount; i++)
            {
                IconResource iconResource = new IconResource();
                pEntry = iconResource.Read(hModule, pEntry);
                _icons.Add(iconResource);
            }

            return pEntry;
        }

        public override void Write(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
