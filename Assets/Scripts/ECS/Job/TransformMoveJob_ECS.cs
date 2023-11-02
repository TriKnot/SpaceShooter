using ECS.Aspect;
using Unity.Burst;
using Unity.Entities;

namespace ECS.Job
{
    [BurstCompile]
    public partial struct TransformMoveJob_ECS : IJobEntity
    {
        private readonly float _deltaTime;
        
        public TransformMoveJob_ECS(float deltaTime)
        {
            _deltaTime = deltaTime;
            __TypeHandle = default;
        }
        
        [BurstCompile]
        private void Execute(MoveAspect_ECS aspectEcs)
        {
            aspectEcs.Move(_deltaTime);
            aspectEcs.Rotate(_deltaTime);
        }


    }
}