using Unity.Entities;

namespace ECS.System
{
    public partial struct AsteroidSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            
        }
    }
    
    public partial struct AsteroidSystem
    {
        public struct SystemState : ICleanupComponentData
        {
            
        }
    }
}