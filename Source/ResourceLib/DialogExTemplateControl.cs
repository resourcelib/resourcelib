using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A container for the DIALOGTEMPLATEEX structure.
    /// </summary>
    public class DialogExTemplateControl : DialogTemplateControlBase
    {
        private User32.DIALOGEXITEMTEMPLATE _header = new User32.DIALOGEXITEMTEMPLATE();

        /// <summary>
        /// X-coordinate, in dialog box units, of the upper-left corner of the dialog box. 
        /// </summary>
        public override Int16 x
        {
            get
            {
                return _header.x;
            }
            set
            {
                _header.x = value;
            }
        }

        /// <summary>
        /// Y-coordinate, in dialog box units, of the upper-left corner of the dialog box.
        /// </summary>
        public override Int16 y
        {
            get
            {
                return _header.y;
            }
            set
            {
                _header.y = value;
            }
        }

        /// <summary>
        /// Width, in dialog box units, of the dialog box.
        /// </summary>
        public override Int16 cx
        {
            get
            {
                return _header.cx;
            }
            set
            {
                _header.cx = value;
            }
        }

        /// <summary>
        /// Height, in dialog box units, of the dialog box.
        /// </summary>
        public override Int16 cy
        {
            get
            {
                return _header.cy;
            }
            set
            {
                _header.cy = value;
            }
        }

        /// <summary>
        /// Dialog style.
        /// </summary>
        public override UInt32 Style
        {
            get
            {
                return _header.style;
            }
            set
            {
                _header.style = value;
            }
        }

        /// <summary>
        /// Extended dialog style.
        /// </summary>
        public override UInt32 ExtendedStyle
        {
            get
            {
                return _header.exStyle;
            }
            set
            {
                _header.exStyle = value;
            }
        }

        /// <summary>
        /// Control identifier.
        /// </summary>
        public Int32 Id
        {
            get
            {
                return _header.id;
            }
            set
            {
                _header.id = value;
            }
        }

        /// <summary>
        /// An extended dialog control template structure.
        /// </summary>
        public DialogExTemplateControl()
        {

        }

        /// <summary>
        /// Read the dialog control.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of the dialog structure.</param>
        internal override IntPtr Read(IntPtr lpRes)
        {
            _header = (User32.DIALOGEXITEMTEMPLATE)Marshal.PtrToStructure(
                lpRes, typeof(User32.DIALOGEXITEMTEMPLATE));

            lpRes = new IntPtr(lpRes.ToInt64() + Marshal.SizeOf(_header));
            return base.Read(lpRes);
        }

        /// <summary>
        /// Write the dialog control to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        public override void Write(BinaryWriter w)
        {
            w.Write((UInt32)_header.helpID);
            w.Write((UInt32)_header.exStyle);
            w.Write((UInt32)_header.style);
            w.Write((Int16)_header.x);
            w.Write((Int16)_header.y);
            w.Write((Int16)_header.cx);
            w.Write((Int16)_header.cy);
            w.Write((Int32)_header.id);
            base.Write(w);
        }

        /// <summary>
        /// Return a string representation of the dialog control.
        /// </summary>
        /// <returns>A single line in the "CLASS name id, dimensions and styles' format.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0} \"{1}\" {2}, {3}, {4}, {5}, {6}, {7}, {8}",
                ControlClass, CaptionId, Id, ControlClass, x, y, cx, cy,
                DialogTemplateUtil.StyleToString<User32.WindowStyles, User32.StaticControlStyles>(Style, ExtendedStyle));

            switch (ControlClass)
            {
                case User32.DialogItemClass.Button:
                    sb.AppendFormat("| {0}", (User32.ButtonControlStyles)(Style & 0xFFFF));
                    break;
                case User32.DialogItemClass.Edit:
                    sb.AppendFormat("| {0}", DialogTemplateUtil.StyleToString<User32.EditControlStyles>(Style & 0xFFFF));
                    break;
            }

            return sb.ToString();
        }
    }
}
