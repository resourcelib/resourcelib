using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A variable information block
    /// </summary>
    public class VariableTableResource : Resource
    {
        Kernel32.STRING_OR_VAR_INFO_HEADER _blockInfo;
        private string _blockKey;
        private Dictionary<UInt16, UInt16> _languages;

        public string BlockKey
        {
            get
            {
                return _blockKey;
            }
        }

        public Dictionary<UInt16, UInt16> Languages
        {
            get
            {
                return _languages;
            }
        }

        public VariableTableResource(IntPtr hResource, UInt16 type, string name, UInt16 wIDLanguage, int size)
            : base(hResource, new IntPtr(type), Marshal.StringToHGlobalAuto(name), wIDLanguage, size)
        {
            Load(hResource);
        }

        public void Load(IntPtr lpRes)
        {
            _blockInfo = (Kernel32.STRING_OR_VAR_INFO_HEADER)Marshal.PtrToStructure(
                lpRes, typeof(Kernel32.STRING_OR_VAR_INFO_HEADER));
            IntPtr pBlockKey = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_blockInfo));
            _blockKey = Marshal.PtrToStringUni(pBlockKey);

            _languages = new Dictionary<UInt16, UInt16>();

            IntPtr pVarData = ResourceUtil.Align(pBlockKey.ToInt32() + (_blockKey.Length + 1) * 2);
            IntPtr pVar = pVarData;
            while (pVar.ToInt32() < (pVarData.ToInt32() + _blockInfo.wValueLength))
            {
                Kernel32.VAR_HEADER var = (Kernel32.VAR_HEADER) Marshal.PtrToStructure(
                    pVar, typeof(Kernel32.VAR_HEADER));
                _languages.Add(var.wLanguageIDMS, var.wCodePageIBM);
                pVar = new IntPtr(pVar.ToInt32() + Marshal.SizeOf(var));
            }
            
        }
    }
}
