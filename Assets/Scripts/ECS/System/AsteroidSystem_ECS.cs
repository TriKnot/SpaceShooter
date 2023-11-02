using ECS.Job;
using Unity.Burst;
using Unity.Entities;

namespace ECS.System
{
    [BurstCompile]
    public partial struct AsteroidSystem_ECS : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var job = new TransformMoveJob_ECS(SystemAPI.Time.DeltaTime);
            job.ScheduleParallel();
        }
    }
}