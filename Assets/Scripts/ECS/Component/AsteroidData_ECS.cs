using Unity.Entities;

namespace ECS.Component
{
    public struct AsteroidDataEcs : IComponentData
    {
        public readonly float ScaleMultiplier;
        public readonly float Mass;
        public bool IsPiece;
        
        public AsteroidDataEcs(float scaleMultiplier, bool isPiece = false)
        {
            ScaleMultiplier = scaleMultiplier;
            Mass = UnityEngine.Mathf.Pow(ScaleMultiplier, 3);
            IsPiece = isPiece;
        }
    }
}
