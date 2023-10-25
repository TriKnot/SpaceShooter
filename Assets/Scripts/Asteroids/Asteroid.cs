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
        [Tooltip("\"Fractured\" is the object that this will break into")] [SerializeField]
        private AsteroidFractured _fracturedPrefab;
        private AsteroidFractured fracturedAsteroid;
        
        [SerializeField] private FloatVariableSO _minScaleMultiplier;
        [SerializeField] private FloatVariableSO _maxScaleMultiplier;
        [SerializeField] private AnimationCurve _massCurve;
        [SerializeField] private AsteroidHealthSystem _healthSystem;
        [SerializeField] private Transform _transform;
        [SerializeField] private BoolVariableSO _useJobsSO;
        
        private float _scaleMultiplier;
        private AsteroidMovement _asteroidMovement;


        private MoveData _asteroidMoveData;
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

        [SerializeField] private IntVariableSO _entityCount;
        
        private ObjectPool<Asteroid> _pool;

        public void Init()
        {            
            float mass = _massCurve.Evaluate(Random.value);
            _scaleMultiplier = Mathf.Lerp(_minScaleMultiplier.Value, _maxScaleMultiplier.Value, mass);
            transform.localScale *= _scaleMultiplier;
            
            AsteroidMoveData = new MoveData()
            {
                Velocity = Random.insideUnitSphere * Random.Range(0, 100) ,
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

            _asteroidMovement.Init( Mathf.Pow(_scaleMultiplier, 3), _asteroidMoveData);
        }
        
        public void OnDeath()
        {
            if(_pool != null)
                ReturnToPool();
            else
                Destroy(gameObject);
            
            fracturedAsteroid = Instantiate(_fracturedPrefab, _transform.position, _transform.rotation); //Spawn in the broken version
            fracturedAsteroid.FractureObject( _scaleMultiplier, AsteroidMoveData); //Initialise the broken version
            _entityCount.Value--;
        }

        public void ReturnToPool()
        {
            _pool.Return(this);
        }

    }
}
