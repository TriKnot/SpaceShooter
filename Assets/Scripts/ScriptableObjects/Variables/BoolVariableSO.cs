using UnityEngine;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "BoolVariable", menuName = "Variable/BoolVariable", order = 1)]
    public class BoolVariableSO : VariableBaseSO<bool>
    {
        public override void SetValue(bool value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<bool> value)
        {
            Value = value.Value;
        }

        public override void AddValue(bool value)
        {
            Debug.LogError("Cannot add a bool to a bool variable.");
        }

        public override void AddValue(VariableBaseSO<bool> value)
        {
            Debug.LogError("Cannot add a bool to a bool variable.");
        }

        public override void SubtractValue(bool value)
        {
            Debug.LogError("Cannot subtract a bool from a bool variable.");
        }

        public override void SubtractValue(VariableBaseSO<bool> value)
        {
            Debug.LogError("Cannot subtract a bool from a bool variable.");
        }

        public override void ResetValue()
        {
            Value = default;
        }
    }

}
