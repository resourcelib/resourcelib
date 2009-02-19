using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    public class VarFileInfo : ResourceTable
    {
        Dictionary<string, VarTable> _variables;

        public Dictionary<string, VarTable> Vars
        {
            get
            {
                return _variables;
            }
        }

        public VarFileInfo(IntPtr lpRes)
        {
            Load(lpRes);
        }

        public override IntPtr Load(IntPtr lpRes)
        {
            _variables = new Dictionary<string, VarTable>();
            IntPtr pChild = base.Load(lpRes);

            while (pChild.ToInt32() < (lpRes.ToInt32() + _header.wLength))
            {
                VarTable res = new VarTable(pChild);
                _variables.Add(res.Key, res);
                pChild = ResourceUtil.Align(pChild.ToInt32() + res.Header.wLength);
            }

            return new IntPtr(lpRes.ToInt32() + _header.wLength);
        }

        public override void Write(BinaryWriter w)
        {
            long headerPos = w.BaseStream.Position;
            base.Write(w);

            Dictionary<string, VarTable>.Enumerator variablesEnum = _variables.GetEnumerator();
            while (variablesEnum.MoveNext())
            {
                variablesEnum.Current.Value.Write(w);
            }
            ResourceUtil.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
        }
    }
}
