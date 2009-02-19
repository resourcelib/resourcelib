using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    public class StringResource
    {
        Kernel32.RESOURCE_HEADER _header;
        string _key;
        string _value;

        public Kernel32.RESOURCE_HEADER Header
        {
            get
            {
                return _header;
            }
        }

        public string Key
        {
            get
            {
                return _key;
            }
        }

        public string StringValue
        {
            get
            {
                if (null == _value)
                    return null;

                return _value.Trim("\0".ToCharArray());
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
        }

        public StringResource(IntPtr lpRes)
        {
            Load(lpRes);
        }

        public void Load(IntPtr lpRes)
        {
            _header = (Kernel32.RESOURCE_HEADER)Marshal.PtrToStructure(
                lpRes, typeof(Kernel32.RESOURCE_HEADER));

            IntPtr pKey = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header));
            _key = Marshal.PtrToStringUni(pKey);

            IntPtr pValue = ResourceUtil.Align(pKey.ToInt32() + (_key.Length + 1) * 2);
            _value = _header.wValueLength > 0 ? Marshal.PtrToStringUni(pValue, _header.wValueLength) : null;
        }

        public void Write(BinaryWriter w)
        {
            // write the block info
            long wStringLengthPos = w.BaseStream.Position;
            // wLength
            w.Write((UInt16)_header.wLength);
            // wValueLength
            w.Write((UInt16)_header.wValueLength);
            // wType
            w.Write((UInt16)_header.wType);
            // szKey
            w.Write(Encoding.Unicode.GetBytes(_key));
            // null terminator
            w.Write((UInt16) 0);
            // pad fixed info
            ResourceUtil.PadToDWORD(w);
            if (_header.wValueLength > 0 && _value != null)
            {
                // Value
                w.Write(Encoding.Unicode.GetBytes(_value));
            }
            ResourceUtil.PadToDWORD(w);
        }
    }
}
