using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Component
{
    public struct MoveDataEcs : IComponentData
    {
        public bool IsActive;
        public float3 Velocity;
        public float3 AngularVelocity;
        
        public MoveDataEcs(bool isActive = false, Vector3 velocity = default, Vector3 angularVelocity = default)
        {
            IsActive = isActive;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
        }
    }
}
