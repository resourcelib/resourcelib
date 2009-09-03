using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A container for a control within a dialog template.
    /// </summary>
    public class DialogTemplateControl : DialogTemplateControlBase
    {
        private User32.DLGITEMTEMPLATE _header = new User32.DLGITEMTEMPLATE();

        /// <summary>
        /// Specifies the x-coordinate, in dialog box units, of the upper-left corner of the dialog box. 
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
        /// Specifies the y-coordinate, in dialog box units, of the upper-left corner of the dialog box.
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
        /// Specifies the width, in dialog box units, of the dialog box.
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
        /// Specifies the height, in dialog box units, of the dialog box.
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
                return _header.dwExtendedStyle;
            }
            set
            {
                _header.dwExtendedStyle = value;
            }
        }

        /// <summary>
        /// Specifies the control identifier.
        /// </summary>
        public UInt16 Id
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
        /// A standard dialog control structure.
        /// </summary>
        public DialogTemplateControl()
        {

        }

        internal override IntPtr Read(IntPtr lpRes)
        {
            _header = (User32.DLGITEMTEMPLATE)Marshal.PtrToStructure(
                lpRes, typeof(User32.DLGITEMTEMPLATE));

            lpRes = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header));
            lpRes = base.Read(lpRes);

            return lpRes;
        }

        /// <summary>
        /// String represetnation of a control.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} \"{1}\" {2}, {3}, {4}, {5}, {6}",
                ControlClass, CaptionId, Id, x, y, cx, cy);
            return sb.ToString();
        }
    }
}
