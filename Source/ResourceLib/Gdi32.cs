using System;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// Gdi32.dll interop functions.
    /// </summary>
    public abstract class Gdi32
    {
        /// <summary>
        /// Bitmap compression options.
        /// </summary>
        public enum BitmapCompression
        {
            /// <summary>
            /// An uncompressed format. 
            /// </summary>
            BI_RGB = 0, 
            /// <summary>
            /// A run-length encoded (RLE) format for bitmaps with 8 bpp. The compression format is a 2-byte format consisting of a count byte followed by a byte containing a color index. For more information, see Bitmap Compression.
            /// </summary>
            BI_RLE8 = 1,
            /// <summary>
            /// An RLE format for bitmaps with 4 bpp. The compression format is a 2-byte format consisting of a count byte followed by two word-length color indexes. For more information, see Bitmap Compression.
            /// </summary>
            BI_RLE4 = 2,
            /// <summary>
            /// Specifies that the bitmap is not compressed and that the color table consists of three DWORD color masks that specify the red, green, and blue components, respectively, of each pixel. This is valid when used with 16- and 32-bpp bitmaps.
            /// </summary>
            BI_BITFIELDS = 3,
            /// <summary>
            /// Windows 98/Me, Windows 2000/XP: Indicates that the image is a JPEG image.
            /// </summary>
            BI_JPEG = 4, 
            /// <summary>
            /// Windows 98/Me, Windows 2000/XP: Indicates that the image is a PNG image.
            /// </summary>
            BI_PNG = 5,
        };

        /// <summary>
        /// A bitmap info header.
        /// See http://msdn.microsoft.com/en-us/library/ms532290.aspx for more information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct BITMAPINFOHEADER
        {
            /// <summary>
            /// Bitmap information size.
            /// </summary>
            public UInt32 biSize;
            /// <summary>
            /// Bitmap width.
            /// </summary>
            public Int32 biWidth;
            /// <summary>
            /// Bitmap height.
            /// </summary>
            public Int32 biHeight;
            /// <summary>
            /// Number of logical planes.
            /// </summary>
            public UInt16 biPlanes;
            /// <summary>
            /// Bitmap bitrate.
            /// </summary>
            public UInt16 biBitCount;
            /// <summary>
            /// Bitmap compression.
            /// </summary>
            public UInt32 biCompression;
            /// <summary>
            /// Image size.
            /// </summary>
            public UInt32 biSizeImage;
            /// <summary>
            /// Horizontal pixel resolution.
            /// </summary>
            public Int32 biXPelsPerMeter;
            /// <summary>
            /// Vertical pixel resolution.
            /// </summary>
            public Int32 biYPelsPerMeter;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 biClrUsed;
            /// <summary>
            /// 
            /// </summary>
            public UInt32 biClrImportant;

            /// <summary>
            /// Returns the current bitmap compression.
            /// </summary>
            public BitmapCompression BitmapCompression
            {
                get
                {
                    return (BitmapCompression) biCompression;
                }
            }
        }

        /// <summary>
        /// RGB data.
        /// http://msdn.microsoft.com/en-us/library/ms997538.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RGBQUAD
        {
            /// <summary>
            /// Blue.
            /// </summary>
            public Byte rgbBlue;
            /// <summary>
            /// Green.
            /// </summary>
            public Byte rgbGreen;
            /// <summary>
            /// Red.
            /// </summary>
            public Byte rgbRed;
            /// <summary>
            /// Reserved.
            /// </summary>
            public Byte rgbReserved;
        }
    }
}
