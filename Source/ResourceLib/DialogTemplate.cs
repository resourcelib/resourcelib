using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A container for the DIALOGTEMPLATE structure.
    /// </summary>
    public class DialogTemplate : DialogTemplateBase
    {
        private User32.DLGTEMPLATE _header = new User32.DLGTEMPLATE();

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
                return _header.dwExtendedStyle;
            }
            set
            {
                _header.dwExtendedStyle = value;
            }
        }

        /// <summary>
        /// Number of dialog items.
        /// </summary>
        public override UInt16 ControlCount
        {
            get 
            {
                return _header.cdit;
            }
        }

        /// <summary>
        /// A standard dialog structure.
        /// </summary>
        public DialogTemplate()
        {

        }

        /// <summary>
        /// Read the dialog resource.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of the dialog structure.</param>
        internal override IntPtr Read(IntPtr lpRes)
        {
            _header = (User32.DLGTEMPLATE) Marshal.PtrToStructure(
                lpRes, typeof(User32.DLGTEMPLATE));

            lpRes = new IntPtr(lpRes.ToInt32() + 18); // Marshal.SizeOf(_header)
            lpRes = base.Read(lpRes);

            if ((Style & (uint)User32.DialogStyles.DS_SETFONT) > 0
                || (Style & (uint)User32.DialogStyles.DS_SHELLFONT) > 0)
            {
                // typeface
                TypeFace = Marshal.PtrToStringUni(lpRes);
                lpRes = new IntPtr(lpRes.ToInt32() + (TypeFace.Length + 1) * 2);
            }

            return lpRes;
        }

        internal override IntPtr AddControl(IntPtr lpRes)
        {
            DialogTemplateControl control = new DialogTemplateControl();
            Controls.Add(control);
            return control.Read(lpRes);
        }

        public override string ToString()
        {
            return string.Format("DIALOG {0}", base.ToString());
        }
    }
}
