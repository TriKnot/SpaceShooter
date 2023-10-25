using Asteroids;
using ScriptableObjects.Variables;
using UnityEngine;
using Util;
using Utils;

namespace Ship
{
    public class Laser : MonoBehaviour, IPoolObject<Laser>
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _maxDistance;
        [SerializeField] private Transform _transform;
        [SerializeField] private float _damage;
        [SerializeField] private Explosion _hitEffectPrefab;
        [SerializeField] private IntVariableSO _asteroidPieceCountSO;
    
        private Vector3 _velocity;
        private float _travelDistance;
        
        private ObjectPool<Laser> _pool;
        
        public void Init(Vector3 startPosition, Quaternion rotation, Vector3 direction)
        {
            _transform.position = startPosition;
            _transform.rotation = rotation;
            _velocity = direction * _speed;
        }
    
        private void Update()
        {
            UpdateMovement();
        }
    
        private void UpdateMovement()
        {
            if (_travelDistance > _maxDistance) ReturnToPool();
        
            Vector3 frameVelocity = _velocity * Time.deltaTime;
            _transform.position += frameVelocity;
            _travelDistance += frameVelocity.magnitude;
            CheckAhead();
        }
    
        private void CheckAhead()
        {
            // Check if there's anything ahead
            if (Physics.Raycast(_transform.position, _velocity, out RaycastHit hit, _speed * Time.deltaTime))
            {
                if (!hit.collider.TryGetComponent(out AsteroidHealthSystem healthSystem)) return;
                HandleHit(hit.collider);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            HandleHit(other);
        }

        private void HandleHit(Collider other)
        {
            if (!other.TryGetComponent(out AsteroidHealthSystem healthSystem)) return;
            DamageAsteroid(healthSystem, other.transform.position);        
        }

        private void DamageAsteroid(AsteroidHealthSystem healthSystem, Vector3 hitPoint)
        {
            healthSystem.Hit(_damage, hitPoint, _asteroidPieceCountSO);
            Vector3 hitPosition = hitPoint + _velocity.normalized * 100.0f;
            Explosion explosion = Instantiate(_hitEffectPrefab, hitPosition, Quaternion.identity);
            explosion.Explode(100);
            
            ReturnToPool();
        }

        public void InitializePool(ObjectPool<Laser> pool)
        {
            _pool = pool;
        }

        public void ReturnToPool()
        {
            ResetValues();
            DisableLaser();
        }

        private void ResetValues()
        {
            _velocity = Vector3.zero;
            _travelDistance = 0.0f;
        }

        private void DisableLaser()
        {
            if (_pool != null)
            {
                _pool.Return(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
