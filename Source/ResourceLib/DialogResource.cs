using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// A dialog template resource.
    /// </summary>
    public class DialogResource : Resource
    {
        private DialogTemplateBase _dlgtemplate = null;

        /// <summary>
        /// A dialog template structure that describes the dialog.
        /// </summary>
        public DialogTemplateBase Template
        {
            get
            {
                return _dlgtemplate;
            }
            set
            {
                _dlgtemplate = value;
            }
        }

        /// <summary>
        /// A structured dialog resource embedded in an executable module.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource handle.</param>
        /// <param name="type">Type of resource.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language id.</param>
        /// <param name="size">Resource size.</param>
        public DialogResource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
            : base(hModule, hResource, type, name, language, size)
        {

        }

        /// <summary> 
        /// A structured dialog resource embedded in an executable module.
        /// </summary>
        public DialogResource()
            : base(IntPtr.Zero, 
                IntPtr.Zero, 
                new ResourceId(Kernel32.ResourceTypes.RT_DIALOG), 
                new ResourceId(1), 
                ResourceUtil.NEUTRALLANGID, 
                0)
        {

        }

        internal override IntPtr Read(IntPtr hModule, IntPtr lpRes)
        {
            switch ((uint)Marshal.ReadInt32(lpRes) >> 16)
            {
                case 0xFFFF:
                    _dlgtemplate = new DialogTemplateEx();
                    break;
                default:
                    _dlgtemplate = new DialogTemplate();
                    break;
            }

            // dialog structure itself
            return _dlgtemplate.Read(lpRes);
        }

        internal override void Write(BinaryWriter w)
        {
            _dlgtemplate.Write(w);
        }

        /// <summary>
        /// Dialog resource in standard resource editor text format.
        /// </summary>
        /// <returns>Multi-line string.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} {1}", 
                Name.IsIntResource() ? Name.ToString() : "\"" + Name.ToString() + "\"",
                _dlgtemplate);
            return sb.ToString();
        }
    }
}
