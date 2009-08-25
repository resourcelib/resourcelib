using System;
using System.Collections.Generic;
using System.Text;

namespace Vestris.ResourceLib
{
    public class IconResource : IconImageResource
    {
        /// <summary>
        /// An existing icon resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        internal IconResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary>
        /// A new icon resource.
        /// </summary>
        public IconResource()
            : base(new ResourceId(Kernel32.ResourceTypes.RT_ICON))
        {

        }

        /// <summary>
        /// Convert into an icon resource that can be written into an executable.
        /// </summary>
        /// <param name="type">Icon type.</param>
        /// <param name="id">Icon ID.</param>
        /// <returns>An icon resource.</returns>
        public IconResource(IconFileIcon icon, ResourceId id)
            : base(icon, new ResourceId(Kernel32.ResourceTypes.RT_ICON), id)
        {

        }
    }
}
