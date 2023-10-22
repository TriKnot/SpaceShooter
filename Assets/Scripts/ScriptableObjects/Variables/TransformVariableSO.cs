using UnityEngine;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "TransformVariable", menuName = "Variable/TransformVariable", order = 1)]
    public class TransformVariableSO : VariableBaseSO<Transform>
    {
        public override void SetValue(Transform value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<Transform> value)
        {
            Value = value.Value;
        }

        public override void AddValue(Transform value)
        {
            Value.position += value.position;
            Value.rotation *= value.rotation;
            Value.localScale += value.localScale;
        }

        public override void AddValue(VariableBaseSO<Transform> value)
        {
            Value.position += value.Value.position;
            Value.rotation *= value.Value.rotation;
            Value.localScale += value.Value.localScale;
        }

        public override void SubtractValue(Transform value)
        {
            Value.position -= value.position;
            Value.rotation = Quaternion.Inverse(value.rotation) * Value.rotation;
            Value.localScale -= value.localScale;
        }

        public override void SubtractValue(VariableBaseSO<Transform> value)
        {
            Value.position -= value.Value.position;
            Value.rotation = Quaternion.Inverse(value.Value.rotation) * Value.rotation;
            Value.localScale -= value.Value.localScale;
        }

        public override void ResetValue()
        {
            Value = default;
        }
    }

}
