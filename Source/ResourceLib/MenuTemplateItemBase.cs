using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A base menu template item.
    /// </summary>
    public abstract class MenuTemplateItemBase
    {
        /// <summary>
        /// Menu item header.
        /// </summary>
        protected User32.MENUITEMTEMPLATE _header = new User32.MENUITEMTEMPLATE();

        /// <summary>
        /// Menu string.
        /// </summary>
        protected string _menuString = null;

        /// <summary>
        /// Menu text.
        /// </summary>
        public string MenuString
        {
            get
            {
                return _menuString;
            }
            set
            {
                _menuString = value;
            }
        }

        /// <summary>
        /// Read the menu item.
        /// </summary>
        /// <param name="lpRes">Address in memory.</param>
        /// <returns>End of the menu item structure.</returns>
        internal virtual IntPtr Read(IntPtr lpRes)
        {
            _menuString = (Marshal.ReadInt16(lpRes) == 0) 
                ? null
                : Marshal.PtrToStringUni(lpRes);

            lpRes = new IntPtr(lpRes.ToInt32() + 
                (_menuString == null ? 2 : (_menuString.Length + 1) * Marshal.SystemDefaultCharSize));

            return lpRes;
        }

        /// <summary>
        /// String representation in the MENU format.
        /// </summary>
        /// <param name="indent">Indent.</param>
        /// <returns>String representation.</returns>
        public abstract string ToString(int indent);

        /// <summary>
        /// String representation in the MENU format.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return ToString(0);
        }
    }
}
