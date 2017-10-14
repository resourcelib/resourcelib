using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A device-independent image consists of a BITMAPINFOHEADER where
    /// bmWidth is the width of the image andbmHeight is double the height 
    /// of the image, followed by the bitmap color table, followed by the image
    /// pixels, followed by the mask pixels.
    /// </summary>
    public class DeviceIndependentBitmap
    {
        private Gdi32.BITMAPINFOHEADER _header = new Gdi32.BITMAPINFOHEADER();
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
        public DeviceIndependentBitmap()
        {

        }

        /// <summary>
        /// A device-independent bitmap.
        /// </summary>
        /// <param name="data">Bitmap data.</param>
        public DeviceIndependentBitmap(byte[] data)
        {
            Data = data;
        }

        /// <summary>
        /// Create a copy of an image.
        /// </summary>
        /// <param name="image">Source image.</param>
        public DeviceIndependentBitmap(DeviceIndependentBitmap image)
        {
            _data = new byte[image._data.Length];
            Buffer.BlockCopy(image._data, 0, _data, 0, image._data.Length);
            _header = image._header;
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

        /// <summary>
        /// Size of the image mask.
        /// </summary>
        private Int32 MaskImageSize
        {
            get
            {
                return (Int32)(_header.biHeight / 2 * GetDIBRowWidth(_header.biWidth));
            }
        }

        private Int32 XorImageSize
        {
            get
            {
                return (Int32)(_header.biHeight / 2 *
                    GetDIBRowWidth(_header.biWidth * _header.biBitCount * _header.biPlanes));
            }
        }

        /// <summary>
        /// Position of the DIB bitmap bits within a DIB bitmap array.
        /// </summary>
        private Int32 XorImageIndex
        {
            get
            {
                return (Int32)(Marshal.SizeOf(_header) +
                    ColorCount * Marshal.SizeOf(new Gdi32.RGBQUAD()));
            }
        }

        /// <summary>
        /// Number of colors in the palette.
        /// </summary>
        private UInt32 ColorCount
        {
            get
            {
                if (_header.biClrUsed != 0)
                    return _header.biClrUsed;

                if (_header.biBitCount <= 8)
                    return (UInt32)(1 << _header.biBitCount);

                return 0;
            }
        }

        private Int32 MaskImageIndex
        {
            get
            {
                return XorImageIndex + XorImageSize;
            }
        }

        /// <summary>
        /// Returns the width of a row in a DIB Bitmap given the number of bits. DIB Bitmap rows always align on a DWORD boundary.
        /// </summary>
        /// <param name="width">Number of bits.</param>
        /// <returns>Width of a row in bytes.</returns>
        private Int32 GetDIBRowWidth(int width)
        {
            return (Int32)((width + 31) / 32) * 4;
        }
    }
}
