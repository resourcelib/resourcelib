using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A placeholder for all unknown resources.
    /// </summary>
    public class UnknownResource : Resource
    {
        /// <summary>
        /// A structured resource embedded in an executable module.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource handle.</param>
        /// <param name="type">Type of resource.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="wIDLanguage">Language id.</param>
        /// <param name="size">Resource size.</param>
        public UnknownResource(IntPtr hModule, IntPtr hResource, IntPtr type, IntPtr name, UInt16 wIDLanguage, int size)
            : base(hModule, hResource, type, name, wIDLanguage, size)
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write the resource to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            throw new NotImplementedException();
        }
    }
}
