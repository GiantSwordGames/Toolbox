using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    
    public enum ScriptableVariableScope
    {
        Scene,
        Application,
        PlayerDied,
        ReturnToMenu,
        BeginRun,
        Undefined,
    }

    public  class ScriptableBoolManager: MonoBehaviour
    {
        
        public class State
        {
            public bool value;
            public Action<bool> onValueChanged;

            public State(bool initialValue)
            {
                value = initialValue;
                onValueChanged = null;
            }
        }
        public static ScriptableBoolManager _instance;

        public  static ScriptableBoolManager instance
        {
            get
            {
                if (_instance == null)
                {
                    if (Application.isPlaying)
                    {
                        _instance = new GameObject("ScriptableBoolManager").AddComponent<ScriptableBoolManager>();
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }

                return _instance;
            }
            
        }

       [ShowNativeProperty]  private int count => _scriptableBools.Count;
        
        private  Dictionary<ScriptableBool, State> _scriptableBools = new Dictionary<ScriptableBool, State>();
        
        
        public static void ResetAll(ScriptableVariableScope scriptableVariableScope)
        {
            
            var keysToReset = new List<ScriptableBool>();

            foreach (var variable in instance._scriptableBools)
            {
                if (variable.Key.scriptableVariableScope == scriptableVariableScope || scriptableVariableScope == ScriptableVariableScope.Application)
                {
                    keysToReset.Add(variable.Key);
                }
            }

            foreach (var key in keysToReset)
            {
                instance._scriptableBools[key] = new State(key.initialValue);
            }
        }

        public static void GetState(ScriptableBool scriptableBool, out State b)
        {
            if (instance._scriptableBools.ContainsKey(scriptableBool) == false)
            {
                instance._scriptableBools.Add(scriptableBool, new State( scriptableBool.initialValue ));
            }
            instance._scriptableBools.TryGetValue(scriptableBool, out b);
        }

        public static void SetValue(ScriptableBool scriptableBool, bool value)
        {
            if (instance._scriptableBools.ContainsKey(scriptableBool) == false)
            {
                instance._scriptableBools.Add(scriptableBool, new State(scriptableBool.initialValue));
            }

            instance._scriptableBools[scriptableBool].value = value;
        }

        [Button]
        private bool LogRegisteredBools()
        {
            foreach (var variable in _scriptableBools)
            {
                Debug.Log($"Registered Bool {variable.Key} {variable.Value.value} {variable.Value.onValueChanged.CountRegisteredListeners()}", variable.Key);
            }

            return true;
            
        }
    }
}