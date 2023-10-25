using System.Collections;
using Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;
using ScriptableObjects.Variables;
using Unity.Burst;
using UnityEngine.Serialization;

namespace Asteroids
{
    [BurstCompile]
    public class AsteroidManager : MonoBehaviour
    {
        [FormerlySerializedAs("_asteroidPrefabs")]
        [Header("Setup")]
        [SerializeField] private AsteroidArraySO _asteroidPrefabsSO;
        [SerializeField] private IntVariableSO _initialAsteroidCountSO;
        [SerializeField] private FloatVariableSO _maxspawnRadiusSO;
        [SerializeField] private FloatVariableSO _minSpawnDistanceSO;
        [SerializeField] private FloatVariableSO _spawnRateSO;
        [SerializeField] private GameObjectVariableSO _cameraGameObjectSO;
        [SerializeField] private TransformVariableSO _playerTransformSO;
        [SerializeField] private BoolVariableSO _usePoolingSO;
        [SerializeField] private AsteroidObjectPoolSO _asteroidPoolSO;
        [SerializeField] private BoolVariableSO _useJobsSO;

        private Asteroid[] _spawnedAsteroids;
        private NativeArray<MoveData> _asteroidMoveDataArray;
        private TransformMoveJob _asteroidMoveJob;
        private JobHandle _asteroidMoveJobHandle;
        private TransformAccessArray _asteroidTransformAccessArray;
        private Camera _camera;
        
        
        private void Start()
        {
            InitializeArrays();
            SpawnStartAsteroids(_playerTransformSO.Value);
            InitializeAsteroids();
            if(_spawnRateSO.Value < 0)
                StartCoroutine(SpawnAsteroidIEnumerator());
        }

        private void InitializeArrays()
        {
            _spawnedAsteroids = new Asteroid[_initialAsteroidCountSO.Value];
            _asteroidMoveDataArray =
                new NativeArray<MoveData>(_initialAsteroidCountSO.Value, Allocator.Persistent);
        }

        private void InitializeAsteroids()
        {
            Transform[] transforms = new Transform[_initialAsteroidCountSO.Value];
            for (int i = 0; i < _spawnedAsteroids.Length; i++)
            {
                transforms[i] = _spawnedAsteroids[i].gameObject.transform;
            }

            _asteroidTransformAccessArray = new TransformAccessArray(transforms);
        }

        private void FixedUpdate()
        {
            if (!_useJobsSO.Value || !_asteroidMoveJobHandle.IsCompleted) return;
            EnsureAsteroidJobComplete();
            ScheduleAsteroidMoveJob();
        }
        
        private void ScheduleAsteroidMoveJob()
        {
            for (int i = 0; i < _initialAsteroidCountSO.Value; i++)
            {
                _asteroidMoveDataArray[i] = _spawnedAsteroids[i].AsteroidMoveData;
            }
            _asteroidMoveJob = new TransformMoveJob(Time.fixedDeltaTime, _asteroidMoveDataArray);
            _asteroidMoveJobHandle = _asteroidMoveJob.Schedule(_asteroidTransformAccessArray);
        }

        private void OnDestroy()
        {
            EnsureAsteroidJobComplete();
            DisposeAsteroidArrays();
        }

        private void EnsureAsteroidJobComplete()
        {
            _asteroidMoveJobHandle.Complete();
        }

        private void DisposeAsteroidArrays()
        {
            _asteroidMoveDataArray.Dispose();
            _asteroidTransformAccessArray.Dispose();
        }
        
        private IEnumerator SpawnAsteroidIEnumerator()
        {
            while (true)
            {
                yield return new WaitForSeconds(_spawnRateSO.Value);
                Vector3 centerPosOffset = GetCenterPosOffset();
                SpawnAsteroid(centerPosOffset);
            }
        }

        private Vector3 GetCenterPosOffset()
        {
            return _playerTransformSO.Value ? _playerTransformSO.Value.position : Vector3.zero;
        }

        private void SpawnStartAsteroids(Transform playerTransform)
        {
            Vector3 centerPosOffset = GetCenterPosOffset();

            for (int i = 0; i < _initialAsteroidCountSO.Value; i++)
            {
                Asteroid asteroid = SpawnAsteroid(centerPosOffset);
                _spawnedAsteroids[i] = asteroid;
                _asteroidMoveDataArray[i] = asteroid.AsteroidMoveData;
            }
        }

        private Asteroid SpawnAsteroid(Vector3 centerPosOffset = default)
        {
            Vector3 spawnPos = GetRandomSpawnPosition(centerPosOffset);
            Asteroid asteroid = _usePoolingSO.Value ? SpawnAsteroidFromPool() : SpawnAsteroidFromPrefab();
            asteroid.Init();
            asteroid.Activate(spawnPos, Random.rotation);
            return asteroid;
        }

        private Vector3 GetRandomSpawnPosition(Vector3 centerPosOffset)
        {
            Vector3 spawnPos;
            int attempts = 0; // Safety measure to prevent infinite loop

            do
            {
                spawnPos = Random.insideUnitSphere * Random.Range(_minSpawnDistanceSO.Value, _maxspawnRadiusSO.Value) + centerPosOffset;
            } while (PointInCameraView(spawnPos) && attempts++ < 100);

            return spawnPos;
        }

        private Asteroid SpawnAsteroidFromPrefab()
        {
            int randAsteroidIndex = Random.Range(0, _asteroidPrefabsSO.Value.Length);
            Asteroid asteroid = Instantiate(_asteroidPrefabsSO.Value[randAsteroidIndex]);
            asteroid.gameObject.SetActive(false);
            return asteroid;
        }

        private Asteroid SpawnAsteroidFromPool()
        {
            return _asteroidPoolSO.Value.Get();
        }

        private bool PointInCameraView(Vector3 point)
        {
            if (!EnsureCameraComponent())
            {
                return false;
            }

            Vector3 viewport = _camera.WorldToViewportPoint(point);
            bool inCameraFrustum = Is01(viewport.x) && Is01(viewport.y);
            bool inFrontOfCamera = viewport.z > 0;
            bool objectBlockingPoint = CheckObjectBlockingPoint(point);

            return inCameraFrustum && inFrontOfCamera && !objectBlockingPoint;
        }

        private bool EnsureCameraComponent()
        {
            if (_camera == null && _cameraGameObjectSO.Value != null)
            {
                _camera = _cameraGameObjectSO.Value.GetComponent<Camera>();
            }

            return _camera != null;
        }

        private bool CheckObjectBlockingPoint(Vector3 point)
        {
            RaycastHit depthCheck;
            bool objectBlockingPoint = false;
            Vector3 directionBetween = (point - _camera.transform.position).normalized;
            float distance = Vector3.Distance(_camera.transform.position, point);

            if (Physics.Raycast(_camera.transform.position, directionBetween, out depthCheck, distance + 0.05f))
            {
                if (depthCheck.point != point)
                {
                    objectBlockingPoint = true;
                }
            }

            return objectBlockingPoint;
        }

        private static bool Is01(float a)
        {
            return a > 0 && a < 1;
        }
    }
    
}
