using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// An embedded SxS manifest.
    /// </summary>
    public class ManifestResource : Resource
    {
        private XmlDocument _manifest = null;

        /// <summary>
        /// Embedded XML manifest.
        /// </summary>
        public XmlDocument Manifest
        {
            get
            {
                return _manifest;
            }
            set
            {
                _manifest = value;
                _size = Encoding.UTF8.GetBytes(_manifest.OuterXml).Length;
            }
        }

        /// <summary>
        /// Manifest type.
        /// </summary>
        public Kernel32.ManifestType ManifestType
        {
            get
            {
                return (Kernel32.ManifestType) _name;
            }
            set
            {
                _name = (IntPtr)value;
            }
        }

        /// <summary>
        /// An existing embedded manifest resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="wIDLanguage">Language ID.</param>
        /// <param name="size">Resource size.</param>
        public ManifestResource(IntPtr hModule, IntPtr hResource, IntPtr type, IntPtr name, UInt16 wIDLanguage, int size)
            : base(hModule, hResource, type, name, wIDLanguage, size)
        {
            Read(hModule, hResource);
        }

        /// <summary>
        /// A new executable CreateProcess manifest.
        /// </summary>
        public ManifestResource()
            : this(Kernel32.ManifestType.CreateProcess)
        {

        }

        /// <summary>
        /// A new executable manifest.
        /// </summary>
        /// <param name="manifestType">Manifest type.</param>
        public ManifestResource(Kernel32.ManifestType manifestType)
            : base(IntPtr.Zero, 
                IntPtr.Zero, 
                new IntPtr((uint) Kernel32.ResourceTypes.RT_MANIFEST), 
                new IntPtr((uint) manifestType), 
                Kernel32.LANG_NEUTRAL, 
                0)
        {
            _manifest = new XmlDocument();
            _manifest.LoadXml(
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<assembly xmlns=\"urn:schemas-microsoft-com:asm.v1\" manifestVersion=\"1.0\" />");
            _size = Encoding.UTF8.GetBytes(_manifest.OuterXml).Length;
        }

        /// <summary>
        /// Read the resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="lpRes">Pointer to the beginning of a resource.</param>
        /// <returns>Pointer to the end of the resource.</returns>
        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            if (_size > 0)
            {
                byte[] data = new byte[_size];
                Marshal.Copy(lpRes, data, 0, data.Length);
                _manifest = new XmlDocument();
                _manifest.PreserveWhitespace = true;
                _manifest.LoadXml(Encoding.UTF8.GetString(data));
                // todo: figure out how to preserve format in a way that size doesn't change
                _size = Encoding.UTF8.GetBytes(_manifest.OuterXml).Length;
            }

            return new IntPtr(lpRes.ToInt32() + _size);
        }

        /// <summary>
        /// Write the resource to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            w.Write(Encoding.UTF8.GetBytes(_manifest.OuterXml));
        }

        /// <summary>
        /// Load a CreateProcess manifest resource from an executable file.
        /// </summary>
        /// <param name="filename">Name of an executable file (.exe or .dll).</param>
        public void LoadFrom(string filename)
        {
            LoadFrom(filename, Kernel32.ManifestType.CreateProcess);
        }

        /// <summary>
        /// Load a manifest resource from an executable file.
        /// </summary>
        /// <param name="filename">Name of an executable file (.exe or .dll).</param>
        /// <param name="manifestType">Manifest resource type.</param>
        public void LoadFrom(string filename, Kernel32.ManifestType manifestType)
        {
            base.LoadFrom(filename, new IntPtr((uint) manifestType),
                new IntPtr((uint)Kernel32.ResourceTypes.RT_MANIFEST),
                Kernel32.LANG_NEUTRAL);
        }

        /// <summary>
        /// Save a manifest resource to an executable file.
        /// </summary>
        /// <param name="filename">Name of an executable file (.exe or .dll).</param>
        public void SaveTo(string filename)
        {
            base.SaveTo(filename, _name,
                new IntPtr((uint)Kernel32.ResourceTypes.RT_MANIFEST), Language);
        }
    }
}
