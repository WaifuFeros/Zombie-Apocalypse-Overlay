using System;
using System.Runtime.InteropServices;
using System.Drawing; // N�cessite System.Drawing.Common pour .NET Core ou Framework

public class TrayIconUtility
{
    // D�claration des constantes utilis�es par l'API Shell_NotifyIcon
    private const int NIM_ADD = 0x00000000;
    private const int NIM_DELETE = 0x00000002;
    private const int NIF_ICON = 0x00000002;
    private const int NIF_TIP = 0x00000004;
    private const int WM_USER = 0x0400;

    // Structure de donn�es pour g�rer l'ic�ne dans la zone de notification
    [StructLayout(LayoutKind.Sequential)]
    public struct NotifyIconData
    {
        public int cbSize;
        public IntPtr hWnd;
        public int uID;
        public int uFlags;
        public int uCallbackMessage;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.LPStr)]
        public string szTip;
    }

    // D�clarations de P/Invoke pour interagir avec l'API Windows
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetConsoleWindow();

    [DllImport("shell32.dll")]
    public static extern int Shell_NotifyIcon(int dwMessage, ref NotifyIconData lpData);

    // M�thode pour initialiser l'ic�ne dans le System Tray
    public static void InitializeTrayIcon(string iconPath, string toolTip)
    {
        IntPtr hIcon = LoadIconFromFile(iconPath);

        // V�rifier que l'ic�ne a bien �t� charg�e
        if (hIcon == IntPtr.Zero)
        {
            Console.WriteLine("Erreur de chargement de l'ic�ne");
            return;
        }

        // Initialisation des donn�es pour l'ic�ne dans la zone de notification
        NotifyIconData notifyIconData = new NotifyIconData
        {
            cbSize = Marshal.SizeOf(typeof(NotifyIconData)),
            hWnd = GetConsoleWindow(), // Fen�tre de la console
            uID = 1,
            uFlags = NIF_ICON | NIF_TIP,
            uCallbackMessage = WM_USER,
            hIcon = hIcon,
            szTip = toolTip
        };

        // Ajouter l'ic�ne au System Tray
        Shell_NotifyIcon(NIM_ADD, ref notifyIconData);

        // Supprimer l'ic�ne lorsque l'application se ferme
        //Shell_NotifyIcon(NIM_DELETE, ref notifyIconData);
    }

    // Fonction pour charger une ic�ne � partir d'un fichier
    private static IntPtr LoadIconFromFile(string filePath)
    {
        try
        {
            // Charger l'ic�ne depuis le fichier .ico et obtenir son handle
            return new Icon(filePath).Handle;
        }
        catch (Exception)
        {
            return IntPtr.Zero;
        }
    }
}
