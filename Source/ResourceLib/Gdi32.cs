using System;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    public abstract class Gdi32
    {
        public enum BitmapCompression
        {
            BI_RGB = 0, // An uncompressed format. 
            BI_RLE8 = 1, // A run-length encoded (RLE) format for bitmaps with 8 bpp. The compression format is a 2-byte format consisting of a count byte followed by a byte containing a color index. For more information, see Bitmap Compression.  
            BI_RLE4 = 2, // An RLE format for bitmaps with 4 bpp. The compression format is a 2-byte format consisting of a count byte followed by two word-length color indexes. For more information, see Bitmap Compression. 
            BI_BITFIELDS = 3, // Specifies that the bitmap is not compressed and that the color table consists of three DWORD color masks that specify the red, green, and blue components, respectively, of each pixel. This is valid when used with 16- and 32-bpp bitmaps. 
            BI_JPEG = 4, // Windows 98/Me, Windows 2000/XP: Indicates that the image is a JPEG image. 
            BI_PNG = 5, // Windows 98/Me, Windows 2000/XP: Indicates that the image is a PNG image.
        };

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/ms532290.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct BITMAPINFOHEADER
        {
            public UInt32 biSize;
            public Int32 biWidth;
            public Int32 biHeight;
            public UInt16 biPlanes;
            public UInt16 biBitCount;
            public UInt32 biCompression;
            public UInt32 biSizeImage;
            public Int32 biXPelsPerMeter;
            public Int32 biYPelsPerMeter;
            public UInt32 biClrUsed;
            public UInt32 biClrImportant;

            public BitmapCompression BitmapCompression
            {
                get
                {
                    return (BitmapCompression) biCompression;
                }
            }
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/ms997538.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct RGBQUAD
        {
            public Byte rgbBlue;
            public Byte rgbGreen;
            public Byte rgbRed;
            public Byte rgbReserved;
        }
    }
}
