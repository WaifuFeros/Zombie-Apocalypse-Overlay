using UnityEngine;
using WMG.Save;

public class SettingLoader
{
    public static string Path => Application.persistentDataPath + "/SettingsSave/";
    public static string FileName => "Settings";
    public static string Extention => ".wmgs";
    public static string FullPath => Path + FileName + Extention;

    public static void Load() => SettingsSave.Current = SaveSystem.Load<SettingsSave>(FullPath);

    public static void SaveCurrent() => SaveSystem.Save(Path, FileName, Extention, SettingsSave.Current);
}
