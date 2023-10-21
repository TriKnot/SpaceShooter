using System.Collections;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidSpawner : MonoBehaviour
    {
        [SerializeField] private Asteroid[] _asteroidPrefabs;
        [SerializeField] private int _initialAsteroidCount;
        [SerializeField] private float _maxspawnRadius;
        [SerializeField] private float _minSpawnDistance;
        [SerializeField] private float _spawnRate;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _playerTransform;
        

        private void Start()
        {
            if (_camera == null)
            {
                Debug.LogError($"Main Camera is not assigned. Please assign the camera in the Inspector of {gameObject.name}.");
                return;
            }
            
            for (int i = 0; i < _initialAsteroidCount; i++)
            {
                SpawnAsteroid();
            }
            StartCoroutine(SpawnAsteroidIEnumerator());
        }

        private IEnumerator SpawnAsteroidIEnumerator()
        {
            while (true)
            {
                yield return new WaitForSeconds(_spawnRate);
                SpawnAsteroid();
            }
        }

        private void SpawnAsteroid()
        {
            int randAsteroidIndex = Random.Range(0, _asteroidPrefabs.Length);
            Vector3 spawnPos;
            int attempts = 0; // safety measure to prevent infinite loop
            do
            {
                spawnPos = Random.insideUnitSphere * Random.Range(_minSpawnDistance, _maxspawnRadius) + _playerTransform.position;
            } while (PointInCameraView(spawnPos) && attempts++ < 100);
            Asteroid asteroid = Instantiate(_asteroidPrefabs[randAsteroidIndex], spawnPos, Random.rotation);
        }

        public bool PointInCameraView(Vector3 point) {
            Vector3 viewport = _camera.WorldToViewportPoint(point);
            bool inCameraFrustum = Is01(viewport.x) && Is01(viewport.y);
            bool inFrontOfCamera = viewport.z > 0;

            RaycastHit depthCheck;
            bool objectBlockingPoint = false;

            Vector3 directionBetween = point - _camera.transform.position;
            directionBetween = directionBetween.normalized;

            float distance = Vector3.Distance(_camera.transform.position, point);

            if(Physics.Raycast(_camera.transform.position, directionBetween, out depthCheck, distance + 0.05f)) {
                if(depthCheck.point != point) {
                    objectBlockingPoint = true;
                }
            }

            return inCameraFrustum && inFrontOfCamera && !objectBlockingPoint;
        }

        public bool Is01(float a) {
            return a > 0 && a < 1;
        }
    }
}
