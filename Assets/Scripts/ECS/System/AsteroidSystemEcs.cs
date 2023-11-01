using ECS.Job;
using Unity.Burst;
using Unity.Entities;

namespace ECS.System
{
    [BurstCompile]
    public partial struct AsteroidSystemEcs : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var job = new TransformMoveJobEcs(SystemAPI.Time.DeltaTime);
            job.ScheduleParallel();
        }
    }
}