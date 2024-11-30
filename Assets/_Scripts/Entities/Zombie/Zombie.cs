using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WMG.ZombieApocalypseOverlay
{
    public class Zombie : MonoBehaviour
    {
        public int HP { get; private set; } = 10;

        public System.Action<Zombie> OnDeath;

        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _renderer;

        [SerializeField, Min(0)] private int _startingHP = 10;
        [SerializeField] private ValueRange _speedRange = new ValueRange(0.8f, 1.2f);
        [SerializeField] private float _speedMultiplier = 0.1f;
        [SerializeField] private ValueRange _killzone = new ValueRange(-9.5f, 9.5f);

        private float _speed = 0.1f;

        private void Awake()
        {
            //EntitySettings.OnAlphaChanged += ChangeAlpha;
            EntitySettings.OnSizeChanged += ChangeSize;
        }

        private void Update()
        {
            transform.Translate(Vector3.right * _speed * Time.deltaTime);
            if (transform.position.x < _killzone.Min || transform.position.x > _killzone.Max)
                Die();
        }

        public void TakeDamage(int damage)
        {
            HP -= damage;
            _animator.SetTrigger("Hit");
            if (HP <= 0)
                Die();
        }

        private void Die()
        {
            ZombieWaveManager.RemoveZombie(this);
            gameObject.SetActive(false);
            Color resetColor = _renderer.color;
            resetColor.r = 1;
            resetColor.g = 1;
            resetColor.b = 1;
            _renderer.color = resetColor;

            OnDeath?.Invoke(this);
        }
        public void PoolReset()
        {
            HP = _startingHP;
            _speed = Random.Range(_speedRange.Min, _speedRange.Max) * _speedMultiplier;
            ZombieWaveManager.AddZombie(this);
        }

        public void ChangeSize(float size) => transform.localScale = Vector3.one * size;

        public void ChangeAlpha(float alpha)
        {
            Color newAlpha = _renderer.color;
            newAlpha.a = alpha;

            _renderer.color = newAlpha;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 center = Vector3.zero;
            center.x += (_killzone.Min + _killzone.Max) / 2;
            center.y = transform.position.y;
            float size = _killzone.Max - _killzone.Min;
            Gizmos.DrawWireCube(center, new Vector3(size, 1, 0));
        }
    } 
}
