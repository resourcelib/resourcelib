using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a hardware-independent icon resource.
    /// </summary>
    public class GroupIconResource : Resource
    {
        /// <summary>
        /// Resource type.
        /// </summary>
        public enum GroupType
        {
            /// <summary>
            /// Icon.
            /// </summary>
            Icon = 1,
            /// <summary>
            /// Cursor.
            /// </summary>
            Cursor = 2
        };

        Kernel32.GRPICONDIR _header = new Kernel32.GRPICONDIR();
        List<IconResource> _icons = new List<IconResource>();

        /// <summary>
        /// Type of the hardware-independent icon resource.
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

        /// <summary>
        /// Icons contained in this hardware-independent icon resource.
        /// </summary>
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
        /// A hardware-independent icon resource.
        /// </summary>
        internal GroupIconResource(IntPtr hModule, IntPtr hResource, IntPtr type, IntPtr name, UInt16 wIDLanguage, int size)
            : base(hModule, hResource, type, name, wIDLanguage, size)
        {
            IntPtr lpRes = Kernel32.LockResource(hResource);

            if (lpRes == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Read(hModule, lpRes);
        }

        /// <summary>
        /// A new hardware-independent icon resource.
        /// </summary>
        public GroupIconResource()
            : base(IntPtr.Zero, 
                IntPtr.Zero, 
                new IntPtr((uint) Kernel32.ResourceTypes.RT_GROUP_ICON),
                new IntPtr(1), 
                1033, 
                Marshal.SizeOf(typeof(Kernel32.GRPICONDIR)))
        {
            GroupIconResourceType = GroupType.Icon;
        }

        /// <summary>
        /// Load a hardware-independent icon resource from an executable file.
        /// </summary>
        /// <param name="filename">Name of an executable file (.exe or .dll).</param>
        public void LoadFrom(string filename)
        {
            base.LoadFrom(filename, new IntPtr(1), 
                new IntPtr((uint) Kernel32.ResourceTypes.RT_GROUP_ICON),
                Kernel32.LANG_NEUTRAL);
        }

        /// <summary>
        /// Save a hardware-independent icon resource to an executable file.
        /// </summary>
        /// <param name="filename">Name of an executable file (.exe or .dll).</param>
        public void SaveTo(string filename)
        {
            base.SaveTo(filename, new IntPtr(int.Parse(Name)), 
                new IntPtr((uint) Kernel32.ResourceTypes.RT_GROUP_ICON), Language);

            foreach (IconResource icon in _icons)
            {
                icon.SaveIconTo(filename);
            }
        }

        /// <summary>
        /// Read a hardware-independent icon resource from a loaded module.
        /// </summary>
        /// <param name="hModule">Loaded executable module.</param>
        /// <param name="lpRes">Pointer to the beginning of a hardware-independent icon resource.</param>
        /// <returns>Pointer to the end of the hardware-independent icon resource.</returns>
        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
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

        /// <summary>
        /// Write a hardware-independent icon resource to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
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
