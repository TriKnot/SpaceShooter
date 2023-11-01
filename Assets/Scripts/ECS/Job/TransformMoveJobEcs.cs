using ECS.Aspect;
using ECS.Component;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.Job
{
    [BurstCompile]
    public partial struct TransformMoveJobEcs : IJobEntity
    {
        private readonly float _deltaTime;
        
        public TransformMoveJobEcs(float deltaTime)
        {
            _deltaTime = deltaTime;
            __TypeHandle = default;
        }
        
        [BurstCompile]
        private void Execute(MoveAspect aspect)
        {
            aspect.Move(_deltaTime);
            aspect.Rotate(_deltaTime);
        }


    }
}