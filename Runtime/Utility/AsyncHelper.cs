using System;
using System.Collections;
using UnityEngine;

namespace GiantSword
{
    public static class AsyncHelper
    {

        public static void WaitForFrame(Action action)
        {
            SafeCoroutineRunner.StartCoroutine(IEWaitForFrame(action));
        }

        private static IEnumerator IEWaitForFrame(Action action)
        {
            yield return null;
            action();
        }

        public static Coroutine Delay(float delay, Action action)
        {
           return SafeCoroutineRunner.StartCoroutine(IEDelay(action, delay));
        }

        private static IEnumerator IEDelay(Action action, float delay)
        {
            if (delay == 0)
            {
                action();
            }
            else
            {
                yield return new WaitForSeconds(delay);
                action();
            }
        }

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return SafeCoroutineRunner.StartCoroutine(routine);
        }
       
        public static void StopRoutine(Coroutine routine)
        { 
            SafeCoroutineRunner.StopCoroutine(routine);
        }

        public static Coroutine WaitForClick()
        {
            return StartCoroutine(IEWaitForClick());
        }
        private static IEnumerator IEWaitForClick()
        {
            yield return null;
            while (Input.GetMouseButtonDown(0) == false)
            {
                yield return null;
            }
        }

        public static void InvokeOnCoroutineComplete(Coroutine coroutine, Action action)
        {
            StartCoroutine(IEInvokeOnCoroutineComplete(coroutine, action));
        }
        
        private static IEnumerator IEInvokeOnCoroutineComplete(Coroutine coroutine, Action action)
        {
            if (coroutine != null)
                yield return coroutine;

            action?.Invoke();
        }
    }
}