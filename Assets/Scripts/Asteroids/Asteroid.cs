using Jobs;
using ScriptableObjects.Variables;
using UnityEngine;
using Util;
using Utils;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class Asteroid : MonoBehaviour, IPoolObject<Asteroid>
    {
        
        [Header("Settings")]
        [SerializeField] private FloatVariableSO _minScaleMultiplier;
        [SerializeField] private FloatVariableSO _maxScaleMultiplier;
        [SerializeField] private BoolVariableSO _usePoolingSO;
        [SerializeField] private AnimationCurve _massCurve;
        [SerializeField] private BoolVariableSO _useJobsSO;
        [SerializeField] private BoolVariableSO _usePhysicsSO;

        [Header("Dependencies")]
        [SerializeField] private IntVariableSO _entityCount;
        [SerializeField] private ExplosionObjectPoolSO _explosionPoolSO;
        [SerializeField] private GameObjectVariableSO _explosionPrefab;
        [SerializeField] private AsteroidObjectPoolSO _asteroidPieceObjectPoolSO;
        [SerializeField] private AsteroidArraySO _asteroidPiecePrefabs;
        
        private Transform _transform;
        private MeshCollider _meshCollider;
        private ObjectPool<Asteroid> _pool;
        private AsteroidMovement _asteroidMovement;
        private AsteroidHealthSystem _healthSystem;
        private MoveData _asteroidMoveData;
        private AsteroidData _asteroidData;
        
        public AsteroidData AsteroidData  => _asteroidData;
        public Transform Transform => _transform;
        public MoveData AsteroidMoveData
        {
            get => _asteroidMoveData;
            private set
            {
                _asteroidMoveData = value;
                //if (_useJobsSO.Value)
                    AsteroidManager.MoveDataHasChanged = true;
            }
        }
        
        private void Awake()
        {
            _healthSystem = GetComponent<AsteroidHealthSystem>();
            _transform = transform;
            if (!_usePhysicsSO.Value)
                Destroy(GetComponent<Collider>());
            float scaleMultiplier = Mathf.Lerp(_minScaleMultiplier.Value, _maxScaleMultiplier.Value, _massCurve.Evaluate(Random.value));
            Init(scaleMultiplier);
        }

        private void Init(float scaleMultiplier)
        {
           _asteroidData = new AsteroidData(scaleMultiplier);
            AsteroidMoveData = new MoveData()
            {
                Velocity = Random.insideUnitSphere * Random.Range(0, 100),
                AngularVelocity = Random.insideUnitSphere * Random.Range(0, 100)
            };
            
            _transform.localScale = Vector3.one * _asteroidData.ScaleMultiplier; 
            gameObject.SetActive(false);

            if (!_useJobsSO.Value)
                _asteroidMovement = gameObject.AddComponent<AsteroidMovement>();

        }
        
        public void InitializePoolObject(ObjectPool<Asteroid> pool)
        {
            _pool = pool;
        }

        public void Activate(Vector3 position, Quaternion rotation, MoveData newMoveData)
        {
            Activate(position, rotation);
            _asteroidMoveData = newMoveData;
        }
        
        public void Activate(Vector3 position, Quaternion rotation)
        {
            gameObject.SetActive(true);
            AsteroidManager.AddAsteroid(this);
            
            _transform.position = position;
            _transform.rotation = rotation;
            
            _entityCount.Value++;
            _asteroidMoveData.IsActive = true;
            
            _healthSystem.Init(this, _asteroidData.ScaleMultiplier);
            
            if (!_useJobsSO.Value)
                _asteroidMovement.Init(_asteroidData.Mass, _asteroidMoveData);
        }
        
        public void OnDeath()
        {
            Fracture();
            _entityCount.Value--;
            
            if (_pool != null)
                ReturnToPool();
            else
            {
                AsteroidManager.RemoveAsteroid(this);
                Destroy(gameObject);
            }            
        }

        private void Fracture()
        {
            Explosion explosion = _usePoolingSO.Value ? _explosionPoolSO.Value.Get() : Instantiate(_explosionPrefab.Value).GetComponent<Explosion>();
            explosion.Explode(_asteroidData.ScaleMultiplier, _transform.position, _transform.rotation);
            
            float pieceScaleMultiplier = _asteroidData.IsPiece ? _asteroidData.ScaleMultiplier / 5 : _asteroidData.ScaleMultiplier;
            if(pieceScaleMultiplier < 3f)
                return;
            
            for(int i = 0; i < pieceScaleMultiplier; i++)
            {
                SpawnPiece(pieceScaleMultiplier);
            }
        }

        private void SpawnPiece(float pieceScaleMultiplier)
        {
            Asteroid piece = _usePoolingSO.Value ? _asteroidPieceObjectPoolSO.Value.Get() : AsteroidSpawner.SpawnAsteroid(_asteroidPiecePrefabs.Value, Vector3.zero);
            
            Vector3 asteroidCenter = _transform.position;
            Vector3 pieceRelativeLocation = Random.insideUnitSphere * (_asteroidData.ScaleMultiplier * Random.Range(0.5f, 2.0f));
            Vector3 piecePosition = asteroidCenter + pieceRelativeLocation;
            Vector3 torqueDirection = Vector3.Cross(pieceRelativeLocation, _asteroidMoveData.AngularVelocity);
            Vector3 torque = torqueDirection * _asteroidData.Mass;
            
            Vector3 calculatedVelocity = torque + _asteroidMoveData.Velocity + pieceRelativeLocation;
            
            calculatedVelocity = Vector3.ClampMagnitude(calculatedVelocity, 100);
            
            MoveData moveData = new MoveData()
            {
                IsActive = true,
                Velocity = calculatedVelocity,
                AngularVelocity = Random.insideUnitSphere * Random.Range(0, 100),
            };
            
            piece.Init(pieceScaleMultiplier);
            piece.AsteroidData.IsPiece = true;
            piece.Activate( piecePosition, _transform.rotation, moveData );
            
        }
        
        public void ReturnToPool()
        {
            _pool.Return(this);
        }
    }
}
