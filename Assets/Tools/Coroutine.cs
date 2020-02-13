using System;
using System.Collections;
using UnityEngine;

namespace Tools
{
    
    public delegate bool Condition<in T>(T time);
    public class Coroutine : MonoBehaviour
    {
        private WaitForFixedUpdate _waitFixed = new WaitForFixedUpdate();
        private WaitForEndOfFrame  _waitFrame = new WaitForEndOfFrame();


        ///<summary>
        /// Calls Coroutine that runs in every fixed Frame.
        /// </summary>
        public void CoroutineFixed(Condition<float> condition,Action action)
        {
            StartCoroutine(FixedCoroutineHandler(condition,action));
        }
        ///<summary>
        /// Calls Coroutine that runs in every frame
        /// </summary>
        public void CoroutineDeltaFrame(Condition<float> condition,Action action)
        {
            StartCoroutine(FrameCoroutineHandler(condition,action));
        }
        ///<summary>
        /// Calls Coroutine that runs in every fixed Frame.
        /// for given time.
        /// </summary>
        public void CoroutineFixedWithWaitTime(float time,Action action)
        {
            StartCoroutine(FixedCoroutineHandler(time,action));
        }
        ///<summary>
        /// Calls Coroutine that runs in every Frame
        /// for given time.
        /// </summary>
        public void CoroutineWithWaitTime(float time,Action action)
        {
            StartCoroutine(FrameCoroutineHandler(time,action));
        }
        ///<summary>
        /// Wait a time and   Calls Coroutine that runs in every frame.
        /// </summary>
        public void CoroutineWaitSeconds(float time,Condition<float> condition,Action action)
        {
            StartCoroutine(WaitSecondsCoroutineHandler(time,condition,action));
        }

        private IEnumerator WaitSecondsCoroutineHandler(float time, Condition<float> condition, Action action)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(time);
            float timeElapsed = 0f;
            while (condition(timeElapsed))
            {
               
                action?.Invoke();
                timeElapsed += time;
                yield return waitForSeconds;
            }
        }

        private IEnumerator FixedCoroutineHandler(Condition<float> condition, Action action)
        {
            float time = 0f;
            while (condition(time))
            {
               
                action?.Invoke();
                time += Time.fixedDeltaTime;
                yield return _waitFixed;
            }
        }
        private IEnumerator FixedCoroutineHandler(float time, Action action)
        {
            float timeElapsed = 0f;
            while (time>=timeElapsed)
            {
               
                action?.Invoke();
                timeElapsed += Time.fixedDeltaTime;
                yield return _waitFixed;
            }
        }
        private IEnumerator FrameCoroutineHandler(Condition<float> condition, Action action)
        {
            float time = 0f;
            while (condition(time))
            {
               
                action?.Invoke();
                time += Time.fixedDeltaTime;
                yield return _waitFrame;
            }
        }
        private IEnumerator FrameCoroutineHandler(float time, Action action)
        {
            float timeElapsed = 0f;
            while (time>=timeElapsed)
            {
               
                action?.Invoke();
                timeElapsed += Time.fixedDeltaTime;
                yield return _waitFrame;
            }
        }
       



        

      
    }
}
