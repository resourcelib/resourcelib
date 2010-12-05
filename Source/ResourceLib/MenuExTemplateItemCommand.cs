using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A command menu item.
    /// </summary>
    public class MenuExTemplateItemCommand : MenuExTemplateItem
    {
        /// <summary>
        /// A command menu item.
        /// </summary>
        public MenuExTemplateItemCommand()
        {

        }

        /// <summary>
        /// Returns true if the item is a separator.
        /// </summary>
        public bool IsSeparator
        {
            get
            {
                return _header.dwType == (uint)User32.MenuFlags.MFT_SEPARATOR
                    || ((_header.bResInfo == 0xFFFF || _header.bResInfo == 0) 
                        && _header.dwMenuId == 0 && _menuString == null);
            }
        }

        /// <summary>
        /// String representation in the MENU format.
        /// </summary>
        /// <param name="indent">Indent.</param>
        /// <returns>String representation.</returns>
        public override string ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            if (IsSeparator)
            {
                sb.AppendLine(string.Format("{0}MENUITEM SEPARATOR",
                    new String(' ', indent)));
            }
            else
            {
                sb.AppendLine(string.Format("{0}MENUITEM \"{1}\", {2}",
                    new String(' ', indent), _menuString == null 
                        ? string.Empty : _menuString.Replace("\t", @"\t"), _header.dwMenuId));
            }
            return sb.ToString();
        }
    }
}
