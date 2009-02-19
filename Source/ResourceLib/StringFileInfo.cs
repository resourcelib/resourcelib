using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    public class StringFileInfo : ResourceTable
    {
        Dictionary<string, StringTable> _strings;

        public Dictionary<string, StringTable> Strings
        {
            get
            {
                return _strings;
            }
        }

        public StringFileInfo(IntPtr lpRes)
        {
            Load(lpRes);
        }

        public override IntPtr Load(IntPtr lpRes)
        {
            _strings = new Dictionary<string, StringTable>();
            IntPtr pChild = base.Load(lpRes);

            // read strings, each string is in a structure described in http://msdn.microsoft.com/en-us/library/aa909025.aspx
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

        public string this[string key]
        {
            get
            {
                Dictionary<string, StringTable>.Enumerator enumerator = _strings.GetEnumerator();
                enumerator.MoveNext();
                return enumerator.Current.Value[key];
            }
            set
            {
                Dictionary<string, StringTable>.Enumerator enumerator = _strings.GetEnumerator();
                enumerator.MoveNext();
                enumerator.Current.Value[key] = value;
            }
        }
    }
}
