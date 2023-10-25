using Jobs;
using ScriptableObjects.Variables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidFractured : MonoBehaviour
    {
        [Header("Fractured Settings")]
        [SerializeField] private AsteroidPiece[] _pieces;
        [SerializeField] private Explosion _explosionPrefab;
        [SerializeField] private BoolVariableSO _usePoolingSO;

        [Header("References")]
        [SerializeField] private IntVariableSO _entityCount;
        [SerializeField] private AsteroidPieceObjectPoolSO _asteroidPieceObjectPoolSO;

        public void FractureObject(float scaleMultiplier, MoveData asteroidMoveData)
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
                
                Vector3 asteroidRotationVelocity = Vector3.Cross(asteroidMoveData.AngularVelocity, piecePosition - asteroidCenter);
                Vector3 calculatedVelocity = asteroidRotationVelocity + (Vector3)asteroidMoveData.Velocity + randomDirectionVelocity;
                calculatedVelocity = Vector3.ClampMagnitude(calculatedVelocity, 100);
                
                SpawnPiece(piece, calculatedVelocity, Random.insideUnitSphere, scaleMultiplier, scaleMultiplier / _pieces.Length);
            }
            gameObject.SetActive(false);
        }

        private void SpawnPiece(AsteroidPiece piece, Vector3 calculatedVelocity, Vector3 insideUnitSphere, float scaleMultiplier, float piecesLength)
        {
            piece.Init(calculatedVelocity, Random.insideUnitSphere, scaleMultiplier, scaleMultiplier / _pieces.Length, _entityCount);
            
            if (_usePoolingSO.Value)
            {
                piece.InitializePool(_asteroidPieceObjectPoolSO.Value);
            }
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
