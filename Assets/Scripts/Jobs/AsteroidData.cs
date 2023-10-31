using UnityEngine;

namespace Jobs
{
    public struct AsteroidData
    {
        public float ScaleMultiplier;
        public float Mass;
        public bool IsPiece;
        
        public AsteroidData(float scaleMultiplier, bool isPiece = false)
        {
            ScaleMultiplier = scaleMultiplier;
            Mass = Mathf.Pow(ScaleMultiplier, 3);
            IsPiece = isPiece;
        }
        
    }
}