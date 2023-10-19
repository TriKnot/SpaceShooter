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

        public void Init(float _scaleMultiplier, Vector3 _velocity, Vector3 _angularVelocity)
        {
            Transform trans = transform;
            trans.localScale *= _scaleMultiplier;
            trans.rotation *= Quaternion.Euler(_angularVelocity * Time.fixedDeltaTime);
            FractureObject(_scaleMultiplier, _velocity, _angularVelocity);
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
                piece.Init(calculatedVelocity, Random.insideUnitSphere * scaleMultiplier, scaleMultiplier, _entityCount);
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
