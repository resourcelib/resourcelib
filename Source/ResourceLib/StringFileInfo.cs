using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a file-version resource. 
    /// It contains version information that can be displayed for a particular language and code page.
    /// http://msdn.microsoft.com/en-us/library/aa908808.aspx
    /// </summary>
    public class StringFileInfo : ResourceTable
    {
        Dictionary<string, StringTable> _strings = new Dictionary<string, StringTable>();

        public Dictionary<string, StringTable> Strings
        {
            get
            {
                return _strings;
            }
        }

        public StringFileInfo()
            : base("StringFileInfo")
        {
            StringTable defaultStringTable = new StringTable("040904b0");
            _strings["040904B0"] = defaultStringTable;
        }

        public StringFileInfo(IntPtr lpRes)
        {
            Read(lpRes);
        }

        public override IntPtr Read(IntPtr lpRes)
        {
            _strings.Clear();
            IntPtr pChild = base.Read(lpRes);

            while (pChild.ToInt32() < (lpRes.ToInt32() + _header.wLength))
            {
                StringTable res = new StringTable(pChild);
                _strings.Add(res.Key, res);
                pChild = ResourceUtil.Align(pChild.ToInt32() + res.Header.wLength);
            }

            return new IntPtr(lpRes.ToInt32() + _header.wLength);
        }

        public override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            base.Write(w);

            Dictionary<string, StringTable>.Enumerator stringsEnum = _strings.GetEnumerator();
            while (stringsEnum.MoveNext())
            {
                stringsEnum.Current.Value.Write(w);
            }

            ResourceUtil.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
        }

        public StringTable Default
        {
            get
            {
                Dictionary<string, StringTable>.Enumerator iter = _strings.GetEnumerator();
                if (iter.MoveNext()) return iter.Current.Value;
                return null;
            }
        }

        public string this[string key]
        {
            get
            {
                return Default[key];
            }
            set
            {
                Default[key] = value;
            }
        }
    }
}
