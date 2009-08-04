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

        /// <summary>
        /// Icon header.
        /// </summary>
        public Kernel32.FILEGRPICONDIRENTRY Header
        {
            get
            {
                return _header;
            }
        }

        /// <summary>
        /// Icon bitmap.
        /// </summary>
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
        /// New icon data.
        /// </summary>
        public IconFileIcon()
        {

        }

        /// <summary>
        /// Icon width.
        /// </summary>
        public Byte Width
        {
            get
            {
                return _header.bWidth;
            }
        }

        /// <summary>
        /// Icon height.
        /// </summary>
        public Byte Height
        {
            get
            {
                return _header.bHeight;
            }
        }

        /// <summary>
        /// Image size in bytes.
        /// </summary>
        public UInt32 ImageSize
        {
            get
            {
                return _header.dwImageSize;
            }
        }

        /// <summary>
        /// Read a single icon (.ico).
        /// </summary>
        /// <param name="lpData">Pointer to the beginning of this icon's data.</param>
        /// <param name="lpAllData">Pointer to the beginning of all icon data.</param>
        /// <returns>Pointer to the end of this icon's data.</returns>
        internal IntPtr Read(IntPtr lpData, IntPtr lpAllData)
        {
            _header = (Kernel32.FILEGRPICONDIRENTRY)Marshal.PtrToStructure(
                lpData, typeof(Kernel32.FILEGRPICONDIRENTRY));

            IntPtr lpImage = new IntPtr(lpAllData.ToInt32() + _header.dwFileOffset);
            _image.Read(lpImage, _header.dwImageSize);

            return new IntPtr(lpData.ToInt32() + Marshal.SizeOf(_header));
        }

        /// <summary>
        /// Icon size as a string.
        /// </summary>
        /// <returns>Icon size in the width x height format.</returns>
        public override string ToString()
        {
            return string.Format("{0}x{1}", Width, Height);
        }

        /// <summary>
        /// Convert into an icon resource that can be written into an executable.
        /// </summary>
        /// <param name="id">Icon ID.</param>
        /// <returns>An icon resource.</returns>
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
