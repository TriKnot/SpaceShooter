namespace ScriptableObjects.Variables
{
    public interface IVariableObserver<T>
    {
        void OnValueChanged(T newValue);
    }
}

