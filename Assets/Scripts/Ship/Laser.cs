using Asteroids;
using UnityEngine;

namespace Ship
{
    public class Laser : MonoBehaviour
    {
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

        }
    
    
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Asteroid asteroid)) return;
            asteroid.FractureObject();
            Destroy(gameObject);
        }
    }
}
