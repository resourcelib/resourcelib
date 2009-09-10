using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A resource table.
    /// </summary>
    public class ResourceTable
    {
        /// <summary>
        /// Resource table header.
        /// </summary>
        protected Kernel32.RESOURCE_HEADER _header;

        /// <summary>
        /// Resource table key.
        /// </summary>
        protected string _key;

        /// <summary>
        /// Resource table key.
        /// </summary>
        public string Key
        {
            get
            {
                return _key;
            }
        }

        /// <summary>
        /// Resource header.
        /// </summary>
        public Kernel32.RESOURCE_HEADER Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
            }
        }

        /// <summary>
        /// A new resource table.
        /// </summary>
        public ResourceTable()
        {

        }

        /// <summary>
        /// An existing resource table.
        /// </summary>
        /// <param name="key">resource key</param>
        public ResourceTable(string key)
        {
            _key = key;
        }

        /// <summary>
        /// An existing resource table.
        /// </summary>
        /// <param name="lpRes">Pointer to resource table data.</param>
        internal ResourceTable(IntPtr lpRes)
        {
            Read(lpRes);
        }

        /// <summary>
        /// Read the resource header, return a pointer to the end of it.
        /// </summary>
        /// <param name="lpRes">Top of header.</param>
        /// <returns>End of header, after the key, aligned at a 32 bit boundary.</returns>
        internal virtual IntPtr Read(IntPtr lpRes)
        {
            _header = (Kernel32.RESOURCE_HEADER) Marshal.PtrToStructure(
                lpRes, typeof(Kernel32.RESOURCE_HEADER));

            IntPtr pBlockKey = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header));
            _key = Marshal.PtrToStringUni(pBlockKey);

            return ResourceUtil.Align(pBlockKey.ToInt32() + (_key.Length + 1) * Marshal.SystemDefaultCharSize);
        }

        /// <summary>
        /// Write the resource table.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal virtual void Write(BinaryWriter w)
        {
            // wLength
            w.Write((UInt16)_header.wLength);
            // wValueLength
            w.Write((UInt16)_header.wValueLength);
            // wType
            w.Write((UInt16)_header.wType);
            // write key
            w.Write(Encoding.Unicode.GetBytes(_key));
            // null-terminator
            w.Write((UInt16)0);
            // pad fixed info
            ResourceUtil.PadToDWORD(w);
        }
    }
}
