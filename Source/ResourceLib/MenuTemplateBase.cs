using System;
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
        public abstract IntPtr Read(IntPtr lpRes);
    }
}
