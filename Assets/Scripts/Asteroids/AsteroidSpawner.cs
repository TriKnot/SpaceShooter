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
            } while (!IsPositionOnScreen(spawnPos) && attempts++ < 100);
            Asteroid asteroid = Instantiate(_asteroidPrefabs[randAsteroidIndex], spawnPos, Random.rotation);
        }
        
        private bool IsPositionOnScreen(Vector3 worldPosition)
        {
            // Convert the world position to screen space
            Vector3 screenPosition = _camera.WorldToViewportPoint(worldPosition);

            // Check if the screen position is within the view
            return screenPosition.x is > 0 and < 1 &&
                   screenPosition.y is > 0 and < 1 &&
                   screenPosition.z > 0;
        }
    }
}
