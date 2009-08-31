using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Drawing.Imaging;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// An embedded bitmap resource.
    /// </summary>
    public class BitmapResource : Resource
    {
        private Gdi32.BITMAPINFOHEADER _header = new Gdi32.BITMAPINFOHEADER();

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

        private DeviceIndependentBitmap _bitmap = null;

        /// <summary>
        /// An existing bitmap resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        public BitmapResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary>
        /// A new bitmap resource.
        /// </summary>
        public BitmapResource()
            : base(IntPtr.Zero,
                IntPtr.Zero,
                new ResourceId(Kernel32.ResourceTypes.RT_BITMAP),
                new ResourceId(1),
                Kernel32.LANG_NEUTRAL,
                0)
        {

        }

        /// <summary>
        /// Read the resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="lpRes">Pointer to the beginning of a resource.</param>
        /// <returns>Pointer to the end of the resource.</returns>
        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            _header = (Gdi32.BITMAPINFOHEADER)Marshal.PtrToStructure(
                lpRes, typeof(Gdi32.BITMAPINFOHEADER));

            byte[] data = new byte[_size];
            Marshal.Copy(lpRes, data, 0, data.Length);
            _bitmap = new DeviceIndependentBitmap(data);

            return new IntPtr(lpRes.ToInt32() + _size);
        }

        /// <summary>
        /// Write the bitmap resource to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            w.Write(ResourceUtil.GetBytes(_header));
            w.Write(_bitmap.Data);
        }

        /// <summary>
        /// Bitmap pixel format.
        /// </summary>
        public PixelFormat PixelFormat
        {
            get
            {
                switch (_header.biBitCount)
                {
                    case 1:
                        return PixelFormat.Format1bppIndexed;
                    case 4:
                        return PixelFormat.Format4bppIndexed;
                    case 8:
                        return PixelFormat.Format8bppIndexed;
                    case 16:
                        return PixelFormat.Format16bppRgb565;
                    case 24:
                        return PixelFormat.Format24bppRgb;
                    case 32:
                        return PixelFormat.Format32bppArgb;
                    default:
                        return PixelFormat.Undefined;
                }
            }
        }

        /// <summary>
        /// Bitmap pixel format English standard string.
        /// </summary>
        public string PixelFormatString
        {
            get
            {
                switch (PixelFormat)
                {
                    case PixelFormat.Format1bppIndexed:
                        return "1-bit B/W";
                    case PixelFormat.Format24bppRgb:
                        return "24-bit True Colors";
                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format32bppRgb:
                        return "32-bit Alpha Channel";
                    case PixelFormat.Format8bppIndexed:
                        return "8-bit 256 Colors";
                    case PixelFormat.Format4bppIndexed:
                        return "4-bit 16 Colors";
                }
                return "Unknown";
            }
        }

        /// <summary>
        /// A device independent bitmap.
        /// </summary>
        public DeviceIndependentBitmap Bitmap
        {
            get
            {
                return _bitmap;
            }
            set
            {
                _bitmap = value;
            }
        }
    }
}
