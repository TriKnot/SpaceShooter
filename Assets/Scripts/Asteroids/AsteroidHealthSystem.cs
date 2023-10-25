using ScriptableObjects.Variables;
using UnityEngine;
using UnityEngine.Events;

namespace Asteroids
{
    public class AsteroidHealthSystem : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private Explosion _explosionPrefab;
        [SerializeField] private AsteroidPieceArraySO _piecePrefabs;

        [Header("Events")]
        [SerializeField] private UnityEvent _onDeathEvent;

        [Header("Current Health")]
        [SerializeField] private float _currentHealth;
        
        public UnityEvent OnDeathEvent => _onDeathEvent;
        
        public void Init(float scaleMultiplier)
        {
            _currentHealth = scaleMultiplier;
        }
        
        public void Hit(float damage, Vector3 hitPoint, IntVariableSO entityCount)
        {
            _currentHealth -= damage;
            
            if (_currentHealth > 0)
            {
                // Spawn 1-3 pieces
                int pieceCount = Random.Range(1, 4);
                for (int i = 0; i < pieceCount; i++)
                {
                    AsteroidPiece piece = Instantiate(_piecePrefabs.Value[Random.Range(0, _piecePrefabs.Value.Length)], hitPoint, Quaternion.identity);

                    // Make it move away from the hit point
                    Vector3 velocity = (piece.transform.position - hitPoint).normalized;
                    // Add some random direction
                    velocity += Random.insideUnitSphere * damage;
                    // Clamp the magnitude
                    velocity = Vector3.ClampMagnitude(velocity, 100.0f);
                    
                    Vector3 angularVelocity = Random.insideUnitSphere * 100.0f;
                    
                    piece.Init(velocity, angularVelocity, _currentHealth / damage, damage, entityCount);
                }
                // If it's not dead, return
                return;
            }
            // If it's dead, explode
            _currentHealth = 0;
            OnDeath();
        }

        private void OnDeath()
        {
            Explosion explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            explosion.Explode(transform.localScale.x / transform.localScale.x);
            
            _onDeathEvent?.Invoke();
        }
    }
}
