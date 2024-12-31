using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Events;

public class TrayIconManager : MonoBehaviour
{
    [SerializeField] private Sprite spriteIcon;
    [SerializeField] private List<NotifyIconOption> options;
    private NotifyIcon _trayIcon;
    private int _width = 32;
    private int _height = 32;

    void Start()
    {
#if (!UNITY_EDITOR)
        _trayIcon = CreateNotifyIcon();
#endif

        //trayIcon = new TrayIcon();

        // Chemin vers l'icône dans le dossier d'Unity
        //Icon icon = TrayIconUtility.ConvertSpriteToIcon(spriteIcon);
        //trayIcon.InitializeTrayIcon(icon, "Zombie Apocalypse Overlay");
    }

    void OnApplicationQuit()
    {
        _trayIcon?.Dispose();
    }

    private NotifyIcon CreateNotifyIcon()
    {
        var icon = new NotifyIcon()
        {
            ContextMenuStrip = new ContextMenuStrip(),
            Icon = SpriteToIcon(spriteIcon, _width, _height),
            Text = "Zombie Apocalypse Overlay",
            Visible = true,
        };

        foreach (NotifyIconOption option in options)
        {
            icon.ContextMenuStrip.Items.Add(option.name, null, (sender, args) => option.OnClick?.Invoke());
        }

        return icon;
    }

    public void Close()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
        UnityEngine.Application.Quit();
    }

    /// <summary>
    /// Convertit une Texture2D Unity en une Icon Windows Forms.
    /// </summary>
    /// <param name="texture">La Texture2D Unity à convertir.</param>
    /// <param name="width">Largeur de l'icône.</param>
    /// <param name="height">Hauteur de l'icône.</param>
    /// <returns>Une Icon Windows Forms.</returns>
    public static System.Drawing.Icon Texture2DToIcon(Texture2D texture, int width, int height)
    {
        // Étape 1 : Encoder la Texture2D en PNG
        byte[] textureBytes = texture.EncodeToPNG();

        // Étape 2 : Charger le tableau d'octets dans un Bitmap
        using (MemoryStream memoryStream = new MemoryStream(textureBytes))
        {
            Bitmap bitmap = new Bitmap(memoryStream);

            // Étape 3 : Redimensionner le Bitmap (optionnel, pour s'assurer de la taille de l'icône)
            Bitmap resizedBitmap = new Bitmap(bitmap, new System.Drawing.Size(width, height));

            // Étape 4 : Convertir le Bitmap en Icon
            return System.Drawing.Icon.FromHandle(resizedBitmap.GetHicon());
        }
    }

    /// <summary>
    /// Convertit un Sprite Unity en une Icon Windows Forms.
    /// </summary>
    /// <param name="sprite">Le Sprite Unity à convertir.</param>
    /// <param name="width">Largeur de l'icône.</param>
    /// <param name="height">Hauteur de l'icône.</param>
    /// <returns>Une Icon Windows Forms.</returns>
    public static System.Drawing.Icon SpriteToIcon(Sprite sprite, int width, int height)
    {
        // Étape 1 : Extraire la texture du Sprite
        Texture2D texture = sprite.texture;

        // Étape 2 : Extraire les dimensions spécifiques du sprite (découpage dans la texture)
        Rect spriteRect = sprite.rect;
        Texture2D croppedTexture = new Texture2D((int)spriteRect.width, (int)spriteRect.height);

        // Copier les pixels du Sprite dans une nouvelle texture
        UnityEngine.Color[] pixels = texture.GetPixels(
            (int)spriteRect.x,
            (int)spriteRect.y,
            (int)spriteRect.width,
            (int)spriteRect.height
        );
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();

        // Étape 3 : Encoder la texture découpée en PNG
        byte[] textureBytes = texture.EncodeToPNG();

        // Étape 4 : Charger les octets PNG dans un Bitmap
        using (MemoryStream memoryStream = new MemoryStream(textureBytes))
        {
            Bitmap bitmap = new Bitmap(memoryStream);

            // Étape 5 : Redimensionner le Bitmap à la taille d'une icône
            Bitmap resizedBitmap = new Bitmap(bitmap, new System.Drawing.Size(width, height));

            // Étape 6 : Convertir le Bitmap en Icon
            return System.Drawing.Icon.FromHandle(resizedBitmap.GetHicon());
        }
    }

    [Serializable]
    struct NotifyIconOption
    {
        public string name;
        public UnityEvent OnClick;
    }
}