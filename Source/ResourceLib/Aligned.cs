using System;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// Creates an 8 bit aligned copy of the buffer if it is not already aligned
    /// </summary>
    internal sealed class Aligned : IDisposable
    {
        private IntPtr lp;
        private IntPtr lpAligned = IntPtr.Zero;
        private int _size;
        private bool disposed = false;

        public IntPtr Ptr
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException(nameof(Aligned));

                if (this.lpAligned != IntPtr.Zero)
                    return this.lpAligned;

                if (lp.ToInt64() % 8 == 0)
                    return lp;                

                IntPtr lpAligned = Marshal.AllocHGlobal(_size);
                Kernel32.CopyMemory(lpAligned, lp, (uint)_size);
                return lpAligned;
            }
        }

        public Aligned(IntPtr lp, int size)
        {
            this.lp = lp;
            _size = size;
        }

        public void Dispose()
        {
            if (disposed || lpAligned == IntPtr.Zero)
                return;

            disposed = true;
            Marshal.FreeHGlobal(lpAligned);
        }
    }
}