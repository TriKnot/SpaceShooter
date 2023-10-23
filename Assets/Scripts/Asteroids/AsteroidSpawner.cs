using System.Collections;
using ScriptableObjects.Variables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidSpawner : MonoBehaviour
    {
        [SerializeField] private Asteroid[] _asteroidPrefabs;
        [SerializeField] private IntVariableSO _initialAsteroidCount;
        [SerializeField] private FloatVariableSO _maxspawnRadius;
        [SerializeField] private FloatVariableSO _minSpawnDistance;
        [SerializeField] private FloatVariableSO _spawnRate;
        [SerializeField] private GameObjectVariableSO _cameraGameObject;
        [SerializeField] private TransformVariableSO _playerTransform;
        
        private Camera _camera;

        private void Awake()
        {
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
                SpawnAsteroid();
            }
        }
        
        private void SpawnStartAsteroids(Transform playerTransform)
        {
            for (int i = 0; i < _initialAsteroidCount.Value; i++)
            {
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
                spawnPos = Random.insideUnitSphere * Random.Range(_minSpawnDistance.Value, _maxspawnRadius.Value) + _playerTransform.Value.position;
            } while (PointInCameraView(spawnPos) && attempts++ < 100);
            Asteroid asteroid = Instantiate(_asteroidPrefabs[randAsteroidIndex], spawnPos, Random.rotation);
        }

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

        public bool Is01(float a) {
            return a > 0 && a < 1;
        }
    }
}
