using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// Resource info manager.
    /// </summary>
    public class ResourceInfo : IDisposable
    {
        private IntPtr _hModule = IntPtr.Zero;
        private Dictionary<string, List<Resource>> _resources;
        private List<string> _resourceTypes = null;

        /// <summary>
        /// A dictionary of resources, the key is the resource type, eg. "REGISTRY" or "16" (version)
        /// </summary>
        public Dictionary<string, List<Resource>> Resources
        {
            get
            {
                return _resources;
            }
        }

        /// <summary>
        /// A shortcut for available resource types
        /// </summary>
        public List<string> ResourceTypes
        {
            get
            {
                return _resourceTypes;
            }
        }

        public ResourceInfo()
        {

        }

        /// <summary>
        /// Unload the previously loaded module
        /// </summary>
        public void Unload()
        {
            if (_hModule != IntPtr.Zero)
            {
                Kernel32.FreeLibrary(_hModule);
                _hModule = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Load an executable or a DLL and read its resources
        /// </summary>
        public void Load(string filename)
        {
            Unload();

            _resourceTypes = new List<string>();
            _resources = new Dictionary<string, List<Resource>>();

            // load DLL
            _hModule = Kernel32.LoadLibraryEx(filename, IntPtr.Zero,
                Kernel32.DONT_RESOLVE_DLL_REFERENCES | Kernel32.LOAD_LIBRARY_AS_DATAFILE);

            if (IntPtr.Zero == _hModule)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            // enumerate resource types
            // for each type, enumerate resource names
            // for each name, enumerate resource languages
            // for each resource language, enumerate actual resources
            if (!Kernel32.EnumResourceTypes(_hModule, EnumResourceTypesImpl, IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Enumerate resource types
        /// </summary>
        private bool EnumResourceTypesImpl(IntPtr hModule, IntPtr lpszType, IntPtr lParam)
        {
            string typename = ResourceUtil.GetResourceName(lpszType);
            _resourceTypes.Add(typename);

            // enumerate resource names
            if (!Kernel32.EnumResourceNames(hModule, lpszType, new Kernel32.EnumResourceNamesDelegate(EnumResourceNamesImpl), IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return true;
        }

        /// <summary>
        /// Enumerate resource names within a resource by type
        /// </summary>
        private bool EnumResourceNamesImpl(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam)
        {
            if (!Kernel32.EnumResourceLanguages(hModule, lpszType, lpszName, new Kernel32.EnumResourceLanguagesDelegate(EnumResourceLanguages), IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return true;
        }

        /// <summary>
        /// Enumerate resource languages within a resource by name
        /// </summary>
        private bool EnumResourceLanguages(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, ushort wIDLanguage, IntPtr lParam)
        {
            string type = ResourceUtil.GetResourceName(lpszType);

            List<Resource> resources = null;
            if (!_resources.TryGetValue(type, out resources))
            {
                resources = new List<Resource>();
                _resources[type] = resources;
            }

            IntPtr hResource = Kernel32.FindResourceEx(hModule, lpszType, lpszName, wIDLanguage);
            IntPtr hResourceGlobal = Kernel32.LoadResource(hModule, hResource);
            int size = Kernel32.SizeofResource(hModule, hResource);

            Resource rc = null;
            switch (ResourceUtil.GetResourceName(lpszType))
            {
                case "16": // Kernel32.RT_VERSION:
                    rc = new VersionResource(hResourceGlobal, lpszType, lpszName, wIDLanguage, size);
                    break;
                // \todo: specialize other resource types
                default:
                    rc = new Resource(hResourceGlobal, lpszType, lpszName, wIDLanguage, size);
                    break;
            }

            resources.Add(rc);
            return true;
        }

        public void Save(string filename)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Unload();
        }
    }
}
