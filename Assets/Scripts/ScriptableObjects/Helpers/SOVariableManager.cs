using UnityEngine;

namespace ScriptableObjects.Variables
{
    public class SOVariableManager : MonoBehaviour
    {

        private void Awake()
        {
            ResettableVariableBase[] variables = Resources.LoadAll<ResettableVariableBase>("ScriptableObjects/Variables");
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
