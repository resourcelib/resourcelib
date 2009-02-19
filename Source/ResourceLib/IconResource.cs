using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in an icon resource.
    /// </summary>
    public class IconResource : Resource
    {
        private Kernel32.GRPICONDIRENTRY _header;
        private IconImage _image = new IconImage();

        public Kernel32.GRPICONDIRENTRY Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
            }
        }

        public IconImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        /// <summary>
        /// An icon resource
        /// </summary>
        public IconResource(IntPtr hModule, IntPtr hResource, IntPtr type, IntPtr name, UInt16 wIDLanguage, int size)
            : base(hModule, hResource, type, name, wIDLanguage, size)
        {
            IntPtr lpRes = Kernel32.LockResource(hResource);

            if (lpRes == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Read(hModule, lpRes);
        }

        public IconResource()
        {

        }


        public Byte Width
        {
            get
            {
                return _header.bWidth;
            }
            set
            {
                _header.bWidth = value;
            }
        }

        public Byte Height
        {
            get
            {
                return _header.bHeight;
            }
            set
            {
                _header.bHeight = value;
            }
        }

        public UInt32 ImageSize
        {
            get
            {
                return _header.dwImageSize;
            }
        }

        public override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            _header = (Kernel32.GRPICONDIRENTRY)Marshal.PtrToStructure(
                lpRes, typeof(Kernel32.GRPICONDIRENTRY));

            IntPtr hIconInfo = Kernel32.FindResource(
                hModule, (IntPtr) _header.nID, (IntPtr)Kernel32.ResourceTypes.RT_ICON);

            if (hIconInfo == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IntPtr hIconRes = Kernel32.LoadResource(hModule, hIconInfo);
            if (hIconRes == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IntPtr dibBits = Kernel32.LockResource(hIconRes);
            if (dibBits == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            _image.Read(dibBits, (uint) Kernel32.SizeofResource(hModule, hIconRes));

            return new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header));
        }

        public PixelFormat PixelFormat
        {
            get
            {
                switch (_header.wBitsPerPixel)
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

        public override string ToString()
        {
            return string.Format("{0}x{1} {2}",
                Width, Height, PixelFormatString);
        }

        public override void Write(BinaryWriter w)
        {
            w.Write(_header.bWidth);
            w.Write(_header.bHeight);
            w.Write(_header.bColors);
            w.Write(_header.bReserved);
            w.Write(_header.wPlanes);
            w.Write(_header.wBitsPerPixel);
            w.Write(_header.dwImageSize);
            w.Write(_header.nID);
            ResourceUtil.PadToWORD(w);
        }

        public void SaveIconTo(string filename)
        {
            SaveTo(filename, new IntPtr(_header.nID), new IntPtr((uint) Kernel32.ResourceTypes.RT_ICON), Language, _image.Data);
        }
    }
}
