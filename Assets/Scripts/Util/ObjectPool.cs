using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Util
{

    public class ObjectPool<T> where T : MonoBehaviour, IPoolObject<T>
    {
        private List<T> _objects = new ();
        private T[] _prefabs;
        
        public List<T> Objects => _objects;

        public ObjectPool(T[] prefabs, int initialSize)
        {
            _prefabs = prefabs;
            InitializePool(initialSize);
        }

        private void InitializePool(int initialSize)
        {
            for (int i = 0; i < initialSize; i++)
            {
                int index = i % _prefabs.Length;
                T obj = Object.Instantiate(_prefabs[index]);
                obj.gameObject.SetActive(false);
                obj.InitializePool(this);
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
            newObj.InitializePool(this);
            _objects.Add(newObj);
            return newObj;
        }
        
        public void Add(T obj)
        {
            _objects.Add(obj);
        }
        
        public void AddRange(T[] objs)
        {
            _objects.AddRange(objs);
        }
        
        public void Remove(T obj)
        {
            _objects.Remove(obj);
        }
        
        public void RemoveRange(T[] objs)
        {
            _objects.RemoveRange(0, objs.Length);
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
        }
    }

}