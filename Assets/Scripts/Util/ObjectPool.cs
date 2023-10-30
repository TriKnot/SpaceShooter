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
        public delegate void ValueChangedHandler(T value);
        public event ValueChangedHandler OnValueChanged;

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
                CreateNewObject(_prefabs[index]);
            }
        }
        
        private T CreateNewObject(T prefab)
        {
            T obj = Object.Instantiate(prefab, _parentObject.transform, true);
            obj.gameObject.SetActive(false);
            obj.InitializePoolObject(this);
            _objects.Add(obj);
            OnValueChanged?.Invoke(obj);
            return obj;
        }
        
        public void ExtendPool(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                int index = i % _prefabs.Length;
                CreateNewObject(_prefabs[index]);
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
            T newObj = CreateNewObject(_prefabs[randomIndex]);
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