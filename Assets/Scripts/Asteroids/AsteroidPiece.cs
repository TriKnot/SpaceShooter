using Jobs;
using ScriptableObjects.Variables;
using UnityEngine;
using Util;
using Utils;

namespace Asteroids
{
    public class AsteroidPiece : MonoBehaviour, IPoolObject<AsteroidPiece>
    {
        private Transform _transform;
        private IntVariableSO _entityCount;
        private AsteroidMovement _asteroidMovement;
        private MoveData _asteroidMoveData;

        private ObjectPool<AsteroidPiece> _pool;
        
        public void Init(Vector3 velocity, Vector3 angularVelocity, float scaleMultiplier, float mass, IntVariableSO entityCount)
        {
            _transform = transform;
            _transform.parent = null;
            _transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
            _entityCount = entityCount;
            _entityCount.Value++;
            
            _asteroidMoveData = new MoveData()
            {
                Velocity = Random.insideUnitSphere * Random.Range(0, 100) + velocity,
                AngularVelocity = Random.insideUnitSphere * Random.Range(0, 100) + angularVelocity 
            };

            _asteroidMovement = gameObject.AddComponent<AsteroidMovement>();
            _asteroidMovement.Init( mass, _asteroidMoveData );

            if (TryGetComponent(out AsteroidHealthSystem healthSystem))
            {
                healthSystem.Init(scaleMultiplier);
                healthSystem.OnDeathEvent.AddListener(OnDeath);
            }        
        }

        private void OnDeath()
        {
            if(_pool != null) ReturnToPool();
            else Destroy(gameObject);
        }

        private void OnDisable()
        {
            if(_entityCount != null) _entityCount.Value--;
        }

        public void InitializePool(ObjectPool<AsteroidPiece> pool)
        {
            _pool = pool;
        }

        public void ReturnToPool()
        {
            _pool.Return(this);
        }
    }
}
