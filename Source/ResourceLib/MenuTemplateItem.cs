using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A base menu template item.
    /// </summary>
    public abstract class MenuTemplateItem
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
            switch ((UInt16) Marshal.ReadInt16(lpRes))
            {
                case 0:
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                default:
                    _menuString = Marshal.PtrToStringUni(lpRes);
                    lpRes = new IntPtr(lpRes.ToInt32() +
                        (_menuString.Length + 1) * Marshal.SystemDefaultCharSize);
                    break;
            }

            return lpRes;
        }

        internal virtual void Write(BinaryWriter w)
        {
            // menu string
            if (_menuString == null)
            {
                w.Write((UInt16) 0);
            }
            else
            {
                w.Write(Encoding.Unicode.GetBytes(_menuString));
                w.Write((UInt16) 0);
                ResourceUtil.PadToWORD(w);
            }
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
