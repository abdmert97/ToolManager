using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class Singleton<T> : MonoBehaviour where T:MonoBehaviour
    {
        public static T Instance { get; set; }


        protected virtual void Awake()
        {
            if (Instance != null && Instance != this as T)
            {
                Destroy(gameObject);
                return;
        
            }

            Instance = this as T;
        }

    }

}
