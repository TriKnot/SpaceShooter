using ScriptableObjects.Variables;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidPiece : MonoBehaviour
    {
        private Vector3 _velocity;
        private Vector3 _angularVelocity;
        private Transform _transform;
        private IntVariableSO _entityCount;
        
        public void Init(Vector3 velocity, Vector3 angularVelocity, float scaleMultiplier, IntVariableSO entityCount)
        {
            _transform = transform;
            _velocity = velocity;
            _angularVelocity = angularVelocity;
            _transform.parent = null;
            _transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
            _entityCount = entityCount;
            _entityCount.Value++;
        }

        private void FixedUpdate()
        {
            _transform.position += _velocity * Time.fixedDeltaTime;
            _transform.rotation *= Quaternion.Euler(_angularVelocity * Time.fixedDeltaTime);
        }
        
        private void OnDisable()
        {
            _entityCount.Value--;
        }
    }
}
