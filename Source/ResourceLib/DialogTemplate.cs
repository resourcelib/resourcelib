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
        private User32.DIALOGTEMPLATE _header = new User32.DIALOGTEMPLATE();

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
            _header = (User32.DIALOGTEMPLATE) Marshal.PtrToStructure(
                lpRes, typeof(User32.DIALOGTEMPLATE));

            lpRes = new IntPtr(lpRes.ToInt32() + 18); // Marshal.SizeOf(_header)
            lpRes = base.Read(lpRes);

            if ((Style & (uint)User32.DialogStyles.DS_SETFONT) > 0
                || (Style & (uint)User32.DialogStyles.DS_SHELLFONT) > 0)
            {
                // typeface
                TypeFace = Marshal.PtrToStringUni(lpRes);
                lpRes = new IntPtr(lpRes.ToInt32() + (TypeFace.Length + 1) * Marshal.SystemDefaultCharSize);
            }

            return ReadControls(lpRes);
        }

        internal override IntPtr AddControl(IntPtr lpRes)
        {
            DialogTemplateControl control = new DialogTemplateControl();
            Controls.Add(control);
            return control.Read(lpRes);
        }

        /// <summary>
        /// Write the dialog template data to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        public override void Write(BinaryWriter w)
        {
            w.Write(_header.style);
            w.Write(_header.dwExtendedStyle);
            w.Write((UInt16) Controls.Count);
            w.Write(_header.x);
            w.Write(_header.y);
            w.Write(_header.cx);
            w.Write(_header.cy);
            
            base.Write(w);
            
            if ((Style & (uint)User32.DialogStyles.DS_SETFONT) > 0
                || (Style & (uint)User32.DialogStyles.DS_SHELLFONT) > 0)
            {
                w.Write(Encoding.Unicode.GetBytes(TypeFace));
                w.Write((UInt16) 0);
            }

            WriteControls(w);
        }

        /// <summary>
        /// Returns a string representation of the dialog.
        /// </summary>
        /// <returns>String in the DIALOG ... format.</returns>
        public override string ToString()
        {
            return string.Format("DIALOG {0}", base.ToString());
        }
    }
}
