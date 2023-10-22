using UnityEngine;

namespace ScriptableObjects.Variables
{
    [CreateAssetMenu(fileName = "GameObjectVariable", menuName = "Variable/GameObjectVariable", order = 1)]
    public class GameObjectVariableSO : VariableBaseSO<GameObject>
    {
        public override void SetValue(GameObject value)
        {
            Value = value;
        }

        public override void SetValue(VariableBaseSO<GameObject> value)
        {
            Value = value.Value;
        }

        public override void AddValue(GameObject value)
        {
            Debug.LogError("Cannot add GameObjects");
        }

        public override void AddValue(VariableBaseSO<GameObject> value)
        {
            Debug.LogError("Cannot add GameObjects");
        }

        public override void SubtractValue(GameObject value)
        {
            Debug.LogError("Cannot subtract GameObjects");
        }

        public override void SubtractValue(VariableBaseSO<GameObject> value)
        {
            Debug.LogError("Cannot subtract GameObjects");
        }

        public override void ResetValue()
        {
            Value = default;
        }
    }

}
