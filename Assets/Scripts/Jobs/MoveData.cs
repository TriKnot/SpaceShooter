using Unity.Burst;
using UnityEngine;

namespace Jobs
{
    [BurstCompile]
    public struct MoveData
    {
        public bool IsActive;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;

        public MoveData(bool isActive = false, Vector3 velocity = default, Vector3 angularVelocity = default)
        {
            IsActive = isActive;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
        }

    }
}