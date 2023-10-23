using Asteroids;
using UnityEngine;
using Util;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "AsteroidPieceObjectPool", menuName = "ObjectPool/AsteroidPieceObjectPool", order = 1)]
    public class AsteroidPieceObjectPoolSO : VariableBaseSO<ObjectPool<AsteroidPiece>>
    {
        [SerializeField] private IntVariableSO _initialPoolSize;
        [SerializeField] private AsteroidPieceArraySO _asteroidPieceSO;
        
        public override void SetValue(ObjectPool<AsteroidPiece> value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<ObjectPool<AsteroidPiece>> value)
        {
            Value = value.Value;
        }

        public override void AddValue(ObjectPool<AsteroidPiece> value)
        {
            Value.AddRange(value.Objects.ToArray());
        }

        public override void AddValue(VariableBaseSO<ObjectPool<AsteroidPiece>> value)
        {
            Value.AddRange(value.Value.Objects.ToArray());
        }

        public override void SubtractValue(ObjectPool<AsteroidPiece> value)
        {
            Value.RemoveRange(value.Objects.ToArray());
        }

        public override void SubtractValue(VariableBaseSO<ObjectPool<AsteroidPiece>> value)
        {
            Value.RemoveRange(value.Value.Objects.ToArray());
        }

        public override void ResetValue()
        {
            Value = new ObjectPool<AsteroidPiece>(_asteroidPieceSO.Value, _initialPoolSize.Value);
        }
        
    }

}
