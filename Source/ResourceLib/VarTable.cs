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
    public class VarTable : ResourceTableHeader
    {
        private Dictionary<UInt16, UInt16> _languages = new Dictionary<UInt16, UInt16>();

        /// <summary>
        /// A dictionary of language and code page identifier pairs.
        /// </summary>
        public Dictionary<UInt16, UInt16> Languages
        {
            get
            {
                return _languages;
            }
        }

        /// <summary>
        /// A new table of language and code page identifier pairs.
        /// </summary>
        public VarTable()
        {

        }

        /// <summary>
        /// A new table of language and code page identifier pairs.
        /// </summary>
        /// <param name="key">Table key.</param>
        public VarTable(string key)
            : base(key)
        {

        }

        /// <summary>
        /// An existing table of language and code page identifier pairs.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of the data.</param>
        internal VarTable(IntPtr lpRes)
        {
            Read(lpRes);
        }

        /// <summary>
        /// Read a table of language and code page identifier pairs.
        /// </summary>
        /// <param name="lpRes">Pointer to the beginning of the data.</param>
        /// <returns></returns>
        internal override IntPtr Read(IntPtr lpRes)
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

        /// <summary>
        /// Write the table of language and code page identifier pairs to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
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

        /// <summary>
        /// Returns a code page identifier for a given language.
        /// </summary>
        /// <param name="key">Language ID.</param>
        /// <returns>Code page identifier.</returns>
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
