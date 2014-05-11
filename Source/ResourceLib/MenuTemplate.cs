using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// Standard menu template.
    /// </summary>
    public class MenuTemplate : MenuTemplateBase
    {
        private User32.MENUTEMPLATE _header = new User32.MENUTEMPLATE();
        private MenuTemplateItemCollection _menuItems = new MenuTemplateItemCollection();

        /// <summary>
        /// Menu items.
        /// </summary>
        public MenuTemplateItemCollection MenuItems
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
            _header = (User32.MENUTEMPLATE) Marshal.PtrToStructure(
                lpRes, typeof(User32.MENUTEMPLATE));
            
            IntPtr lpMenuItem = new IntPtr(lpRes.ToInt64() + Marshal.SizeOf(_header) + _header.wOffset);
            return _menuItems.Read(lpMenuItem);
        }

        /// <summary>
        /// Write menu template to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            w.Write(_header.wVersion);
            w.Write(_header.wOffset);
            ResourceUtil.Pad(w, _header.wOffset);
            _menuItems.Write(w);
        }

        /// <summary>
        /// String representation of the menu in the MENU format.
        /// </summary>
        /// <returns>String representation of the menu.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MENU");
            sb.Append(_menuItems.ToString());
            return sb.ToString();
        }
    }
}
