using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a file-version resource. It contains language 
    /// and code page formatting information for the strings. A code page is an ordered character set.
    /// See http://msdn.microsoft.com/en-us/library/aa909192.aspx for more information.
    /// </summary>
    public class StringTable : ResourceTableHeader
    {
        Dictionary<string, StringResource> _strings = new Dictionary<string,StringResource>();

        /// <summary>
        /// Resource strings.
        /// </summary>
        public Dictionary<string, StringResource> Strings
        {
            get
            {
                return _strings;
            }
        }

        /// <summary>
        /// A new string table.
        /// </summary>
        public StringTable()
        {
        
        }

        /// <summary>
        /// A new string table.
        /// </summary>
        /// <param name="key">String table key.</param>
        public StringTable(string key)
            : base(key)
        {
            _header.wType = (UInt16)Kernel32.RESOURCE_HEADER_TYPE.StringData;
        }

        /// <summary>
        /// An existing string table.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of the table.</param>
        internal StringTable(IntPtr lpRes)
        {
            Read(lpRes);
        }

        /// <summary>
        /// Read a string table.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of the string table.</param>
        /// <returns>Pointer to the end of the string table.</returns>
        internal override IntPtr Read(IntPtr lpRes)
        {
            _strings.Clear();
            IntPtr pChild = base.Read(lpRes);

            while (pChild.ToInt32() < (lpRes.ToInt32() + _header.wLength))
            {
                StringResource res = new StringResource(pChild);
                _strings.Add(res.Key, res);
                pChild = ResourceUtil.Align(pChild.ToInt32() + res.Header.wLength);
            }

            return new IntPtr(lpRes.ToInt32() + _header.wLength);
        }

        /// <summary>
        /// Write the string table to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            base.Write(w);

            Dictionary<string, StringResource>.Enumerator stringsEnum = _strings.GetEnumerator();
            while (stringsEnum.MoveNext())
            {
                stringsEnum.Current.Value.Write(w);
            }

            ResourceUtil.PadToDWORD(w);
            ResourceUtil.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
        }
        
        /// <summary>
        /// The four most significant digits of the key represent the language identifier.
        /// Each Microsoft Standard Language identifier contains two parts: the low-order 10 bits 
        /// specify the major language, and the high-order 6 bits specify the sublanguage.
        /// </summary>
        public UInt16 LanguageID
        {
            get
            {
                if (string.IsNullOrEmpty(_key)) 
                    return 0;

                return Convert.ToUInt16(_key.Substring(0, 4), 16);
            }
            set
            {
                _key = string.Format("{0:x4}{1:x4}", value, CodePage);
            }
        }

        /// <summary>
        /// The four least significant digits of the key represent the code page for which the data is formatted.
        /// </summary>
        public UInt16 CodePage
        {
            get
            {
                if (string.IsNullOrEmpty(_key))
                    return 0;

                return Convert.ToUInt16(_key.Substring(4, 4), 16);
            }
            set
            {
                _key = string.Format("{0:x4}{1:x4}", LanguageID, value);
            }
        }

        /// <summary>
        /// Returns an entry within the string table.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>An entry within the string table.</returns>
        public string this[string key]
        {
            get
            {
                return _strings[key].Value;
            }
            set
            {
                StringResource sr = null;
                if (!_strings.TryGetValue(key, out sr))
                {
                    sr = new StringResource(key);
                    _strings.Add(key, sr);
                }

                sr.Value = value;
            }
        }
    }
}
