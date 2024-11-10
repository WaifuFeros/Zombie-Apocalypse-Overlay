using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _hitMarker;

    [Space]
    [SerializeField] private int _baseDamage = 2;
    [SerializeField, Min(1)] private int _penetrationLevel = 1;
    [SerializeField] private float _chargePerShot = 5;

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

        var zombiesHit = ZombieWaveManager.GetZombiesByRange(transform.position, _penetrationLevel);

        foreach (var zombie in zombiesHit)
        {
            DealDamage(zombie);
        }

        ZombieWaveManager.AddCharge(_chargePerShot);
    }

    private void DealDamage(Zombie zombieHit)
    {
        zombieHit.TakeDamage(_baseDamage);
        _hitMarker.transform.position = zombieHit.transform.position;
        _hitMarker.Emit(1);
    }
}
