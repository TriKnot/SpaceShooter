using ECS.Component;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Aspect
{
    [BurstCompile]
    public readonly partial struct AsteroidManagerAspect_ECS : IAspect
    {
        private readonly Entity _entity;
        
        private readonly RefRO<AsteroidManagerProperties_ECS> _properties;
        private readonly RefRW<AsteroidRandom_ECS> _random;
        
        public int AsteroidStartCount => _properties.ValueRO._asteroidStartCount;
        public Entity AsteroidPrefab => _properties.ValueRO._asteroidPrefab;
        // public Entity AsteroidPrefab => _properties.ValueRO._asteroidPrefab[_random.ValueRW.Value.NextInt(0, _properties.ValueRO._asteroidPrefab.Length)];
        
        
        [BurstCompile]
        public float3 GetRandomPosition()
        {
            float3 position = _random.ValueRW.Value.NextFloat3Direction() * _random.ValueRW.Value.NextFloat(_properties.ValueRO._minspawnRadius, _properties.ValueRO._maxspawnRadius);
            return position;
        }
        
        [BurstCompile]
        public float GetRandomFloat(float min = float.MinValue, float max = float.MaxValue)
        {
            return _random.ValueRW.Value.NextFloat(min, max);
        }
        
        [BurstCompile] 
        public float3 GetRandomFloat3(float min = float.MinValue, float max = float.MaxValue)
        {
            return _random.ValueRW.Value.NextFloat3Direction() * GetRandomFloat(min, max);
        }
        
        [BurstCompile] 
        public float3 GetRandomRadiansFloat3(float min = float.MinValue, float max = float.MaxValue)
        {
            return math.radians(GetRandomFloat3(min, max));
        }
    }
}