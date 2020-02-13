using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Tools
{
    public class ToolManager : Singleton<ToolManager>
    {

        public BoundCalculator boundCalculator;
        public Coroutine coroutine;
        public MeshOperation meshOperation;
        public ObjectPool objectPool;
        public VectorOperation vectorOperation;


        private static readonly Type BoundType = typeof(BoundCalculator);
        private static readonly Type CoroutineType = typeof(Coroutine);
        private static readonly Type MeshOperationType = typeof(MeshOperation);
        private static readonly Type ObjectPoolType = typeof(ObjectPool);
        private static readonly Type VectorOperationType = typeof(VectorOperation);


        public T GetTool<T>() where T : Component
        {
            Type type = typeof(T);
            Component neededClass = FindScript(type);
            if (neededClass != null)
                return neededClass as T;
            T obj = gameObject.AddComponent<T>(); 
            SetClass(obj,type);
            return obj;
        }

        public T GetToolComponent<T>(GameObject gameObject) where T : Component
        {
            Type type = typeof(T);
            Component neededClass = FindScript(type);
            T obj =  gameObject.AddComponent<T>(); 
            return obj;
        }

        private Component FindScript(Type type)
          {
              if (type == BoundType)
              {
                  if (boundCalculator == null)
                      return null;
                  return boundCalculator;
              }
              else if (type == CoroutineType) 
              {
                  if (coroutine == null)
                      return null;
                  return coroutine ;
              }
              else if (type == MeshOperationType) 
              {
                  if (meshOperation == null)
                      return null;
                  return meshOperation;
              }
              else if (type == ObjectPoolType) 
              {
                  if (objectPool == null)
                      return null;
                  return objectPool;
              }
              else if (type == VectorOperationType) 
              {
                  if (vectorOperation == null)
                      return null;
                  return vectorOperation;
              }

              return null;
          }

        private void SetClass<T>(T cls, Type type)
        {
            if (type == BoundType)
            {
                boundCalculator = cls as BoundCalculator;
            }
            else if (type == CoroutineType)
            {
                coroutine = cls as Coroutine;
            }
            else if (type == MeshOperationType)
            {
                meshOperation = cls as MeshOperation;
            }
            else if (type == ObjectPoolType)
            {
                objectPool = cls as ObjectPool;
            }
            else if (type == VectorOperationType)
            {
                vectorOperation = cls as VectorOperation;
            }
        }
    }

    
}

