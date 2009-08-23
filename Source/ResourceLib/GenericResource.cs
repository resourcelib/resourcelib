using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A generic resource.
    /// </summary>
    public class GenericResource : Resource
    {
        /// <summary>
        /// Raw resource data.
        /// </summary>
        protected byte[] _data = null;

        /// <summary>
        /// Raw resource data.
        /// </summary>
        public byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// An unstructured generic resource embedded in an executable module.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource handle.</param>
        /// <param name="type">Type of resource.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="wIDLanguage">Language id.</param>
        /// <param name="size">Resource size.</param>
        public GenericResource(IntPtr hModule, IntPtr hResource, IntPtr type, IntPtr name, UInt16 wIDLanguage, int size)
            : base(hModule, hResource, type, name, wIDLanguage, size)
        {
            Read(hModule, hResource);
        }

        /// <summary>
        /// Read a generic resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="lpRes">Pointer to the beginning of a resource.</param>
        /// <returns>Pointer to the end of the resource.</returns>
        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            if (_size > 0)
            {
                _data = new byte[_size];
                Marshal.Copy(lpRes, _data, 0, _data.Length);
            }

            return new IntPtr(lpRes.ToInt32() + _size);
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
