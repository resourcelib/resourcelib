using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A popup menu item.
    /// </summary>
    public class MenuTemplateItemPopup : MenuTemplateItemBase
    {
        MenuTemplateItemCollection _subMenuItems = new MenuTemplateItemCollection();

        /// <summary>
        /// Sub menu items.
        /// </summary>
        public MenuTemplateItemCollection SubMenuItems
        {
            get
            {
                return _subMenuItems;
            }
            set
            {
                _subMenuItems = value;
            }
        }

        /// <summary>
        /// A popup menu item.
        /// </summary>
        public MenuTemplateItemPopup()
        {

        }

        /// <summary>
        /// Read a popup menu item.
        /// </summary>
        /// <param name="lpRes">Address in memory.</param>
        /// <returns>End of the menu item structure.</returns>
        internal override IntPtr Read(IntPtr lpRes)
        {
            _header = (User32.MENUITEMTEMPLATE) Marshal.PtrToStructure(
                lpRes, typeof(User32.MENUITEMTEMPLATE));

            lpRes = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header));
            lpRes = base.Read(lpRes);

            return _subMenuItems.Read(lpRes);
        }

        /// <summary>
        /// String representation in the MENU format.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0}POPUP \"{1}\"",
                new String(' ', indent), 
                _menuString == null ? string.Empty : _menuString.Replace("\t", @"\t")));
            sb.Append(_subMenuItems.ToString(indent));
            return sb.ToString();
        }
    }
}
