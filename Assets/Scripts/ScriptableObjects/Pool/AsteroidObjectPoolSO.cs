using Asteroids;
using UnityEngine;
using UnityEngine.Serialization;
using Util;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "AsteroidObjectPool", menuName = "ObjectPool/AsteroidObjectPool", order = 1)]
    public class AsteroidObjectPoolSO : VariableBaseSO<ObjectPool<Asteroid>>
    {
        [SerializeField] private IntVariableSO _initialPoolSize;
        [SerializeField] private AsteroidArraySO _asteroidPrefabSO;
        [SerializeField] private AsteroidArraySO _cubeAsteroidPrefabSO;
        [SerializeField] private BoolVariableSO _usePoolingSO;
        [SerializeField] private BoolVariableSO _useCubeMeshSO;

        public override void SetValue(ObjectPool<Asteroid> value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<ObjectPool<Asteroid>> value)
        {
            Value = value.Value;
        }

        public override void AddValue(ObjectPool<Asteroid> value)
        {
            Value.AddRange(value.Objects.ToArray());
        }

        public override void AddValue(VariableBaseSO<ObjectPool<Asteroid>> value)
        {
            Value.AddRange(value.Value.Objects.ToArray());
        }

        public override void SubtractValue(ObjectPool<Asteroid> value)
        {
            Value.RemoveRange(value.Objects.ToArray());
        }

        public override void SubtractValue(VariableBaseSO<ObjectPool<Asteroid>> value)
        {
            Value.RemoveRange(value.Value.Objects.ToArray());
        }

        public override void ResetValue()
        {
            if(_usePoolingSO.Value)
            {
                Value = _useCubeMeshSO.Value
                    ? new ObjectPool<Asteroid>(_cubeAsteroidPrefabSO.Value, _initialPoolSize.Value)
                    : new ObjectPool<Asteroid>(_asteroidPrefabSO.Value, _initialPoolSize.Value);
                return;
            }            
            Value = new ObjectPool<Asteroid>(null, 0);
        }
        
    }

}
