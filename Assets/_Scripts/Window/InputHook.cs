using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace WMG.OverlayWindow
{
    /// <summary>
    /// Classe principale pour gérer les hooks clavier et souris.
    /// Capture les entrées utilisateur globalement même lorsque l'application n'est pas en focus.
    /// </summary>
    public static class InputHook
    {
        // Hashset pour garder en mémoire quelles touches sont appuyées
        private static HashSet<KeyCode> _keysPressed = new HashSet<KeyCode>();

        // Constantes pour identifier les types de hooks
        private const int WH_KEYBOARD_LL = 13; // Hook clavier bas-niveau
        private const int WH_MOUSE_LL = 14;    // Hook souris bas-niveau

        // Constantes pour les événements Windows
        private const int WM_KEYDOWN = 0x0100;      // Touche pressée
        private const int WM_KEYUP = 0x0101;        // Touche relâchée
        private const int WM_LBUTTONDOWN = 0x0201;  // Clic gauche
        private const int WM_RBUTTONDOWN = 0x0204;  // Clic droit
        private const int WM_MBUTTONDOWN = 0x0207;  // Clic du milieu

        // Enumérations pour les types d'événements gérés
        public enum MouseClickType { LeftClick, RightClick, MiddleClick }
        public enum HookType { Keyboard, Mouse }

        /// <summary>
        /// Délégué pour le hook clavier bas-niveau.
        /// </summary>
        /// <param name="nCode">Le code indiquant comment traiter l'événement.</param>
        /// <param name="wParam">Paramètre pour l'événement Windows.</param>
        /// <param name="lParam">Paramètre additionnel contenant des informations sur l'événement.</param>
        /// <returns>Un pointeur vers le prochain hook de la chaîne.</returns>
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Délégué pour le hook souris bas-niveau.
        /// </summary>
        /// <param name="nCode">Le code indiquant comment traiter l'événement.</param>
        /// <param name="wParam">Paramètre pour l'événement Windows.</param>
        /// <param name="lParam">Paramètre additionnel contenant des informations sur l'événement.</param>
        /// <returns>Un pointeur vers le prochain hook de la chaîne.</returns>
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);


        // Délégués pointant sur les callbacks pour clavier et souris
        private static LowLevelKeyboardProc _keyboardProc = KeyboardHookCallback;
        private static LowLevelMouseProc _mouseProc = MouseHookCallback;

        // Handles pour stocker les hooks actifs
        private static IntPtr _keyboardHookID = IntPtr.Zero;
        private static IntPtr _mouseHookID = IntPtr.Zero;

        // Événements pour notifier les actions détectées
        public static event Action<KeyCode> KeyPressed;
        public static event Action<KeyCode> KeyReleased;
        public static event Action<MouseClickType> MouseClicked;

        /// <summary>
        /// Active un hook (clavier ou souris).
        /// </summary>
        public static void EnableHook(HookType hookType)
        {
            switch (hookType)
            {
                case HookType.Keyboard:
                    if (_keyboardHookID == IntPtr.Zero)
                        _keyboardHookID = SetHook(WH_KEYBOARD_LL, _keyboardProc);
                    break;

                case HookType.Mouse:
                    if (_mouseHookID == IntPtr.Zero)
                        _mouseHookID = SetHook(WH_MOUSE_LL, _mouseProc);
                    break;
            }
        }

        /// <summary>
        /// Désactive un hook actif.
        /// </summary>
        public static void DisableHook(HookType hookType)
        {
            switch (hookType)
            {
                case HookType.Keyboard:
                    if (_keyboardHookID != IntPtr.Zero)
                    {
                        UnhookWindowsHookEx(_keyboardHookID);
                        _keyboardHookID = IntPtr.Zero;
                    }
                    break;

                case HookType.Mouse:
                    if (_mouseHookID != IntPtr.Zero)
                    {
                        UnhookWindowsHookEx(_mouseHookID);
                        _mouseHookID = IntPtr.Zero;
                    }
                    break;
            }
        }

        /// <summary>
        /// Définit un hook système avec l'API Windows.
        /// </summary>
        private static IntPtr SetHook(int idHook, Delegate proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(idHook, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        /// <summary>
        /// Callback pour capturer les événements clavier.
        /// </summary>
        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Vérifie si l'événement doit être traité
            if (nCode >= 0)
            {
                // Récupère le code de la touche virtuelle
                int vkCode = Marshal.ReadInt32(lParam);

                // Convertit le code virtuel en KeyCode Unity
                KeyCode keyCode = InputHookUtility.ConvertVirtualKeyCodeToKeyCode(vkCode);

                // Si la touche est valide (non mappée à KeyCode.None)
                if (keyCode != KeyCode.None)
                {
                    // Vérifie si l'événement est une pression de touche (KeyDown)
                    if (wParam == (IntPtr)WM_KEYDOWN)
                    {
                        // Si la touche n'est pas déjà dans le HashSet, c'est la première pression
                        if (!_keysPressed.Contains(keyCode))
                        {
                            _keysPressed.Add(keyCode); // Ajoute la touche au HashSet
                            KeyPressed?.Invoke(keyCode); // Déclenche l'événement de touche pressée
                        }
                    }
                    // Vérifie si l'événement est un relâchement de touche (KeyUp)
                    else if (wParam == (IntPtr)WM_KEYUP)
                    {
                        // Retire la touche du HashSet lorsqu'elle est relâchée
                        if (_keysPressed.Contains(keyCode))
                        {
                            _keysPressed.Remove(keyCode);
                            KeyReleased?.Invoke(keyCode); // Déclenche l'événement de touche relâchée
                        }
                    }
                }
            }

            // Passe l'événement au prochain hook de la chaîne
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
        }


        /// <summary>
        /// Callback pour capturer les événements souris.
        /// </summary>
        private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)WM_LBUTTONDOWN)
                {
                    MouseClicked?.Invoke(MouseClickType.LeftClick);
                }
                else if (wParam == (IntPtr)WM_RBUTTONDOWN)
                {
                    MouseClicked?.Invoke(MouseClickType.RightClick);
                }
                else if (wParam == (IntPtr)WM_MBUTTONDOWN)
                {
                    MouseClicked?.Invoke(MouseClickType.MiddleClick);
                }
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam); // Appelle le prochain hook
        }

        // Fonctions externes de l'API Windows
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
