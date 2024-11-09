using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class SoldierShoot : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        if (!_animator)
            _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        KeyboardHook.KeyPressed += (keyCode) => Shoot();
        KeyboardHook.SetHook();
    }

    private void OnApplicationQuit()
    {
        KeyboardHook.Unhook();
    }

    private void Shoot()
    {
        _animator.SetTrigger("Shoot");
    }
}
