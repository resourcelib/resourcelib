using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// Extended menu template.
    /// </summary>
    public class MenuExTemplate : MenuTemplateBase
    {
        private User32.MENUEXTEMPLATE _header = new User32.MENUEXTEMPLATE();
        private MenuExTemplateItemCollection _menuItems = new MenuExTemplateItemCollection();

        /// <summary>
        /// Menu items.
        /// </summary>
        public MenuExTemplateItemCollection MenuItems
        {
            get
            {
                return _menuItems;
            }
            set
            {
                _menuItems = value;
            }
        }

        /// <summary>
        /// Read the menu template.
        /// </summary>
        /// <param name="lpRes">Address in memory.</param>
        internal override IntPtr Read(IntPtr lpRes)
        {
            _header = (User32.MENUEXTEMPLATE) Marshal.PtrToStructure(
                lpRes, typeof(User32.MENUEXTEMPLATE));

            IntPtr lpMenuItem = ResourceUtil.Align(lpRes.ToInt32() 
                + Marshal.SizeOf(_header) 
                + _header.wOffset);

            return _menuItems.Read(lpMenuItem);
        }

        /// <summary>
        /// Write the menu template.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(System.IO.BinaryWriter w)
        {
            long head = w.BaseStream.Position;
            // write header
            w.Write(_header.wVersion);
            w.Write(_header.wOffset);
            // w.Write(_header.dwHelpId);
            // pad to match the offset value
            ResourceUtil.Pad(w, (UInt16) (_header.wOffset - 4));
            // seek to the beginning of the menu item per offset value
            // this may be behind, ie. the help id structure is part of the first popup menu
            w.BaseStream.Seek(head + _header.wOffset + 4, System.IO.SeekOrigin.Begin);
            // write menu items
            _menuItems.Write(w);
        }

        /// <summary>
        /// String representation of the menu in the MENUEX format.
        /// </summary>
        /// <returns>String representation of the menu.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MENUEX");
            sb.Append(_menuItems.ToString());
            return sb.ToString();
        }
    }
}
