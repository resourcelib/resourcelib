using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    public abstract class ResourceUtil
    {
        public static bool IsIntResource(IntPtr value)
        {
            if (((uint)value) > UInt16.MaxValue)
                return false;

            return true;
        }

        public static uint GetResourceID(IntPtr value)
        {
            if (IsIntResource(value))
                return (uint)value;

            throw new System.NotSupportedException(value.ToString());
        }

        public static string GetResourceName(IntPtr value)
        {
            if (IsIntResource(value))
                return value.ToString();

            return Marshal.PtrToStringUni((IntPtr)value);
        }

        public static IntPtr Align(Int32 p)
        {
            return new IntPtr((p + 3) & ~3);
        }

        public static IntPtr Align(IntPtr p)
        {
            return Align(p.ToInt32());
        }

        public static long PadToWORD(BinaryWriter w)
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

        public static long PadToDWORD(BinaryWriter w)
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
        /// Write the difference between current position and pos to address.
        /// </summary>
        public static void WriteAt(BinaryWriter w, long value, long address)
        {
            long cur = w.BaseStream.Position;
            w.Seek((int) address, SeekOrigin.Begin);
            w.Write((UInt16) value);
            w.Seek((int) cur, SeekOrigin.Begin);
        }

        public static long Pad(BinaryWriter w, UInt16 len)
        {
            while (len-- > 0)
                w.Write((byte) 0);
            return w.BaseStream.Position;
        }

        public static UInt16 NEUTRALLANGID
        {
            get
            {
                return MAKELANGID(Kernel32.LANG_NEUTRAL, Kernel32.SUBLANG_NEUTRAL);
            }
        }

        public static UInt16 USENGLISHLANGID
        {
            get
            {
                return ResourceUtil.MAKELANGID(Kernel32.LANG_ENGLISH, Kernel32.SUBLANG_ENGLISH_US);
            }
        }

        public static UInt16 MAKELANGID(int primary, int sub)
        {
            return (UInt16) ((((UInt16)sub) << 10) | ((UInt16)primary));
        }

        public static UInt16 PRIMARYLANGID(UInt16 lcid)
        {
            return (UInt16) (((UInt16)lcid) & 0x3ff);
        }

        public static UInt16 SUBLANGID(UInt16 lcid)
        {
            return (UInt16) (((UInt16)lcid) >> 10);
        }

        public static byte[] GetBytes<T>(T anything)
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
