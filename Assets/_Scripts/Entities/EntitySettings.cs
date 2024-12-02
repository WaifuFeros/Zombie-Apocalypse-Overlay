using System;
using UnityEngine;
using UnityEngine.UI;

namespace WMG.ZombieApocalypseOverlay
{
    public class EntitySettings : MonoBehaviour, ISavedComponent<SettingsSave>
    {
        public static Action<float> OnAlphaChanged;
        public static Action<float> OnSizeChanged;

        public bool IsHovering { get; set; }

        [Header("Settings")]
        [SerializeField] private Slider _alphaSlider;
        [SerializeField] private Slider _hoveredAlphaSlider;
        [SerializeField] private Slider _sizeSlider;

        [Header("Hover")]
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Camera _camera;

        [Header("Alpha")]
        [SerializeField] private RawImage _texture;
        [SerializeField] private Transform _trigger;
        [SerializeField] private float _fadeDuration = 0.1f;

        private float _size;

        private float _baseAlpha;
        private float _hoveredAlpha;
        private float _currentAlpha;
        private float _targetAlpha;
        private Color _currentColor = Color.white;
        private float _fadeDelta => (1 / _fadeDuration) * Time.deltaTime;

        private bool _lastFrameHovering;

        private void Start()
        {
            OnAlphaChanged += SetAlpha;

            _alphaSlider.value = _baseAlpha;
            _hoveredAlphaSlider.value = _hoveredAlpha;
            _sizeSlider.value = _size;

            _currentAlpha = _baseAlpha;
            _targetAlpha = _baseAlpha;
            SetAlpha(_baseAlpha, true);
        }

        private void Update()
        {
            HoverHandle();

            _currentAlpha = Mathf.MoveTowards(_currentAlpha, _targetAlpha, _fadeDelta);
            SetRendererAlpha(_currentAlpha);
        }

        public void OnLoad(SettingsSave save)
        {
            _baseAlpha = save.Alpha;
            _hoveredAlpha = save.HoveredAlpha;
            _size = save.EntitySize;
        }

        public void OnSave(SettingsSave save)
        {
            save.Alpha = _baseAlpha;
            save.HoveredAlpha = _hoveredAlpha;
            save.EntitySize = _size;
        }

        public void ChangeAlpha(float alpha)
        {
            SetAlpha(alpha);

            OnAlphaChanged?.Invoke(_baseAlpha);
        }

        public void SetAlpha(float alpha)
        {
            alpha = Mathf.Clamp01(alpha);
            _baseAlpha = alpha;
            _targetAlpha = IsHovering ? _hoveredAlpha : _baseAlpha;
        }

        public void SetAlpha(float alpha, bool setRenderer = false)
        {
            SetAlpha(alpha);
            if (setRenderer)
            {
                _currentAlpha = alpha;
                SetRendererAlpha(alpha);
            }
        }

        public void SetRendererAlpha(float alpha)
        {
            _currentColor.a = alpha;
            _texture.color = _currentColor;
        }

        public void ChangeHoveredAlpha(float hoveredAlpha)
        {
            _hoveredAlpha = Mathf.Clamp01(hoveredAlpha);
        }

        public void ChangeSize(float size)
        {
            _size = Mathf.Max(0f, size);
            _trigger.localScale = _trigger.localScale.With(y: size);

            OnSizeChanged?.Invoke(_size);
        }

        private void HoverHandle()
        {
            var bounds = _collider.bounds;
            IsHovering = _collider.bounds.Contains(_camera.ScreenToWorldPoint(Input.mousePosition).With(z: 0));

            if (IsHovering != _lastFrameHovering)
                HoverTexture(IsHovering);

            _lastFrameHovering = IsHovering;
        }

        public void HoverTexture(bool hovered)
        {
            if (hovered)
            {
                _targetAlpha = _hoveredAlpha;
            }
            else
            {
                _targetAlpha = _baseAlpha;
            }
        }

        private void OnApplicationQuit()
        {
            OnAlphaChanged = null;
            OnSizeChanged = null;
        }
    }
}
