using System.Collections.Generic;
using System.Linq;
using Asteroids;
using Unity.VisualScripting;
using UnityEngine;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "AsteroidPieceArray", menuName = "Variable/AsteroidPieceArray", order = 1)]
    public class AsteroidPieceArraySO : VariableBaseSO<AsteroidPiece[]>
    {
        public override void SetValue(AsteroidPiece[] value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<AsteroidPiece[]> value)
        {
            Value = value.Value;
        }

        public override void AddValue(AsteroidPiece[] value)
        {
            Value.AddRange(value);
        }

        public override void AddValue(VariableBaseSO<AsteroidPiece[]> value)
        {
            Value.AddRange(value.Value);
        }

        public override void SubtractValue(AsteroidPiece[] value)
        {
            List<AsteroidPiece> asteroidPieces = Value.ToList();
            asteroidPieces.RemoveAll(x => value.Contains(x));
            Value = asteroidPieces.ToArray();
        }

        public override void SubtractValue(VariableBaseSO<AsteroidPiece[]> value)
        {
            SubtractValue(value.Value);
        }

        public override void ResetValue()
        {
            Value = default;
        }
    }

}
