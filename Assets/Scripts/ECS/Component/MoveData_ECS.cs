using Unity.Burst;
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
        
    }
}
