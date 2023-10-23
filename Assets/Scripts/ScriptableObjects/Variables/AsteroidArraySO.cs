using System.Collections.Generic;
using System.Linq;
using Asteroids;
using Unity.VisualScripting;
using UnityEngine;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "AsteroidArray", menuName = "Variable/AsteroidArray", order = 1)]
    public class AsteroidArraySO : VariableBaseSO<Asteroid[]>
    {
        public override void SetValue(Asteroid[] value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<Asteroid[]> value)
        {
            Value = value.Value;
        }

        public override void AddValue(Asteroid[] value)
        {
            Value.AddRange(value);
        }

        public override void AddValue(VariableBaseSO<Asteroid[]> value)
        {
            Value.AddRange(value.Value);
        }

        public override void SubtractValue(Asteroid[] value)
        {
            List<Asteroid> asteroidPieces = Value.ToList();
            asteroidPieces.RemoveAll(x => value.Contains(x));
            Value = asteroidPieces.ToArray();
        }

        public override void SubtractValue(VariableBaseSO<Asteroid[]> value)
        {
            SubtractValue(value.Value);
        }

        public override void ResetValue()
        {
            Value = default;
        }
    }

}
