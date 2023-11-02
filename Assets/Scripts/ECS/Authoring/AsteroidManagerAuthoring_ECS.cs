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
        [SerializeField] private AsteroidAuthoringArraySO _asteroidPrefabsSO;
        [SerializeField] private AsteroidAuthoringArraySO _cubeAsteroidPrefabsSO;
        [SerializeField] private BoolVariableSO _useCubeMeshSO;
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
                AsteroidAuthoring_ECS[] entityArray = authoring._useCubeMeshSO.Value ?
                    authoring._cubeAsteroidPrefabsSO.Value : authoring._asteroidPrefabsSO.Value;
                NativeArray<Entity> asteroidPrefabs = new NativeArray<Entity>(entityArray.Length, Allocator.Temp);
                for (int i = 0; i < entityArray.Length; i++)
                {
                    asteroidPrefabs[i] = GetEntity(entityArray[i], TransformUsageFlags.Dynamic);
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