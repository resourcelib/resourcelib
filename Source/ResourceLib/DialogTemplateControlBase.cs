using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A dialog template.
    /// </summary>
    public abstract class DialogTemplateControlBase
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

        private ResourceId _captionId = null;
        private ResourceId _controlClassId = null;
        private byte[] _creationData = null;

        /// <summary>
        /// Dialog caption.
        /// </summary>
        public ResourceId CaptionId
        {
            get
            {
                return _captionId;
            }
            set
            {
                _captionId = value;
            }
        }

        /// <summary>
        /// Window class Id.
        /// </summary>
        public ResourceId ControlClassId
        {
            get
            {
                return _controlClassId;
            }
            set
            {
                _controlClassId = value;
            }
        }

        /// <summary>
        /// Window class of the control.
        /// </summary>
        public User32.DialogItemClass ControlClass
        {
            get
            {
                return (User32.DialogItemClass) ControlClassId.Id;
            }
        }

        /// <summary>
        /// Additional creation data.
        /// </summary>
        public byte[] CreationData
        {
            get
            {
                return _creationData;
            }
            set
            {
                _creationData = value;
            }
        }

        internal virtual IntPtr Read(IntPtr lpRes)
        {
            // control class
            lpRes = DialogTemplateUtil.ReadResourceId(lpRes, out _controlClassId);
            // caption
            lpRes = DialogTemplateUtil.ReadResourceId(lpRes, out _captionId);

            // optional/additional creation data
            switch ((UInt16)Marshal.ReadInt16(lpRes))
            {
                case 0x0000: // no data
                    lpRes = new IntPtr(lpRes.ToInt64() + 2);
                    break;
                default:
                    UInt16 size = (UInt16)Marshal.ReadInt16(lpRes);
                    _creationData = new byte[size];
                    Marshal.Copy(lpRes, _creationData, 0, _creationData.Length);
                    lpRes = new IntPtr(lpRes.ToInt64() + size);
                    break;
            }

            return lpRes;
        }

        /// <summary>
        /// Write the dialog control to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        public virtual void Write(BinaryWriter w)
        {
            // control class
            DialogTemplateUtil.WriteResourceId(w, _controlClassId);
            // caption
            DialogTemplateUtil.WriteResourceId(w, _captionId);

            if (_creationData == null)
            {
                w.Write((UInt16) 0);
            }
            else
            {
                ResourceUtil.PadToWORD(w);
                w.Write((UInt16) _creationData.Length);
                w.Write(_creationData);
            }
        }
    }
}
