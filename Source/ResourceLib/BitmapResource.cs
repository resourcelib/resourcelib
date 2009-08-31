using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// An embedded bitmap resource.
    /// </summary>
    public class BitmapResource : Resource
    {
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
            w.Write(_bitmap.Data);
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
