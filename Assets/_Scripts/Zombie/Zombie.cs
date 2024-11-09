using System;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [field:SerializeField] public int hp { get; private set; } = 10;
    [SerializeField] private float speed = 5;

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
        Destroy(gameObject);
    }
}
