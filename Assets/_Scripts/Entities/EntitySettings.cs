using System;
using UnityEngine;
using UnityEngine.UI;

namespace WMG.ZombieApocalypseOverlay
{
    public class EntitySettings : MonoBehaviour, ISavedComponent<SettingsSave>
    {
        public static Action<float> OnAlphaChanged;
        public static Action<float> OnSizeChanged;

        [SerializeField] private Slider _alphaSlider;
        [SerializeField] private Slider _sizeSlider;
        [SerializeField] private RawImage _texture;

        private Color _currentColor = Color.white;
        private float _alpha;
        private float _size;

        private void Start()
        {
            OnAlphaChanged += SetAlpha;
            _alphaSlider.value = _alpha;
            _sizeSlider.value = _size;
            SetAlpha(_alpha);
        }

        public void SetAlpha(float alpha)
        {
            _currentColor.a = alpha;
            _texture.color = _currentColor;
        }

        public void ChangeAlpha(float alpha)
        {
            _alpha = Mathf.Clamp01(alpha);

            OnAlphaChanged?.Invoke(_alpha);
        }

        public void ChangeSize(float size)
        {
            _size = Mathf.Max(0f, size);

            OnSizeChanged?.Invoke(_size);
        }

        public void OnLoad(SettingsSave save)
        {
            _alpha = save.Alpha;
            _size = save.EntitySize;
        }

        public void OnSave(SettingsSave save)
        {
            save.Alpha = _alphaSlider.value;
            save.EntitySize = _sizeSlider.value;
        }

        private void OnApplicationQuit()
        {
            OnAlphaChanged = null;
            OnSizeChanged = null;
        }
    }
}
