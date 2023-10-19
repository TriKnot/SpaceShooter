using UnityEngine;
using UnityEngine.Events;

namespace Asteroids
{
    public class AsteroidHealthSystem : MonoBehaviour
    {
        [SerializeField] private Explosion _explosionPrefab;
        [SerializeField] private UnityEvent _onDeathEvent;
        
        [SerializeField] private float _currentHealth;
        public void Init(float scaleMultiplier)
        {
            _currentHealth = scaleMultiplier;
        }
        
        public void Hit(float damage)
        {
            _currentHealth -= damage;
            if (_currentHealth > 0) return;
            _currentHealth = 0;
            OnDeath();
        }

        private void OnDeath()
        {
            Explosion explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            explosion.Explode(transform.localScale.x);
            
            _onDeathEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}