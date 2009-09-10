using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A command menu item.
    /// </summary>
    public class MenuTemplateItemCommand : MenuTemplateItemBase
    {
        private UInt16 _menuId = 0;

        /// <summary>
        /// Command menu id.
        /// </summary>
        public UInt16 MenuId
        {
            get
            {
                return _menuId;
            }
            set
            {
                _menuId = value;
            }
        }

        /// <summary>
        /// A command menu item.
        /// </summary>
        public MenuTemplateItemCommand()
        {

        }

        /// <summary>
        /// Read a command menu item.
        /// </summary>
        /// <param name="lpRes">Address in memory.</param>
        /// <returns>End of the menu item structure.</returns>
        internal override IntPtr Read(IntPtr lpRes)
        {
            _header = (User32.MENUITEMTEMPLATE)Marshal.PtrToStructure(
                lpRes, typeof(User32.MENUITEMTEMPLATE));

            lpRes = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header));

            _menuId = (UInt16) Marshal.ReadInt16(lpRes);
            lpRes = new IntPtr(lpRes.ToInt32() + 2);
            
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
                return ((_header.mtOption & (uint)User32.MenuFlags.MF_SEPARATOR) > 0) ||
                    (_header.mtOption == 0 && _menuString == null && _menuId == 0);
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
                    new String(' ', indent),
                    _menuString == null ? string.Empty : _menuString.Replace("\t", @"\t"), _menuId));
            }
            return sb.ToString();
        }
    }
}
