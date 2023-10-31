using ScriptableObjects.Variables;
using UnityEngine;

namespace Ship
{
    public class ShipConfig : MonoBehaviour
    {
        [SerializeField] private BoolVariableSO _usePhysicsSO;
        
        private void Awake()
        {
            if (!_usePhysicsSO.Value)
            {
                Destroy(GetComponent<Rigidbody>());
                Destroy(GetComponent<Collider>());
            }
        }
    }
}
