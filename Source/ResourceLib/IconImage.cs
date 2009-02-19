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

        byte[] _data = null;

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

        public Gdi32.BITMAPINFOHEADER Header
        {
            get
            {
                return _header;
            }
        }

        public int Size
        {
            get
            {
                return _data.Length;
            }
        }

        public IconImage()
        {

        }

        /// <summary>
        /// Load a .ico file
        /// </summary>
        /// <param name="filename">.ico filename</param>
        public IconImage(string filename)
        {
            Data = File.ReadAllBytes(filename);
        }

        public void Read(IntPtr lpData, uint size)
        {
            _header = (Gdi32.BITMAPINFOHEADER)Marshal.PtrToStructure(
                lpData, typeof(Gdi32.BITMAPINFOHEADER));

            _data = new byte[size];
            Marshal.Copy(lpData, _data, 0, _data.Length);
        }
    }
}
