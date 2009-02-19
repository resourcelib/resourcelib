using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    public class ResourceTable
    {
        protected Kernel32.RESOURCE_HEADER _header;
        protected string _key;

        public string Key
        {
            get
            {
                return _key;
            }
        }

        public Kernel32.RESOURCE_HEADER Header
        {
            get
            {
                return _header;
            }
        }

        public ResourceTable()
        {

        }

        public ResourceTable(IntPtr lpRes)
        {
            Load(lpRes);
        }

        /// <summary>
        /// Load the resource header, return a pointer to the end of it.
        /// </summary>
        /// <param name="lpRes">top of header</param>
        /// <returns>end of header, after the key, aligned at 32bit</returns>
        public virtual IntPtr Load(IntPtr lpRes)
        {
            _header = (Kernel32.RESOURCE_HEADER) Marshal.PtrToStructure(
                lpRes, typeof(Kernel32.RESOURCE_HEADER));

            IntPtr pBlockKey = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header));
            _key = Marshal.PtrToStringUni(pBlockKey);

            return ResourceUtil.Align(pBlockKey.ToInt32() + (_key.Length + 1) * 2);
        }

        public virtual void Write(BinaryWriter w)
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
