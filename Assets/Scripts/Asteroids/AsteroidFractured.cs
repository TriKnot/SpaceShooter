using System;
using ScriptableObjects.Variables;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidFractured : MonoBehaviour
    {
        [SerializeField] private AsteroidPiece[] _pieces;
        [SerializeField] private IntVariableSO _entityCount;
        [SerializeField] private Explosion _explosionPrefab;

        public void FractureObject(float scaleMultiplier, Vector3 velocity, Vector3 angularVelocity)
        {
            Explosion explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            explosion.Explode(scaleMultiplier);
            
            foreach (AsteroidPiece piece in _pieces)
            {
                if(piece == null) continue;
                
                piece.gameObject.SetActive(true);
                
                Vector3 piecePosition = piece.transform.position;
                Vector3 asteroidCenter = transform.position;
                
                Vector3 randomDirectionVelocity = Random.insideUnitSphere * scaleMultiplier;
                
                Vector3 asteroidRotationVelocity = Vector3.Cross(angularVelocity, piecePosition - asteroidCenter);
                Vector3 calculatedVelocity = asteroidRotationVelocity + velocity + randomDirectionVelocity;
                calculatedVelocity = Vector3.ClampMagnitude(calculatedVelocity, 100);
                piece.Init(calculatedVelocity, Random.insideUnitSphere, scaleMultiplier, scaleMultiplier / _pieces.Length, _entityCount);
            }
            gameObject.SetActive(false);
        }

        public void Activate(Transform parentTransform, float scaleMultiplier, Vector3 velocity, Vector3 angularVelocity)
        {
            transform.position = parentTransform.position;
            FractureObject(scaleMultiplier, velocity, angularVelocity);
        }
        
        private void OnEnable()
        {
            _entityCount.Value++;
        }
        
        private void OnDisable()
        {
            _entityCount.Value--;
        }

    }
}
