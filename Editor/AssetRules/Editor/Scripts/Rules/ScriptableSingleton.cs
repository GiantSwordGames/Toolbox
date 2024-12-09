using System;
using UnityEditor;
using UnityEngine;

namespace RichardPieterse.AssetRules
{
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        protected static object _lockState = new object();
        public static Type InstanceType;
        private static T _instance;
        public static T instance
        {
            get
            {
                lock(_lockState)
                {
                    if(_instance == null)
                    {
#if UNITY_EDITOR
                        string[] guids = AssetDatabase.FindAssets(nameof(T));
                        foreach(string guid in guids)
                        {
                            string path = AssetDatabase.GUIDToAssetPath(guid);
                            T inst = AssetDatabase.LoadAssetAtPath<T>(path);
                            if(inst != null)
                            {
                                _instance = inst;
                                InstanceType = typeof(T);
                                return _instance;
                            }
                        }
#endif
                        string resourcesPath = "Singleton";
                        T[] assetInstances = Resources.LoadAll<T>(resourcesPath);
                        if(assetInstances == null || assetInstances.Length == 0)
                        {
                            throw new Exception($"Could not find any ScriptableSingleton<{typeof(T)}> in Resources/{resourcesPath}.");
                        }
                        else if(assetInstances.Length > 1)
                        {
                            Debug.LogWarning($"{typeof(ScriptableSingleton<T>).ToString()} There are multiple instances found of {typeof(T)} .");
                        }
    
                        _instance = assetInstances[0];
                    }
    
                    return _instance;
                }
            }
        }
    }
}