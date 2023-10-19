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
        

        private void Start()
        {
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
            Asteroid asteroid = Instantiate(_asteroidPrefabs[randAsteroidIndex], Random.insideUnitSphere * Random.Range(_minSpawnDistance, _maxspawnRadius), Random.rotation);
        }
    }
}
