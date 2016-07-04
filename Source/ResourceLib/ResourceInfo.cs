using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// Resource info manager.
    /// </summary>
    public class ResourceInfo : IEnumerable<Resource>, IDisposable
    {
        private Exception _innerException = null;
        private IntPtr _hModule = IntPtr.Zero;
        private Dictionary<ResourceId, List<Resource>> _resources;
        private List<ResourceId> _resourceTypes = null;

        /// <summary>
        /// A dictionary of resources, the key is the resource type, eg. "REGISTRY" or "16" (version).
        /// </summary>
        public Dictionary<ResourceId, List<Resource>> Resources
        {
            get
            {
                return _resources;
            }
        }

        /// <summary>
        /// A shortcut for available resource types.
        /// </summary>
        public List<ResourceId> ResourceTypes
        {
            get
            {
                return _resourceTypes;
            }
        }

        /// <summary>
        /// A new resource info.
        /// </summary>
        public ResourceInfo()
        {

        }

        /// <summary>
        /// Unload the previously loaded module.
        /// </summary>
        public void Unload()
        {
            if (_hModule != IntPtr.Zero)
            {
                Kernel32.FreeLibrary(_hModule);
                _hModule = IntPtr.Zero;
            }

            _innerException = null;
        }

        /// <summary>
        /// Load an executable or a DLL and read its resources.
        /// </summary>
        /// <param name="filename">Source filename.</param>
        public void Load(string filename)
        {
            Unload();

            _resourceTypes = new List<ResourceId>();
            _resources = new Dictionary<ResourceId, List<Resource>>();

            // load DLL
            _hModule = Kernel32.LoadLibraryEx(filename, IntPtr.Zero,
                Kernel32.DONT_RESOLVE_DLL_REFERENCES | Kernel32.LOAD_LIBRARY_AS_DATAFILE);

            if (IntPtr.Zero == _hModule)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            try
            {
                // enumerate resource types
                // for each type, enumerate resource names
                // for each name, enumerate resource languages
                // for each resource language, enumerate actual resources
                if (!Kernel32.EnumResourceTypes(_hModule, EnumResourceTypesImpl, IntPtr.Zero))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            catch (Exception ex)
            {
                throw new LoadException(string.Format("Error loading '{0}'.", filename),
                    _innerException, ex);
            }
        }

        /// <summary>
        /// Enumerate resource types.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="lpszType">Resource type.</param>
        /// <param name="lParam">Additional parameter.</param>
        /// <returns>TRUE if successful.</returns>
        private bool EnumResourceTypesImpl(IntPtr hModule, IntPtr lpszType, IntPtr lParam)
        {
            ResourceId type = new ResourceId(lpszType);
            _resourceTypes.Add(type);

            // enumerate resource names
            if (!Kernel32.EnumResourceNames(hModule, lpszType, new Kernel32.EnumResourceNamesDelegate(EnumResourceNamesImpl), IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return true;
        }

        /// <summary>
        /// Enumerate resource names within a resource by type
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="lpszType">Resource type.</param>
        /// <param name="lpszName">Resource name.</param>
        /// <param name="lParam">Additional parameter.</param>
        /// <returns>TRUE if successful.</returns>
        private bool EnumResourceNamesImpl(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam)
        {
            if (!Kernel32.EnumResourceLanguages(hModule, lpszType, lpszName, new Kernel32.EnumResourceLanguagesDelegate(EnumResourceLanguages), IntPtr.Zero))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return true;
        }

        /// <summary>
        /// Create a resource of a given type.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResourceGlobal">Pointer to the resource in memory.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="wIDLanguage">Language ID.</param>
        /// <param name="size">Size of resource.</param>
        /// <returns>A specialized or a generic resource.</returns>
        protected Resource CreateResource(
            IntPtr hModule,
            IntPtr hResourceGlobal,
            ResourceId type,
            ResourceId name,
            UInt16 wIDLanguage,
            int size)
        {
            if (type.IsIntResource())
            {
                switch (type.ResourceType)
                {
                    case Kernel32.ResourceTypes.RT_VERSION:
                        return new VersionResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    case Kernel32.ResourceTypes.RT_GROUP_CURSOR:
                        return new CursorDirectoryResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    case Kernel32.ResourceTypes.RT_GROUP_ICON:
                        return new IconDirectoryResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    case Kernel32.ResourceTypes.RT_MANIFEST:
                        return new ManifestResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    case Kernel32.ResourceTypes.RT_BITMAP:
                        return new BitmapResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    case Kernel32.ResourceTypes.RT_MENU:
                        return new MenuResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    case Kernel32.ResourceTypes.RT_DIALOG:
                        return new DialogResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    case Kernel32.ResourceTypes.RT_STRING:
                        return new StringResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    case Kernel32.ResourceTypes.RT_FONTDIR:
                        return new FontDirectoryResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    case Kernel32.ResourceTypes.RT_FONT:
                        return new FontResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    case Kernel32.ResourceTypes.RT_ACCELERATOR:
                        return new AcceleratorResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    case Kernel32.ResourceTypes.RT_ICON:
                        return new IconResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    case Kernel32.ResourceTypes.RT_CURSOR:
                        return new CursorResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
                    default:
                        System.Diagnostics.Debug.WriteLine(string.Format("CreateResource: unhandled type {0}, returning GenericResource", type.ResourceType));
                        break;
                }
            }

            return new GenericResource(hModule, hResourceGlobal, type, name, wIDLanguage, size);
        }

        /// <summary>
        /// Enumerate resource languages within a resource by name
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="lpszType">Resource type.</param>
        /// <param name="lpszName">Resource name.</param>
        /// <param name="wIDLanguage">Language ID.</param>
        /// <param name="lParam">Additional parameter.</param>
        /// <returns>TRUE if successful.</returns>
        private bool EnumResourceLanguages(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, UInt16 wIDLanguage, IntPtr lParam)
        {
            List<Resource> resources = null;
            ResourceId type = new ResourceId(lpszType);
            if (!_resources.TryGetValue(type, out resources))
            {
                resources = new List<Resource>();
                _resources[type] = resources;
            }

            ResourceId name = new ResourceId(lpszName);
            IntPtr hResource = Kernel32.FindResourceEx(hModule, lpszType, lpszName, wIDLanguage);
            IntPtr hResourceGlobal = Kernel32.LoadResource(hModule, hResource);
            int size = Kernel32.SizeofResource(hModule, hResource);

            try
            {
                resources.Add(CreateResource(hModule, hResourceGlobal, type, name, wIDLanguage, size));
            }
            catch (Exception ex)
            {
                _innerException = new Exception(string.Format("Error loading resource '{0}' {1} ({2}).", name, type.TypeName, wIDLanguage), ex);
                throw ex;
            }

            return true;
        }

        /// <summary>
        /// Save resource to a file.
        /// </summary>
        /// <param name="filename">Target filename.</param>
        public void Save(string filename)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dispose resource info object.
        /// </summary>
        public void Dispose()
        {
            Unload();
        }

        /// <summary>
        /// A collection of resources.
        /// </summary>
        /// <param name="type">Resource type.</param>
        /// <returns>A collection of resources of a given type.</returns>
        public List<Resource> this[Kernel32.ResourceTypes type]
        {
            get
            {
                return _resources[new ResourceId(type)];
            }
            set
            {
                _resources[new ResourceId(type)] = value;
            }
        }

        /// <summary>
        /// A collection of resources.
        /// </summary>
        /// <param name="type">Resource type.</param>
        /// <returns>A collection of resources of a given type.</returns>
        public List<Resource> this[string type]
        {
            get
            {
                return _resources[new ResourceId(type)];
            }
            set
            {
                _resources[new ResourceId(type)] = value;
            }
        }

        #region IEnumerable<Resource> Members

        /// <summary>
        /// Enumerates all resources within this resource info collection.
        /// </summary>
        /// <returns>Resources enumerator.</returns>
        public IEnumerator<Resource> GetEnumerator()
        {
            Dictionary<ResourceId, List<Resource>>.Enumerator resourceTypesEnumerator = _resources.GetEnumerator();
            while(resourceTypesEnumerator.MoveNext())
            {
                List<Resource>.Enumerator resourceEnumerator = resourceTypesEnumerator.Current.Value.GetEnumerator();
                while (resourceEnumerator.MoveNext())
                {
                    yield return resourceEnumerator.Current;
                }
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Enumerates all resources within this resource info collection.
        /// </summary>
        /// <returns>Resources enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
