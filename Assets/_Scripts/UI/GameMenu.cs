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
    [SerializeField] private InputField _TargetFrameRateField;

    private void Start()
    {
        TargetFrameRateInit();
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

    private void TargetFrameRateInit()
    {
        if (_TargetFrameRateField == null)
            return;

        _TargetFrameRateField.SetTextWithoutNotify(Application.targetFrameRate.ToString());
        _TargetFrameRateField.onEndEdit.AddListener(SetTargetFrameRate);
    }

    private void SetQualityLevel(int qualityLevel) => QualitySettings.SetQualityLevel(qualityLevel);

    public void SetTargetFrameRate(string targetFrameRate)
    {
        SetTargetFrameRate(targetFrameRate.FormatToInt());
    }

    public void SetTargetFrameRate(int targetFrameRate)
    {
        targetFrameRate = Mathf.Max(targetFrameRate, 30);
        Application.targetFrameRate = targetFrameRate;
    }

    public void LoadGame(int buildIndex) => SceneManager.LoadScene(buildIndex);

    public void QuitApplication() => Application.Quit();

    private void OnApplicationQuit()
    {
        _qualityDropdown?.onValueChanged.RemoveListener(SetQualityLevel);
    }

    public void OnLoad(SettingsSave save)
    {
        QualitySettings.SetQualityLevel(save.QualityLevel);
        SetTargetFrameRate(save.TargetFrameRate);
    }

    public void OnSave(SettingsSave save)
    {
        save.QualityLevel = QualitySettings.GetQualityLevel();
        save.TargetFrameRate = Application.targetFrameRate;
    }
}
