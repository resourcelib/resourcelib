using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This is the icon bitmap/image.
    /// </summary>
    public class IconImage
    {
        private Gdi32.BITMAPINFOHEADER _header = new Gdi32.BITMAPINFOHEADER();

        //private Gdi32.RGBQUAD[] _icColors;
        //private Byte[] icXOR;
        //private Byte[] icAND;

        private byte[] _data = null;

        /// <summary>
        /// Raw image data.
        /// </summary>
        public byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;

                IntPtr pData = Marshal.AllocHGlobal(Marshal.SizeOf(_header));
                try
                {
                    Marshal.Copy(_data, 0, pData, Marshal.SizeOf(_header));
                    _header = (Gdi32.BITMAPINFOHEADER)Marshal.PtrToStructure(
                        pData, typeof(Gdi32.BITMAPINFOHEADER));
                }
                finally
                {
                    Marshal.FreeHGlobal(pData);
                }
            }
        }

        /// <summary>
        /// Bitmap info header.
        /// </summary>
        public Gdi32.BITMAPINFOHEADER Header
        {
            get
            {
                return _header;
            }
        }

        /// <summary>
        /// Bitmap size in bytes.
        /// </summary>
        public int Size
        {
            get
            {
                return _data.Length;
            }
        }

        /// <summary>
        /// A new icon image.
        /// </summary>
        public IconImage()
        {

        }

        /// <summary>
        /// Load an existing (.ico) file.
        /// </summary>
        /// <param name="filename">Path to an .ico file.</param>
        public IconImage(string filename)
        {
            Data = File.ReadAllBytes(filename);
        }

        /// <summary>
        /// Read icon data.
        /// </summary>
        /// <param name="lpData">Pointer to the beginning of icon data.</param>
        /// <param name="size">Icon data size.</param>
        internal void Read(IntPtr lpData, uint size)
        {
            _header = (Gdi32.BITMAPINFOHEADER)Marshal.PtrToStructure(
                lpData, typeof(Gdi32.BITMAPINFOHEADER));

            _data = new byte[size];
            Marshal.Copy(lpData, _data, 0, _data.Length);
        }
    }
}
