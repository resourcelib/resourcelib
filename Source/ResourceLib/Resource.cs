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
        protected string _type;
        protected string _name;
        protected ushort _language;
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

        public Resource()
        {

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
