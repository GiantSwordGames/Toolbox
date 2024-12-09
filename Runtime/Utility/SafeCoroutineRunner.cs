using System.Collections;
using UnityEngine;

namespace GiantSword
{
    public class SafeCoroutineRunner : MonoBehaviour
    {
        private static SafeCoroutineRunner _instance;
        public static SafeCoroutineRunner instance
        {
            get
            {
                if (_instance == null && Application.isPlaying)
                {
                    _instance = new GameObject("CoroutineRunner").AddComponent<SafeCoroutineRunner>();
                    _instance.hideFlags = HideFlags.HideAndDontSave;
                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        public new static Coroutine StartCoroutine(IEnumerator routine)
        {
            MonoBehaviour monoBehaviour = instance;
            return monoBehaviour.StartCoroutine(routine);
        }
        
        public new static void StopCoroutine(Coroutine routine)
        {
            MonoBehaviour monoBehaviour = instance;
             monoBehaviour.StopCoroutine(routine);
        }
        
    }
}