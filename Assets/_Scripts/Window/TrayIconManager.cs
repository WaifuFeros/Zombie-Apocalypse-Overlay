using System.Drawing;
using UnityEngine;
using UnityTrayPlugin;

public class TrayIconManager : MonoBehaviour
{
    [SerializeField] private Sprite spriteIcon;
    private TrayIcon trayIcon;

    void Start()
    {
        trayIcon = new TrayIcon();

        // Chemin vers l'icône dans le dossier d'Unity
        //Icon icon = TrayIconUtility.ConvertSpriteToIcon(spriteIcon);
        //trayIcon.InitializeTrayIcon(icon, "Zombie Apocalypse Overlay");
    }

    void OnApplicationQuit()
    {
        trayIcon.Dispose();
    }
}
