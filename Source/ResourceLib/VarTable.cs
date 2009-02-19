using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a file-version resource. It typically contains a 
    /// list of language and code page identifier pairs that the version of the application or DLL supports.
    /// http://msdn.microsoft.com/en-us/library/bb202818.aspx
    /// </summary>
    public class VarTable : ResourceTable
    {
        private Dictionary<UInt16, UInt16> _languages = new Dictionary<UInt16, UInt16>();

        public Dictionary<UInt16, UInt16> Languages
        {
            get
            {
                return _languages;
            }
        }

        public VarTable()
        {

        }

        public VarTable(string key)
            : base(key)
        {

        }

        public VarTable(IntPtr lpRes)
        {
            Read(lpRes);
        }

        public override IntPtr Read(IntPtr lpRes)
        {
            _languages.Clear();
            IntPtr pVar = base.Read(lpRes);

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
            long headerPos = w.BaseStream.Position;
            base.Write(w);

            Dictionary<UInt16, UInt16>.Enumerator languagesEnum = _languages.GetEnumerator();
            long valuePos = w.BaseStream.Position;
            while (languagesEnum.MoveNext())
            {
                // id
                w.Write((UInt16) languagesEnum.Current.Key);
                // code page
                w.Write((UInt16) languagesEnum.Current.Value);
            }

            ResourceUtil.WriteAt(w, w.BaseStream.Position - valuePos, headerPos + 2);
            ResourceUtil.PadToDWORD(w);
            ResourceUtil.WriteAt(w, w.BaseStream.Position - headerPos, headerPos);
        }

        public UInt16 this[UInt16 key]
        {
            get
            {
                return _languages[key];
            }
            set
            {
                _languages[key] = value;
            }
        }
    }
}
