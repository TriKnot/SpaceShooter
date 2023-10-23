using UnityEngine;

namespace ScriptableObjects.Variables
{
    public abstract class ResettableVariableBase : ScriptableObject
    {
        
        [SerializeField]
        private bool resetOnAwake = true;
        
        public bool ResetOnAwake
        {
            get { return resetOnAwake; }
        }
        
        public abstract void ResetValue();

    }
}