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
    public abstract class MenuExTemplateItem
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
            _header = (User32.MENUEXITEMTEMPLATE) Marshal.PtrToStructure(
                lpRes, typeof(User32.MENUEXITEMTEMPLATE));

            lpRes = new IntPtr(lpRes.ToInt32() 
                + Marshal.SizeOf(_header));

            switch ((UInt32) Marshal.ReadInt32(lpRes))
            {
                case 0:
                    break;
                default:
                    _menuString = Marshal.PtrToStringUni(lpRes);
                    lpRes = new IntPtr(lpRes.ToInt32() +
                        (_menuString.Length + 1) * Marshal.SystemDefaultCharSize);
                    break;
            }

            return lpRes;
        }

        /// <summary>
        /// Write the menu item to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal virtual void Write(BinaryWriter w)
        {
            // header
            w.Write(_header.dwType);
            w.Write(_header.dwState);
            w.Write(_header.dwMenuId);
            w.Write(_header.bResInfo);
            // menu string
            if (_menuString != null)
            {
                w.Write(Encoding.Unicode.GetBytes(_menuString));
                w.Write((UInt16) 0);
                ResourceUtil.PadToDWORD(w);
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
