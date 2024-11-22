using UnityEngine;
using WMG.Save;

public class SettingLoader
{
    private static string Path = Application.persistentDataPath + "/SettingsSave/";
    private static string FileName = "Settings";
    private static string Extention = ".wmgs";
    private static string FullPath = Path + FileName + Extention;

    public static void Load() => SettingsSave.Current = SaveSystem.Load<SettingsSave>(FullPath);

    public static void SaveCurrent() => SaveSystem.Save(Path, FileName, Extention, SettingsSave.Current);
}
