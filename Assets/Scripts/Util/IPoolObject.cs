using UnityEngine;

namespace Utils
{
   public interface IPoolObject<T> where T : MonoBehaviour, IPoolObject<T>
    {
        void Initialize(Util.ObjectPool<T> pool);
        void ReturnToPool();
    }
}