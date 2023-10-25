using Jobs;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidMovement : MonoBehaviour
    {
        private float _mass;
        private bool _collisionIsOn = false;

        public MoveData AsteroidMoveData { get; set; }

        public float Mass => _mass;

        public void Init(float mass, MoveData moveData)
        {
            AsteroidMoveData = moveData; 
            _mass = mass;
            
            Invoke(nameof(EnableCollision), 0.1f);
        }

        public void FixedUpdate()
        {
            // Move the asteroid
            transform.position += (Vector3)AsteroidMoveData.Velocity * Time.fixedDeltaTime;
            transform.rotation *= Quaternion.Euler((Vector3)AsteroidMoveData.AngularVelocity * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_collisionIsOn) return;
            if (!other.gameObject.TryGetComponent(out AsteroidMovement asteroid)) return;
            // Let the larger asteroid send the collision message
            if (asteroid.Mass > _mass) return;
            Vector3 hitPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            CollisionManager.CalculateCollision(this, asteroid, hitPoint);
        }

        private void EnableCollision()
        {
            _collisionIsOn = true;
        }
        
    }
}