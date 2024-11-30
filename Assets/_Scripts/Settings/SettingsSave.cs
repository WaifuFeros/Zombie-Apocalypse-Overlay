using System;
using UnityEngine;
using WMG;
using WMG.Save;

[System.Serializable]
public class SettingsSave : SaveFile
{
    #region Current settings and OnChanged callback
    public static Action<SettingsSave> OnChanged;

    private static SettingsSave _current;
    public static SettingsSave Current
    {
        get
        {
            if (_current == null)
            {
                _current = new SettingsSave();
            }
            return _current;
        }
        set
        {
            _current = value;
            OnChanged?.Invoke(_current);
        }
    }

    public static void ApplyChangedSettings() => OnChanged?.Invoke(_current);
    #endregion

    // General
    public float GroundHeight = 0.2f;
    public float EntitySize = 1;
    public float Alpha = 1;

    // Zombies
    public int ZombieLimit = 10;
    public int ZombieHP = 10;
    public ValueRange ZombieSpeed = new ValueRange(0.8f, 3f);
    public ValueRange SpawnBoundX = new ValueRange(-9.5f, -9.5f);
    public ValueRange SpawnBoundY = new ValueRange(-0.2f, 0.2f);

    // Charge Generation
    public float ChargeOverTime = 20;
    public float ChargeOnClick = 70;
    public float ChargePerShot = 5;
}
