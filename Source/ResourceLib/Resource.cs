using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A version resource.
    /// </summary>
    public class Resource
    {
        private string _type;
        private string _name;
        private ushort _language;
        private IntPtr _hResource = IntPtr.Zero;
        private int _size;

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
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public Resource(IntPtr hResource, IntPtr type, IntPtr name, ushort wIDLanguage, int size)
        {
            _type = ResourceUtil.GetResourceName(type);
            _name = ResourceUtil.GetResourceName(name);
            _language = wIDLanguage;
            _hResource = hResource;
            _size = size;
        }
    }
}
