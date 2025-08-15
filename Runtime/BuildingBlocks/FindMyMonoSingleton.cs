using UnityEngine;

namespace GiantSword
{
    public class FindMyMonoSingleton<T> : MonoBehaviour where T : FindMyMonoSingleton<T>
    {
        private static T _instance;

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
                    }
                }
                
                return _instance;
            }
        }

        public static bool HasInstance()
        {
            return _instance != null;
        }

        protected virtual void OnEnable()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
        }
    }
}