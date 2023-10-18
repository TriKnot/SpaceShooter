using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidFractured : MonoBehaviour
    {
        [SerializeField] private AsteroidPiece[] _pieces;

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
                
                // Move the asteroid piece away from the center of the asteroid in relation to the scalemultiplier
                Transform pieceTransform = piece.transform;
                pieceTransform.localPosition *= scaleMultiplier * 0.05f;
                
                Vector3 piecePosition = pieceTransform.position;
                Vector3 asteroidCenter = transform.position;
                
                Vector3 randomDirectionVelocity = Random.insideUnitSphere * scaleMultiplier;
                
                Vector3 asteroidRotationVelocity = Vector3.Cross(angularVelocity, piecePosition - asteroidCenter);
                Vector3 calculatedVelocity = asteroidRotationVelocity + velocity + randomDirectionVelocity;
                calculatedVelocity = Vector3.ClampMagnitude(calculatedVelocity, 100);
                piece.Init(calculatedVelocity, Random.insideUnitSphere * scaleMultiplier, scaleMultiplier);
            }
            Destroy(gameObject);
        }

    }
}
