using UnityEngine;

namespace Ship
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] private Laser _laserPrefab;
        [SerializeField] private Transform _laserSpawnPoint;
        [SerializeField] private Vector3 _laserStartRotation;

        public void ShootLaser()
        {
            // Instantiate a laser
            Laser laser = Instantiate(_laserPrefab, _laserSpawnPoint.position, _laserSpawnPoint.rotation * Quaternion.Euler(_laserStartRotation));
            laser.Init(_laserSpawnPoint.forward);
        }

    }
}
