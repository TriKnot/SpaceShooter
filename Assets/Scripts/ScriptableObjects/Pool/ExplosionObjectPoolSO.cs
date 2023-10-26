using Asteroids;
using UnityEngine;
using Util;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "ExplosionObjectPool", menuName = "ObjectPool/ExplosionObjectPool", order = 1)]
    public class ExplosionObjectPoolSO : VariableBaseSO<ObjectPool<Explosion>>
    {
        [SerializeField] private IntVariableSO _initialPoolSize;
        [SerializeField] private Explosion[] _prefabs;
        [SerializeField] private BoolVariableSO _usePoolingSO;
        
        public override void SetValue(ObjectPool<Explosion> value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<ObjectPool<Explosion>> value)
        {
            Value = value.Value;
        }

        public override void AddValue(ObjectPool<Explosion> value)
        {
            Value.AddRange(value.Objects.ToArray());
        }

        public override void AddValue(VariableBaseSO<ObjectPool<Explosion>> value)
        {
            Value.AddRange(value.Value.Objects.ToArray());
        }

        public override void SubtractValue(ObjectPool<Explosion> value)
        {
            Value.RemoveRange(value.Objects.ToArray());
        }

        public override void SubtractValue(VariableBaseSO<ObjectPool<Explosion>> value)
        {
            Value.RemoveRange(value.Value.Objects.ToArray());
        }

        public override void ResetValue()
        {
            Value = _usePoolingSO.Value
                ? new ObjectPool<Explosion>(_prefabs, _initialPoolSize.Value)
                : new ObjectPool<Explosion>(null, 0);        }
        
    }

}
