using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class ZombieWaveManager : MonoBehaviour
{
    private static List<Zombie> All = new List<Zombie>();
    private static ZombieWaveManager Instance;
    public static int ZombieCount => All.Count;

    [field: SerializeField] public int ZombieLimit { get; private set; }
    private bool CanSpawnZombie => ZombieLimit - ZombieCount > 0;
    [SerializeField] private GameObject _zombiePrefab;
    [SerializeField] private Transform _zombieSpawnPoint;
    [SerializeField] private Vector2 _spawnBounds = new Vector2(-5, 0);
    [SerializeField] private Vector2 _spawnHeightBounds = new Vector2(-0.25f, 0.25f);
    [SerializeField] private float _startingCharge = 100;
    [SerializeField] private float _chargeLimit = 100;

    [Space]
    [SerializeField] private float _chargeOverTime = 20;
    [SerializeField] private float _chargeOnClick = 70;

    [Header("Editor")]
    [SerializeField] private float _spawnBoundHeight = 1;

    private float _zombieSpawnCharge = 0;
    private int _zombieCount;

    private ObjectPool<Zombie> _pool;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _zombieSpawnCharge = _startingCharge;

        SettingsSave.OnChanged += OnSettingsChanged;

        InputHook.MouseClicked += MouseClick;
    }

    private void Start()
    {
        _pool = new ObjectPool<Zombie>(PoolInit, PoolGet, defaultCapacity: ZombieLimit);

        for (int i = 0; i < ZombieLimit; i++)
        {
            _pool.Release(PoolInit());
        }
    }

    private void OnSettingsChanged(SettingsSave save)
    {
        _zombieSpawnPoint.localPosition = new Vector3(0f, save.GroundHeight, 0f);

        ZombieLimit = save.ZombieLimit;
        _chargeOverTime = save.ChargeOverTime;
        _chargeOnClick = save.ChargeOnClick;
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
            if (!CanSpawnZombie)
                continue;

            Vector3 spawnPos = _zombieSpawnPoint.position.With(x: Random.Range(_spawnBounds.x, _spawnBounds.y));
            spawnPos.y += Random.Range(_spawnHeightBounds.x, _spawnHeightBounds.y);

            //Instantiate(_zombiePrefab, spawnPos, Quaternion.identity, _zombieSpawnPoint);
            var zombie = _pool.Get();
            zombie.transform.position = spawnPos;
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

    private void MouseClick(InputHook.MouseClickType obj)
    {
        if (obj == InputHook.MouseClickType.LeftClick)
            AddCharge(_chargeOnClick);
    }

    private Zombie PoolInit()
    {
        var go = Instantiate(_zombiePrefab, _zombieSpawnPoint);
        go.SetActive(false);

        var zombie = go.GetComponent<Zombie>();
        zombie.OnDeath += OnZombieDied;

        return zombie;
    }

    private void PoolGet(Zombie zombie)
    {
        zombie.PoolReset();
        zombie.gameObject.SetActive(true);
    }

    private void OnZombieDied(Zombie zombie)
    {
        _pool.Release(zombie);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = _zombieSpawnPoint.position;
        center.x += (_spawnBounds.x + _spawnBounds.y) / 2;
        float size = _spawnBounds.y - _spawnBounds.x;
        Gizmos.DrawWireCube(center, new Vector3(size, _spawnBoundHeight, 0));
    }
}
