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
        private float _lastShotTime;
        private bool _bufferShot = false;
        
        private void Update()
        {
            // Check if the cooldown is over
            if (_isOnCooldown && Time.time - _lastShotTime > cooldownTime)
            {
                _isOnCooldown = false;
            }
            
            if(_bufferShot && !_isOnCooldown){
                ShootLaser();
            }
        }

        private void SpawnLaser()
        {
            // Instantiate a laser
            Laser laser = Instantiate(_laserPrefab, _laserSpawnPoint.position, _laserSpawnPoint.rotation * Quaternion.Euler(_laserStartRotation));
            laser.Init(_laserSpawnPoint.forward);
            Debug.Log("PewPew!");
        }

        public void ShootLaser()
        {
            if (_isOnCooldown)
            {
                _bufferShot = true;
                return;
            }
            // Spawn a laser shot
            SpawnLaser();
            // Set the cooldown and update the last shot time
            _isOnCooldown = true;
            _lastShotTime = Time.time;
            _bufferShot = false;
        }

    }
}
