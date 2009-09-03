using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A dialog template.
    /// </summary>
    public abstract class DialogTemplateBase
    {
        /// <summary>
        /// Specifies the x-coordinate, in dialog box units, of the upper-left corner of the dialog box. 
        /// </summary>
        public abstract Int16 x { get; set; }
        /// <summary>
        /// Specifies the y-coordinate, in dialog box units, of the upper-left corner of the dialog box.
        /// </summary>
        public abstract Int16 y { get; set; }
        /// <summary>
        /// Specifies the width, in dialog box units, of the dialog box.
        /// </summary>
        public abstract Int16 cx { get; set; }
        /// <summary>
        /// Specifies the height, in dialog box units, of the dialog box.
        /// </summary>
        public abstract Int16 cy { get; set; }
        /// <summary>
        /// Specifies the style of the dialog box.
        /// </summary>
        public abstract UInt32 Style { get; set; }
        /// <summary>
        /// Specifies the optional extended style of the dialog box.
        /// </summary>
        public abstract UInt32 ExtendedStyle { get; set; }
        /// <summary>
        /// Number of items in this structure.
        /// </summary>
        public abstract UInt16 ControlCount { get; }

        private string _caption = null;
        private ResourceId _menuId = null;
        private ResourceId _windowClassId = null;
        private UInt16 _pointSize = 0;
        private string _typeface = null;
        private List<DialogTemplateControlBase> _controls = new List<DialogTemplateControlBase>();

        /// <summary>
        /// The name of the typeface for the font.
        /// </summary>
        public String TypeFace
        {
            get
            {
                return _typeface;
            }
            set
            {
                _typeface = value;
            }
        }

        /// <summary>
        /// Specifies the point size of the font to use for the text in the dialog box and its controls.
        /// </summary>
        public UInt16 PointSize
        {
            get
            {
                return _pointSize;
            }
            set
            {
                _pointSize = value;
            }
        }
        
        /// <summary>
        /// Dialog caption.
        /// </summary>
        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
            }
        }

        /// <summary>
        /// Menu resource Id.
        /// </summary>
        public ResourceId MenuId
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
        /// Window class Id.
        /// </summary>
        public ResourceId WindowClassId
        {
            get
            {
                return _windowClassId;
            }
            set
            {
                _windowClassId = value;
            }
        }

        public List<DialogTemplateControlBase> Controls
        {
            get
            {
                return _controls;
            }
            set
            {
                _controls = value;
            }
        }

        /// <summary>
        /// String representation of the dialog style.
        /// </summary>
        /// <returns>String in the STYLE s1 | s2 | ... | s3 format.</returns>
        private string StyleToString()
        {
            StringBuilder styleb = new StringBuilder();
            foreach (User32.WindowStyles s in Enum.GetValues(typeof(User32.WindowStyles)))
            {
                if ((Style & (uint)s) > 0)
                {
                    styleb.Append(styleb.Length == 0 ? "STYLE " : " | ");
                    styleb.Append(s);
                }
            }

            foreach (User32.DialogStyles s in Enum.GetValues(typeof(User32.DialogStyles)))
            {
                if ((Style & (uint)s) > 0)
                {
                    styleb.Append(styleb.Length == 0 ? "STYLE " : " | ");
                    styleb.Append(s);
                }
            }

            return styleb.ToString();
        }

        /// <summary>
        /// String representation of the dialog extended style.
        /// </summary>
        /// <returns>String in the EXSTYLE s1 | s2 | ... | s3 format.</returns>
        private string ExtendedStyleToString()
        {
            StringBuilder styleb = new StringBuilder();

            foreach (User32.WindowStyles s in Enum.GetValues(typeof(User32.WindowStyles)))
            {
                if ((ExtendedStyle & (uint)s) > 0)
                {
                    styleb.Append(styleb.Length == 0 ? "EXSTYLE " : " | ");
                    styleb.Append(s);
                }
            }

            foreach (User32.ExtendedDialogStyles s in Enum.GetValues(typeof(User32.ExtendedDialogStyles)))
            {
                if ((ExtendedStyle & (uint)s) > 0)
                {
                    styleb.Append(styleb.Length == 0 ? "EXSTYLE " : " | ");
                    styleb.Append(s);
                }
            }

            return styleb.ToString();
        }

        /// <summary>
        /// Dialog template representation in a standard text format.
        /// </summary>
        /// <returns>Multiline string.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0}, {1}, {2}, {3}", x, y, x + cx, y + cy));

            string style = StyleToString();
            if (!string.IsNullOrEmpty(style))
                sb.AppendLine(style);

            string exstyle = ExtendedStyleToString();
            if (!string.IsNullOrEmpty(exstyle))
                sb.AppendLine(exstyle);

            sb.AppendLine(string.Format("CAPTION \"{0}\"", _caption));
            sb.AppendLine(string.Format("FONT {0}, \"{1}\"", _pointSize, _typeface));

            if (_controls.Count > 0)
            {
                sb.AppendLine("{");
                foreach (DialogTemplateControlBase control in _controls)
                {
                    sb.AppendLine(" " + control.ToString());
                }
                sb.AppendLine("}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// String represetnation of a control.
        /// </summary>
        /// <returns></returns>
        public virtual string ToControlString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} \"{1}\" {2}, {3}, {4}, {5}",
                WindowClassId, Caption, x, y, cx, cy);
            return sb.ToString();
        }

        internal virtual IntPtr Read(IntPtr lpRes)
        {
            // menu
            switch ((UInt16)Marshal.ReadIntPtr(lpRes))
            {
                case 0x0000: // no menu and the array has no other elements
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                case 0xFFFF: // one additional element that specifies the ordinal value of a menu resource in an executable file
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    MenuId = new ResourceId((UInt32)Marshal.ReadInt32(lpRes));
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                default: // null-terminated Unicode string that specifies the name of a menu resource in an executable file
                    MenuId = new ResourceId(Marshal.PtrToStringUni(lpRes));
                    lpRes = new IntPtr(lpRes.ToInt32() + (MenuId.Name.Length + 1) * 2);
                    break;
            }

            // window class
            switch ((UInt16)Marshal.ReadIntPtr(lpRes))
            {
                case 0x0000: // no predefined system window class
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                case 0xFFFF: // one additional element that specifies the ordinal value of a predefined system window class
                    lpRes = new IntPtr(lpRes.ToInt32() + 4);
                    WindowClassId = new ResourceId((UInt16) Marshal.ReadInt16(lpRes));
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                default: // null-terminated Unicode string that specifies the name of a predefined system window class
                    WindowClassId = new ResourceId(Marshal.PtrToStringUni(lpRes));
                    lpRes = new IntPtr(lpRes.ToInt32() + (WindowClassId.Name.Length + 1) * 2);
                    break;
            }

            Caption = Marshal.PtrToStringUni(lpRes);
            lpRes = new IntPtr(lpRes.ToInt32() + (Caption.Length + 1) * 2);

            if ((Style & (uint) User32.DialogStyles.DS_SETFONT) > 0
                || (Style & (uint) User32.DialogStyles.DS_SHELLFONT) > 0)
            {
                // point size
                PointSize = (UInt16)Marshal.ReadInt16(lpRes);
                lpRes = new IntPtr(lpRes.ToInt32() + 2);
            }

            return lpRes;
        }

        internal virtual IntPtr ReadControl(IntPtr lpRes)
        {
            // window class
            switch ((UInt16)Marshal.ReadIntPtr(lpRes))
            {
                case 0x0000: // no predefined system window class
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                case 0xFFFF: // one additional element that specifies the ordinal value of a predefined system window class
                    lpRes = new IntPtr(lpRes.ToInt32() + 4);
                    WindowClassId = new ResourceId((UInt16) Marshal.ReadInt16(lpRes));
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                default: // null-terminated Unicode string that specifies the name of a predefined system window class
                    WindowClassId = new ResourceId(Marshal.PtrToStringUni(lpRes));
                    lpRes = new IntPtr(lpRes.ToInt32() + (WindowClassId.Name.Length + 1) * 2);
                    break;
            }

            Caption = Marshal.PtrToStringUni(lpRes);
            lpRes = new IntPtr(lpRes.ToInt32() + (Caption.Length + 1) * 2);

            // optional creation data
            // window class
            switch ((UInt16)Marshal.ReadIntPtr(lpRes))
            {
                case 0x0000: // no predefined system window class
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                default: // null-terminated Unicode string that specifies the name of a predefined system window class
                    Int16 size = Marshal.ReadInt16(lpRes);
                    lpRes = new IntPtr(lpRes.ToInt32() + size);
                    break;
            }

            return lpRes;
        }

        internal abstract IntPtr AddControl(IntPtr lpRes);

        internal IntPtr ReadControls(IntPtr lpRes)
        {
            for (int i = 0; i < ControlCount; i++)
            {
                lpRes = AddControl(lpRes);
            }

            return lpRes;
        }
    }
}
