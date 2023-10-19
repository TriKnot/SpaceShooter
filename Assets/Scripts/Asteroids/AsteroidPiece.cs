using ScriptableObjects.Variables;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidPiece : MonoBehaviour
    {
        private Transform _transform;
        private IntVariableSO _entityCount;
        
        private AsteroidMovement _asteroidMovement;
        
        public void Init(Vector3 velocity, Vector3 angularVelocity, float scaleMultiplier, float mass, IntVariableSO entityCount)
        {
            _transform = transform;
            _transform.parent = null;
            _transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
            _entityCount = entityCount;
            _entityCount.Value++;
            
            _asteroidMovement = gameObject.AddComponent<AsteroidMovement>();
            _asteroidMovement.Init(scaleMultiplier, mass, velocity, angularVelocity);
        }
        
        private void OnDisable()
        {
            _entityCount.Value--;
        }

        public void OnHit()
        {
            Destroy(gameObject);
        }
    }
}
