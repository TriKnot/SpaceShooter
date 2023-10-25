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
        [Header("Prefabs")]
        [SerializeField] private AsteroidFractured _fracturedPrefab;
        
        [Header("Settings")]
        [SerializeField] private FloatVariableSO _minScaleMultiplier;
        [SerializeField] private FloatVariableSO _maxScaleMultiplier;
        [SerializeField] private AnimationCurve _massCurve;
        [SerializeField] private AsteroidHealthSystem _healthSystem;
        [SerializeField] private BoolVariableSO _useJobsSO;
        
        private float _scaleMultiplier;
        private MoveData _asteroidMoveData;

        [Header("Dependencies")]
        [SerializeField] private Transform _transform;
        [SerializeField] private IntVariableSO _entityCount;
        private ObjectPool<Asteroid> _pool;
        private AsteroidMovement _asteroidMovement;

        public MoveData AsteroidMoveData
        {
            get => _asteroidMoveData;
            set
            {
                _asteroidMoveData = value;
                if (_asteroidMovement != null)
                    _asteroidMovement.AsteroidMoveData = value;
            }
        }

        public void Init()
        {            
            float mass = _massCurve.Evaluate(Random.value);
            _scaleMultiplier = Mathf.Lerp(_minScaleMultiplier.Value, _maxScaleMultiplier.Value, mass);
            transform.localScale *= _scaleMultiplier;
            
            AsteroidMoveData = new MoveData()
            {
                Velocity = Random.insideUnitSphere * Random.Range(0, 100),
                AngularVelocity = Random.insideUnitSphere * Random.Range(0, 100)
            };

            if (_useJobsSO.Value) return;
            
            _asteroidMovement = gameObject.AddComponent<AsteroidMovement>();
        }
        
        public void InitializePool(ObjectPool<Asteroid> pool)
        {
            _pool = pool;
        }

        public void Activate(Vector3 position, Quaternion rotation)
        {
            _transform.position = position;
            _transform.rotation = rotation;
            gameObject.SetActive(true);
            _entityCount.Value++;
            _healthSystem.Init(_scaleMultiplier);
            _asteroidMoveData.IsActive = true;
            
            if (_useJobsSO.Value) return;

            _asteroidMovement.Init(Mathf.Pow(_scaleMultiplier, 3), _asteroidMoveData);
        }
        
        public void OnDeath()
        {
            if (_pool != null)
                ReturnToPool();
            else
                Destroy(gameObject);
            
            var fracturedAsteroid = Instantiate(_fracturedPrefab, _transform.position, _transform.rotation);
            fracturedAsteroid.FractureObject(_scaleMultiplier, AsteroidMoveData);
            _entityCount.Value--;
        }

        public void ReturnToPool()
        {
            _pool.Return(this);
        }
    }
}
