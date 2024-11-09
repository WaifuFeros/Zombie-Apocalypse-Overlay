using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TransparentWindowMultiDesktop : MonoBehaviour
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private const int GWL_EXSTYLE = -20;
    private const uint WS_EX_LAYERED = 0x00080000;
    private const uint WS_EX_TRANSPARENT = 0x00000020;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    private void Start()
    {
        IntPtr hWnd = GetActiveWindow();

        // Étendre la transparence dans la fenêtre
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        // Configurer la fenêtre pour être "click-through"
        uint currentStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
        SetWindowLong(hWnd, GWL_EXSTYLE, currentStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);

        // Placer la fenêtre en topmost
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

        // Configurer la fenêtre pour qu'elle apparaisse sur tous les bureaux
        MakeWindowShowOnAllDesktops(hWnd);
    }

    private void MakeWindowShowOnAllDesktops(IntPtr hWnd)
    {
        Guid CLSID_VirtualDesktopManager = new Guid("aa509086-5ca9-4c25-8f95-589d3c07b48a");
        Type vdmType = Type.GetTypeFromCLSID(CLSID_VirtualDesktopManager);
        IVirtualDesktopManager vdm = (IVirtualDesktopManager)Activator.CreateInstance(vdmType);

        vdm.IsWindowOnCurrentVirtualDesktop(hWnd, out bool onCurrentDesktop);

        if (!onCurrentDesktop)
        {
            vdm.MoveWindowToDesktop(hWnd, IntPtr.Zero); // Place la fenêtre sur tous les bureaux
        }
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("a5cd92ff-29be-454c-8d04-d82879fb3f1b")]
    private interface IVirtualDesktopManager
    {
        void IsWindowOnCurrentVirtualDesktop(IntPtr hWnd, out bool onCurrentDesktop);
        void MoveWindowToDesktop(IntPtr hWnd, IntPtr desktopId);
    }
}
