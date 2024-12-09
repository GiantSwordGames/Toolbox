using UnityEngine;
using UnityEngine.SceneManagement;

namespace GiantSword
{
    public static  class ScriptableVariableManager 
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            ScriptableBoolManager boolManager = ScriptableBoolManager.instance;
            ScriptableFloatManager floatManager = ScriptableFloatManager.instance;
        }

        private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            ResetAll(ScriptableVariableScope.Scene);
        }

        public static void ResetAll(ScriptableVariableScope scriptableVariableScope)
        {
            ScriptableBoolManager.ResetAll(scriptableVariableScope);
            ScriptableFloatManager.ResetAll(scriptableVariableScope);
        }   
    }
}