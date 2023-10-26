using UnityEngine;

namespace Utils
{
   public interface IPoolObject<T> where T : MonoBehaviour, IPoolObject<T>
    {
        void InitializePoolObject(Util.ObjectPool<T> pool);
        void ReturnToPool();
    }
}