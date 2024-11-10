using System;
using System.Drawing;
using System.IO;
using UnityEngine;
using Color = UnityEngine.Color;
using UnityEngine.UI;  // Pour Sprite et Texture2D

public static class TrayIconUtilityOld
{
    public static Icon ConvertSpriteToIcon(Sprite sprite)
    {
        // Étape 1 : Convertir le Sprite en Texture2D
        Texture2D texture = SpriteToTexture2D(sprite);

        // Étape 2 : Encoder Texture2D en PNG pour obtenir un tableau de bytes
        byte[] pngData = texture.EncodeToPNG();

        // Étape 3 : Charger l'image en tant qu'Image pour System.Drawing
        using (MemoryStream ms = new MemoryStream(pngData))
        {
            Bitmap bitmap = new Bitmap(ms);

            // Étape 4 : Redimensionner l'image au format d'une icône (16x16, 32x32, etc.)
            Bitmap resizedBitmap = new Bitmap(bitmap, new Size(32, 32));

            // Utilisez Icon.FromHandle pour créer une icône à partir du Bitmap
            IntPtr iconHandle = resizedBitmap.GetHicon();
            return Icon.FromHandle(iconHandle);
        }
    }

    public static Texture2D SpriteToTexture2D(Sprite sprite)
    {
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                  (int)sprite.textureRect.y,
                                                  (int)sprite.textureRect.width,
                                                  (int)sprite.textureRect.height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
