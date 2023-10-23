using System.Collections;
using ScriptableObjects.Variables;
using UnityEngine;
using UnityEngine.Serialization;
using Util;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidSpawner : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Asteroid[] _asteroidPrefabs;
        [SerializeField] private IntVariableSO _initialAsteroidCount;
        [SerializeField] private IntVariableSO _asteroidPoolInitCount;
        [SerializeField] private FloatVariableSO _maxspawnRadius;
        [SerializeField] private FloatVariableSO _minSpawnDistance;
        [SerializeField] private FloatVariableSO _spawnRate;
        [SerializeField] private GameObjectVariableSO _cameraGameObject;
        [SerializeField] private TransformVariableSO _playerTransform;
        
        [SerializeField] private BoolVariableSO _usePoolingSO;
        private ObjectPool<Asteroid> _asteroidPool;
        
        private Camera _camera;

        private void Awake()
        {
            if(_usePoolingSO.Value)
            {
                // Initialize the pool
                _asteroidPool = new ObjectPool<Asteroid>(_asteroidPrefabs, _asteroidPoolInitCount.Value);
            }
            
            // Subscribe to the event that is fired when the player is spawned
            _playerTransform.ValueChanged += SpawnStartAsteroids;
        }

        private void Start()
        {
            if (_cameraGameObject == null)
            {
                Debug.LogError($"Main Camera is not assigned. Please assign the camera in the Inspector of {gameObject.name}.");
                return;
            }
            
            StartCoroutine(SpawnAsteroidIEnumerator());
        }

        private IEnumerator SpawnAsteroidIEnumerator()
        {
            while (true)
            {
                yield return new WaitForSeconds(_spawnRate.Value);
                Vector3 centerPosOffset = Vector3.zero;
                if(_playerTransform.Value) centerPosOffset = _playerTransform.Value.position;
                SpawnAsteroid(centerPosOffset);
            }
        }
        
        private void SpawnStartAsteroids(Transform playerTransform)
        {
            // Spawn asteroids around the player
            Vector3 centerPosOffset = Vector3.zero;
            if(playerTransform) centerPosOffset = playerTransform.position;
            
            for (int i = 0; i < _initialAsteroidCount.Value; i++)
            {
                SpawnAsteroid(centerPosOffset);
            }
            
            // Unsubscribe from the event
            _playerTransform.ValueChanged -= SpawnStartAsteroids;
        }

        private void SpawnAsteroid(Vector3 centerPosOffset = default)
        {
            if(_usePoolingSO.Value)
            {
                SpawnAsteroidFromPool(centerPosOffset);
            }
            else
            {
                SpawnAsteroidFromPrefab(centerPosOffset);
            }        
        }

        private void SpawnAsteroidFromPrefab(Vector3 centerPosOffset)
        {
            int randAsteroidIndex = Random.Range(0, _asteroidPrefabs.Length);
            Vector3 spawnPos;
            int attempts = 0; // safety measure to prevent infinite loop
            do
            {
                spawnPos = Random.insideUnitSphere * Random.Range(_minSpawnDistance.Value, _maxspawnRadius.Value) + centerPosOffset;
            } while (PointInCameraView(spawnPos) && attempts++ < 100);
            Asteroid asteroid = Instantiate(_asteroidPrefabs[randAsteroidIndex], spawnPos, Random.rotation);
            asteroid.gameObject.SetActive(false);
            asteroid.Initialize(null);
            asteroid.Activate(spawnPos, Random.rotation);
        }

        private void SpawnAsteroidFromPool(Vector3 centerPosOffset = default)
        {
            Vector3 spawnPos;
            int attempts = 0; // safety measure to prevent infinite loop
            do
            {
                spawnPos = Random.insideUnitSphere * Random.Range(_minSpawnDistance.Value, _maxspawnRadius.Value) + centerPosOffset;
            } while (PointInCameraView(spawnPos) && attempts++ < 100);
            Asteroid asteroid = _asteroidPool.Get();
            asteroid.Activate(spawnPos, Random.rotation);
        }

        #region Camera Viewport Check
        private bool PointInCameraView(Vector3 point) {
            
            if(_camera == null) {
                _camera = _cameraGameObject.Value.GetComponent<Camera>();
            }
            
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

        private static bool Is01(float a) {
            return a > 0 && a < 1;
        }

        #endregion
    }
}
