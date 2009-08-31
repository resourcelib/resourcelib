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
    }
}
