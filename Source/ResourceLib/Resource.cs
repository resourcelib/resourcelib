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
    public class Resource
    {
        protected IntPtr _type;
        protected IntPtr _name;
        protected ushort _language;
        protected IntPtr _hModule = IntPtr.Zero;
        protected IntPtr _hResource = IntPtr.Zero;
        protected int _size = 0;

        public int Size
        {
            get
            {
                return _size;
            }
        }

        public ushort Language
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

        public string Type
        {
            get
            {
                return ResourceUtil.GetResourceName(_type);
            }
        }

        public string Name
        {
            get
            {
                return ResourceUtil.GetResourceName(_name);
            }
        }

        public Resource()
        {

        }

        public Resource(IntPtr hModule, IntPtr hResource, IntPtr type, IntPtr name, ushort wIDLanguage, int size)
        {
            _hModule = hModule;
            _type = type;
            _name = name;
            _language = wIDLanguage;
            _hResource = hResource;
            _size = size;
        }

        public static byte[] LoadBytesFrom(string filename, IntPtr name, IntPtr type, ushort lang)
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

        public void LoadFrom(string filename, IntPtr name, IntPtr type, ushort lang)
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

        public virtual IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            throw new NotImplementedException();
        }

        public virtual void Write(BinaryWriter w)
        {
            throw new NotImplementedException();
        }

        public byte[] WriteAndGetBytes()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms, Encoding.Default);
            Write(w);
            w.Close();
            return ms.ToArray();
        }

        public void SaveTo(string filename, IntPtr name, IntPtr type, ushort langid)
        {
            byte[] data = WriteAndGetBytes();
            SaveTo(filename, name, type, langid, data);
        }

        public void DeleteFrom(string filename)
        {
            Delete(filename, _name, _type, _language);
        }

        /// <summary>
        /// Delete a resource.
        /// </summary>
        public static void Delete(string filename, IntPtr name, IntPtr type, ushort lang)
        {
            SaveTo(filename, name, type, lang, null);
        }

        public static void SaveTo(string filename, IntPtr name, IntPtr type, ushort lang, byte[] data)
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
