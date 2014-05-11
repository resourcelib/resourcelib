using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// An extended popup menu item.
    /// </summary>
    public class MenuExTemplateItemPopup : MenuExTemplateItem
    {
        private UInt32 _dwHelpId = 0;
        MenuExTemplateItemCollection _subMenuItems = new MenuExTemplateItemCollection();

        /// <summary>
        /// Sub menu items.
        /// </summary>
        public MenuExTemplateItemCollection SubMenuItems
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
        /// An extended popup menu item.
        /// </summary>
        public MenuExTemplateItemPopup()
        {

        }

        /// <summary>
        /// Read an extended popup menu item.
        /// </summary>
        /// <param name="lpRes">Address in memory.</param>
        /// <returns>End of the menu item structure.</returns>
        internal override IntPtr Read(IntPtr lpRes)
        {
            lpRes = base.Read(lpRes);
            
            lpRes = ResourceUtil.Align(lpRes);
            _dwHelpId = (UInt32) Marshal.ReadInt32(lpRes);
            lpRes = new IntPtr(lpRes.ToInt64() + 4);

            return _subMenuItems.Read(lpRes);
        }

        /// <summary>
        /// Write the menu item to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            base.Write(w);
            ResourceUtil.PadToDWORD(w);
            w.Write(_dwHelpId);
            _subMenuItems.Write(w);
        }

        /// <summary>
        /// String representation in the MENUEX format.
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
