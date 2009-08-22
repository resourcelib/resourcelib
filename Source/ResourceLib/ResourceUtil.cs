using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// Resource utilities.
    /// </summary>
    public abstract class ResourceUtil
    {
        /// <summary>
        /// Returns true if the IntPtr value represents a resource identifier.
        /// </summary>
        /// <param name="value">Raw pointer.</param>
        /// <returns>True if the pointer is a resource identifier.</returns>
        public static bool IsIntResource(IntPtr value)
        {
            if (((uint)value) > UInt16.MaxValue)
                return false;

            return true;
        }

        /// <summary>
        /// Converts a pointer into a resource identifier.
        /// </summary>
        /// <param name="value">Raw pointer.</param>
        /// <returns>Resource identifier.</returns>
        public static uint GetResourceID(IntPtr value)
        {
            if (IsIntResource(value))
                return (uint)value;

            throw new System.NotSupportedException(value.ToString());
        }

        /// <summary>
        /// Converts a pointer into a resource name.
        /// </summary>
        /// <param name="value">Raw pointer.</param>
        /// <returns>Resource name.</returns>
        public static string GetResourceName(IntPtr value)
        {
            if (IsIntResource(value))
                return value.ToString();

            return Marshal.PtrToStringUni(value);
        }

        /// <summary>
        /// Align an address to a 4-byte boundary.
        /// </summary>
        /// <param name="p">Address in memory.</param>
        /// <returns>4-byte aligned pointer.</returns>
        internal static IntPtr Align(Int32 p)
        {
            return new IntPtr((p + 3) & ~3);
        }

        /// <summary>
        /// Align a pointer to a 4-byte boundary.
        /// </summary>
        /// <param name="p">Pointer to an address in memory.</param>
        /// <returns>4-byte aligned pointer.</returns>
        internal static IntPtr Align(IntPtr p)
        {
            return Align(p.ToInt32());
        }

        /// <summary>
        /// Pad data to a WORD.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        /// <returns>New position within the binary stream.</returns>
        internal static long PadToWORD(BinaryWriter w)
        {
            long pos = w.BaseStream.Position;

            if (pos % 2 != 0)
            {
                long count = 2 - pos % 2;
                Pad(w, (UInt16)count);
                pos += count;
            }

            return pos;
        }

        /// <summary>
        /// Pad data to a DWORD.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        /// <returns>New position within the binary stream.</returns>
        internal static long PadToDWORD(BinaryWriter w)
        {
            long pos = w.BaseStream.Position;

            if (pos % 4 != 0)
            {
                long count = 4 - pos % 4;
                Pad(w, (UInt16) count);
                pos += count;
            }

            return pos;
        }

        /// <summary>
        /// Write a value at a given position.
        /// Used to write a size of data in an earlier located header.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        /// <param name="value">Value to write.</param>
        /// <param name="address">Address to write the value at.</param>
        internal static void WriteAt(BinaryWriter w, long value, long address)
        {
            long cur = w.BaseStream.Position;
            w.Seek((int) address, SeekOrigin.Begin);
            w.Write((UInt16) value);
            w.Seek((int) cur, SeekOrigin.Begin);
        }

        /// <summary>
        /// Pad bytes.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        /// <param name="len">Number of bytes to write.</param>
        /// <returns>New position within the stream.</returns>
        internal static long Pad(BinaryWriter w, UInt16 len)
        {
            while (len-- > 0)
                w.Write((byte) 0);
            return w.BaseStream.Position;
        }

        /// <summary>
        /// Neutral language ID.
        /// </summary>
        public static UInt16 NEUTRALLANGID
        {
            get
            {
                return MAKELANGID(Kernel32.LANG_NEUTRAL, Kernel32.SUBLANG_NEUTRAL);
            }
        }

        /// <summary>
        /// US-English language ID.
        /// </summary>
        public static UInt16 USENGLISHLANGID
        {
            get
            {
                return ResourceUtil.MAKELANGID(Kernel32.LANG_ENGLISH, Kernel32.SUBLANG_ENGLISH_US);
            }
        }

        /// <summary>
        /// Make a language ID from a primary language ID (low-order 10 bits) and a sublanguage (high-order 6 bits).
        /// </summary>
        /// <param name="primary">Primary language ID.</param>
        /// <param name="sub">Sublanguage ID.</param>
        /// <returns>Microsoft language ID.</returns>
        public static UInt16 MAKELANGID(int primary, int sub)
        {
            return (UInt16) ((((UInt16)sub) << 10) | ((UInt16)primary));
        }

        /// <summary>
        /// Return the primary language ID from a Microsoft language ID.
        /// </summary>
        /// <param name="lcid">Microsoft language ID</param>
        /// <returns>primary language ID (low-order 10 bits)</returns>
        public static UInt16 PRIMARYLANGID(UInt16 lcid)
        {
            return (UInt16) (((UInt16)lcid) & 0x3ff);
        }

        /// <summary>
        /// Return the sublanguage ID from a Microsoft language ID.
        /// </summary>
        /// <param name="lcid">Microsoft language ID.</param>
        /// <returns>Sublanguage ID (high-order 6 bits).</returns>
        public static UInt16 SUBLANGID(UInt16 lcid)
        {
            return (UInt16) (((UInt16)lcid) >> 10);
        }

        /// <summary>
        /// Returns the memory representation of an object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="anything">Data.</param>
        /// <returns>Object's representation in memory.</returns>
        internal static byte[] GetBytes<T>(T anything)
        {
            int rawsize = Marshal.SizeOf(anything);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(anything, buffer, false);
            byte[] rawdatas = new byte[rawsize];
            Marshal.Copy(buffer, rawdatas, 0, rawsize);
            Marshal.FreeHGlobal(buffer);
            return rawdatas;
        }
    }
}
