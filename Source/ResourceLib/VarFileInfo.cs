using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a file-version resource. 
    /// It contains version information not dependent on a particular language and code page combination.
    /// http://msdn.microsoft.com/en-us/library/aa909193.aspx
    /// </summary>
    public class VarFileInfo : ResourceTable
    {
        Dictionary<string, VarTable> _variables = new Dictionary<string, VarTable>();

        public Dictionary<string, VarTable> Vars
        {
            get
            {
                return _variables;
            }
        }

        public VarFileInfo()
            : base("VarFileInfo")
        {
            _header.wType = (UInt16)Kernel32.RESOURCE_HEADER_TYPE.StringData;
        }

        public VarFileInfo(IntPtr lpRes)
        {
            Read(lpRes);
        }

        public override IntPtr Read(IntPtr lpRes)
        {
            _variables.Clear();
            IntPtr pChild = base.Read(lpRes);

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

        public VarTable Default
        {
            get
            {
                Dictionary<string, VarTable>.Enumerator variablesEnum = _variables.GetEnumerator();
                if (variablesEnum.MoveNext()) return variablesEnum.Current.Value;
                return null;
            }
        }

        public UInt16 this[UInt16 language]
        {
            get
            {
                return Default[language];
            }
            set
            {
                Default[language] = value;
            }
        }
    }
}
