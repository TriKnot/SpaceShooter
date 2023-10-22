﻿using ScriptableObjects.Variables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class Asteroid : MonoBehaviour
    {
        [Tooltip("\"Fractured\" is the object that this will break into")]
        public AsteroidFractured fractured;
        
        [SerializeField] private FloatVariableSO _minScaleMultiplier;
        [SerializeField] private FloatVariableSO _maxScaleMultiplier;
        [SerializeField] private AnimationCurve _massCurve;
        [SerializeField] private AsteroidHealthSystem _healthSystem;
        
        private float _scaleMultiplier;
        private AsteroidMovement _asteroidMovement;
        
        [SerializeField] private IntVariableSO _entityCount;

        private void Awake()
        {
            float mass = _massCurve.Evaluate(Random.value);
            _scaleMultiplier = Mathf.Lerp(_minScaleMultiplier.Value, _maxScaleMultiplier.Value, mass);
            transform.localScale *= _scaleMultiplier;
            _asteroidMovement = gameObject.AddComponent<AsteroidMovement>();
            _asteroidMovement.Init(_scaleMultiplier, Mathf.Pow(_scaleMultiplier, 3));
            _healthSystem.Init(_scaleMultiplier);
        }

        public void OnDeath()
        {
            Transform trans = transform;
            AsteroidFractured fracturedAsteroid = Instantiate(fractured, trans.position, trans.rotation); //Spawn in the broken version
            fracturedAsteroid.Init(_scaleMultiplier,_asteroidMovement.Velocity, _asteroidMovement.AngularVelocity); //Initialise the broken version
            Destroy(gameObject); //Destroy the object to stop it getting in the way
        }
        
        private void OnEnable()
        {
            _entityCount.Value++;
        }
        
        private void OnDisable()
        {
            _entityCount.Value--;
        }
    }
}
