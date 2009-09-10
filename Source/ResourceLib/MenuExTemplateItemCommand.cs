using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A command menu item.
    /// </summary>
    public class MenuExTemplateItemCommand : MenuExTemplateItemBase
    {
        /// <summary>
        /// A command menu item.
        /// </summary>
        public MenuExTemplateItemCommand()
        {

        }

        /// <summary>
        /// Read a command menu item.
        /// </summary>
        /// <param name="lpRes">Address in memory.</param>
        /// <returns>End of the menu item structure.</returns>
        internal override IntPtr Read(IntPtr lpRes)
        {
            _header = (User32.MENUEXITEMTEMPLATE)Marshal.PtrToStructure(
                lpRes, typeof(User32.MENUEXITEMTEMPLATE));

            lpRes = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header));
            lpRes = base.Read(lpRes);

            return lpRes;
        }

        /// <summary>
        /// Returns true if the item is a separator.
        /// </summary>
        public bool IsSeparator
        {
            get
            {
                return _header.dwType == (uint)User32.MenuFlags.MFT_SEPARATOR
                    || ((_header.dwOptions == 0xFFFF || _header.dwOptions == 0) 
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
