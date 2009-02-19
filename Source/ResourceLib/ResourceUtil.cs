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
            if (((uint)value) > ushort.MaxValue)
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
                Pad(w, (ushort)count);
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
                Pad(w, (ushort) count);
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

        public static long Pad(BinaryWriter w, ushort len)
        {
            while (len-- > 0)
                w.Write((byte) 0);
            return w.BaseStream.Position;
        }

        public static int NEUTRALLANGID
        {
            get
            {
                return MAKELANGID(Kernel32.LANG_NEUTRAL, Kernel32.SUBLANG_NEUTRAL);
            }
        }

        public static int MAKELANGID(int primary, int sub)
        {
            return (((ushort)sub) << 10) | ((ushort)primary);
        }

        public static int PRIMARYLANGID(int lcid)
        {
            return ((ushort)lcid) & 0x3ff;
        }

        public static int SUBLANGID(int lcid)
        {
            return ((ushort)lcid) >> 10;
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
