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
        /// <param name="language">Language id.</param>
        /// <param name="size">Resource size.</param>
        public GenericResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {
            Read(hModule, hResource);
        }

        /// <summary>
        /// A generic resource.
        /// </summary>
        /// <param name="name">Resource name.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="language">Resource language.</param>
        public GenericResource(ResourceId type, ResourceId name, UInt16 language)
        {
            _type = type;
            _name = name;
            _language = language;
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
            w.Write(_data);
        }

        /// <summary>
        /// Save a generic reosurce.
        /// </summary>
        /// <param name="filename">Name of an executable file (.exe or .dll).</param>
        public void SaveTo(string filename)
        {
            base.SaveTo(filename,
                _name,
                _type,
                _language);
        }

        /// <summary>
        /// Load a generic resource.
        /// </summary>
        /// <param name="filename">Source file.</param>
        /// <param name="lang">Resource language.</param>
        public void LoadFrom(string filename)
        {
            base.LoadFrom(
                filename, 
                _name,
                _type,
                _language);
        }
    }
}
