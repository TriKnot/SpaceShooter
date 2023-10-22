using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Variables
{
    public class AddOnStartBase<T> : MonoBehaviour
    {
        [SerializeField] protected List<VariableBaseSO<T>> variables = new();
    }
}