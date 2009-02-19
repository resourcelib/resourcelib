using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a .ico file.
    /// </summary>
    public class IconFile
    {
        public enum GroupType
        {
            Icon = 1,
            Cursor = 2
        };

        Kernel32.FILEGRPICONDIR _header = new Kernel32.FILEGRPICONDIR();
        List<IconFileIcon> _icons = new List<IconFileIcon>();

        /// <summary>
        /// Type of the group icon resource.
        /// </summary>
        public GroupType Type
        {
            get
            {
                return (GroupType)_header.wType;
            }
            set
            {
                _header.wType = (byte)value;
            }
        }

        public List<IconFileIcon> Icons
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

        public IconFile(string filename)
        {
            LoadFrom(filename);
        }

        /// <summary>
        /// Load from a .ico file
        /// </summary>
        /// <param name="filename">an icon file (.ico)</param>
        public void LoadFrom(string filename)
        {
            byte[] _data = File.ReadAllBytes(filename);

            IntPtr lpData = Marshal.AllocHGlobal(_data.Length);
            try
            {
                Marshal.Copy(_data, 0, lpData, _data.Length);
                Read(lpData);
            }
            finally
            {
                Marshal.FreeHGlobal(lpData);
            }
        }

        public IntPtr Read(IntPtr lpData)
        {
            _icons.Clear();

            _header = (Kernel32.FILEGRPICONDIR)Marshal.PtrToStructure(
                lpData, typeof(Kernel32.FILEGRPICONDIR));

            IntPtr lpEntry = new IntPtr(lpData.ToInt32() + Marshal.SizeOf(_header));

            for (int i = 0; i < _header.wCount; i++)
            {
                IconFileIcon iconFileIcon = new IconFileIcon();
                lpEntry = iconFileIcon.Read(lpEntry, lpData);
                _icons.Add(iconFileIcon);
            }

            return lpEntry;
        }

        public GroupIconResource ConvertToGroupIconResource()
        {
            GroupIconResource groupIconResource = new GroupIconResource();
            switch (Type)
            {
                case GroupType.Icon:
                    groupIconResource.Type = GroupIconResource.GroupType.Icon;
                    break;
                case GroupType.Cursor:
                    groupIconResource.Type = GroupIconResource.GroupType.Cursor;
                    break;
            }

            for (int id = 0; id < Icons.Count; id++)
            {
                IconResource iconResource = Icons[id].ConvertToIconResource((UInt16) id);
                groupIconResource.Icons.Add(iconResource);
            }

            return groupIconResource;
        }
    }
}
