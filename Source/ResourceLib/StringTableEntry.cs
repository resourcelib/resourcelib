using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a file-version resource. It contains a string 
    /// that describes a specific aspect of a file, such as a file's version, its copyright notices, 
    /// or its trademarks.
    /// http://msdn.microsoft.com/en-us/library/aa909025.aspx
    /// </summary>
    public class StringTableEntry
    {
        private Kernel32.RESOURCE_HEADER _header;
        private string _key;

        /// <summary>
        /// The value is always stored double-null-terminated.
        /// </summary>
        private string _value;

        /// <summary>
        /// When set to true the length in the header will also contain the padding bytes when writing to a stream.
        /// The MSDN reference (http://www.webcitation.org/6zBLYbvww) does not clarify which variant is 'right'.
        /// </summary>
        public static bool ConsiderPaddingForLength = false;

        /// <summary>
        /// String resource header.
        /// </summary>
        public Kernel32.RESOURCE_HEADER Header
        {
            get
            {
                return _header;
            }
        }

        /// <summary>
        /// Key.
        /// </summary>
        public string Key
        {
            get
            {
                return _key;
            }
        }

        /// <summary>
        /// String value (removing the double-null-terminator).
        /// </summary>
        public string StringValue
        {
            get
            {
                if (_value == null)
                    return _value;

                return _value.Substring(0, _value.Length - 1);
            }
        }

        /// <summary>
        /// Value.
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == null)
                {
                    _value = null;
                    _header.wValueLength = 0;
                }
                else
                {
                    if (value.Length == 0 || value[value.Length - 1] != '\0')
                        _value = value + '\0';
                    else
                        _value = value;

                    _header.wValueLength = (UInt16) _value.Length;
                }
            }
        }

        /// <summary>
        /// A new string resource.
        /// </summary>
        /// <param name="key">Key.</param>
        public StringTableEntry(string key)
        {
            _key = key;
            _header.wType = 1;
            _header.wLength = 0;
            _header.wValueLength = 0;
        }

        /// <summary>
        /// An existing string resource.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of a string resource.</param>
        internal StringTableEntry(IntPtr lpRes)
        {
            Read(lpRes);
        }

        /// <summary>
        /// Read a string resource.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of a string resource.</param>
        internal void Read(IntPtr lpRes)
        {
            _header = (Kernel32.RESOURCE_HEADER)Marshal.PtrToStructure(
                lpRes, typeof(Kernel32.RESOURCE_HEADER));

            IntPtr pKey = new IntPtr(lpRes.ToInt64() + Marshal.SizeOf(_header));
            _key = Marshal.PtrToStringUni(pKey);

            IntPtr pValue = ResourceUtil.Align(pKey.ToInt64() + (_key.Length + 1) * Marshal.SystemDefaultCharSize);
            if (_header.wValueLength > 0)
            {
                _value = Marshal.PtrToStringUni(pValue, _header.wValueLength);
                if (_value.Length == 0 || _value[_value.Length - 1] != '\0')
                    _value += '\0';
            }
        }

        /// <summary>
        /// Write a string resource to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal void Write(BinaryWriter w)
        {
            // write the block info
            long headerPos = w.BaseStream.Position;
            // wLength
            w.Write((UInt16) _header.wLength);
            // wValueLength
            w.Write((UInt16) _header.wValueLength);
            // wType
            w.Write((UInt16) _header.wType);
            // szKey
            w.Write(Encoding.Unicode.GetBytes(_key));
            // null terminator
            w.Write((UInt16) 0);
            // pad fixed info
            ResourceUtil.PadToDWORD(w);
            long valuePos = w.BaseStream.Position;
            if (_value != null)
            {
                // value (always double-null-terminated)
                w.Write(Encoding.Unicode.GetBytes(_value));
            }
            // wValueLength
            ResourceUtil.WriteAt(w, (w.BaseStream.Position - valuePos) / Marshal.SystemDefaultCharSize, headerPos + 2);

            if (ConsiderPaddingForLength)
                ResourceUtil.PadToDWORD(w);

            // wLength
            ResourceUtil.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
        }
    }
}
