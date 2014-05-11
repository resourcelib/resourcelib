using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A string, RT_STRING resource.
    /// Each string resource block has 16 strings, each represented as an ordered pair 
    /// (length, text). Length is a WORD that specifies the size, in terms of the number of characters, 
    /// in the text that follows. Text follows length and contains the string in Unicode without the 
    /// NULL terminating character. There may be no characters in text, in which case length is zero.
    /// </summary>
    public class StringResource : Resource
    {
        private Dictionary<UInt16, string> _strings = new Dictionary<UInt16, string>();

        /// <summary>
        /// String collection in this resource.
        /// </summary>
        public Dictionary<UInt16, string> Strings
        {
            get
            {
                return _strings;
            }
            set
            {
                _strings = value;
            }
        }

        /// <summary>
        /// Returns a string of a given Id.
        /// </summary>
        /// <param name="id">String Id.</param>
        /// <returns>A string of a given Id.</returns>
        public string this[UInt16 id]
        {
            get
            {
                return _strings[id];
            }
            set
            {
                _strings[id] = value;
            }
        }

        /// <summary>
        /// A new string resource.
        /// </summary>
        public StringResource()
            : base(IntPtr.Zero,
                IntPtr.Zero,
                new ResourceId(Kernel32.ResourceTypes.RT_STRING),
                null,
                ResourceUtil.NEUTRALLANGID,
                0)
        {

        }

        /// <summary>
        /// A new string resource of a given block id.
        /// </summary>
        /// <param name="blockId">Block id.</param>
        public StringResource(ResourceId blockId)
            : base(IntPtr.Zero,
                IntPtr.Zero,
                new ResourceId(Kernel32.ResourceTypes.RT_STRING),
                blockId,
                ResourceUtil.NEUTRALLANGID,
                0)
        {

        }

        /// <summary>
        /// A new string resource of a given block id.
        /// </summary>
        /// <param name="blockId">Block id.</param>
        public StringResource(UInt16 blockId)
            : this(new ResourceId(blockId))
        {

        }

        /// <summary>
        /// An existing string resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        public StringResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary>
        /// A string with ID, stringId, is located in the block with ID given by the following formula.
        /// http://support.microsoft.com/kb/q196774/
        /// </summary>
        public static UInt16 GetBlockId(int stringId)
        {
            return (UInt16)((stringId / 16) + 1);
        }

        /// <summary>
        /// String table block id.
        /// </summary>
        public UInt16 BlockId
        {
            get
            {
                return (UInt16) Name.Id.ToInt64();
            }
            set
            {
                Name = new ResourceId(value);
            }
        }

        /// <summary>
        /// Read the strings.
        /// </summary>
        /// <param name="hModule">Handle to a module.</param>
        /// <param name="lpRes">Pointer to the beginning of the string table.</param>
        /// <returns>Address of the end of the string table.</returns>
        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            for (int i = 0; i < 16; i++)
            {
                UInt16 len = (UInt16)Marshal.ReadInt16(lpRes);
                if (len != 0)
                {
                    UInt16 id = (UInt16) ((BlockId - 1) * 16 + i);
                    IntPtr lpString = new IntPtr(lpRes.ToInt64() + 2);
                    string s = Marshal.PtrToStringUni(lpString, len);
                    _strings.Add(id, s);
                }
                lpRes = new IntPtr(lpRes.ToInt64() + 2 + (len * Marshal.SystemDefaultCharSize));
            }

            return lpRes;
        }

        internal override void Write(System.IO.BinaryWriter w)
        {
            for (int i = 0; i < 16; i++)
            {
                UInt16 id = (UInt16)((BlockId - 1) * 16 + i);
                string s = null;
                if (_strings.TryGetValue(id, out s))
                {
                    w.Write((UInt16) s.Length);
                    w.Write(Encoding.Unicode.GetBytes(s));
                }
                else
                {
                    w.Write((UInt16) 0);
                }
            }
        }

        /// <summary>
        /// String representation of the strings resource.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("STRINGTABLE");
            sb.AppendLine("BEGIN");
            Dictionary<UInt16, string>.Enumerator stringEnumerator = _strings.GetEnumerator();
            while (stringEnumerator.MoveNext())
            {
                sb.AppendLine(string.Format(" {0} {1}",
                    stringEnumerator.Current.Key, stringEnumerator.Current.Value));
            }
            sb.AppendLine("END");
            return sb.ToString();
        }
    }
}
