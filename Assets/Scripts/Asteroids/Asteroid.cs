using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Asteroids
{
    public class Asteroid : MonoBehaviour
    {
        [Tooltip("\"Fractured\" is the object that this will break into")]
        public GameObject fractured;
        
        private Vector3 _velocity;
        private Vector3 _angularVelocity;

        public void FixedUpdate()
        {
            // Move the asteroid
            transform.position += _velocity * Time.fixedDeltaTime;
            transform.rotation *= Quaternion.Euler(_angularVelocity * Time.fixedDeltaTime);
        }
        
        public void FractureObject()
        {
            Transform trans = transform;
            Instantiate(fractured, trans.position, trans.rotation); //Spawn in the broken version
            Destroy(gameObject); //Destroy the object to stop it getting in the way
        }
    }
}
