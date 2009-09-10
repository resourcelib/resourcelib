using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A collection of menu items.
    /// </summary>
    public class MenuTemplateItemCollection : List<MenuTemplateItemBase>
    {
        /// <summary>
        /// A collection of menu items.
        /// </summary>
        public MenuTemplateItemCollection()
        {

        }

        /// <summary>
        /// Read the menu item collection.
        /// </summary>
        /// <param name="lpRes">Address in memory.</param>
        /// <returns>End of the menu item structure.</returns>
        internal IntPtr Read(IntPtr lpRes)
        {
            while(true)
            {
                User32.MENUITEMTEMPLATE childItem = (User32.MENUITEMTEMPLATE)Marshal.PtrToStructure(
                    lpRes, typeof(User32.MENUITEMTEMPLATE));

                MenuTemplateItemBase childMenu = null;
                if ((childItem.mtOption & (uint)User32.MenuFlags.MF_POPUP) > 0)
                    childMenu = new MenuTemplateItemPopup();
                else 
                    childMenu = new MenuTemplateItemCommand();

                lpRes = childMenu.Read(lpRes);
                Add(childMenu);

                if ((childItem.mtOption & (uint)User32.MenuFlags.MF_END) != 0)
                    break;
            }

            return lpRes;
        }

        /// <summary>
        /// String representation in the MENU format.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return ToString(0);
        }

        /// <summary>
        /// String representation in the MENU format.
        /// </summary>
        /// <param name="indent">Indent.</param>
        /// <returns>String representation.</returns>
        public string ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            if (Count > 0)
            {
                sb.AppendLine(string.Format("{0}BEGIN", new String(' ', indent)));
                foreach (MenuTemplateItemBase child in this)
                {
                    sb.Append(child.ToString(indent + 1));
                }
                sb.AppendLine(string.Format("{0}END", new String(' ', indent)));
            }
            return sb.ToString();
        }
    }
}
