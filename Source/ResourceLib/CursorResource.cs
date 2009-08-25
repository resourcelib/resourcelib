using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.ResourceLib
{
    public class CursorResource : IconImageResource
    {
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
        /// <param name="type">Icon type.</param>
        /// <param name="id">Icon ID.</param>
        /// <returns>An icon resource.</returns>
        public CursorResource(IconFileIcon icon, ResourceId id)
            : base(icon, new ResourceId(Kernel32.ResourceTypes.RT_CURSOR), id)
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
                return (UInt16) ((Image.Data[1] << 8)
                    + Image.Data[0]);
            }
            set
            {
                Image.Data[0] = (byte)(value & 0xFF);
                Image.Data[1] = (byte)(value >> 8);
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
                return (UInt16)((Image.Data[3] << 8)
                    + Image.Data[2]);
            }
            set
            {
                Image.Data[2] = (byte)(value & 0xFF);
                Image.Data[3] = (byte)(value >> 8);
            }
        }
    }
}
