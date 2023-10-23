using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Util
{

    public class ObjectPool<T> where T : MonoBehaviour, IPoolObject<T>
    {
        private List<T> _objects = new ();
        private T[] _prefabs;

        public ObjectPool(T[] prefabs, int initialSize)
        {
            this._prefabs = prefabs;
            InitializePool(initialSize);
        }

        private void InitializePool(int initialSize)
        {
            for (int i = 0; i < initialSize; i++)
            {
                int index = i % _prefabs.Length;
                T obj = Object.Instantiate(_prefabs[index]);
                obj.gameObject.SetActive(false);
                obj.Initialize(this);
                _objects.Add(obj);
            }
        }

        public T Get()
        {
            foreach (T obj in _objects)
            {
                if (!obj.gameObject.activeSelf)
                {
                    obj.gameObject.SetActive(true);
                    return obj;
                }
            }
            
            int randomIndex = Random.Range(0, _prefabs.Length);
            T newObj = Object.Instantiate(_prefabs[randomIndex]);
            newObj.Initialize(this);
            _objects.Add(newObj);
            return newObj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
        }
    }

}