using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    public class StringTable : ResourceTable
    {
        Dictionary<string, StringResource> _strings;

        public Dictionary<string, StringResource> Strings
        {
            get
            {
                return _strings;
            }
        }

        public StringTable(IntPtr lpRes)
        {
            Load(lpRes);
        }

        public override IntPtr Load(IntPtr lpRes)
        {
            _strings = new Dictionary<string, StringResource>();
            IntPtr pChild = base.Load(lpRes);

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
            base.Write(w);

            Dictionary<string, StringResource>.Enumerator stringsEnum = _strings.GetEnumerator();
            while (stringsEnum.MoveNext())
            {
                stringsEnum.Current.Value.Write(w);
            }
        }
    }
}
