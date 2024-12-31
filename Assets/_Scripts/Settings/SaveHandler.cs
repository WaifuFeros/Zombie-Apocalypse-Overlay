using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WMG.Save;

namespace WMG
{
    public class SaveHandler : MonoBehaviour
    {
        private List<ISavedComponent<SettingsSave>> components;
        private bool _dontSave;

        private void Awake()
        {
            SettingLoader.Load();
            if (SettingsSave.Current == null)
            {
                Debug.LogError("No save found. Creating one.");
                SettingsSave.Current = new SettingsSave();
                SettingsSave.Current.TargetFrameRate = (int)Screen.resolutions.Select(x => x.refreshRateRatio.value).OrderByDescending(x => x).First();
                Debug.LogError($"set frame rate : {SettingsSave.Current.TargetFrameRate}");
            }
            components = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISavedComponent<SettingsSave>>().ToList();

            foreach (var component in components)
            {
                component.OnLoad(SettingsSave.Current);
            }
        }

        private void OnApplicationQuit()
        {
            SettingsSave.OnChanged = null;

            if (_dontSave)
                return;

            foreach (var component in components)
            {
                component.OnSave(SettingsSave.Current);
            }
            SettingLoader.SaveCurrent();
        }

        public void DeleteSave()
        {
            _dontSave = true;
            SaveSystem.DeleteFile(SettingLoader.FullPath);
            SettingsSave.Current = null;
            Application.Quit();
        }
    }

    public interface ISavedComponent<T> where T : SaveFile
    {
        void OnLoad(T save);
        void OnSave(T save);
    }
}
