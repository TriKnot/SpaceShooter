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
        
        private float _scaleMultiplier;
        private float _mass;
        private MoveData _asteroidMoveData;

        [Header("Dependencies")]
        [SerializeField] private IntVariableSO _entityCount;
        [SerializeField] private ExplosionObjectPoolSO _explosionPoolSO;
        [SerializeField] private AsteroidObjectPoolSO _asteroidPieceObjectPoolSO;
        private Transform _transform;
        private ObjectPool<Asteroid> _pool;
        private AsteroidMovement _asteroidMovement;
        private AsteroidHealthSystem _healthSystem;
        
        public MoveData AsteroidMoveData { get => _asteroidMoveData; private set => _asteroidMoveData = value; }

        private void Awake()
        {
            _healthSystem = GetComponent<AsteroidHealthSystem>();
            _transform = transform;
            float scaleMultiplier = Mathf.Lerp(_minScaleMultiplier.Value, _maxScaleMultiplier.Value, _massCurve.Evaluate(Random.value));
            Init(scaleMultiplier);
        }

        private void Init(float scaleMultiplier)
        {
            _scaleMultiplier = scaleMultiplier;
            
            _transform.localScale = Vector3.one * _scaleMultiplier; 
            _mass = Mathf.Pow(_scaleMultiplier, 3);
            
            AsteroidMoveData = new MoveData()
            {
                Velocity = Random.insideUnitSphere * Random.Range(0, 100),
                AngularVelocity = Random.insideUnitSphere * Random.Range(0, 100)
            };
            
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
            
            _transform.position = position;
            _transform.rotation = rotation;
            
            _entityCount.Value++;
            _asteroidMoveData.IsActive = true;
            
            _healthSystem.Init(this, _scaleMultiplier);
            
            if (!_useJobsSO.Value)
                _asteroidMovement.Init(Mathf.Pow(_scaleMultiplier, 3), _asteroidMoveData);
        }
        
        public void OnDeath()
        {
            Fracture();
            _entityCount.Value--;
            
            if (_pool != null)
                ReturnToPool();
            else
                Destroy(gameObject);
            
        }

        private void Fracture()
        {
            // TODO Clear up this mess
            // Explosion explosion = _explosionPoolSO.Value.Get();
            // explosion.transform.position = _transform.position;
            // explosion.transform.rotation = _transform.rotation;
            // explosion.InitializePoolObject(_explosionPoolSO.Value);
            // explosion.Explode(_scaleMultiplier);

            // TODO: Decide how many pieces to spawn
            for(int i = 0; i < _scaleMultiplier; i++)
            {
                SpawnPiece();
            }
        }

        private void SpawnPiece()
        {
            Asteroid piece = _asteroidPieceObjectPoolSO.Value.Get();
            
            Vector3 asteroidCenter = _transform.position;
            Vector3 randomDirectionVelocity = Random.insideUnitSphere * _scaleMultiplier;
            Vector3 piecePosition = asteroidCenter + randomDirectionVelocity;
                
            Vector3 parentAsteroidRotationVelocity = Vector3.Cross(_asteroidMoveData.AngularVelocity, asteroidCenter - asteroidCenter);
            Vector3 calculatedVelocity = parentAsteroidRotationVelocity + _asteroidMoveData.Velocity + randomDirectionVelocity;
            
            calculatedVelocity = Vector3.ClampMagnitude(calculatedVelocity, 100);
            
            MoveData moveData = new MoveData()
            {
                IsActive = true,
                Velocity = calculatedVelocity,
                AngularVelocity = Random.insideUnitSphere * Random.Range(0, 100),
            };
            
            piece.Init(_scaleMultiplier);
            piece.Activate( piecePosition, _transform.rotation, moveData );
            
        }
        
        public void ReturnToPool()
        {
            _pool.Return(this);
        }
    }
}
