using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class ObjectPool : MonoBehaviour
    {
        public enum PoolType
        {
            Dynamic,
            Static
        }
    
        public PoolType poolType = PoolType.Dynamic;
        public int poolSize = 0;
        public GameObject poolObject;
        private Stack<GameObject> _pool;
        private Transform _poolParent;
       
        private void Awake()
        {
         
            _poolParent = new GameObject("Pool").transform;
            _pool = new Stack<GameObject>();
            if (poolType != PoolType.Static) return;
            for (int i = 0; i < poolSize; i++)
            {
                var obj =  Instantiate(poolObject,_poolParent);
                obj.SetActive(false);
                _pool.Push(obj);
            }
        }

        public GameObject GetObjectFromPool()
        {
      
            if (_pool.Count!=0)
            {
                var obj = _pool.Pop();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                return Instantiate(poolObject,_poolParent);
            }
        }

        public void SendToPool(GameObject obj,bool resetTransform = true)
        {
            obj.SetActive(false);
            _pool.Push(obj);
            obj.transform.SetParent(_poolParent);
            if (!resetTransform) return;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }

    
        
    
    }
}
