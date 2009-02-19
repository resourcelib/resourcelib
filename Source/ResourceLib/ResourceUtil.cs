using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

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
    }
}
