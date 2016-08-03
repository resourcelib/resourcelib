using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// Standard accelerator.
    /// </summary>
    public class Accelerator
    {
        private User32.ACCEL _accel = new User32.ACCEL();

        /// <summary>
        /// Read the accelerator.
        /// </summary>
        /// <param name="lpRes">Address in memory.</param>
        internal IntPtr Read(IntPtr lpRes)
        {
            _accel = (User32.ACCEL) Marshal.PtrToStructure(
                lpRes, typeof(User32.ACCEL));

            return new IntPtr(lpRes.ToInt64() + Marshal.SizeOf(_accel));
        }

        /// <summary>
        /// Write accelerator to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal void Write(BinaryWriter w)
        {
            w.Write(_accel.fVirt);
            w.Write(_accel.key);
            w.Write(_accel.cmd);
            ResourceUtil.PadToWORD(w);
        }

        /// <summary>
        /// String representation of the accelerator key.
        /// </summary>
        public string Key
        {
            get
            {
                string key = Enum.GetName(typeof(User32.VirtualKeys), _accel.key);
                return string.IsNullOrEmpty(key) ? ((char)_accel.key).ToString() : key;
            }
            set
            {
                try//will work if given string is a Virtual Key e.g. "VK_NUMPAD1"
                {
                    uint i = (uint)Enum.Parse(typeof(User32.VirtualKeys), value);
                    _accel.key = (UInt16)i;
                }
                catch (Exception e)//otherwise, use ASCII-code of given Key e.g. key "Z" == 90
                {  
                    char c = value.ToCharArray()[0];
                    UInt16 i = (UInt16)c;
                    _accel.key = i;
                }
            }
        }

        /// <summary>
        /// String representation of the used flags.
        /// When using set method, several flags have to be split with ',' e.g. "VIRTKEY, NOINVERT, CONTROL"
        /// </summary>
        public string Flags
        {
            set
            {
                    string[] sArgs = value.Split(',');
                    uint itmp=0;
                    foreach(string s in sArgs)
                    {
                        itmp+= (uint)Enum.Parse(typeof(User32.AcceleratorVirtualKey), s);
                    }
                    _accel.fVirt = (UInt16)itmp;
            }
        }

        /// <summary>
        /// An unsigned integer value that identifies the accelerator.
        /// </summary>
        public UInt32 Command
        {
            get
            {
                return _accel.cmd;
            }
            set
            {
                _accel.cmd = value;
            }
        }

        /// <summary>
        /// String representation of the accelerator.
        /// </summary>
        /// <returns>String representation of the accelerator.</returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}",
                Key, Command, ResourceUtil.FlagsToString<User32.AcceleratorVirtualKey>(
                    _accel.fVirt).Replace(" |", ","));
        }
    }
}
