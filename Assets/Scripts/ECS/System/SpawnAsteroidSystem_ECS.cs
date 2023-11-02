using ECS.Aspect;
using ECS.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS.System
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpawnAsteroidSystem_ECS : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AsteroidManagerProperties_ECS>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            Entity asteroidManagerEntity = SystemAPI.GetSingletonEntity<AsteroidManagerProperties_ECS>();
            
            AsteroidManagerAspect_ECS asteroidManagerAspect = SystemAPI.GetAspect<AsteroidManagerAspect_ECS>(asteroidManagerEntity);
            
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            
            for (int i = 0; i < asteroidManagerAspect.AsteroidStartCount; i++)
            {
                Entity entity = ecb.Instantiate(asteroidManagerAspect.AsteroidPrefab);
                ecb.SetComponent(entity, new LocalTransform
                {
                    Position =  asteroidManagerAspect.GetRandomPosition(),
                    Rotation = quaternion.identity,
                    Scale = asteroidManagerAspect.GetRandomFloat(0.5f, 100f)
                });
                ecb.SetComponent(entity, new MoveDataEcs
                {
                    Velocity = asteroidManagerAspect.GetRandomFloat3(1.0f, 100.0f),
                    AngularVelocity = asteroidManagerAspect.GetRandomRadiansFloat3(1.0f, 100.0f)
                });
            }
            
            ecb.Playback(state.EntityManager);

        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}