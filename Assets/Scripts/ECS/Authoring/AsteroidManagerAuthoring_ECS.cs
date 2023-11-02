using ECS.Component;
using ScriptableObjects.Variables;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace ECS.Authoring
{
    [BurstCompile]
    public class AsteroidManagerAuthoring_ECS : MonoBehaviour
    {
        [Header("Setup Asteroids")]
        [SerializeField] private AsteroidAuthoringArraySO _asteroidPrefab;
        [SerializeField] private IntVariableSO _initialAsteroidCountSO;
        [SerializeField] private FloatVariableSO _maxspawnRadiusSO;
        [SerializeField] private FloatVariableSO _minSpawnDistanceSO;
        [SerializeField] private FloatVariableSO _spawnRateSO;
        [SerializeField] private TransformVariableSO _playerTransformSO;
        
        private uint _randomSeed;

        public class AsteroidManagerBaker : Baker<AsteroidManagerAuthoring_ECS>
        {
            public override void Bake(AsteroidManagerAuthoring_ECS authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                NativeArray<Entity> asteroidPrefabs = new NativeArray<Entity>(authoring._asteroidPrefab.Value.Length, Allocator.Temp);
                for (int i = 0; i < authoring._asteroidPrefab.Value.Length; i++)
                {
                    asteroidPrefabs[i] = GetEntity(authoring._asteroidPrefab.Value[i], TransformUsageFlags.Dynamic);
                }
                
                AddComponent(entity, new AsteroidManagerProperties_ECS(
                    authoring._initialAsteroidCountSO.Value,
                    authoring._minSpawnDistanceSO.Value,
                    authoring._maxspawnRadiusSO.Value,
                    authoring._spawnRateSO.Value,
                    asteroidPrefabs[0]
                    ));

                AddComponent(entity, new AsteroidRandom_ECS
                {
                    Value = Random.CreateFromIndex(authoring._randomSeed)
                });
            }
        }

    }
}