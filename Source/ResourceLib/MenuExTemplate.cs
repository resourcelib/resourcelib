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
        public override IntPtr Read(IntPtr lpRes)
        {
            _header = (User32.MENUEXTEMPLATE) Marshal.PtrToStructure(
                lpRes, typeof(User32.MENUEXTEMPLATE));

            IntPtr lpMenuItem = new IntPtr(lpRes.ToInt32() 
                + _header.wOffset // offset from offset field
                + 4 // offset of the offset field
                );

            return _menuItems.Read(lpMenuItem);
        }


        /// <summary>
        /// String representation of the menu in the MENUEX format.
        /// </summary>
        /// <returns>String representation of the menu.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("MENUEX {0}", _header.dwHelpId));
            sb.Append(_menuItems.ToString());
            return sb.ToString();
        }
    }
}
