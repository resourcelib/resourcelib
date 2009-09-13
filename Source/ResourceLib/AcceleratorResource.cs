using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// An accelerator, RT_ACCELERATOR resource.
    /// An accelerator provides the user with access to an application's command set.
    /// </summary>
    public class AcceleratorResource : Resource
    {
        private List<Accelerator> _accelerators = new List<Accelerator>();

        /// <summary>
        /// Accelerator keys.
        /// </summary>
        public List<Accelerator> Accelerators
        {
            get
            {
                return _accelerators;
            }
            set
            {
                _accelerators = value;
            }
        }

        /// <summary>
        /// A new accelerator resource.
        /// </summary>
        public AcceleratorResource()
            : base(IntPtr.Zero,
                IntPtr.Zero,
                new ResourceId(Kernel32.ResourceTypes.RT_ACCELERATOR),
                null,
                ResourceUtil.NEUTRALLANGID,
                0)
        {

        }


        /// <summary>
        /// An existing accelerator resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        public AcceleratorResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary>
        /// Read the accelerators table.
        /// </summary>
        /// <param name="hModule">Handle to a module.</param>
        /// <param name="lpRes">Pointer to the beginning of the accelerator table.</param>
        /// <returns>Address of the end of the accelerator table.</returns>
        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            int count = _size / Marshal.SizeOf(typeof(User32.ACCEL));
            for (int i = 0; i < count; i++)
            {
                Accelerator accelerator = new Accelerator();
                lpRes = accelerator.Read(lpRes);
                _accelerators.Add(accelerator);
            }
            return lpRes;
        }

        internal override void Write(System.IO.BinaryWriter w)
        {
            foreach (Accelerator accelerator in _accelerators)
            {
                accelerator.Write(w);
            }            
        }

        /// <summary>
        /// String representation of the accelerators resource.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0} ACCELERATORS", Name));
            sb.AppendLine("BEGIN");
            foreach(Accelerator accelerator in _accelerators)
            {
                sb.AppendLine(string.Format(" {0}", accelerator));
            }
            sb.AppendLine("END");
            return sb.ToString();
        }
    }
}
