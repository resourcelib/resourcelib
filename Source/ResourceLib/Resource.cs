using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A version resource.
    /// </summary>
    public abstract class Resource
    {
        /// <summary>
        /// Resource type.
        /// </summary>
        protected IntPtr _type;
        /// <summary>
        /// Resource name.
        /// </summary>
        protected IntPtr _name;
        /// <summary>
        /// Resource language.
        /// </summary>
        protected UInt16 _language;
        /// <summary>
        /// Loaded binary nodule.
        /// </summary>
        protected IntPtr _hModule = IntPtr.Zero;
        /// <summary>
        /// Pointer to the resource.
        /// </summary>
        protected IntPtr _hResource = IntPtr.Zero;
        /// <summary>
        /// Resource size.
        /// </summary>
        protected int _size = 0;

        /// <summary>
        /// Version resource size in bytes.
        /// </summary>
        public int Size
        {
            get
            {
                return _size;
            }
        }

        /// <summary>
        /// Language ID.
        /// </summary>
        public UInt16 Language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
            }
        }

        /// <summary>
        /// Resource type.
        /// </summary>
        public string Type
        {
            get
            {
                return ResourceUtil.GetResourceName(_type);
            }
        }

        /// <summary>
        /// Resource name.
        /// </summary>
        public string Name
        {
            get
            {
                return ResourceUtil.GetResourceName(_name);
            }
        }

        /// <summary>
        /// A new resource.
        /// </summary>
        internal Resource()
        {

        }

        /// <summary>
        /// A structured resource embedded in an executable module.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource handle.</param>
        /// <param name="type">Type of resource.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="wIDLanguage">Language ID.</param>
        /// <param name="size">Resource size.</param>
        internal Resource(IntPtr hModule, IntPtr hResource, IntPtr type, IntPtr name, UInt16 wIDLanguage, int size)
        {
            _hModule = hModule;
            _type = type;
            _name = name;
            _language = wIDLanguage;
            _hResource = hResource;
            _size = size;
        }

        /// <summary>
        /// Load resource bytes from an executable file.
        /// </summary>
        /// <param name="filename">Executable (.exe or .dll) file.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="lang">Resource language.</param>
        /// <returns>Resource data.</returns>
        internal static byte[] LoadBytesFrom(string filename, IntPtr name, IntPtr type, UInt16 lang)
        {
            IntPtr hModule = IntPtr.Zero;

            try
            {
                hModule = Kernel32.LoadLibraryEx(filename, IntPtr.Zero,
                    Kernel32.DONT_RESOLVE_DLL_REFERENCES | Kernel32.LOAD_LIBRARY_AS_DATAFILE);

                if (IntPtr.Zero == hModule)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                IntPtr hRes = Kernel32.FindResourceEx(hModule, type, name, lang);
                if (IntPtr.Zero == hRes)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                IntPtr hGlobal = Kernel32.LoadResource(hModule, hRes);
                if (IntPtr.Zero == hGlobal)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                IntPtr lpRes = Kernel32.LockResource(hGlobal);

                if (lpRes == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                int size = Kernel32.SizeofResource(hModule, hRes);
                if (size <= 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                
                byte[] bytes = new byte[size];
                Marshal.Copy(lpRes, bytes, 0, size);

                return bytes;
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                    Kernel32.FreeLibrary(hModule);
            }
        }

        /// <summary>
        /// Load a resource from an executable (.exe or .dll) file.
        /// </summary>
        /// <param name="filename">An executable (.exe or .dll) file.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="lang">Resource language.</param>
        internal void LoadFrom(string filename, IntPtr name, IntPtr type, UInt16 lang)
        {
            IntPtr hModule = IntPtr.Zero;

            try
            {                
                hModule = Kernel32.LoadLibraryEx(filename, IntPtr.Zero,
                    Kernel32.DONT_RESOLVE_DLL_REFERENCES | Kernel32.LOAD_LIBRARY_AS_DATAFILE);

                if (IntPtr.Zero == hModule)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                IntPtr hRes = Kernel32.FindResourceEx(hModule, type, name, lang);
                if (IntPtr.Zero == hRes)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                IntPtr hGlobal = Kernel32.LoadResource(hModule, hRes);
                if (IntPtr.Zero == hGlobal)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                IntPtr lpRes = Kernel32.LockResource(hGlobal);

                if (lpRes == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                _size = Kernel32.SizeofResource(hModule, hRes);
                if (_size <= 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                _type = type;
                _name = name;
                _language = lang;

                Read(hModule, lpRes);
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                    Kernel32.FreeLibrary(hModule);
            }
        }

        /// <summary>
        /// Read a resource from a previously loaded module.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="lpRes">Pointer to the beginning of the resource.</param>
        /// <returns>Pointer to the end of the resource.</returns>
        internal abstract IntPtr Read(IntPtr hModule, IntPtr lpRes);

        /// <summary>
        /// Write the resource to a memory stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal abstract void Write(BinaryWriter w);

        /// <summary>
        /// Return resource data.
        /// </summary>
        /// <returns>Resource data.</returns>
        public byte[] WriteAndGetBytes()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms, Encoding.Default);
            Write(w);
            w.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// Save a resource to an executable (.exe or .dll) file.
        /// </summary>
        /// <param name="filename">Path to an executable file.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="langid">Language id.</param>
        internal void SaveTo(string filename, IntPtr name, IntPtr type, UInt16 langid)
        {
            byte[] data = WriteAndGetBytes();
            SaveTo(filename, name, type, langid, data);
        }

        /// <summary>
        /// Delete a resource from an executable (.exe or .dll) file.
        /// </summary>
        /// <param name="filename">Path to an executable file.</param>
        public void DeleteFrom(string filename)
        {
            Delete(filename, _name, _type, _language);
        }

        /// <summary>
        /// Delete a resource from an executable (.exe or .dll) file.
        /// </summary>
        /// <param name="filename">Path to an executable file.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="lang">Resource language.</param>
        internal static void Delete(string filename, IntPtr name, IntPtr type, UInt16 lang)
        {
            SaveTo(filename, name, type, lang, null);
        }

        /// <summary>
        /// Save a resource to an executable (.exe or .dll) file.
        /// </summary>
        /// <param name="filename">Path to an executable file.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="lang">Resource language.</param>
        /// <param name="data">Resource data.</param>
        internal static void SaveTo(string filename, IntPtr name, IntPtr type, UInt16 lang, byte[] data)
        {
            IntPtr h = Kernel32.BeginUpdateResource(filename, false);

            if (h == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            if (!Kernel32.UpdateResource(h, type, name,
                lang, data, (data == null ? 0 : (uint) data.Length)))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (!Kernel32.EndUpdateResource(h, false))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
