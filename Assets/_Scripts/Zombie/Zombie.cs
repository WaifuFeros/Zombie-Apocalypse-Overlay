using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Zombie : MonoBehaviour
{
    public int HP { get; private set; } = 10;

    public System.Action<Zombie> OnDeath;
    
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _renderer;

    [SerializeField, Min(0)] private int _startingHP = 10;
    [SerializeField] private Vector2 _speedRange = new Vector2(0.8f, 1.2f);
    [SerializeField] private float _speedMultiplier = 0.1f;
    [SerializeField] private Vector2 _killzone = new Vector2(-9.5f, 9.5f);

    private float _speed = 0.1f;

    private void Update()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);
        if (transform.position.x < _killzone.x || transform.position.x > _killzone.y)
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
        _renderer.color = Color.white;
        OnDeath?.Invoke(this);
    }
    public void PoolReset()
    {
        HP = _startingHP;
        _speed = Random.Range(_speedRange.x, _speedRange.y) * _speedMultiplier;
        ZombieWaveManager.AddZombie(this);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = Vector3.zero;
        center.x += (_killzone.x + _killzone.y) / 2;
        center.y = transform.position.y;
        float size = _killzone.y - _killzone.x;
        Gizmos.DrawWireCube(center, new Vector3(size, 1, 0));
    }
}
