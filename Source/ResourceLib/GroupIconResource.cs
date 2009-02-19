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

        Kernel32.GRPICONDIR _header = new Kernel32.GRPICONDIR();
        List<IconResource> _icons = new List<IconResource>();

        /// <summary>
        /// Type of the group icon resource.
        /// </summary>
        public GroupType GroupIconResourceType
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
            : base(IntPtr.Zero, 
                IntPtr.Zero, 
                new IntPtr((uint)Kernel32.ResourceTypes.RT_GROUP_ICON), 
                Marshal.StringToHGlobalUni("#1"), 1033, 
                Marshal.SizeOf(typeof(Kernel32.GRPICONDIR)))
        {
            GroupIconResourceType = GroupType.Icon;
        }

        /// <summary>
        /// Load from an executable file
        /// </summary>
        /// <param name="filename">an executable file (.exe or .dll)</param>
        public void LoadFrom(string filename)
        {
            base.LoadFrom(filename, Marshal.StringToHGlobalUni("#1"), 
                new IntPtr((uint) Kernel32.ResourceTypes.RT_GROUP_ICON));
        }

        public void SaveTo(string filename)
        {
            base.SaveTo(filename, 1, (uint) Kernel32.ResourceTypes.RT_GROUP_ICON, 1033);

            foreach (IconResource icon in _icons)
            {
                icon.SaveIconTo(filename);
            }
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
            w.Write((UInt16) _header.wReserved);
            w.Write((UInt16) _header.wType);
            w.Write((UInt16) _icons.Count);
            ResourceUtil.PadToWORD(w);
            foreach(IconResource icon in _icons)
            {
                icon.Write(w);
            }
        }
    }
}
