using UnityEngine;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "Vector3Variable", menuName = "Variable/Vector3Variable", order = 1)]
    public class Vector3VariableSO : VariableBaseSO<Vector3>
    {
        public override void SetValue(Vector3 value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<Vector3> value)
        {
            Value = value.Value;
        }

        public override void AddValue(Vector3 value)
        {
            Value += value;
        }

        public override void AddValue(VariableBaseSO<Vector3> value)
        {
            Value += value.Value;
        }

        public override void SubtractValue(Vector3 value)
        {
            Value -= value;
        }

        public override void SubtractValue(VariableBaseSO<Vector3> value)
        {
            Value -= value.Value;
        }

        public override void ResetValue()
        {
            Value = default;
        }
    }

}
