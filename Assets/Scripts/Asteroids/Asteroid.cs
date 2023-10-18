using UnityEngine;

namespace Asteroids
{
    public class Asteroid : MonoBehaviour
    {
        [Tooltip("\"Fractured\" is the object that this will break into")]
        public GameObject fractured;

        public void FractureObject()
        {
            Transform trans = transform;
            Instantiate(fractured, trans.position, trans.rotation); //Spawn in the broken version
            Destroy(gameObject); //Destroy the object to stop it getting in the way
        }
    }
}
