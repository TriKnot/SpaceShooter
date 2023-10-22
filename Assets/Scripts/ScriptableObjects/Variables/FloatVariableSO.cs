using UnityEngine;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "FloatVariable", menuName = "Variable/FloatVariable", order = 1)]
    public class FloatVariableSO : VariableBaseSO<float>
    {
        public override void SetValue(float value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<float> value)
        {
            Value = value.Value;
        }

        public override void AddValue(float value)
        {
            Value += value;
        }

        public override void AddValue(VariableBaseSO<float> value)
        {
            Value += value.Value;
        }

        public override void SubtractValue(float value)
        {
            Value -= value;
        }

        public override void SubtractValue(VariableBaseSO<float> value)
        {
            Value -= value.Value;
        }

        public override void ResetValue()
        {
            Value = default;
        }
    }

}
