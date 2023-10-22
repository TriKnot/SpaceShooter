using UnityEngine;

namespace ScriptableObjects.Variables
{
    public class SOVariableManager : MonoBehaviour
    {

        private IntVariableSO[] _intVariables;

        private void Awake()
        {
            _intVariables = Resources.FindObjectsOfTypeAll<IntVariableSO>();
            
            foreach (IntVariableSO intVariable in _intVariables)
            {
                if( intVariable != null && intVariable.ResetOnAwake)
                    intVariable.ResetValue();
            }
        }
    }
}
