using System;
using System.Runtime.InteropServices;

namespace Vestris.ResourceLib
{
    /// <summary>
    /// User32.dll functions.
    /// </summary>
    public abstract class User32
    {
        /// <summary>
        /// Contains information about an icon or a cursor. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct ICONINFO
        {
            /// <summary>
            /// Specifies whether this structure defines an icon or a cursor. 
            /// A value of TRUE specifies an icon; FALSE specifies a cursor. 
            /// </summary>
            public bool IsIcon;
            /// <summary>
            /// Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot spot is 
            /// always in the center of the icon, and this member is ignored.
            /// </summary>
            public int xHotspot;
            /// <summary>
            /// Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot spot 
            /// is always in the center of the icon, and this member is ignored.
            /// </summary>
            public int yHotspot;
            /// <summary>
            /// Specifies the icon bitmask bitmap. 
            /// </summary>
            public IntPtr MaskBitmap;
            /// <summary>
            /// Handle to the icon color bitmap.
            /// </summary>
            public IntPtr ColorBitmap;
        };

        /// <summary>
        /// Retrieve a handle to a device context (DC) for the client area of a specified window or for the entire screen. 
        /// </summary>
        /// <param name="hWnd">A handle to the window whose DC is to be retrieved. If this value is NULL, GetDC retrieves the DC for the entire screen.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the DC for the specified window's client area. 
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// Releases a device context (DC), freeing it for use by other applications.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose DC is to be released.</param>
        /// <param name="hDC">A handle to the DC to be released.</param>
        /// <returns>
        /// The return value indicates whether the DC was released. If the DC was released, the return value is 1.
        /// If the DC was not released, the return value is zero.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        /// <summary>
        /// Destroys an icon and frees any memory the icon occupied.
        /// </summary>
        /// <param name="hIcon">Handle to the icon to be destroyed.</param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern int DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// Creates an icon or cursor from an ICONINFO structure.
        /// </summary>
        /// <param name="piconInfo">Pointer to an ICONINFO structure the function uses to create the icon or cursor.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the icon or cursor that is created.
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("user32,dll")]
        internal static extern IntPtr CreateIconIndirect(ref ICONINFO piconInfo);

        /// <summary>
        /// The DLGTEMPLATE structure defines the dimensions and style of a dialog box. 
        /// This structure, always the first in a standard template for a dialog box, 
        /// also specifies the number of controls in the dialog box and therefore specifies 
        /// the number of subsequent DLGITEMTEMPLATE structures in the template.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct DLGTEMPLATE
        {
            /// <summary>
            /// Specifies the style of the dialog box.
            /// </summary>
            public UInt32 style;
            /// <summary>
            /// Extended styles for a window.
            /// </summary>
            public UInt32 dwExtendedStyle;
            /// <summary>
            /// Specifies the number of items in the dialog box. 
            /// </summary>
            public UInt16 cdit;
            /// <summary>
            /// Specifies the x-coordinate, in dialog box units, of the upper-left corner of the dialog box. 
            /// </summary>
            public Int16 x;
            /// <summary>
            /// Specifies the y-coordinate, in dialog box units, of the upper-left corner of the dialog box.
            /// </summary>
            public Int16 y;
            /// <summary>
            /// Specifies the width, in dialog box units, of the dialog box.
            /// </summary>
            public Int16 cx;
            /// <summary>
            /// Specifies the height, in dialog box units, of the dialog box.
            /// </summary>
            public Int16 cy;
        };

        /// <summary>
        /// The DLGITEMTEMPLATE structure defines the dimensions and style of a control in a dialog box.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct DLGITEMTEMPLATE
        {
            /// <summary>
            /// Specifies the style of the control.
            /// </summary>
            public UInt32 style;
            /// <summary>
            /// Extended styles for a window.
            /// </summary>
            public UInt32 dwExtendedStyle;
            /// <summary>
            /// Specifies the number of items in the dialog box. 
            /// </summary>
            public UInt16 cdit;
            /// <summary>
            /// Specifies the x-coordinate, in dialog box units, of the upper-left corner of the control. 
            /// </summary>
            public Int16 x;
            /// <summary>
            /// Specifies the y-coordinate, in dialog box units, of the upper-left corner of the control.
            /// </summary>
            public Int16 y;
            /// <summary>
            /// Specifies the width, in dialog box units, of the control.
            /// </summary>
            public Int16 cx;
            /// <summary>
            /// Specifies the height, in dialog box units, of the control.
            /// </summary>
            public Int16 cy;
            /// <summary>
            /// Specifies the control identifier.
            /// </summary>
            public UInt16 id;
        };

        /// <summary>
        /// An extended dialog box template begins with a DLGTEMPLATEEX header that describes
        /// the dialog box and specifies the number of controls in the dialog box. For each 
        /// control in a dialog box, an extended dialog box template has a block of data that
        /// uses the DLGITEMTEMPLATEEX format to describe the control. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct DLGTEMPLATEEX
        {
            /// <summary>
            /// Specifies the version number of the extended dialog box template. This member must be 1.
            /// </summary>
            public UInt16 dlgVer;
            /// <summary>
            /// Indicates whether a template is an extended dialog box template. 
            /// </summary>
            public UInt16 signature;
            /// <summary>
            /// Specifies the help context identifier for the dialog box window. When the system
            /// sends a WM_HELP message, it passes this value in the wContextId member of the 
            /// HELPINFO structure. 
            /// </summary>
            public UInt32 helpID;
            /// <summary>
            /// Specifies extended windows styles.
            /// </summary>
            public UInt32 exStyle;
            /// <summary>
            /// Specifies the style of the dialog box.
            /// </summary>
            public UInt32 style;
            /// <summary>
            /// Specifies the number of controls in the dialog box.
            /// </summary>
            public UInt16 cDlgItems;
            /// <summary>
            /// Specifies the x-coordinate, in dialog box units, of the upper-left corner of the dialog box.
            /// </summary>
            public Int16 x;
            /// <summary>
            /// Specifies the y-coordinate, in dialog box units, of the upper-left corner of the dialog box.
            /// </summary>
            public Int16 y;
            /// <summary>
            /// Specifies the width, in dialog box units, of the dialog box.
            /// </summary>
            public Int16 cx;
            /// <summary>
            /// Specifies the height, in dialog box units, of the dialog box.
            /// </summary>
            public Int16 cy;
        };

        /// <summary>
        /// A control entry in an extended dialog template.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct DLGITEMTEMPLATEEX
        {
            /// <summary>
            /// Specifies the help context identifier for the dialog box window. When the system
            /// sends a WM_HELP message, it passes this value in the wContextId member of the 
            /// HELPINFO structure. 
            /// </summary>
            public UInt32 helpID;
            /// <summary>
            /// Specifies extended windows styles.
            /// </summary>
            public UInt32 exStyle;
            /// <summary>
            /// Specifies the style of the dialog box.
            /// </summary>
            public UInt32 style;
            /// <summary>
            /// Specifies the x-coordinate, in dialog box units, of the upper-left corner of the dialog box.
            /// </summary>
            public Int16 x;
            /// <summary>
            /// Specifies the y-coordinate, in dialog box units, of the upper-left corner of the dialog box.
            /// </summary>
            public Int16 y;
            /// <summary>
            /// Specifies the width, in dialog box units, of the dialog box.
            /// </summary>
            public Int16 cx;
            /// <summary>
            /// Specifies the height, in dialog box units, of the dialog box.
            /// </summary>
            public Int16 cy;
            /// <summary>
            /// Specifies the control identifier.
            /// </summary>
            public UInt32 id;
        };

        /// <summary>
        /// Window styles.
        /// </summary>
        public enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_CAPTION = 0x00C00000, /* WS_BORDER | WS_DLGFRAME */
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,
            // WS_MINIMIZEBOX = 0x00020000,
            // WS_MAXIMIZEBOX = 0x00010000,
            // WS_TILED = WS_OVERLAPPED,
            // WS_ICONIC = WS_MINIMIZE,
            // WS_SIZEBOX = WS_THICKFRAME,
            // WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
        };

        /// <summary>
        /// Dialog styles.
        /// </summary>
        public enum DialogStyles : uint
        {
            DS_ABSALIGN = 0x01,
            DS_SYSMODAL = 0x02,
            DS_LOCALEDIT = 0x20, /* Edit items get ,ocal storage. */
            DS_SETFONT = 0x40, /* User specified font for Dlg controls */
            DS_MODALFRAME = 0x80, /* Can be combined with WS_CAPTION */
            DS_NOIDLEMSG = 0x100, /* WM_ENTERID,E message will not be sent */
            DS_SETFOREGROUND = 0x200, /* not in win3.1 */
            DS_3DLOOK = 0x0004,
            DS_FIXEDSYS = 0x0008,
            DS_NOFAILCREATE = 0x0010,
            DS_CONTROL = 0x0400,
            DS_CENTER = 0x0800,
            DS_CENTERMOUSE = 0x1000,
            DS_CONTEXTHELP = 0x2000,
            DS_SHELLFONT = DS_SETFONT | DS_FIXEDSYS,
            DS_USEPIXELS = 0x8000
        };

        /// <summary>
        /// Extended dialog styles.
        /// </summary>
        public enum ExtendedDialogStyles : uint
        {
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
            WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
            WS_EX_LAYERED = 0x00080000,
            WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
            WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring
            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000,
        }

        /// <summary>
        /// Possible DLGITEMTEMPLATEEX WindowClass ordinals.
        /// </summary>
        public enum DLGITEMTEMPLATEEX_WindowClass : uint
        {
            Button = 0x0080,
            Edit = 0x0081,
            Static = 0x0082,
            ListBox = 0x0083,
            ScrollBar = 0x0084,
            ComboBox = 0x0085
        };
    }
}
