using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{

    /// <summary>
    /// A font directory entry.
    /// </summary>
    public class FontDirectoryEntry
    {
        private UInt16 _fontOrdinal = 0;
        private User32.FONTDIRENTRY _font = new User32.FONTDIRENTRY();
        private string _faceName;
        private string _deviceName;

        /// <summary>
        /// Font ordinal.
        /// </summary>
        public UInt16 FontOrdinal
        {
            get
            {
                return _fontOrdinal;
            }
            set
            {
                _fontOrdinal = value;
            }
        }

        /// <summary>
        /// Typeface name of the font.
        /// </summary>
        public string FaceName
        {
            get
            {
                return _faceName;
            }
            set
            {
                _faceName = value;
            }
        }

        /// <summary>
        /// Specifies the name of the device if this font file is designated for a specific device.
        /// </summary>
        public string DeviceName
        {
            get
            {
                return _deviceName;
            }
            set
            {
                _deviceName = value;
            }
        }

        /// <summary>
        /// Font information.
        /// </summary>
        public User32.FONTDIRENTRY Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
            }
        }

        /// <summary>
        /// A new font directory entry.
        /// </summary>
        public FontDirectoryEntry()
        {
        }

        /// <summary>
        /// Read the font directory entry.
        /// </summary>
        /// <param name="lpRes">Pointer in memory.</param>
        /// <returns>Pointer to the end of the font directory entry.</returns>
        internal IntPtr Read(IntPtr lpRes)
        {
            IntPtr lpHead = lpRes;

            _fontOrdinal = (UInt16) Marshal.ReadInt16(lpRes);
            lpRes = new IntPtr(lpRes.ToInt32() + 2);

            _font = (User32.FONTDIRENTRY)Marshal.PtrToStructure(
                lpRes, typeof(User32.FONTDIRENTRY));

            lpRes = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_font));

            _deviceName = Marshal.PtrToStringAnsi(lpRes);
            lpRes = new IntPtr(lpRes.ToInt32() + _deviceName.Length + 1);

            _faceName = Marshal.PtrToStringAnsi(lpRes);
            lpRes = new IntPtr(lpRes.ToInt32() + _faceName.Length + 1);

            return lpRes;
        }

        /// <summary>
        /// Write the font directory entry to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        public void Write(BinaryWriter w)
        {
            w.Write(_fontOrdinal);
            w.Write(ResourceUtil.GetBytes(_font));
            
            // device name
            if (string.IsNullOrEmpty(_deviceName))
            {
                w.Write((byte)0);
            }
            else
            {
                w.Write(Encoding.ASCII.GetBytes(_deviceName));
            }

            // face name
            if (string.IsNullOrEmpty(_faceName))
            {
                w.Write((byte)0);
            }
            else
            {
                w.Write(Encoding.ASCII.GetBytes(_faceName));
            }
        }
    }
}
