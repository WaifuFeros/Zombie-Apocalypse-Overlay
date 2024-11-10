using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

public class ZombieWaveManager : MonoBehaviour
{
    private static List<Zombie> All = new List<Zombie>();
    private static ZombieWaveManager Instance;
    public static int ZombieCount => All.Count;

    [field: SerializeField] public int ZombieLimit { get; private set; }
    private bool CanSpawnZombie => ZombieLimit - ZombieCount > 0;
    [SerializeField] private GameObject _zombiePrefab;
    [SerializeField] private Transform _zombieSpawnPoint;
    [SerializeField] private float _chargeLimit = 100;
    [SerializeField] private float _chargeOverTime = 10;

    private float _zombieSpawnCharge = 0;
    private int _zombieCount;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        AddCharge(_chargeOverTime * Time.deltaTime);
        _zombieCount = ZombieCount;
    }

    public static bool AddZombie(Zombie zombie)
    {
        if (All.Count >= Instance.ZombieLimit)
            return false;

        All.Add(zombie);
        return true;
    }

    public static bool RemoveZombie(Zombie zombie)
    {
        return All.Remove(zombie);
    }

    public static List<Zombie> GetZombiesByRange(Vector3 attackerPosition, int count = 1)
    {
        return All.OrderBy(x => (x.transform.position - attackerPosition).sqrMagnitude).ToList().GetRange(0, Mathf.Min(count, ZombieCount));
    }

    private void SpawnZombie()
    {
        var countToSpawn = Mathf.FloorToInt(_zombieSpawnCharge / _chargeLimit);
        countToSpawn = Mathf.Min(countToSpawn, ZombieLimit - ZombieCount);
        for (int i = 0; i < countToSpawn; i++)
        {
            if (ZombieCount >= ZombieLimit)
                continue;

            Instantiate(_zombiePrefab, _zombieSpawnPoint.position, Quaternion.identity, _zombieSpawnPoint); 
            _zombieSpawnCharge -= _chargeLimit;
        }
    }

    public static void AddCharge(float charge)
    {
        if (!Instance.CanSpawnZombie)
            return;

        Instance._zombieSpawnCharge += charge;

        if (Instance._zombieSpawnCharge >= Instance._chargeLimit)
            Instance.SpawnZombie();
    }
}