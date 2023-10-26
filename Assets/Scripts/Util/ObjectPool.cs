using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Util
{

    public class ObjectPool<T> where T : MonoBehaviour, IPoolObject<T>
    {
        private List<T> _objects = new ();
        private T[] _prefabs;
        private GameObject _parentObject;
        
        public List<T> Objects => _objects;

        public ObjectPool(T[] prefabs, int initialSize)
        {
            // Store the prefabs
            _prefabs = prefabs;
            // Create a parent object for the pool
            _parentObject = Object.Instantiate(new GameObject($"{typeof(T).Name} Pool"), Vector3.zero, Quaternion.identity);
            // Initialize the pool
            InitializePool(initialSize);
        }

        private void InitializePool(int initialSize)
        {
            for (int i = 0; i < initialSize; i++)
            {
                int index = i % _prefabs.Length;
                T obj = Object.Instantiate(_prefabs[index], _parentObject.transform, true);
                obj.gameObject.SetActive(false);
                obj.InitializePoolObject(this);
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
                    obj.transform.parent = null;
                    return obj;
                }
            }
            
            int randomIndex = Random.Range(0, _prefabs.Length);
            T newObj = Object.Instantiate(_prefabs[randomIndex]);
            newObj.InitializePoolObject(this);
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
            obj.transform.parent = _parentObject.transform;
        }
    }

}