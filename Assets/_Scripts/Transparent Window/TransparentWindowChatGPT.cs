using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TransparentWindowChatGPT : MonoBehaviour
{
    // Import de la fonction GetActiveWindow pour obtenir le handle de la fenêtre actuelle
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    // Import de la fonction SetWindowLong pour définir des styles de fenêtre
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    // Import de la fonction pour obtenir le style actuel de la fenêtre
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    // Import de la fonction pour définir la position de la fenêtre
    [DllImport("user32.dll")]
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

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    private void Start()
    {
        IntPtr hWnd = GetActiveWindow();

        // Étendre la transparence dans la fenêtre
        MARGINS margin = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margin);

        // Appliquer les styles WS_EX_LAYERED et WS_EX_TRANSPARENT pour rendre la fenêtre "click-through"
        uint currentStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
        SetWindowLong(hWnd, GWL_EXSTYLE, currentStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);

        // Configurer la fenêtre pour qu'elle reste toujours au-dessus des autres (topmost)
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
    }
}
