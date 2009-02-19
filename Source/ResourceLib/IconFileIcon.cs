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
    /// This structure depicts the organization of icon data in a .ico file.
    /// </summary>
    public class IconFileIcon 
    {
        private Kernel32.FILEGRPICONDIRENTRY _header;
        private IconImage _image = new IconImage();

        public Kernel32.FILEGRPICONDIRENTRY Header
        {
            get
            {
                return _header;
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

        public IconFileIcon()
        {

        }

        public Byte Width
        {
            get
            {
                return _header.bWidth;
            }
        }

        public Byte Height
        {
            get
            {
                return _header.bHeight;
            }
        }

        public UInt32 ImageSize
        {
            get
            {
                return _header.dwImageSize;
            }
        }

        public IntPtr Read(IntPtr lpData, IntPtr lpAllData)
        {
            _header = (Kernel32.FILEGRPICONDIRENTRY)Marshal.PtrToStructure(
                lpData, typeof(Kernel32.FILEGRPICONDIRENTRY));

            IntPtr lpImage = new IntPtr(lpAllData.ToInt32() + _header.dwFileOffset);
            _image.Read(lpImage, _header.dwImageSize);

            return new IntPtr(lpData.ToInt32() + Marshal.SizeOf(_header));
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", Width, Height);
        }

        /// <summary>
        /// Convert into an icon resource that can be written into a .dll or .exe.
        /// </summary>
        public IconResource ConvertToIconResource(UInt16 id)
        {
            IconResource iconResource = new IconResource();
            Kernel32.GRPICONDIRENTRY header = new Kernel32.GRPICONDIRENTRY();
            header.bColors = _header.bColors;
            header.bHeight = _header.bHeight;
            header.bReserved = _header.bReserved;
            header.bWidth = _header.bWidth;
            header.dwImageSize = _header.dwImageSize;
            header.wBitsPerPixel = _header.wBitsPerPixel;
            header.wPlanes = _header.wPlanes;
            header.nID = id;
            iconResource.Header = header;
            iconResource.Image = _image;
            return iconResource;
        }
    }
}
