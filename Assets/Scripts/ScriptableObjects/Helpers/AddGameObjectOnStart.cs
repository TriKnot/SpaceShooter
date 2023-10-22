using UnityEngine;

namespace ScriptableObjects.Variables
{
    public class AddGameObjectOnStart : AddOnStartBase<GameObject>
    {
        private void Start()
        {
            foreach (var variable in variables)
            {
                variable.Value = gameObject;
            }
        }
    }
}