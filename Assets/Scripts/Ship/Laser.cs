using Asteroids;
using UnityEngine;

namespace Ship
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] private Vector3 _targetSize;
        [SerializeField] private float _speed;
        [SerializeField] private float _maxDistance;
        [SerializeField] private Transform _transform;
    
        private Vector3 _direction;
    
        private Vector3 _velocity;
        private float _travelDistance;
        private bool _isFullSize;

        public void Init(Vector3 direction)
        {
            _velocity = direction * _speed;
        }
    
        private void FixedUpdate()
        {
            // Check if it's time to despawn
            if(_travelDistance > _maxDistance) Destroy(gameObject);
        
            // Move and update the distance travelled
            Vector3 frameVelocity = _velocity * Time.fixedDeltaTime;
            _transform.position += frameVelocity;
            _travelDistance += frameVelocity.magnitude;
        
            // Scale up
            if (_isFullSize) return;
        
            _transform.localScale = Vector3.Lerp(_transform.localScale, _targetSize, Time.fixedDeltaTime );
        
            if(_transform.localScale == _targetSize)
                _isFullSize = true;
        }
    
    
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Hit something!");
            if (!other.TryGetComponent(out Asteroid asteroid)) return;
            asteroid.FractureObject();
            Destroy(gameObject);
        }
    }
}
