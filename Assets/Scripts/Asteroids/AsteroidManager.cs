using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Jobs;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using ScriptableObjects.Variables;

namespace Asteroids
{
    public class AsteroidManager : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private BoolVariableSO _usePoolingSO;
        [SerializeField] private BoolVariableSO _useJobsSO;
        
        [Header("Setup Asteroids")]
        [SerializeField] private AsteroidArraySO _asteroidPrefabsSO;
        [SerializeField] private IntVariableSO _initialAsteroidCountSO;
        [SerializeField] private FloatVariableSO _maxspawnRadiusSO;
        [SerializeField] private FloatVariableSO _minSpawnDistanceSO;
        [SerializeField] private FloatVariableSO _spawnRateSO;
        [SerializeField] private TransformVariableSO _playerTransformSO;
        [SerializeField] private AsteroidObjectPoolSO _asteroidPoolSO;

        [Header("Setup Asteroid Pieces")]
        [SerializeField] private AsteroidObjectPoolSO _asteroidPiecesPoolSO;
        
        [Header("References Asteroids")]
        private Asteroid[] _asteroids;
        private NativeArray<MoveData> _asteroidMoveDataArray;
        private TransformMoveJob _asteroidMoveJob;
        private JobHandle _asteroidMoveJobHandle;
        private TransformAccessArray _asteroidTransformAccessArray;
        
        [Header("References Asteroid Pieces")]
        private Asteroid[] _asteroidPieces;
        private NativeArray<MoveData> _asteroidPieceMoveDataArray;
        private TransformMoveJob _asteroidPieceMoveJob;
        private JobHandle _asteroidPieceMoveJobHandle;
        private TransformAccessArray _asteroidPieceTransformAccessArray;
        
        [Header("Runtime")]
        private bool _shouldSpawnAsteroids = true;
        private CancellationTokenSource _cancellationTokenSource;
        bool _moveJobsActive = false;
        int _asteroidCount = 0;

        public static bool MoveDataHasChanged { get; set; } = false;

        private async void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;
            InitializeArrays();
            await SpawnStartAsteroidsAsync(cancellationToken);
            InitializeTransformArrays();
            UpdateMoveDataArrays();
            
            if (_spawnRateSO.Value > 0)
                StartCoroutine(SpawnAsteroidIEnumerator());
        }

        private void InitializeArrays()
        {
            // Asteroids
            _asteroids = _asteroidPoolSO.Value.Objects.ToArray();
            _asteroidMoveDataArray = new NativeArray<MoveData>(_asteroids.Length, Allocator.Persistent);

            // Asteroid Pieces
            _asteroidPieces = _asteroidPiecesPoolSO.Value.Objects.ToArray();
            _asteroidPieceMoveDataArray = new NativeArray<MoveData>(_asteroidPieces.Length, Allocator.Persistent);

        }

        private void InitializeTransformArrays()
        {
            // Check if the arrays have been initialized
            if(_asteroidTransformAccessArray.isCreated)
            {
                EnsureJobComplete(_asteroidMoveJobHandle);
                _asteroidTransformAccessArray.Dispose();
            }            
            if(_asteroidPieceTransformAccessArray.isCreated)
            {
                EnsureJobComplete(_asteroidPieceMoveJobHandle);
                _asteroidPieceTransformAccessArray.Dispose();
            }            
            
            // Asteroids
            Transform[] transforms = new Transform[_asteroids.Length];
            for (int i = 0; i < _asteroids.Length; i++)
            {
                transforms[i] = _asteroids[i].gameObject.transform;
            }
            _asteroidTransformAccessArray = new TransformAccessArray(transforms);
            
            // Asteroid Pieces
            transforms = new Transform[_asteroidPieces.Length];
            for (int i = 0; i < _asteroidPieces.Length; i++)
            {
                transforms[i] = _asteroidPieces[i].gameObject.transform;
            }
            _asteroidPieceTransformAccessArray = new TransformAccessArray(transforms);
        }
        
        private void FixedUpdate()
        {
            if(!_useJobsSO.Value || !_moveJobsActive ) return;
            
            if(MoveDataHasChanged)
            {
                EnsureJobComplete(_asteroidMoveJobHandle);
                EnsureJobComplete(_asteroidPieceMoveJobHandle);
                UpdateMoveDataArrays();
            }
            
            if ( _asteroidMoveJobHandle.IsCompleted)
            {
                EnsureJobComplete(_asteroidMoveJobHandle);
                _asteroidMoveJobHandle = CreateAsteroidMoveJob(_asteroids, _asteroidMoveDataArray)
                    .Schedule(_asteroidTransformAccessArray);
            }
            if( _asteroidPieceMoveJobHandle.IsCompleted)
            {
                EnsureJobComplete(_asteroidPieceMoveJobHandle);
                _asteroidPieceMoveJobHandle = CreateAsteroidMoveJob(_asteroidPieces, _asteroidPieceMoveDataArray)
                    .Schedule(_asteroidPieceTransformAccessArray);
            }
        }
        
        private TransformMoveJob CreateAsteroidMoveJob(Asteroid[] asteroids, NativeArray<MoveData> moveDataArray)
        {
            if(MoveDataHasChanged)
            {
                for (int i = 0; i < asteroids.Length; i++)
                {
                    moveDataArray[i] = asteroids[i].AsteroidMoveData;
                }
            }
            return new TransformMoveJob(Time.fixedDeltaTime, moveDataArray);
        }
        
        private void UpdateMoveDataArrays()
        {
            for (int i = 0; i < _asteroids.Length; i++)
            {
                _asteroidMoveDataArray[i] = _asteroids[i].AsteroidMoveData;
            }
                        
            for (int i = 0; i < _asteroidPieces.Length; i++)
            {
                _asteroidPieceMoveDataArray[i] = _asteroidPieces[i].AsteroidMoveData;
            }
            MoveDataHasChanged = false;
        }

        private void OnDestroy()
        {
            if (_useJobsSO.Value)
            {
                EnsureJobComplete(_asteroidMoveJobHandle);
                EnsureJobComplete(_asteroidPieceMoveJobHandle);
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
            
            
            
            // Asteroid Pieces
            if (_asteroidPieceMoveDataArray is { IsCreated: true, Length: > 0 }) 
                _asteroidPieceMoveDataArray.Dispose();
            if (_asteroidPieceTransformAccessArray is { isCreated: true, length: > 0 })
                _asteroidPieceTransformAccessArray.Dispose();
        }

        private IEnumerator SpawnAsteroidIEnumerator()
        {
            while (_shouldSpawnAsteroids)
            {
                yield return new WaitForSeconds(_spawnRateSO.Value);
                SpawnAsteroid();
            }
        }

        private async Task SpawnStartAsteroidsAsync(CancellationToken cancellationToken)
        {
            int maxPerFrame = 200;
            int count = _initialAsteroidCountSO.Value;
            int iterations = count / maxPerFrame;
            int remainder = count % maxPerFrame;
            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < maxPerFrame; j++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    Task<Asteroid> task = SpawnAsteroid();
                    Asteroid asteroid = await task;
                    _asteroids[i] = asteroid;
                    _asteroidMoveDataArray[i] = asteroid.AsteroidMoveData;
                }
                await Task.Yield();
            }
            for (int i = 0; i < remainder; i++)
            {
                Task<Asteroid> task = SpawnAsteroid();
                Asteroid asteroid = await task;
                _asteroids[^i] = asteroid;
                _asteroidMoveDataArray[^i] = asteroid.AsteroidMoveData;
            }

            _moveJobsActive = true;
        }


        private async Task<Asteroid> SpawnAsteroid()
        {
            Vector3 spawnPos;
            do
            {
                spawnPos = _playerTransformSO.Value.position + 
                               Random.insideUnitSphere * 
                               Random.Range(_minSpawnDistanceSO.Value, _maxspawnRadiusSO.Value);
                
            } while (!AsteroidSpawner.IsValidSpawnPosition(spawnPos));
            
            Asteroid asteroid = _usePoolingSO.Value ? 
                AsteroidSpawner.SpawnAsteroid(_asteroidPoolSO.Value, spawnPos) : 
                AsteroidSpawner.SpawnAsteroid(_asteroidPrefabsSO.Value, spawnPos); 
            
            if(_asteroidCount >= Mathf.FloorToInt(_asteroidPoolSO.Value.Objects.Count * 0.95f))
            {
                bool jobsStopped = false;
                // Check if move jobs are active and if so, stop them
                if (_useJobsSO.Value && _moveJobsActive)
                {
                    _moveJobsActive = false;
                    // Ensure the jobs are complete before disposing the arrays
                    EnsureJobComplete(_asteroidMoveJobHandle);
                    EnsureJobComplete(_asteroidPieceMoveJobHandle);
                    jobsStopped = true;
                }
                
                // Extend the pool by 20% of the current pool size if the pool is getting full up to a max of 1000
                int extendAmount = Mathf.FloorToInt(_asteroidPoolSO.Value.Objects.Count * 0.2f);
                extendAmount = Mathf.Clamp(extendAmount, 0, 1000);
                _asteroidPoolSO.Value.ExtendPool(extendAmount);

                // Reinitialize the arrays
                DisposeAsteroidArrays();
                InitializeArrays(); 
                InitializeTransformArrays();
                UpdateMoveDataArrays();

                // Restart the move jobs
                MoveDataHasChanged = true;
                
                // If we stopped the jobs, restart them
                if(jobsStopped)
                    _moveJobsActive = true;
                
                await Task.Yield();
            }
            
            _asteroidCount++;
            
            return asteroid;
        }
    }
}
