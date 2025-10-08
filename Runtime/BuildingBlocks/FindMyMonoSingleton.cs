using UnityEngine;

namespace JamKit
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
                    _instance = CompaitibilityHelper.FindObjectOfType<T>();
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
        
        public void ForceInstance()
        {
            if (_instance == null)
            {
                _instance = (T)this;
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