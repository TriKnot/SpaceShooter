using ScriptableObjects.Variables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class Asteroid : MonoBehaviour
    {
        [Tooltip("\"Fractured\" is the object that this will break into")]
        public AsteroidFractured fractured;
        
        [SerializeField] private float _minScaleMultiplier;
        [SerializeField] private float _maxScaleMultiplier;
        
        private float _scaleMultiplier;
        private Vector3 _velocity;
        private Vector3 _angularVelocity;

        private void Awake()
        {
            _scaleMultiplier = Random.Range(_minScaleMultiplier, _maxScaleMultiplier);
            transform.localScale *= _scaleMultiplier;
            _velocity = Random.insideUnitSphere * Random.Range(0, 100);
            _angularVelocity = Random.insideUnitSphere * Random.Range(0, 100);
        }

        public void FixedUpdate()
        {
            // Move the asteroid
            transform.position += _velocity * Time.fixedDeltaTime;
            transform.rotation *= Quaternion.Euler(_angularVelocity * Time.fixedDeltaTime);
        }
        
        public void FractureObject()
        {
            Transform trans = transform;
            AsteroidFractured fracturedAsteroid = Instantiate(fractured, trans.position, trans.rotation); //Spawn in the broken version
            fracturedAsteroid.Init(_scaleMultiplier, _velocity, _angularVelocity); //Initialise the broken version
            Destroy(gameObject); //Destroy the object to stop it getting in the way
        }
        
        [SerializeField] private IntVariableSO _entityCount;
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
