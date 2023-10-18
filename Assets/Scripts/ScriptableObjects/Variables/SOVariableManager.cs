using UnityEngine;

namespace ScriptableObjects.Variables
{
    public class SOVariableManager : MonoBehaviour
    {

        private VariableBaseSO<int>[] _intVariables;

        private void Awake()
        {
            foreach (VariableBaseSO<int> intVariable in _intVariables)
            {
                if( intVariable != null && intVariable.ResetOnAwake)
                    intVariable.ResetValue();
            }
        }
    }
}
