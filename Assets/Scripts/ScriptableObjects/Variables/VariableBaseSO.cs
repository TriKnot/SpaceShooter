using System;
using UnityEngine;

namespace ScriptableObjects.Variables
{
    public abstract class VariableBaseSO<T> : ScriptableObject
    {
        [SerializeField]
        private bool resetOnAwake = true;
        
        [SerializeField]
        private T value;

        public T Value
        {
            get { return value; }
            set
            {
                if (!value.Equals(this.value))
                {
                    this.value = value;
                    OnValueChanged(this.value);
                }
            }
        }
        
        public event Action<T> ValueChanged;

        public void RegisterObserver(IVariableObserver<T> observer)
        {
            ValueChanged += observer.OnValueChanged;
        }

        public void UnregisterObserver(IVariableObserver<T> observer)
        {
            ValueChanged -= observer.OnValueChanged;
        }

        protected virtual void OnValueChanged(T newValue)
        {
            ValueChanged?.Invoke(newValue);
        }
        
        public bool ResetOnAwake
        {
            get { return resetOnAwake; }
        }
        
        public abstract void SetValue(T value);
        
        public abstract void SetValue(VariableBaseSO<T> value);
        
        public abstract void AddValue(T value);
        
        public abstract void AddValue(VariableBaseSO<T> value);
        
        public abstract void SubtractValue(T value);
        
        public abstract void SubtractValue(VariableBaseSO<T> value);
                
        public abstract void ResetValue();
        
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}

