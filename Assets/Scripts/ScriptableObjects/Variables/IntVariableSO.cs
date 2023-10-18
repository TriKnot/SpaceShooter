using UnityEngine;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "IntVariable", menuName = "Variable/IntVariable", order = 1)]
    public class IntVariableSO : VariableBaseSO<int>
    {
        public override void SetValue(int value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<int> value)
        {
            Value = value.Value;
        }

        public override void AddValue(int value)
        {
            Value += value;
        }

        public override void AddValue(VariableBaseSO<int> value)
        {
            Value += value.Value;
        }

        public override void SubtractValue(int value)
        {
            Value -= value;
        }

        public override void SubtractValue(VariableBaseSO<int> value)
        {
            Value -= value.Value;
        }

        public override void ResetValue()
        {
            Value = default;
        }
    }

}
