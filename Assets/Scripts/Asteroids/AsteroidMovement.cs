using System.Collections;
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

            StartCoroutine(EnableCollisionDelayed());
        }

        public void FixedUpdate()
        {
            MoveAsteroid();
        }

        private void MoveAsteroid()
        {
            // Move the asteroid
            Vector3 velocity = (Vector3)AsteroidMoveData.Velocity;
            Vector3 angularVelocity = (Vector3)AsteroidMoveData.AngularVelocity;
            transform.position += velocity * Time.fixedDeltaTime;
            transform.rotation *= Quaternion.Euler(angularVelocity * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_collisionIsOn || !TryHandleCollision(other)) return;
        }

        private bool TryHandleCollision(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out AsteroidMovement asteroid)) return false;
            if (asteroid.Mass > _mass) return false;
            Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);
            CollisionManager.CalculateCollision(this, asteroid, hitPoint);
            return true;
        }
        
        private IEnumerator EnableCollisionDelayed()
        {
            yield return new WaitForSeconds(0.1f);
            EnableCollision();
        }
        
        private void EnableCollision()
        {
            _collisionIsOn = true;
        }
    }
}