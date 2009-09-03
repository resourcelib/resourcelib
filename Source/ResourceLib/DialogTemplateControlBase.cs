using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A dialog template.
    /// </summary>
    public abstract class DialogTemplateControlBase
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
        
        private ResourceId _captionId = null;
        private ResourceId _controlClassId = null;

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
        public string ControlClass
        {
            get
            {
                if (ControlClassId == null)
                    return string.Empty;

                foreach (User32.DLGITEMTEMPLATEEX_WindowClass wc in Enum.GetValues(typeof(User32.DLGITEMTEMPLATEEX_WindowClass)))
                {
                    if (ControlClassId.Equals(new ResourceId((uint)wc)))
                        return wc.ToString();
                }

                return ControlClassId.ToString();
            }
        }

        internal virtual IntPtr Read(IntPtr lpRes)
        {
            // window class
            switch ((UInt16)Marshal.ReadIntPtr(lpRes))
            {
                case 0xFFFF: // one additional element that specifies the ordinal value of a predefined system window class
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    ControlClassId = new ResourceId((UInt16)Marshal.ReadInt16(lpRes));
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                default: // null-terminated Unicode string that specifies the name of a predefined system window class
                    ControlClassId = new ResourceId(Marshal.PtrToStringUni(lpRes));
                    lpRes = new IntPtr(lpRes.ToInt32() + (ControlClassId.Name.Length + 1) * 2);
                    break;
            }

            switch ((UInt16)Marshal.ReadIntPtr(lpRes))
            {
                case 0x0000: // no predefined caption
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                case 0xFFFF: // one additional element that specifies the ordinal value of a caption
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    CaptionId = new ResourceId((UInt16)Marshal.ReadInt16(lpRes));
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                default: // null-terminated Unicode string that specifies the name of a caption
                    CaptionId = new ResourceId(Marshal.PtrToStringUni(lpRes));
                    lpRes = new IntPtr(lpRes.ToInt32() + (CaptionId.Name.Length + 1) * 2);
                    break;
            }

            // optional creation data
            switch ((UInt16)Marshal.ReadIntPtr(lpRes))
            {
                case 0x0000: // no creation data
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                default:
                    // creation data size
                    Int16 size = Marshal.ReadInt16(lpRes);
                    lpRes = new IntPtr(lpRes.ToInt32() + size);
                    break;
            }

            return lpRes;
        }
    }
}
