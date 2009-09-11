using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A menu template header.
    /// </summary>
    public abstract class MenuTemplateBase
    {
        /// <summary>
        /// Read the menu template header.
        /// </summary>
        /// <param name="lpRes">Address in memory.</param>
        internal abstract IntPtr Read(IntPtr lpRes);

        /// <summary>
        /// Write the menu to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal abstract void Write(BinaryWriter w);
    }
}
