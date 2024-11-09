using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class KeyboardHook
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;

    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;

    // Utilisation d'Action<KeyCode> pour notifier lorsqu'une touche est pressée
    public static event Action<KeyCode> KeyPressed;

    public static void SetHook()
    {
        _hookID = SetHook(_proc);
    }

    public static void Unhook()
    {
        UnhookWindowsHookEx(_hookID);
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            KeyCode keyCode = ConvertVirtualKeyCodeToKeyCode(vkCode);

            if (keyCode != KeyCode.None)  // Si la conversion est valide
            {
                KeyPressed?.Invoke(keyCode);  // Utilise Action<KeyCode> pour notifier
            }
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    // Conversion de codes de touches virtuelles en KeyCode de Unity
    private static KeyCode ConvertVirtualKeyCodeToKeyCode(int vkCode)
    {
        switch (vkCode)
        {
            case 0x41: return KeyCode.A;
            case 0x42: return KeyCode.B;
            case 0x43: return KeyCode.C;
            case 0x44: return KeyCode.D;
            case 0x45: return KeyCode.E;
            case 0x46: return KeyCode.F;
            case 0x47: return KeyCode.G;
            case 0x48: return KeyCode.H;
            case 0x49: return KeyCode.I;
            case 0x4A: return KeyCode.J;
            case 0x4B: return KeyCode.K;
            case 0x4C: return KeyCode.L;
            case 0x4D: return KeyCode.M;
            case 0x4E: return KeyCode.N;
            case 0x4F: return KeyCode.O;
            case 0x50: return KeyCode.P;
            case 0x51: return KeyCode.Q;
            case 0x52: return KeyCode.R;
            case 0x53: return KeyCode.S;
            case 0x54: return KeyCode.T;
            case 0x55: return KeyCode.U;
            case 0x56: return KeyCode.V;
            case 0x57: return KeyCode.W;
            case 0x58: return KeyCode.X;
            case 0x59: return KeyCode.Y;
            case 0x5A: return KeyCode.Z;
            case 0x30: return KeyCode.Alpha0;
            case 0x31: return KeyCode.Alpha1;
            case 0x32: return KeyCode.Alpha2;
            case 0x33: return KeyCode.Alpha3;
            case 0x34: return KeyCode.Alpha4;
            case 0x35: return KeyCode.Alpha5;
            case 0x36: return KeyCode.Alpha6;
            case 0x37: return KeyCode.Alpha7;
            case 0x38: return KeyCode.Alpha8;
            case 0x39: return KeyCode.Alpha9;
            case 0x0D: return KeyCode.Return;
            case 0x20: return KeyCode.Space;
            case 0x08: return KeyCode.Backspace;
            case 0x09: return KeyCode.Tab;
            case 0x10: return KeyCode.LeftShift;
            case 0x11: return KeyCode.LeftControl;
            case 0x1B: return KeyCode.Escape;
            case 0x25: return KeyCode.LeftArrow;
            case 0x27: return KeyCode.RightArrow;
            case 0x26: return KeyCode.UpArrow;
            case 0x28: return KeyCode.DownArrow;
            default: return KeyCode.None;
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}
