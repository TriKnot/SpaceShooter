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
    
        private Vector3 _direction;
    
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
            // Check if it's time to despawn
            if(_travelDistance > _maxDistance) ReturnToPool();
        
            // Move and update the distance travelled
            Vector3 frameVelocity = _velocity * Time.deltaTime;
            _transform.position += frameVelocity;
            _travelDistance += frameVelocity.magnitude;
            CheckAhead();
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out AsteroidHealthSystem healthSystem)) return;
            // If it is, fracture it
            Hit(healthSystem, other.transform.position);        
        }
        
        private void CheckAhead()
        {
            // Check if there's anything ahead
            if (!Physics.Raycast(_transform.position, _velocity, out var hit, _speed * Time.deltaTime)) return;
            // If there is, check if it's got a health system
            if (!hit.collider.TryGetComponent(out AsteroidHealthSystem healthSystem)) return;
            // If it is, fracture it
            Hit(healthSystem, hit.point);
        }
        
        private void Hit(AsteroidHealthSystem healthSystem, Vector3 hitPoint)
        {
            // Damage the asteroid
            healthSystem.Hit(_damage, hitPoint, _asteroidPieceCountSO);
            // Spawn the hit effect 
            Vector3 hitPosition = hitPoint + _velocity.normalized * 100.0f;
            Explosion explosion = Instantiate(_hitEffectPrefab, hitPosition, Quaternion.identity);
            explosion.Explode(100);
            
            // Despawn the laser
            ReturnToPool();
        }

        public void InitializePool(ObjectPool<Laser> pool)
        {
            _pool = pool;
        }

        public void ReturnToPool()
        {
            // Reset values
            _velocity = Vector3.zero;
            _travelDistance = 0.0f;
            
            // Disable the laser
            if(_pool != null)
                _pool.Return(this);
            else
                Destroy(gameObject);
        }
    }
}
