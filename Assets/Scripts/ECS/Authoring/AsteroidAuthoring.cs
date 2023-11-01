using ECS.Component;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring
{
    public class AsteroidAuthoring : MonoBehaviour
    {
        
        public class AsteroidBaker : Baker<AsteroidAuthoring>
        {
            public override void Bake(AsteroidAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new AsteroidDataEcs(1));
                
                Vector3 velocity = Random.insideUnitCircle.normalized * 5;
                Vector3 angularVelocity = Random.insideUnitSphere.normalized * 5;
                AddComponent(entity, new MoveDataEcs(true, velocity, angularVelocity));
            }
        }
        
    }
}