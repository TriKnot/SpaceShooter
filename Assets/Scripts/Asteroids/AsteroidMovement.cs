using System;
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
        public Vector3 AngularVelocity => _angularVelocity;
        public float Mass => _mass;


        public void Init(float scaleMultiplier, float mass, Vector3 initialVelocity = default, Vector3 initialAngularVelocity = default)
        {
            _velocity = Random.insideUnitSphere * Random.Range(0, 100) + initialVelocity;
            _angularVelocity = Random.insideUnitSphere * Random.Range(0, 100) + initialAngularVelocity;
            _mass = mass * scaleMultiplier;
            
            Invoke(nameof(EnableCollision), 1f);
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
            CollisionManager.CalculateCollision(this, asteroid);
        }

        private void EnableCollision()
        {
            collisionIsOn = true;
        }

    }
}