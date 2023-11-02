using ECS.Component;
using Unity.Entities;
using UnityEngine;

namespace ECS.Authoring
{
    public class AsteroidAuthoring_ECS : MonoBehaviour
    {
        public class AsteroidBaker : Baker<AsteroidAuthoring_ECS>
        {
            public override void Bake(AsteroidAuthoring_ECS authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new AsteroidDataEcs(1));
                
                AddComponent(entity, new MoveDataEcs());
            }
        }
        
    }
}