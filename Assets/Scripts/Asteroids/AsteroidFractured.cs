using System;
using ScriptableObjects.Variables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidFractured : MonoBehaviour
    {
        [SerializeField] private AsteroidPiece[] _pieces;
        [SerializeField] private IntVariableSO _entityCount;

        public void Init(float scaleMultiplier, Vector3 velocity, Vector3 angularVelocity)
        {
            Transform trans = transform;
            trans.localScale *= scaleMultiplier;
            trans.rotation *= Quaternion.Euler(angularVelocity * Time.fixedDeltaTime);
            FractureObject(scaleMultiplier, velocity, angularVelocity);
        }


        private void FractureObject(float scaleMultiplier, Vector3 velocity, Vector3 angularVelocity)
        {
            foreach (AsteroidPiece piece in _pieces)
            {
                piece.gameObject.SetActive(true);
                
                Vector3 piecePosition = piece.transform.position;
                Vector3 asteroidCenter = transform.position;
                
                Vector3 randomDirectionVelocity = Random.insideUnitSphere * scaleMultiplier;
                
                Vector3 asteroidRotationVelocity = Vector3.Cross(angularVelocity, piecePosition - asteroidCenter);
                Vector3 calculatedVelocity = asteroidRotationVelocity + velocity + randomDirectionVelocity;
                calculatedVelocity = Vector3.ClampMagnitude(calculatedVelocity, 100);
                piece.Init(calculatedVelocity, Random.insideUnitSphere * scaleMultiplier, scaleMultiplier, scaleMultiplier / _pieces.Length, _entityCount);
            }
            Destroy(gameObject);
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
