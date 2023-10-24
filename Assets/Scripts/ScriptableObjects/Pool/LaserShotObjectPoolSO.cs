using Ship;
using UnityEngine;
using Util;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "LaserShotObjectPool", menuName = "ObjectPool/LaserShotObjectPool", order = 1)]
    public class LaserShotObjectPoolSO : VariableBaseSO<ObjectPool<Laser>>
    {
        [SerializeField] private IntVariableSO _initialPoolSize;
        [SerializeField] private Laser[] _laserPrefabs;
        [SerializeField] private BoolVariableSO _usePoolingSO;
        
        public override void SetValue(ObjectPool<Laser> value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<ObjectPool<Laser>> value)
        {
            Value = value.Value;
        }

        public override void AddValue(ObjectPool<Laser> value)
        {
            Value.AddRange(value.Objects.ToArray());
        }

        public override void AddValue(VariableBaseSO<ObjectPool<Laser>> value)
        {
            Value.AddRange(value.Value.Objects.ToArray());
        }

        public override void SubtractValue(ObjectPool<Laser> value)
        {
            Value.RemoveRange(value.Objects.ToArray());
        }

        public override void SubtractValue(VariableBaseSO<ObjectPool<Laser>> value)
        {
            Value.RemoveRange(value.Value.Objects.ToArray());
        }

        public override void ResetValue()
        {
            Value = _usePoolingSO.Value
                ? new ObjectPool<Laser>(_laserPrefabs, _initialPoolSize.Value)
                : new ObjectPool<Laser>(null, 0);        }
        
    }

}
