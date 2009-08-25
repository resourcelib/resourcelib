using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a hardware-independent cursor resource.
    /// </summary>
    public class CursorDirectoryResource : DirectoryResource<CursorResource>
    {
        /// <summary>
        /// A hardware-independent cursor resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        internal CursorDirectoryResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary>
        /// A new hardware-independent cursor resource.
        /// </summary>
        public CursorDirectoryResource()
            : base(Kernel32.ResourceTypes.RT_GROUP_CURSOR)
        {

        }

        /// <summary>
        /// A new collection of cursors that can be embedded into an executable file.
        /// </summary>
        public CursorDirectoryResource(IconFile iconFile)
            : base(Kernel32.ResourceTypes.RT_GROUP_CURSOR)
        {            
            for (UInt16 id = 0; id < iconFile.Icons.Count; id++)
            {
                CursorResource cursorResource = new CursorResource(
                    iconFile.Icons[id], new ResourceId(id));
                // add hotspot data on top of the resource, not present in the same structure in the .cur file
                byte[] dataWithHotspot = new byte[cursorResource.Image.Data.Length + 4];
                Buffer.BlockCopy(cursorResource.Image.Data, 0, dataWithHotspot, 4, cursorResource.Image.Data.Length);
                cursorResource.ImageSize = (UInt32) dataWithHotspot.Length;
                cursorResource.Image.Data = dataWithHotspot;
                // cursor structure abuses planes and bits per pixel for cursor data
                cursorResource.HotspotX = iconFile.Icons[id].Header.wPlanes;
                cursorResource.HotspotY = iconFile.Icons[id].Header.wBitsPerPixel;
                Icons.Add(cursorResource);
            }
        }
    }
}
