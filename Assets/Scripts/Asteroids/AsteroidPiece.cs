using UnityEngine;

namespace Asteroids
{
    public class AsteroidPiece : MonoBehaviour
    {
        private Vector3 _velocity;
        private Vector3 _angularVelocity;
        
        public void Init(Vector3 velocity, Vector3 angularVelocity)
        {
            _velocity = velocity;
            _angularVelocity = angularVelocity;
            transform.parent = null;
        }

        private void FixedUpdate()
        {
            transform.position += _velocity * Time.fixedDeltaTime;
            transform.rotation *= Quaternion.Euler(_angularVelocity * Time.fixedDeltaTime);
        }
    }
}
