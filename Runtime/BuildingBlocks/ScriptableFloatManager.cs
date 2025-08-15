using System;
using System.Collections.Generic;
using UnityEngine;

namespace JamKit
{
    public class ScriptableFloatManager : MonoBehaviour
    {
        public class State
        {
            public float value;
            public Action<float> onValueChanged;

            public State(float initialValue)
            {
                value = initialValue;
            }
        }

        private Dictionary<ScriptableFloat, State> _scriptableFloats = new Dictionary<ScriptableFloat, State>();
        private static ScriptableFloatManager _instance;

        public  static ScriptableFloatManager instance
        {
            get
            {
                if (_instance == null)
                {
                    if (Application.isPlaying)
                    {
                        _instance = new GameObject("ScriptableFloatManager").AddComponent<ScriptableFloatManager>();
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }

                return _instance;
            }
            
        }

        public static void GetState(ScriptableFloat scriptableFloat, out State state)
        {
            if (instance._scriptableFloats.TryGetValue(scriptableFloat, out state) == false)
            {
                state = new State(scriptableFloat.initialValue);
                instance._scriptableFloats.Add(scriptableFloat, state);
            }
        }

        public static void SetValue(ScriptableFloat scriptableFloat, float value)
        {
            if (!instance._scriptableFloats.ContainsKey(scriptableFloat))
            {
                instance._scriptableFloats.Add(scriptableFloat, new State(scriptableFloat.initialValue));
            }

            instance._scriptableFloats[scriptableFloat].value = value;
        }

        public static void ResetAll(ScriptableVariableScope scriptableVariableScope)
        {
            var keysToReset = new List<ScriptableFloat>();

            foreach (var variable in instance._scriptableFloats)
            {
                if (variable.Key.scriptableVariableScope == scriptableVariableScope)
                {
                    keysToReset.Add(variable.Key);
                }
            }

            foreach (var key in keysToReset)
            {
                instance._scriptableFloats[key] = new State(key.initialValue);
            }
        }

    }
}