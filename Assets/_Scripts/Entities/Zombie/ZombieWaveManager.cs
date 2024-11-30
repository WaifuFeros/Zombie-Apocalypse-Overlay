using JetBrains.Annotations;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using WMG.OverlayWindow;

namespace WMG.ZombieApocalypseOverlay
{
    public class ZombieWaveManager : MonoBehaviour, ISavedComponent<SettingsSave>
    {
        private static List<Zombie> All = new List<Zombie>();
        private static ZombieWaveManager Instance;
        public static int ZombieCount => All.Count;

        [field: SerializeField] public int ZombieLimit { get; private set; }
        private bool CanSpawnZombie => (ZombieLimit - ZombieCount) > 0;
        [SerializeField] private GameObject _zombiePrefab;
        [SerializeField] private Transform _zombieSpawnPoint;
        [SerializeField] private ValueRange _spawnBoundsX = new ValueRange(-9.5f, 0f);
        [SerializeField] private ValueRange _spawnBoundsY = new ValueRange(-0.25f, 0.25f);
        [SerializeField] private float _startingCharge = 100;
        [SerializeField] private float _chargeLimit = 100;

        [Space]
        [SerializeField] private float _chargeOverTime = 20;
        [SerializeField] private float _chargeOnClick = 70;

        [Header("Settings")]
        [SerializeField] private InputField _groundHeightField;
        [SerializeField] private Slider _groundHeightSlider;
        [SerializeField] private InputField _zombieLimitField;

        private ObjectPool<Zombie> _pool;

        private float _zombieSpawnCharge = 0;
        private int _zombieCount;
        private float _alpha;
        private float _size;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _zombieSpawnCharge = _startingCharge;

            InputHook.MouseClicked += MouseClick;
            //EntitySettings.OnAlphaChanged += x => _alpha = x;
            EntitySettings.OnSizeChanged += x => _size = x;
        }

        private void Start()
        {
            _pool = new ObjectPool<Zombie>(PoolInit, PoolGet, defaultCapacity: ZombieLimit);

            for (int i = 0; i < ZombieLimit; i++)
            {
                _pool.Release(PoolInit());
            }

            Settings_InitFields();
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

                Vector3 spawnPos = _zombieSpawnPoint.position.With(x: Random.Range(_spawnBoundsX.Min, _spawnBoundsX.Max));
                spawnPos.y += Random.Range(_spawnBoundsY.Min, _spawnBoundsY.Max);

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

        #region Pooling
        private Zombie PoolInit()
        {
            var go = Instantiate(_zombiePrefab, _zombieSpawnPoint);
            go.SetActive(false);

            var zombie = go.GetComponent<Zombie>();
            //zombie.ChangeAlpha(_alpha);
            zombie.ChangeSize(_size);
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
        #endregion

        private void OnDrawGizmosSelected()
        {
            Vector3 center = _zombieSpawnPoint.position;
            center.x += (_spawnBoundsX.Min + _spawnBoundsX.Max) / 2;
            center.y += (_spawnBoundsY.Min + _spawnBoundsY.Max) / 2;
            Vector3 size = new Vector3(_spawnBoundsX.Max - _spawnBoundsX.Min, _spawnBoundsY.Max - _spawnBoundsY.Min, 0);
            Gizmos.DrawWireCube(center, size);
        }

        #region Settings
        public void OnLoad(SettingsSave save)
        {
            Settings_SetGroundHeight(save.GroundHeight);
            ZombieLimit = save.ZombieLimit;
            //_chargeOverTime = save.ChargeOverTime;
        }

        public void OnSave(SettingsSave save)
        {
            save.GroundHeight = _zombieSpawnPoint.transform.localPosition.y;
            save.ZombieLimit = ZombieLimit;
            //save.ChargeOverTime = _chargeOverTime;
        } 

        private void Settings_InitFields()
        {
            float height = _zombieSpawnPoint.transform.localPosition.y;
            _groundHeightField.text = height.ToString();
            _groundHeightSlider.value = height;
            _zombieLimitField.text = ZombieLimit.ToString();
        }

        public void Settings_SetGroundHeight(string height)
        {
            _zombieSpawnPoint.transform.localPosition = _zombieSpawnPoint.transform.localPosition.With(y: FormatStringToFloat(height));
        }

        public void Settings_SetGroundHeight(float height)
        {
            _zombieSpawnPoint.transform.localPosition = _zombieSpawnPoint.transform.localPosition.With(y: height);
        }

        public void Settings_SetZombieLimit(string limit)
        {
            var limitInt = Mathf.Max(int.Parse(limit), 0);
            ZombieLimit = limitInt;
            _zombieLimitField.text = limitInt.ToString();
        }

        public void Settings_SetChargeOverTime(string chargeOverTime) => _chargeOverTime = FormatStringToFloat(chargeOverTime);

        private float FormatStringToFloat(string value) => string.IsNullOrEmpty(value) ? 0f : float.Parse(value.Replace('.', ','));
        #endregion
    }
}
