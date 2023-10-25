using System.Collections;
using Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using ScriptableObjects.Variables;

namespace Asteroids
{
    [BurstCompile]
    public class AsteroidManager : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private AsteroidArraySO _asteroidPrefabsSO;
        [SerializeField] private IntVariableSO _initialAsteroidCountSO;
        [SerializeField] private FloatVariableSO _maxspawnRadiusSO;
        [SerializeField] private FloatVariableSO _minSpawnDistanceSO;
        [SerializeField] private FloatVariableSO _spawnRateSO;
        [SerializeField] private TransformVariableSO _playerTransformSO;
        [SerializeField] private BoolVariableSO _usePoolingSO;
        [SerializeField] private AsteroidObjectPoolSO _asteroidPoolSO;
        [SerializeField] private BoolVariableSO _useJobsSO;

        private Asteroid[] _asteroids;
        private NativeArray<MoveData> _asteroidMoveDataArray;
        private TransformMoveJob _asteroidMoveJob;
        private JobHandle _asteroidMoveJobHandle;
        private TransformAccessArray _asteroidTransformAccessArray;

        private void Start()
        {
            InitializeArrays();
            SpawnStartAsteroids();
            InitializeAsteroids();
            if (_spawnRateSO.Value > 0)
                StartCoroutine(SpawnAsteroidIEnumerator());
        }

        private void InitializeArrays()
        {
            _asteroids = _asteroidPoolSO.Value.Objects.ToArray();
            _asteroidMoveDataArray = new NativeArray<MoveData>(_asteroids.Length, Allocator.Persistent);
        }

        private void InitializeAsteroids()
        {
            Transform[] transforms = new Transform[_asteroids.Length];
            for (int i = 0; i < _asteroids.Length; i++)
            {
                transforms[i] = _asteroids[i].gameObject.transform;
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
            for (int i = 0; i < _asteroids.Length; i++)
            {
                _asteroidMoveDataArray[i] = _asteroids[i].AsteroidMoveData;
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
                SpawnAsteroid();
            }
        }

        private void SpawnStartAsteroids()
        {
            for (int i = 0; i < _initialAsteroidCountSO.Value; i++)
            {
                Asteroid asteroid = SpawnAsteroid();
                _asteroids[i] = asteroid;
                _asteroidMoveDataArray[i] = asteroid.AsteroidMoveData;
            }
        }

        private Asteroid SpawnAsteroid()
        {
            Asteroid asteroid = AsteroidSpawner.SpawnAsteroid(
                _asteroidPrefabsSO.Value, _minSpawnDistanceSO.Value, _maxspawnRadiusSO.Value, _usePoolingSO,
                _playerTransformSO.Value, _asteroidPoolSO.Value);
            return asteroid;
        }
    }
}
