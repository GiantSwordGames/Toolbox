using UnityEngine;

namespace JamKit
{
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = RuntimeEditorHelper.FindAsset<T>();
                }

                return _instance;
            }
        }
        
    }
}