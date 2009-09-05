using System;
using System.Text;
using System.IO;
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
        /// X-coordinate, in dialog box units, of the upper-left corner of the dialog box. 
        /// </summary>
        public abstract Int16 x { get; set; }
        /// <summary>
        /// Y-coordinate, in dialog box units, of the upper-left corner of the dialog box.
        /// </summary>
        public abstract Int16 y { get; set; }
        /// <summary>
        /// Width, in dialog box units, of the dialog box.
        /// </summary>
        public abstract Int16 cx { get; set; }
        /// <summary>
        /// Height, in dialog box units, of the dialog box.
        /// </summary>
        public abstract Int16 cy { get; set; }
        /// <summary>
        /// Style of the dialog box.
        /// </summary>
        public abstract UInt32 Style { get; set; }
        /// <summary>
        /// Optional extended style of the dialog box.
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
        /// Point size of the font to use for the text in the dialog box and its controls.
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

        /// <summary>
        /// Controls within this dialog.
        /// </summary>
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
        /// Dialog template representation in a standard text format.
        /// </summary>
        /// <returns>Multiline string.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0}, {1}, {2}, {3}", x, y, x + cx, y + cy));

            string style = DialogTemplateUtil.StyleToString<User32.WindowStyles, User32.DialogStyles>(Style);
            if (!string.IsNullOrEmpty(style))
                sb.AppendLine("STYLE " + style);

            string exstyle = DialogTemplateUtil.StyleToString<User32.WindowStyles, User32.ExtendedDialogStyles>(ExtendedStyle);
            if (!string.IsNullOrEmpty(exstyle))
                sb.AppendLine("EXSTYLE " + exstyle);

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
            lpRes = DialogTemplateUtil.ReadResourceId(lpRes, out _menuId);
            // window class
            lpRes = DialogTemplateUtil.ReadResourceId(lpRes, out _windowClassId);
            // caption
            Caption = Marshal.PtrToStringUni(lpRes);
            lpRes = new IntPtr(lpRes.ToInt32() + (Caption.Length + 1) * 2);

            if ((Style & (uint)User32.DialogStyles.DS_SETFONT) > 0
                || (Style & (uint)User32.DialogStyles.DS_SHELLFONT) > 0)
            {
                // point size
                PointSize = (UInt16)Marshal.ReadInt16(lpRes);
                lpRes = new IntPtr(lpRes.ToInt32() + 2);
            }

            return lpRes;
        }

        internal abstract IntPtr AddControl(IntPtr lpRes);

        internal IntPtr ReadControls(IntPtr lpRes)
        {
            for (int i = 0; i < ControlCount; i++)
            {
                lpRes = ResourceUtil.Align(lpRes);
                lpRes = AddControl(lpRes);
            }

            return lpRes;
        }

        internal void WriteControls(BinaryWriter w)
        {
            foreach(DialogTemplateControlBase control in Controls)
            {
                ResourceUtil.PadToDWORD(w);
                control.Write(w);
            }
        }

        /// <summary>
        /// Write the resource to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        public virtual void Write(BinaryWriter w)
        {
            // menu
            DialogTemplateUtil.WriteResourceId(w, _menuId);
            // window class
            DialogTemplateUtil.WriteResourceId(w, _windowClassId);
            // caption
            w.Write(Encoding.Unicode.GetBytes(Caption));
            w.Write((UInt16)0);
            // point size
            if ((Style & (uint)User32.DialogStyles.DS_SETFONT) > 0
                || (Style & (uint)User32.DialogStyles.DS_SHELLFONT) > 0)
            {
                w.Write((UInt16)PointSize);
            }
        }
    }
}
