using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A base menu template item.
    /// </summary>
    public abstract class MenuExTemplateItemBase
    {
        /// <summary>
        /// Menu item header.
        /// </summary>
        protected User32.MENUEXITEMTEMPLATE _header = new User32.MENUEXITEMTEMPLATE();
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
            switch ((UInt32)Marshal.ReadInt32(lpRes))
            {
                case 0:
                    lpRes = ResourceUtil.Align(lpRes.ToInt32() + 2);
                    break;
                case 0xFFFF:
                    lpRes = ResourceUtil.Align(lpRes.ToInt32() + 4);
                    break;
                default:
                    _menuString = Marshal.PtrToStringUni(lpRes);
                    lpRes = ResourceUtil.Align(lpRes.ToInt32() +
                        (_menuString.Length + 1) * Marshal.SystemDefaultCharSize);
                    break;
            }


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
