using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids
{
    public class AsteroidsFractured : MonoBehaviour
    {
        [SerializeField] private AsteroidPiece[] _pieces;

        private void Awake()
        {
            FractureObject();
        }

        private void FractureObject()
        {
            foreach (AsteroidPiece piece in _pieces)
            {
                piece.gameObject.SetActive(true);
                piece.Init(Random.insideUnitSphere * 50, Random.insideUnitSphere * 5);
            }
            Destroy(gameObject);
        }
    }
}
