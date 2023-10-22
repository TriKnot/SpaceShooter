using UnityEngine;

namespace ScriptableObjects.Variables
{
    public class AddTransformOnStart : AddOnStartBase<Transform>
    {
        private void Start()
        {
            foreach (var variable in variables)
            {
                variable.Value = transform;
            }
        }
    }
}