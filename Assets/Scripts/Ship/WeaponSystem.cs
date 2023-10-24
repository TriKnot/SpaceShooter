using ScriptableObjects.Variables;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

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
            // Check if the cooldown is over
            if (_shotTimer > 0.0f)
            {
                _shotTimer -= Time.fixedDeltaTime;
                return;
            }
            
            // Check if we want to shoot
            if(!ShouldShoot) return;
            
            ShootLaser();
        }
        
        private void SpawnLaser()
        {
            // Instantiate a laser
            Laser laser = Instantiate(_laserPrefab);
            laser.Init(_laserSpawnPoint.position, _laserSpawnPoint.rotation * Quaternion.Euler(_laserStartRotation), _laserSpawnPoint.forward);
        }
        
        private void GetLaserFromPool()
        {
            // Instantiate a laser
            Laser laser = _laserPool.Value.Get();
            laser.Init(_laserSpawnPoint.position, _laserSpawnPoint.rotation * Quaternion.Euler(_laserStartRotation), _laserSpawnPoint.forward);
            laser.transform.parent = _laserSpawnPoint;
        }

        private void ShootLaser()
        {
            // Spawn a laser shot
            if(_usePoolingSO.Value)
            {
                GetLaserFromPool();
            }
            else
            {
                SpawnLaser();
            }            
            // Set the cooldown and update the last shot time
            _shotTimer = cooldownTime;
        }

    }
}
