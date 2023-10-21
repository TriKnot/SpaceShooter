using System;
using UnityEngine;

namespace Ship
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] private Laser _laserPrefab;
        [SerializeField] private Transform _laserSpawnPoint;
        [SerializeField] private Vector3 _laserStartRotation;
        [SerializeField] private float cooldownTime = 0.5f;

        private bool _isOnCooldown = false;
        private float _shotTimer;

        public bool ShouldShoot { get; set; } = false;
        
        private void Update()
        {
            // Check if the cooldown is over
            if (_shotTimer > 0.0f)
            {
                _shotTimer -= Time.deltaTime;
                return;
            }
            
            // Check if we want to shoot
            if(!ShouldShoot) return;
            
            ShootLaser();
        }

        private void SpawnLaser()
        {
            // Instantiate a laser
            Laser laser = Instantiate(_laserPrefab, _laserSpawnPoint.position, _laserSpawnPoint.rotation * Quaternion.Euler(_laserStartRotation));
            laser.Init(_laserSpawnPoint.forward);
        }

        private void ShootLaser()
        {
            // Spawn a laser shot
            SpawnLaser();
            // Set the cooldown and update the last shot time
            _shotTimer = cooldownTime;
        }

    }
}
