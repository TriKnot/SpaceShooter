using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Component
{
    public struct AsteroidRandom_ECS : IComponentData
    {
        public Random Value;
    }
}