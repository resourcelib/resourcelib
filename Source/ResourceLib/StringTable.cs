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
    /// http://msdn.microsoft.com/en-us/library/aa909192.aspx
    /// </summary>
    public class StringTable : ResourceTable
    {
        Dictionary<string, StringResource> _strings = new Dictionary<string,StringResource>();

        public Dictionary<string, StringResource> Strings
        {
            get
            {
                return _strings;
            }
        }

        public StringTable()
        {
        
        }

        public StringTable(string key)
            : base(key)
        {
            _header.wType = (UInt16)Kernel32.RESOURCE_HEADER_TYPE.StringData;
        }

        public StringTable(IntPtr lpRes)
        {
            Read(lpRes);
        }

        public override IntPtr Read(IntPtr lpRes)
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

        public override void Write(BinaryWriter w)
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
