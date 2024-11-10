using UnityEngine;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{
    [SerializeField] private Text _comboText;

    [SerializeField] private float _timeBeforeComboReset = 5f;

    private string GetComboText => $"{_currentCombo} HIT{(_currentCombo > 1 ? "S" : string.Empty)}";
    private int _currentCombo;
    private float _lastKeyPress;

    private void Start()
    {
        InputHook.KeyPressed += KeyPressed;
    }

    private void Update()
    {
        if (_lastKeyPress + _timeBeforeComboReset < Time.time)
            ResetCombo();
    }

    private void ResetCombo()
    {
        _comboText.gameObject.SetActive(false);
        _currentCombo = 0;
        _comboText.text = GetComboText;
    }

    private void KeyPressed(KeyCode obj)
    {
        _currentCombo++;
        _comboText.text = GetComboText;
        _comboText.gameObject.SetActive(true);
        _lastKeyPress = Time.time;
    }

}
