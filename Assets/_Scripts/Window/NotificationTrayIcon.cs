using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Drawing; // Nécessite System.Drawing.Common

public class NotificationTrayIcon : MonoBehaviour
{

    [DllImport("shell32.dll")]
    public static extern int Shell_NotifyIcon(int dwMessage, ref NotifyIconData lpData);

    private const int NIM_ADD = 0x00000000;
    private const int NIM_DELETE = 0x00000002;
    private const int NIF_ICON = 0x00000002;
    private const int NIF_TIP = 0x00000004;

    private NotifyIconData notifyIconData;

    //[SerializeField] private Sprite notificationIconSprite; // Exposé dans l'inspecteur
    [SerializeField] private Texture2D notificationIconTexture; // Exposé dans l'inspecteur

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

    private void Start()
    {
        if (notificationIconTexture == null)
        {
            Debug.LogError("Aucun image assigné pour l'icône de notification.");
            return;
        }

        notifyIconData = new NotifyIconData();
        notifyIconData.cbSize = Marshal.SizeOf(notifyIconData);
        notifyIconData.uID = 1;
        notifyIconData.uFlags = NIF_ICON | NIF_TIP;
        notifyIconData.szTip = "Zombie Apocalypse Overlay";

        // Convertir le Sprite en Bitmap, puis en HICON
        notifyIconData.hIcon = Texture2DToIntPtr(notificationIconTexture);
        //notifyIconData.hIcon = ConvertSpriteToIcon(notificationIconSprite);
        //notifyIconData.hIcon = LoadIconFromFile(iconPath);

        // Ajouter l'icône dans la zone de notification
        Shell_NotifyIcon(NIM_ADD, ref notifyIconData);
    }

    private void OnDestroy()
    {
        Shell_NotifyIcon(NIM_DELETE, ref notifyIconData);
    }

    private IntPtr ConvertSpriteToIcon(Sprite sprite)
    {
        Texture2D texture = sprite.texture;

        // Convertir Texture2D en Bitmap
        Bitmap bitmap = new Bitmap(texture.width, texture.height);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                UnityEngine.Color color = texture.GetPixel(x, y);
                System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(
                    (int)(color.a * 255),
                    (int)(color.r * 255),
                    (int)(color.g * 255),
                    (int)(color.b * 255));
                bitmap.SetPixel(x, y, drawingColor);
            }
        }

        // Convertir Bitmap en HICON
        return bitmap.GetHicon();
    }

    // Fonction pour charger une icône à partir d'un fichier
    private static IntPtr LoadIconFromFile(string filePath)
    {
        try
        {
            // Charger l'icône depuis le fichier .ico et obtenir son handle
            return new Icon(filePath).Handle;
        }
        catch (Exception)
        {
            return IntPtr.Zero;
        }
    }

    public static IntPtr Texture2DToIntPtr(Texture2D texture)
    {
        // Convertir la texture en un tableau d'octets (PNG)
        byte[] textureBytes = texture.EncodeToPNG();

        // Allouer un bloc de mémoire pour les octets de l'image
        IntPtr ptr = Marshal.AllocHGlobal(textureBytes.Length);

        // Copier les octets de la texture dans la mémoire allouée
        Marshal.Copy(textureBytes, 0, ptr, textureBytes.Length);

        return ptr;
    }
}
