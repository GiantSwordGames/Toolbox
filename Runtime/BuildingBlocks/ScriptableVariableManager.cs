using UnityEngine;
using UnityEngine.SceneManagement;

namespace GiantSword
{
    public static  class ScriptableVariableManager 
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            ScriptableBoolManager boolManager = ScriptableBoolManager.instance;
            ScriptableFloatManager floatManager = ScriptableFloatManager.instance;
        }

        // reset on unload so that value are correct on Awake
        private static void OnSceneUnloaded(Scene arg0)
        {
            ResetAll(ScriptableVariableScope.Scene);
        }

        public static void ResetAll(ScriptableVariableScope scriptableVariableScope)
        {
            ScriptableBoolManager.ResetAll(scriptableVariableScope);
            ScriptableFloatManager.ResetAll(scriptableVariableScope);
            ScriptableEventManager.ResetAll(scriptableVariableScope);
        }   
    }
}