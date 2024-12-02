using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WMG;

public class GameMenu : MonoBehaviour, ISavedComponent<SettingsSave>
{
    [SerializeField] private Dropdown _qualityDropdown;

    private void Start()
    {
        QualityLevelInit();
    }

    private void QualityLevelInit()
    {
        if (_qualityDropdown == null)
            return;

        var qualityLevels = QualitySettings.names;

        _qualityDropdown.options.Clear();
        foreach (var level in qualityLevels)
        {
            _qualityDropdown.options.Add(new Dropdown.OptionData(level));
        }

        _qualityDropdown.SetValueWithoutNotify(QualitySettings.GetQualityLevel());
        _qualityDropdown.onValueChanged.AddListener(SetQualityLevel);
    }

    private void SetQualityLevel(int qualityLevel) => QualitySettings.SetQualityLevel(qualityLevel);

    public void LoadGame(int buildIndex) => SceneManager.LoadScene(buildIndex);

    public void QuitApplication() => Application.Quit();

    private void OnApplicationQuit()
    {
        _qualityDropdown?.onValueChanged.RemoveListener(SetQualityLevel);
    }

    public void OnLoad(SettingsSave save)
    {
        QualitySettings.SetQualityLevel(save.QualityLevel);
    }

    public void OnSave(SettingsSave save)
    {
        save.QualityLevel = QualitySettings.GetQualityLevel();
    }
}
