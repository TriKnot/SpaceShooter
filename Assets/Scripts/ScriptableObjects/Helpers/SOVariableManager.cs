using UnityEngine;

namespace ScriptableObjects.Variables
{
    public class SOVariableManager : MonoBehaviour
    {

        private void Awake()
        {
            ResettableVariableBase[] variables = Resources.FindObjectsOfTypeAll(typeof(ResettableVariableBase)) as ResettableVariableBase[];
            foreach (ResettableVariableBase variable in variables)
            {
                if (variable.ResetOnAwake)
                {
                    variable.ResetValue();
                }
            }
            
        }
    }
}
