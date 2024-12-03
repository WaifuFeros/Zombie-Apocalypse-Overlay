using UnityEngine;

namespace WMG.OverlayWindow
{
    /// <summary>
    /// Classe utilitaire pour convertir les codes de touches virtuelles Windows en KeyCode Unity.
    /// </summary>
    public static class InputHookUtility
    {
        /// <summary>
        /// Convertit un code de touche virtuelle Windows en KeyCode Unity.
        /// </summary>
        /// <param name="vkCode">Code de touche virtuelle Windows.</param>
        /// <returns>Le KeyCode correspondant, ou KeyCode.None si non mappé.</returns>
        public static KeyCode ConvertVirtualKeyCodeToKeyCode(int vkCode)
        {
            switch (vkCode)
            {
                // Lettres A-Z
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

                // Chiffres 0-9
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

                // Touches de contrôle
                case 0x1B: return KeyCode.Escape;
                case 0x08: return KeyCode.Backspace;
                case 0x09: return KeyCode.Tab;
                case 0x0D: return KeyCode.Return;
                case 0x20: return KeyCode.Space;
                case 0x2E: return KeyCode.Delete;
                case 0x2D: return KeyCode.Insert;
                case 0x5B: return KeyCode.LeftMeta;
                case 0x5C: return KeyCode.RightMeta;
                case 0x21: return KeyCode.PageUp;
                case 0x22: return KeyCode.PageDown;
                case 0x23: return KeyCode.End;
                case 0x2C: return KeyCode.Print;

                // Flèches directionnelles
                case 0x25: return KeyCode.LeftArrow;
                case 0x26: return KeyCode.UpArrow;
                case 0x27: return KeyCode.RightArrow;
                case 0x28: return KeyCode.DownArrow;

                // Touches de fonction F1-F12
                case 0x70: return KeyCode.F1;
                case 0x71: return KeyCode.F2;
                case 0x72: return KeyCode.F3;
                case 0x73: return KeyCode.F4;
                case 0x74: return KeyCode.F5;
                case 0x75: return KeyCode.F6;
                case 0x76: return KeyCode.F7;
                case 0x77: return KeyCode.F8;
                case 0x78: return KeyCode.F9;
                case 0x79: return KeyCode.F10;
                case 0x7A: return KeyCode.F11;
                case 0x7B: return KeyCode.F12;

                // Pavé numérique
                case 0x60: return KeyCode.Keypad0;
                case 0x61: return KeyCode.Keypad1;
                case 0x62: return KeyCode.Keypad2;
                case 0x63: return KeyCode.Keypad3;
                case 0x64: return KeyCode.Keypad4;
                case 0x65: return KeyCode.Keypad5;
                case 0x66: return KeyCode.Keypad6;
                case 0x67: return KeyCode.Keypad7;
                case 0x68: return KeyCode.Keypad8;
                case 0x69: return KeyCode.Keypad9;
                case 0x6A: return KeyCode.KeypadMultiply;
                case 0x6B: return KeyCode.KeypadPlus;
                case 0x6D: return KeyCode.KeypadMinus;
                case 0x6E: return KeyCode.KeypadPeriod;
                case 0x6F: return KeyCode.KeypadDivide;

                // Symboles
                case 0xBA: return KeyCode.Semicolon; // ;
                case 0xBB: return KeyCode.Equals;   // =
                case 0xBC: return KeyCode.Comma;    // ,
                case 0xBD: return KeyCode.Minus;    // -
                case 0xBE: return KeyCode.Period;   // .
                case 0xBF: return KeyCode.Slash;    // /
                case 0xC0: return KeyCode.BackQuote; // `
                case 0xDB: return KeyCode.LeftBracket; // [
                case 0xDC: return KeyCode.Backslash; // \
                case 0xDD: return KeyCode.RightBracket; // ]
                case 0xDE: return KeyCode.Quote; // '

                // Modificateurs
                case 0x10: return KeyCode.LeftShift;
                case 0x11: return KeyCode.LeftControl;
                case 0x12: return KeyCode.LeftAlt;
                case 0xA0: return KeyCode.LeftShift;
                case 0xA1: return KeyCode.RightShift;
                case 0xA2: return KeyCode.LeftControl;
                case 0xA3: return KeyCode.RightControl;
                case 0xA4: return KeyCode.LeftAlt;
                case 0xA5: return KeyCode.RightAlt;

                // Cas non pris en charge
                default: return KeyCode.None; // Non mappé
            }
        }
    }
}
