using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A font directory, RT_FONTDIR resource.
    /// </summary>
    public class FontDirectoryResource : Resource
    {
        private List<FontDirectoryEntry> _fonts = new List<FontDirectoryEntry>();
        private byte[] _reserved = null;

        /// <summary>
        /// Number of fonts in this directory.
        /// </summary>
        public List<FontDirectoryEntry> Fonts
        {
            get
            {
                return _fonts;
            }
            set
            {
                _fonts = value;
            }
        }

        /// <summary>
        /// A new font resource.
        /// </summary>
        public FontDirectoryResource()
            : base(IntPtr.Zero,
                IntPtr.Zero,
                new ResourceId(Kernel32.ResourceTypes.RT_FONTDIR),
                null,
                ResourceUtil.NEUTRALLANGID,
                0)
        {

        }

        /// <summary>
        /// An existing font resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource ID.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        public FontDirectoryResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary>
        /// Read the font resource.
        /// </summary>
        /// <param name="hModule">Handle to a module.</param>
        /// <param name="lpRes">Pointer to the beginning of the font structure.</param>
        /// <returns>Address of the end of the font structure.</returns>
        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            IntPtr lpHead = lpRes;

            UInt16 count = (UInt16)Marshal.ReadInt16(lpRes);
            lpRes = new IntPtr(lpRes.ToInt64() + 2);

            for (int i = 0; i < count; i++)
            {
                FontDirectoryEntry fontEntry = new FontDirectoryEntry();
                lpRes = fontEntry.Read(lpRes);
                _fonts.Add(fontEntry);
            }

            int reservedLen = _size - (int)(lpRes.ToInt64() - lpHead.ToInt64());
            _reserved = new byte[reservedLen];
            Marshal.Copy(lpRes, _reserved, 0, reservedLen);

            return lpRes;
        }

        /// <summary>
        /// Write the font directory to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            w.Write((UInt16) _fonts.Count);
            foreach (FontDirectoryEntry fontEntry in _fonts)
            {
                fontEntry.Write(w);
            }
            w.Write(_reserved);
        }
    }
}
