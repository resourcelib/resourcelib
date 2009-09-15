using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A font, RT_FONT resource.
    /// </summary>
    public class FontResource : GenericResource
    {
        /// <summary>
        /// A new font resource.
        /// </summary>
        public FontResource()
            : base(IntPtr.Zero,
                IntPtr.Zero,
                new ResourceId(Kernel32.ResourceTypes.RT_FONT),
                null,
                ResourceUtil.NEUTRALLANGID,
                0)
        {

        }

        /// <summary>
        /// An existing font resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        public FontResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary>
        /// Read the font resource.
        /// </summary>
        /// <param name="hModule">Handle to a module.</param>
        /// <param name="lpRes">Pointer to the beginning of the font structure.</param>
        /// <returns>Address of the end of the font structure.</returns>
        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            return base.Read(hModule, lpRes);
        }

        /// <summary>
        /// Write the font resource to a binary writer.
        /// </summary>
        /// <param name="w">Binary writer.</param>
        internal override void Write(BinaryWriter w)
        {
            base.Write(w);
        }
    }
}
