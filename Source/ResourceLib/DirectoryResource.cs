using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// This structure depicts the organization of data in a hardware-independent icon resource.
    /// </summary>
    public class DirectoryResource<ImageResourceType> : Resource
        where ImageResourceType : IconImageResource, new()
    {
        Kernel32.GRPICONDIR _header = new Kernel32.GRPICONDIR();
        List<ImageResourceType> _icons = new List<ImageResourceType>();

        /// <summary>
        /// Returns the type of the resource in this group.
        /// </summary>
        public Kernel32.ResourceTypes ResourceType
        {
            get
            {
                switch (_header.wType)
                {
                    case 1:
                        return Kernel32.ResourceTypes.RT_ICON;
                    case 2:
                        return Kernel32.ResourceTypes.RT_CURSOR;
                    default:
                        throw new NotSupportedException();
                }
            }
        }
               
        /// <summary>
        /// Icons contained in this hardware-independent icon resource.
        /// </summary>
        public List<ImageResourceType> Icons
        {
            get
            {
                return _icons;
            }
            set
            {
                _icons = value;
            }
        }

        /// <summary>
        /// A hardware-independent icon resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        internal DirectoryResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary>
        /// A new hardware-independent icon resource.
        /// </summary>
        public DirectoryResource(Kernel32.ResourceTypes resourceType)
            : base(IntPtr.Zero,
                IntPtr.Zero,
                new ResourceId(resourceType),
                new ResourceId(1),
                ResourceUtil.NEUTRALLANGID,
                Marshal.SizeOf(typeof(Kernel32.GRPICONDIR)))
        {
            switch(resourceType)
            {
                case Kernel32.ResourceTypes.RT_GROUP_CURSOR:
                    _header.wType = 2;
                    break;
                case Kernel32.ResourceTypes.RT_GROUP_ICON:
                    _header.wType = 1;
                    break;
                default:
                    throw new NotSupportedException();
            }            
        }

        /// <summary>
        /// Save a hardware-independent icon resource to an executable file.
        /// </summary>
        /// <param name="filename">Name of an executable file (.exe or .dll).</param>
        public override void SaveTo(string filename)
        {
            base.SaveTo(filename);

            foreach (ImageResourceType icon in _icons)
            {
                icon.SaveIconTo(filename);
            }
        }

        /// <summary>
        /// Read a hardware-independent icon resource from a loaded module.
        /// </summary>
        /// <param name="hModule">Loaded executable module.</param>
        /// <param name="lpRes">Pointer to the beginning of a hardware-independent icon resource.</param>
        /// <returns>Pointer to the end of the hardware-independent icon resource.</returns>
        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            _icons.Clear();

            _header = (Kernel32.GRPICONDIR)Marshal.PtrToStructure(
                lpRes, typeof(Kernel32.GRPICONDIR));

            IntPtr pEntry = new IntPtr(lpRes.ToInt32() + Marshal.SizeOf(_header));

            for (UInt16 i = 0; i < _header.wImageCount; i++)
            {
                ImageResourceType iconImageResource = new ImageResourceType();
                pEntry = iconImageResource.Read(hModule, pEntry);
                _icons.Add(iconImageResource);
            }

            return pEntry;
        }

        /// <summary>
        /// Write a hardware-independent icon resource to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            w.Write((UInt16)_header.wReserved);
            w.Write((UInt16)_header.wType);
            w.Write((UInt16)_icons.Count);
            ResourceUtil.PadToWORD(w);
            foreach (ImageResourceType icon in _icons)
            {
                icon.Write(w);
            }
        }
    }
}
