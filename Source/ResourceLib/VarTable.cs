using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A variable information block
    /// </summary>
    public class VarTable : ResourceTable
    {
        private Dictionary<UInt16, UInt16> _languages;

        public Dictionary<UInt16, UInt16> Languages
        {
            get
            {
                return _languages;
            }
        }

        public VarTable(IntPtr lpRes)
        {
            Load(lpRes);
        }

        public override IntPtr Load(IntPtr lpRes)
        {
            _languages = new Dictionary<UInt16, UInt16>();
            IntPtr pVar = base.Load(lpRes);

            while (pVar.ToInt32() < (lpRes.ToInt32() + _header.wLength))
            {
                Kernel32.VAR_HEADER var = (Kernel32.VAR_HEADER) Marshal.PtrToStructure(
                    pVar, typeof(Kernel32.VAR_HEADER));
                _languages.Add(var.wLanguageIDMS, var.wCodePageIBM);
                pVar = new IntPtr(pVar.ToInt32() + Marshal.SizeOf(var));
            }

            return new IntPtr(lpRes.ToInt32() + _header.wLength);
        }

        public override void Write(BinaryWriter w)
        {
            base.Write(w);

            Dictionary<UInt16, UInt16>.Enumerator languagesEnum = _languages.GetEnumerator();
            while (languagesEnum.MoveNext())
            {
                // id
                w.Write((UInt16) languagesEnum.Current.Key);
                // code page
                w.Write((UInt16) languagesEnum.Current.Value);
            }
        }
    }
}
