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

        private void Awake()
        {
            SettingLoader.Load();
            if (SettingsSave.Current == null)
                SettingsSave.Current = new SettingsSave();
            components = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ISavedComponent<SettingsSave>>().ToList();

            foreach (var component in components)
            {
                component.OnLoad(SettingsSave.Current);
            }
        }

        private void OnApplicationQuit()
        {
            foreach (var component in components)
            {
                component.OnSave(SettingsSave.Current);
            }
            SettingLoader.SaveCurrent();
        }
    }

    public interface ISavedComponent<T> where T : SaveFile
    {
        void OnLoad(T save);
        void OnSave(T save);
    }
}
