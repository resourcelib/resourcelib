using System;
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
        public override IntPtr Read(IntPtr lpRes)
        {
            _header = (User32.MENUTEMPLATE) Marshal.PtrToStructure(
                lpRes, typeof(User32.MENUTEMPLATE));
            
            IntPtr lpMenuItem = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header) + _header.wOffset);
            return _menuItems.Read(lpMenuItem);
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
