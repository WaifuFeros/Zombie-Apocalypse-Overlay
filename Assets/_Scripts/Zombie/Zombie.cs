using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    private static List<Zombie> All = new List<Zombie>();

    [field:SerializeField] public int hp { get; private set; } = 10;
    [SerializeField] private float speed = 5;

    private void Start()
    {
        All.Add(this);
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
        All.Remove(this);
        Destroy(gameObject);
    }

    public static void DamageZombie(int damage)
    {
        if (All.Count <= 0)
            return;

        All[0].TakeDamage(damage);
    }
}
