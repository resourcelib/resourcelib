using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// Dialog template utility functions.
    /// </summary>
    internal abstract class DialogTemplateUtil
    {
        /// <summary>
        /// Read a dialog resource id.       
        /// </summary>
        /// <param name="lpRes">Address in memory.</param>
        /// <param name="rc">Resource read.</param>
        /// <returns></returns>
        internal static IntPtr ReadResourceId(IntPtr lpRes, out ResourceId rc)
        {
            rc = null;

            switch ((UInt16) Marshal.ReadInt16(lpRes))
            {
                case 0x0000: // no predefined resource
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                case 0xFFFF: // one additional element that specifies the ordinal value of the resource
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    rc = new ResourceId((UInt16)Marshal.ReadInt16(lpRes));
                    lpRes = new IntPtr(lpRes.ToInt32() + 2);
                    break;
                default: // null-terminated Unicode string that specifies the name of the resource
                    rc = new ResourceId(Marshal.PtrToStringUni(lpRes));
                    lpRes = new IntPtr(lpRes.ToInt32() + (rc.Name.Length + 1) * 2);
                    break;
            }

            return lpRes;
        }

        internal static void WriteResourceId(BinaryWriter w, ResourceId rc)
        {
            if (rc == null)
            {
                w.Write((UInt16) 0);
            }
            else if (rc.IsIntResource())
            {
                w.Write((UInt16) 0xFFFF);
                w.Write((UInt16) rc.Id);
            }
            else
            {
                ResourceUtil.PadToWORD(w);
                w.Write(Encoding.Unicode.GetBytes(rc.Name));
                w.Write((UInt16)0);
            }
        }

        /// <summary>
        /// String representation of the dialog or control style.
        /// </summary>
        /// <param name="style">Dialog or control style.</param>
        /// <returns>String in the "s1 | s2 | ... | s3" format.</returns>
        internal static List<string> StyleToStringList<T>(UInt32 style)
        {
            List<string> styles = new List<string>();

            foreach (T s in Enum.GetValues(typeof(T)))
            {
                UInt32 s_ui = Convert.ToUInt32(s);
                // style enum may be a flag or equal (including zero)
                if ((style & s_ui) > 0 || style == s_ui)
                {
                    styles.Add(s.ToString());
                }
            }

            return styles;
        }

        /// <summary>
        /// String representation of the dialog or control style of two types.
        /// </summary>
        /// <param name="style">Dialog or control style.</param>
        /// <returns>String in the "s1 | s2 | ... | s3" format.</returns>
        internal static string StyleToString<W, D>(UInt32 style)
        {
            List<string> styles = new List<string>();
            styles.AddRange(StyleToStringList<W>(style));
            styles.AddRange(StyleToStringList<D>(style));
            return String.Join(" | ", styles.ToArray());
        }

        /// <summary>
        /// String representation of the dialog or control styles of two types.
        /// </summary>
        /// <param name="style">Dialog or control style.</param>
        /// <param name="exstyle">Dialog or control extended style.</param>
        /// <returns>String in the "s1 | s2 | ... | s3" format.</returns>
        internal static string StyleToString<W, D>(UInt32 style, UInt32 exstyle)
        {
            List<string> styles = new List<string>();
            styles.AddRange(StyleToStringList<W>(style));
            styles.AddRange(StyleToStringList<D>(exstyle));
            return String.Join(" | ", styles.ToArray());
        }

        /// <summary>
        /// String representation of the dialog or control style of one type.
        /// </summary>
        /// <param name="style">Dialog or control style.</param>
        /// <returns>String in the "s1 | s2 | ... | s3" format.</returns>
        internal static string StyleToString<W>(UInt32 style)
        {
            List<string> styles = new List<string>();
            styles.AddRange(StyleToStringList<W>(style));
            return String.Join(" | ", styles.ToArray());
        }
    }
}
