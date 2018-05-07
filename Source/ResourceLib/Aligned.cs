using System;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// Creates an 8 bit aligned copy of the buffer if it is not already aligned
    /// </summary>
    internal sealed class Aligned : IDisposable
    {
        private IntPtr _ptr;
        private bool allocated;
        private bool Disposed => _ptr == IntPtr.Zero;

        public IntPtr Ptr
        {
            get
            {
                if (Disposed)
                    throw new ObjectDisposedException(nameof(Aligned));

                return _ptr;
            }
        }

        public Aligned(IntPtr lp, int size)
        {
            if (lp == IntPtr.Zero)
                throw new ArgumentException("Cannot align a null pointer.", nameof(lp));

            if (lp.ToInt64() % 8 == 0)
            {
                _ptr = lp;
                allocated = false;
            }
            else
            {
                _ptr = Marshal.AllocHGlobal(size);
                allocated = true;
                Kernel32.MoveMemory(_ptr, lp, (uint)size);
            }
        }

        public void Dispose()
        {
            if (!allocated || Disposed)
                return;

            Marshal.FreeHGlobal(_ptr);
            _ptr = IntPtr.Zero;
        }
    }
}