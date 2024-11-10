using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [Space]
    [SerializeField] private int _baseDamage = 2;

    private void Awake()
    {
        if (!_animator)
            _animator = GetComponent<Animator>();
    }

    private void Start()
    {
#if (!UNITY_EDITOR)
        KeyboardHook.KeyPressed += (keyCode) => Shoot();
        KeyboardHook.SetHook();
#endif
    }

#if (UNITY_EDITOR)
    private void Update()
    {
        if (Input.anyKeyDown)
            Shoot();
    }
#endif

        private void OnApplicationQuit()
    {
        KeyboardHook.Unhook();
    }

    private void Shoot()
    {
        _animator.SetTrigger("Shoot");

        Zombie.DamageZombie(_baseDamage);
    }
}
