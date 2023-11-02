using System.Collections.Generic;
using System.Linq;
using Asteroids;
using ECS.Authoring;
using Unity.VisualScripting;
using UnityEngine;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "AsteroidEntityArray", menuName = "Variable/AsteroidEntityArray", order = 1)]
    public class AsteroidAuthoringArraySO : VariableBaseSO<AsteroidAuthoring_ECS[]>
    {
        public override void SetValue(AsteroidAuthoring_ECS[] value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<AsteroidAuthoring_ECS[]> value)
        {
            Value = value.Value;
        }

        public override void AddValue(AsteroidAuthoring_ECS[] value)
        {
            Value.AddRange(value);
        }

        public override void AddValue(VariableBaseSO<AsteroidAuthoring_ECS[]> value)
        {
            Value.AddRange(value.Value);
        }

        public override void SubtractValue(AsteroidAuthoring_ECS[] value)
        {
            List<AsteroidAuthoring_ECS> asteroidPieces = Value.ToList();
            asteroidPieces.RemoveAll(x => value.Contains(x));
            Value = asteroidPieces.ToArray();
        }

        public override void SubtractValue(VariableBaseSO<AsteroidAuthoring_ECS[]> value)
        {
            SubtractValue(value.Value);
        }

        public override void ResetValue()
        {
            Value = default;
        }
    }

}
