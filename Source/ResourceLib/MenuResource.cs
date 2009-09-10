using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A generic resource.
    /// </summary>
    public class MenuResource : Resource
    {
        private MenuTemplateBase _menu = null;

        /// <summary>
        /// Menu template.
        /// </summary>
        public MenuTemplateBase Menu
        {
            get
            {
                return _menu;
            }
            set
            {
                _menu = value;
            }
        }

        /// <summary>
        /// A structured menu resource embedded in an executable module.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource handle.</param>
        /// <param name="type">Type of resource.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language id.</param>
        /// <param name="size">Resource size.</param>
        public MenuResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary>
        /// Read a menu resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="lpRes">Pointer to the beginning of a resource.</param>
        /// <returns>Pointer to the end of the resource.</returns>
        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            UInt16 version = (UInt16) Marshal.ReadInt16(lpRes);
            switch (version)
            {
                case 0:
                    _menu = new MenuTemplate();
                    break;
                case 1:
                    _menu = new MenuExTemplate();
                    break;
                default:
                    throw new NotSupportedException(string.Format(
                        "Unexpected menu header version {0}", version));
            }
            
            return _menu.Read(lpRes);
        }

        /// <summary>
        /// Write the menu resource to a binary stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal override void Write(BinaryWriter w)
        {
            
        }

        /// <summary>
        /// String representation of the menu resource in the MENU format.
        /// </summary>
        /// <returns>String representation of the menu resource.</returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Menu.ToString());
        }
    }
}
