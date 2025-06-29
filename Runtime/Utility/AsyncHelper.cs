using System;
using System.Collections;
using UnityEngine;

namespace GiantSword
{
    public static class AsyncHelper
    {

        public static Action onUpdate
        {
            get => SafeCoroutineRunner.instance.onUpdate;
            set => SafeCoroutineRunner.instance.onUpdate = value;
        }

        public static void DelayByFrame(Action action)
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
            return SafeCoroutineRunner.StartCoroutine(IEDelay(action, delay, false));
        }
        public static Coroutine DelayUnscaled(float delay, Action action)
        {
            return SafeCoroutineRunner.StartCoroutine(IEDelay(action, delay, true));
        }

        private static IEnumerator IEDelay(Action action, float delay, bool unscaledTime = false)
        {
            if (delay == 0)
            {
                action();
            }
            else
            {
                if (unscaledTime)
                {
                    yield return new WaitForSecondsRealtime(delay);
                }
                else
                {
                    yield return new WaitForSeconds(delay);
                }
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

        public static void Lerp( float duration, Action<float> lerpFunction)
        {
            SafeCoroutineRunner.StartCoroutine(IELerp(duration, lerpFunction));
        }
        
        private static IEnumerator IELerp( float duration, Action<float> lerpFunction)
        {
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float lerp = timer / duration;
                lerp = Mathf.Clamp01(lerp);
                lerpFunction(lerp);
                yield return null;
            }
            lerpFunction(1);
        }
    }
}