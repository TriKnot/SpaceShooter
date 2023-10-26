using ScriptableObjects.Variables;
using UnityEngine;

namespace Asteroids
{
    public class AsteroidHealthSystem : MonoBehaviour
    {
        private Asteroid _asteroid;
        
        [Header("Current Health")]
        [SerializeField] private float _currentHealth;
        
        public void Init(Asteroid asteroid, float scaleMultiplier)
        {
            _asteroid = asteroid;
            _currentHealth = scaleMultiplier;
        }
        
        public void Hit(float damage, Vector3 hitPoint)
        {
            _currentHealth -= damage;
            
            if (_currentHealth > 0)
            {
                // TODO: Spawn asteroid pieces on hit
                // Spawn 1-3 pieces
                // int pieceCount = Random.Range(1, 4);
                // for (int i = 0; i < pieceCount; i++)
                // {
                //     // Get a random piece   
                //     Asteroid piece = _asteroidPieceSO.Value.Get();
                //     
                //     // Make it move away from the hit point
                //     Vector3 startPosition = (transform.position - hitPoint).normalized;
                //     // Add some random direction
                //     startPosition += Random.insideUnitSphere * damage;
                //     // Clamp the magnitude
                //     startPosition = Vector3.ClampMagnitude(startPosition, 100.0f);
                //     
                //     Quaternion rotation = Quaternion.Euler(Random.insideUnitSphere * 360);
                //     
                //     piece.Init();
                //     piece.Activate( startPosition, rotation );
                // }
                // If it's not dead, return
                return;
            }
            // If it's dead, explode
            _currentHealth = 0;
            _asteroid.OnDeath();
        }
    }
}
