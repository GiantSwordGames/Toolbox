using UnityEngine;

namespace GiantSword
{
    public class SimpleMonoSingleton<T> : MonoBehaviour where T : SimpleMonoSingleton<T>
    {
        public static T _instance;

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(T) + " is needed in the scene, but there is none.");
                    }
                }
                
                return _instance;
            }
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