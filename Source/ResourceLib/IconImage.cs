using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This is the icon bitmap/image.
    /// </summary>
    public class IconImage
    {
        private Gdi32.BITMAPINFOHEADER _header;

        //private Gdi32.RGBQUAD[] _icColors;
        //private Byte[] icXOR;
        //private Byte[] icAND;

        byte[] _data;

        public Gdi32.BITMAPINFOHEADER Header
        {
            get
            {
                return _header;
            }
        }

        public IconImage()
        {

        }

        public void Read(IntPtr lpData, int size)
        {
            _header = (Gdi32.BITMAPINFOHEADER)Marshal.PtrToStructure(
                lpData, typeof(Gdi32.BITMAPINFOHEADER));

            _data = new byte[size];
            Marshal.Copy(lpData, _data, 0, _data.Length);
        }        
    }
}
