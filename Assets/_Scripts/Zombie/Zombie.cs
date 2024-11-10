using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [field:SerializeField] public int hp { get; private set; } = 10;
    [SerializeField] private float speed = 5;

    private void Start()
    {
        ZombieWaveManager.AddZombie(this);
    }

    private void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
            Die();
    }

    private void Die()
    {
        ZombieWaveManager.RemoveZombie(this);
        Destroy(gameObject);
    }
}
