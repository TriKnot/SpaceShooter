using ECS.Component;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Aspect
{
    [BurstCompile]
    public readonly partial struct MoveAspect : IAspect
    {
        private readonly RefRW<LocalTransform> _localTransform;
        private readonly RefRO<MoveDataEcs> _moveData;

        [BurstCompile]
        public void Move(float deltaTime)
        {
            _localTransform.ValueRW.Position += _moveData.ValueRO.Velocity * deltaTime;
        }

        [BurstCompile]
        public void Rotate(float deltaTime)
        {
            quaternion rotationDelta = quaternion.EulerXYZ(_moveData.ValueRO.AngularVelocity * deltaTime);
            _localTransform.ValueRW.Rotation = math.mul(_localTransform.ValueRO.Rotation, rotationDelta);
        }
        
    }
}