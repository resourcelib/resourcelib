using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a cursor resource.
    /// </summary>
    public class CursorResource : IconImageResource
    {
        private UInt16 _hotspotx = 0;
        private UInt16 _hotspoty = 0;

        /// <summary>
        /// An existing cursor resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        internal CursorResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary>
        /// A new cursor resource.
        /// </summary>
        public CursorResource()
            : base(new ResourceId(Kernel32.ResourceTypes.RT_CURSOR))
        {

        }
       
        /// <summary>
        /// Convert into an icon resource that can be written into an executable.
        /// </summary>
        /// <param name="icon">Icon image.</param>
        /// <param name="id">Icon Id.</param>
        /// <param name="language">Language.</param>
        /// <returns>An icon resource.</returns>
        public CursorResource(IconFileIcon icon, ResourceId id, UInt16 language)
            : base(icon, new ResourceId(Kernel32.ResourceTypes.RT_CURSOR), id, language)
        {

        }

        /// <summary>
        /// Horizontal hotspot coordinate.
        /// The hot spot of a cursor is the point to which Windows refers in tracking the cursor's position. 
        /// </summary>
        public UInt16 HotspotX
        {
            get
            {
                return _hotspotx;
            }
            set
            {
                _hotspotx = value;
            }
        }

        /// <summary>
        /// Vertical hot spot coordinate.
        /// The hot spot of a cursor is the point to which Windows refers in tracking the cursor's position. 
        /// </summary>
        public UInt16 HotspotY
        {
            get
            {
                return _hotspoty;
            }
            set
            {
                _hotspoty = value;
            }
        }

        /// <summary>
        /// Write the cursor data to a file.
        /// </summary>
        /// <param name="filename">Target executable file.</param>
        public override void SaveIconTo(string filename)
        {
            byte[] dataWithHotspot = new byte[Image.Data.Length + 4];
            Buffer.BlockCopy(Image.Data, 0, dataWithHotspot, 4, Image.Data.Length);
            dataWithHotspot[0] = (byte)(HotspotX & 0xFF);
            dataWithHotspot[1] = (byte)(HotspotX >> 8);
            dataWithHotspot[2] = (byte)(HotspotY & 0xFF);
            dataWithHotspot[3] = (byte)(HotspotY >> 8);

            SaveTo(filename,
                _type,
                new ResourceId(_header.nID),
                _language,
                dataWithHotspot);
        }

        /// <summary>
        /// Read DIB image.
        /// </summary>
        /// <param name="dibBits">DIB bits.</param>
        /// <param name="size">DIB size.</param>
        internal override void ReadImage(IntPtr dibBits, UInt32 size)
        {
            _hotspotx = (UInt16) Marshal.ReadInt16(dibBits);
            dibBits = new IntPtr(dibBits.ToInt32() + sizeof(UInt16));
            _hotspoty = (UInt16) Marshal.ReadInt16(dibBits);
            dibBits = new IntPtr(dibBits.ToInt32() + sizeof(UInt16));

            base.ReadImage(dibBits, size - 2 * sizeof(UInt16));
        }
    }
}
