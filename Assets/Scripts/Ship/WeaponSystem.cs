using ScriptableObjects.Variables;
using UnityEngine;

namespace Ship
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] private LaserShotObjectPoolSO _laserPool;
        [SerializeField] private Laser _laserPrefab;
        [SerializeField] private Transform _laserSpawnPoint;
        [SerializeField] private Vector3 _laserStartRotation;
        [SerializeField] private float cooldownTime = 0.5f;
        
        [SerializeField] private BoolVariableSO _usePoolingSO;

        private float _shotTimer;

        public bool ShouldShoot { get; set; } = false;
        
        private void FixedUpdate()
        {
            if (_shotTimer > 0.0f)
            {
                _shotTimer -= Time.fixedDeltaTime;
                return;
            }
            
            if (ShouldShoot)
            {
                ShootLaser();
            }
        }

        private void ShootLaser()
        {
            if (_usePoolingSO.Value)
            {
                GetLaserFromPool();
            }
            else
            {
                SpawnLaser(_laserSpawnPoint.position, _laserSpawnPoint.rotation * Quaternion.Euler(_laserStartRotation), _laserSpawnPoint.forward);
            }

            _shotTimer = cooldownTime;
        }
        
        private void SpawnLaser(Vector3 position, Quaternion rotation, Vector3 direction)
        {
            Laser laser = InstantiateLaser(position, rotation, direction);
            laser.transform.parent = _laserSpawnPoint;
        }
        private Laser InstantiateLaser(Vector3 position, Quaternion rotation, Vector3 direction)
        {
            Laser laser = _usePoolingSO.Value ? _laserPool.Value.Get() : Instantiate(_laserPrefab);
            laser.Init(position, rotation, direction);
            return laser;
        }

        private void GetLaserFromPool()
        {
            Laser laser = InstantiateLaser(_laserSpawnPoint.position, _laserSpawnPoint.rotation * Quaternion.Euler(_laserStartRotation), _laserSpawnPoint.forward);
            laser.transform.parent = _laserSpawnPoint;
        }
    }
}
