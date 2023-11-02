using Unity.Entities;

namespace ECS.Component
{
    public struct AsteroidManagerProperties_ECS : IComponentData
    {
        public int _asteroidStartCount;
        public float _minspawnRadius;
        public float _maxspawnRadius;
        public float _spawnRate;
        public Entity _asteroidPrefab;

        public AsteroidManagerProperties_ECS(
            int asteroidStartCount,
            float minspawnRadius, 
            float maxspawnRadius, 
            float spawnRate, 
            Entity asteroidPrefab)
        {
            _asteroidStartCount = asteroidStartCount;
            _minspawnRadius = minspawnRadius;
            _maxspawnRadius = maxspawnRadius;
            _spawnRate = spawnRate;
            _asteroidPrefab = asteroidPrefab;
        }
    }
}
