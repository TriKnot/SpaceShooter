using System;
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
        
        private float _scaleMultiplier;
        private AsteroidMovement _asteroidMovement;
        
        [SerializeField] private IntVariableSO _entityCount;
        
        private ObjectPool<Asteroid> _pool;


        public void Initialize(ObjectPool<Asteroid> pool)
        {
            _pool = pool;
            
            float mass = _massCurve.Evaluate(Random.value);
            _scaleMultiplier = Mathf.Lerp(_minScaleMultiplier.Value, _maxScaleMultiplier.Value, mass);
            transform.localScale *= _scaleMultiplier;
            _asteroidMovement = gameObject.AddComponent<AsteroidMovement>();
            
            fracturedAsteroid = Instantiate(_fracturedPrefab, _transform.position, _transform.rotation); //Spawn in the broken version
            fracturedAsteroid.Init(); //Initialise the broken version
        }

        public void Activate(Vector3 position, Quaternion rotation)
        {
            _asteroidMovement.Init(_scaleMultiplier, Mathf.Pow(_scaleMultiplier, 3));
            _transform.position = position;
            _transform.rotation = rotation;
            gameObject.SetActive(true);
            _entityCount.Value++;
            _healthSystem.Init(_scaleMultiplier);
        }
        
        public void OnDeath()
        {
            ReturnToPool();
        }

        public void ReturnToPool()
        {
            fracturedAsteroid.Activate(_transform, _scaleMultiplier,_asteroidMovement.Velocity, _asteroidMovement.AngularVelocity);
            _entityCount.Value--;

            if(_pool != null) _pool.Return(this);
        }

    }
}
