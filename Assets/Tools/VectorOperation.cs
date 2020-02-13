using System;
using UnityEngine;

namespace Tools
{
   
    public class VectorOperation : MonoBehaviour
    {

        private Coroutine coroutine;


        private void Awake()
        {
        
            coroutine = ToolManager.Instance.GetTool<Coroutine>();
            if (coroutine != null)
            {
                Debug.Log("I worked");                
            }
        }


        // vectorOperation.SmoothVectorLerp(transform.position,new Vector3(0,0,Random.Range(-10,10)),.5f,false,SetPosition );
        public void ExponentialVectorLerp(Vector3 vector1,Vector3 vector2,float speed,bool isFixed,Action<Vector3> action)
        {
            if (isFixed)
            {
                coroutine.CoroutineFixed(t =>
                    {
                        vector1 = Vector3.Lerp(vector1, vector2, t*speed);
                        action?.Invoke(vector1);
                       
                        return vector1 != vector2;
                    },
                    null);
            }
            else
            {
                coroutine.CoroutineDeltaFrame(t =>
                    {
                        vector1 = Vector3.Lerp(vector1, vector2, t*speed);
                        action?.Invoke(vector1);
                        
                        return vector1 != vector2;
                    },
                    null);
            }
        }
        public void SmoothVectorLerp(Vector3 vector1,Vector3 vector2,float speed,bool isFixed,Action<Vector3> action)
        {
            float distance =  Vector3.Distance(vector1,vector2);
            Vector3 distanceVector = (vector2 - vector1).normalized;
            var fixMove = Time.fixedDeltaTime*speed*distanceVector;
            if (isFixed)
            {
               
                coroutine.CoroutineFixed(t =>
                    {
                        var condition = CompareVector3(vector1, vector2, fixMove);
                        vector1 = condition ? vector2 : vector1+fixMove;
                        action?.Invoke(vector1);
                        return !condition;
                    },
                    null);
            }
            else
            {
               
                coroutine.CoroutineDeltaFrame(t =>
                    {
                        var condition = CompareVector3(vector1, vector2, fixMove);
                        vector1 = condition ? vector2 : vector1+fixMove;
                        action?.Invoke(vector1);
                        return !condition;
                    },
                    null);
            }
        }

        private bool CompareVector3(Vector3 v1, Vector3 v2,Vector3 distanceVector)
        {
            if (Math.Abs(v1.x - v2.x) >Mathf.Abs(distanceVector.x))
                return false;
            if (Math.Abs(v1.y - v2.y) >Mathf.Abs(distanceVector.y))
                return false;
            if (Math.Abs(v1.z - v2.z) >Mathf.Abs(distanceVector.z))
                return false;

            return true;
        }
    }
}
