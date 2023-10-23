using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidMovement : MonoBehaviour
    {
        private Vector3 _velocity;
        private Vector3 _angularVelocity;
        private float _mass;

        private bool collisionIsOn = false;

        public Vector3 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public Vector3 AngularVelocity
        {
            get => _angularVelocity;
            set => _angularVelocity = value;
        }
        
        public float Mass => _mass;


        public void Init(float scaleMultiplier, float mass, Vector3 initialVelocity = default, Vector3 initialAngularVelocity = default)
        {
            _velocity = Random.insideUnitSphere * Random.Range(0, 100) + initialVelocity;
            _angularVelocity = Random.insideUnitSphere * Random.Range(0, 100) + initialAngularVelocity;
            _mass = mass;
            
            Invoke(nameof(EnableCollision), 0.1f);
        }

        public void FixedUpdate()
        {
            // Move the asteroid
            transform.position += _velocity * Time.fixedDeltaTime;
            transform.rotation *= Quaternion.Euler(_angularVelocity * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!collisionIsOn) return;
            if (!other.gameObject.TryGetComponent(out AsteroidMovement asteroid)) return;
            // Let the larger asteroid send the collision message
            if (asteroid.Mass > _mass) return;
            Vector3 hitPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            CollisionManager.CalculateCollision(this, asteroid, hitPoint);
        }

        private void EnableCollision()
        {
            collisionIsOn = true;
        }

        public void AddAngularVelocity(Vector3 collisionTorque1)
        {
            _angularVelocity += collisionTorque1 / _mass;
        }
        //
        // private void OnDrawGizmos()
        // {
        //     Vector3 startPos = transform.position + _velocity.normalized * transform.localScale.magnitude;
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawRay(startPos, _velocity);
        // }
    }
}