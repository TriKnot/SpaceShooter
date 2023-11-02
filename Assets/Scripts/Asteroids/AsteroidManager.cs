using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using ScriptableObjects.Variables;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidManager : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private BoolVariableSO _usePoolingSO;
        [SerializeField] private BoolVariableSO _useJobsSO;
        [SerializeField] private BoolVariableSO _useCubeMeshSO;
        
        [Header("Setup Asteroids")]
        [SerializeField] private AsteroidArraySO _asteroidPrefabsSO;
        [SerializeField] private IntVariableSO _initialAsteroidCountSO;
        [SerializeField] private FloatVariableSO _maxspawnRadiusSO;
        [SerializeField] private FloatVariableSO _minSpawnDistanceSO;
        [SerializeField] private FloatVariableSO _spawnRateSO;
        [SerializeField] private TransformVariableSO _playerTransformSO;
        [SerializeField] private AsteroidObjectPoolSO _asteroidPoolSO;
        [SerializeField] private AsteroidObjectPoolSO _asteroidPiecePoolSO;
        
        [Header("References Asteroids")]
        private static readonly List<Asteroid> Asteroids = new List<Asteroid>();
        private NativeArray<MoveData> _asteroidMoveDataArray;
        private TransformMoveJob _asteroidMoveJob;
        private JobHandle _asteroidMoveJobHandle;
        private TransformAccessArray _asteroidTransformAccessArray;
        
        [Header("Runtime")]
        private bool _shouldSpawnAsteroids = true;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        bool _moveJobsActive;
        int _activeAsteroidCount;
        int _targetAsteroidCount;

        public static bool MoveDataHasChanged { get; set; } = false;

        private async void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _targetAsteroidCount = _initialAsteroidCountSO.Value;
            
            // Make sure pool is initialized properly if we are using pooling
            if (_usePoolingSO.Value)
            {
                EnsurePoolInitialized();
            }
            
            InitializeArrays();
            
            await SpawnStartAsteroidsAsync(_cancellationToken);
            
            InitializeTransformArrays();
            UpdateMoveDataArrays();
            
            if (_spawnRateSO.Value > 0)
                StartCoroutine(SpawnAsteroidIEnumerator());
        }

        private void EnsurePoolInitialized()
        {
            if (_asteroidPoolSO.Value.Objects.Count + _asteroidPiecePoolSO.Value.Objects.Count < _initialAsteroidCountSO.Value)
            {
                int extendAmount = Mathf.FloorToInt(_initialAsteroidCountSO.Value - _asteroidPoolSO.Value.Objects.Count);
                extendAmount += Mathf.FloorToInt(_initialAsteroidCountSO.Value * 0.2f);
                _asteroidPoolSO.Value.ExtendPool(extendAmount);
                
                extendAmount = Mathf.FloorToInt(_initialAsteroidCountSO.Value - _asteroidPiecePoolSO.Value.Objects.Count);
                extendAmount += Mathf.FloorToInt(_initialAsteroidCountSO.Value * 0.2f);
                _asteroidPiecePoolSO.Value.ExtendPool(extendAmount);
            }
            ClearAllAsteroids();
            AddAsteroids(_asteroidPoolSO.Value.Objects);
            AddAsteroids(_asteroidPiecePoolSO.Value.Objects);
        }

        private void InitializeArrays()
        {
            // Asteroids
            _asteroidMoveDataArray = new NativeArray<MoveData>(_targetAsteroidCount, Allocator.Persistent);
        }

        private void InitializeTransformArrays()
        {
            // Check if the arrays have been initialized
            if(_asteroidTransformAccessArray.isCreated)
            {
                EnsureJobComplete(_asteroidMoveJobHandle);
                _asteroidTransformAccessArray.Dispose();
            }            
            
            // Asteroids
            Transform[] transforms = new Transform[Asteroids.Count];
            for (int i = 0; i < Asteroids.Count; i++)
            {
                Asteroid asteroid = Asteroids[i];
                if (asteroid == null)
                {
                    Asteroids.RemoveAt(i);
                    i--;
                    continue;
                }
                transforms[i] = asteroid.Transform;
            }
            _asteroidTransformAccessArray = new TransformAccessArray(transforms);

        }
        
        private void FixedUpdate()
        {
            if(!_useJobsSO.Value || !_moveJobsActive ) return;
            
            if(MoveDataHasChanged)
            {
                EnsureJobComplete(_asteroidMoveJobHandle);
                if(_cancellationToken.IsCancellationRequested)
                    return;
                UpdateMoveDataArrays();
            }
            
            _asteroidMoveJobHandle = CreateAsteroidMoveJob(_asteroidMoveDataArray)
                .Schedule(_asteroidTransformAccessArray);

        }

        private void LateUpdate()
        {
            if (!_asteroidMoveJobHandle.IsCompleted || _cancellationToken.IsCancellationRequested) return;
            
            EnsureJobComplete(_asteroidMoveJobHandle);
        }

        private TransformMoveJob CreateAsteroidMoveJob(NativeArray<MoveData> moveDataArray)
        {            
            if(_asteroidTransformAccessArray.length != _asteroidMoveDataArray.Length)
                InitializeTransformArrays();
            return new TransformMoveJob(Time.fixedDeltaTime, moveDataArray);
        }
        
        private void UpdateMoveDataArrays()
        {
            if (_asteroidMoveDataArray.Length != Asteroids.Count)
            {
                if(_asteroidMoveDataArray.IsCreated)
                    _asteroidMoveDataArray.Dispose();
                _asteroidMoveDataArray = new NativeArray<MoveData>(Asteroids.Count, Allocator.Persistent);
            }
            for (int i = 0; i < Asteroids.Count; i++)
            {
                _asteroidMoveDataArray[i] = Asteroids[i].AsteroidMoveData;
            }
            MoveDataHasChanged = false;
        }

        private void OnDestroy()
        {
            if (_useJobsSO.Value)
            {
                EnsureJobComplete(_asteroidMoveJobHandle);
            }
            DisposeAsteroidArrays();
            _cancellationTokenSource.Cancel();
            _shouldSpawnAsteroids = false;
        }

        private void EnsureJobComplete(JobHandle jobHandle)
        {
            jobHandle.Complete();
        }

        private void DisposeAsteroidArrays()
        {
            // Check if the arrays have been initialized
            
            // Asteroids
            if (_asteroidMoveDataArray is { IsCreated: true, Length: > 0 }) 
                _asteroidMoveDataArray.Dispose();
            if (_asteroidTransformAccessArray is { isCreated: true, length: > 0 })
                _asteroidTransformAccessArray.Dispose();
            
        }

        private IEnumerator SpawnAsteroidIEnumerator()
        {
            while (_shouldSpawnAsteroids)
            {
                yield return new WaitForSeconds(_spawnRateSO.Value);
                SpawnAsteroidAsync();
            }
        }

        private async Task SpawnStartAsteroidsAsync(CancellationToken cancellationToken)
        {
            int maxPerFrame = 200;
            int iterations = _initialAsteroidCountSO.Value / maxPerFrame;
            int remainder = _initialAsteroidCountSO.Value % maxPerFrame;
            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < maxPerFrame; j++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    await SpawnAsteroidAsync();
                }
                await Task.Yield();
            }
            for (int i = 0; i < remainder; i++)
            {
                await SpawnAsteroidAsync();
            }

            _moveJobsActive = true;
        }
        
        private async Task SpawnAsteroidAsync()
        {
            Vector3 spawnPos = _playerTransformSO.Value ? 
                _playerTransformSO.Value.position : Vector3.zero;
            do
            {
                spawnPos += Random.insideUnitSphere * 
                            Random.Range(_minSpawnDistanceSO.Value, _maxspawnRadiusSO.Value);
                
            } while (!AsteroidSpawner.IsValidSpawnPosition(spawnPos));

            if (_usePoolingSO.Value)
                AsteroidSpawner.SpawnAsteroid(_asteroidPoolSO.Value, spawnPos);
            else
                AsteroidSpawner.SpawnAsteroid(_asteroidPrefabsSO.Value, spawnPos); 
            
            if(_activeAsteroidCount >= Mathf.FloorToInt(_asteroidPoolSO.Value.Objects.Count * 0.95f))
            {
                Extend();
                await Task.Yield();
            }
            
            _activeAsteroidCount++;
            
        }

        private void Extend()
        {
            bool jobsStopped = false;
            
            // Check if move jobs are active and if so, stop them
            if (_useJobsSO.Value && _moveJobsActive)
            {
                _moveJobsActive = false;
                // Ensure the jobs are complete before disposing the arrays
                EnsureJobComplete(_asteroidMoveJobHandle);
                jobsStopped = true;
            }
                
            // Extend the pool by 20% of the current pool size if the pool is getting full up to a max of 1000
            int extendAmount = Mathf.FloorToInt(_asteroidPoolSO.Value.Objects.Count * 0.2f);
            extendAmount = Mathf.Clamp(extendAmount, 0, 1000);
            
            if(_usePoolingSO.Value)
                _asteroidPoolSO.Value.ExtendPool(extendAmount);

            // Restart the move jobs
            MoveDataHasChanged = true;
                
            // If we stopped the jobs, restart them
            if(jobsStopped)
                _moveJobsActive = true;
                
        }
        
        public static void AddAsteroid(Asteroid value)
        {
            Asteroids.Add(value);
        }
        
        public static void RemoveAsteroid(Asteroid value)
        {
            Asteroids.Remove(value);
        }
        public static void AddAsteroids(IEnumerable<Asteroid> values)
        {
            List<Asteroid> enumerable = values.ToList();
            Asteroids.AddRange(enumerable);
        }
        public static void RemoveAsteroids(IEnumerable<Asteroid> values)
        {
            List<Asteroid> enumerable = values.ToList();
            Asteroids.RemoveAll(enumerable.Contains);
        }
        
        public static void ClearAllAsteroids()
        {
            Asteroids.Clear();
        }
    }
}
