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
        private static byte[] utf8_bom = { 0xef, 0xbb, 0xbf };

        private byte[] _data = null;
        private XmlDocument _manifest = null;

        /// <summary>
        /// Embedded XML manifest.
        /// </summary>
        public XmlDocument Manifest
        {
            get
            {
                if (_manifest == null && _data != null)
                {
                    bool unicodeBOM = (_data.Length >= 3 
                        && _data[0] == utf8_bom[0] 
                        && _data[1] == utf8_bom[1] 
                        && _data[2] == utf8_bom[2]);

                    string manifestXml = Encoding.UTF8.GetString(
                        _data, 
                        unicodeBOM ? 3 : 0, 
                        unicodeBOM ? _data.Length - 3 : _data.Length);

                    _manifest = new XmlDocument();
                    _manifest.LoadXml(manifestXml);
                }

                return _manifest;
            }
            set
            {
                _manifest = value;
                _data = null;
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
                return (Kernel32.ManifestType) _name.Id;
            }
            set
            {
                _name = new ResourceId((IntPtr) value);
            }
        }

        /// <summary>
        /// An existing embedded manifest resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        public ManifestResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

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
                new ResourceId(Kernel32.ResourceTypes.RT_MANIFEST), 
                new ResourceId((uint) manifestType), 
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
                _manifest = null;
                _data = new byte[_size];
                Marshal.Copy(lpRes, _data, 0, _data.Length);
            }

            return new IntPtr(lpRes.ToInt64() + _size);
        }

        /// <summary>
        /// Write the resource to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            if (_manifest != null)
            {
                w.Write(Encoding.UTF8.GetBytes(_manifest.OuterXml));
            }
            else if (_data != null)
            {
                w.Write(_data);
            }
        }

        /// <summary>
        /// Load a manifest resource from an executable file.
        /// </summary>
        /// <param name="filename">Name of an executable file (.exe or .dll).</param>
        /// <param name="manifestType">Manifest resource type.</param>
        public void LoadFrom(string filename, Kernel32.ManifestType manifestType)
        {
            base.LoadFrom(filename, 
                new ResourceId(Kernel32.ResourceTypes.RT_MANIFEST),
                new ResourceId((uint)manifestType),
                Kernel32.LANG_NEUTRAL);
        }
    }
}
