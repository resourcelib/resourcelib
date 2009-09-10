using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A collection of menu items.
    /// </summary>
    public class MenuExTemplateItemCollection : List<MenuExTemplateItemBase>
    {
        /// <summary>
        /// A collection of extended menu items.
        /// </summary>
        public MenuExTemplateItemCollection()
        {

        }

        /// <summary>
        /// Read the menu item collection.
        /// </summary>
        /// <param name="lpRes">Address in memory.</param>
        /// <returns>End of the menu item structure.</returns>
        internal IntPtr Read(IntPtr lpRes)
        {
            UInt32 dwContextHelpID = (UInt32)Marshal.ReadInt32(lpRes);
            lpRes = new IntPtr(lpRes.ToInt32() + 4);

            while (true)
            {
                User32.MENUEXITEMTEMPLATE childItem = (User32.MENUEXITEMTEMPLATE)Marshal.PtrToStructure(
                    lpRes, typeof(User32.MENUEXITEMTEMPLATE));

                MenuExTemplateItemBase childMenu = null;
                if (childItem.dwOptions == (uint) User32.MenuResourceType.Sub)
                    childMenu = new MenuExTemplateItemPopup();
                else
                    childMenu = new MenuExTemplateItemCommand();

                lpRes = childMenu.Read(lpRes);
                Add(childMenu);

                if (childItem.dwOptions == (uint) User32.MenuResourceType.Last)
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
                foreach (MenuExTemplateItemBase child in this)
                {
                    sb.Append(child.ToString(indent + 1));
                }
                sb.AppendLine(string.Format("{0}END", new String(' ', indent)));
            }
            return sb.ToString();
        }
    }
}
