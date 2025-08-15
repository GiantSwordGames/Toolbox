using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public  class ScriptableEventManager: MonoBehaviour
    {
        
        public class State
        {
            public Action listeners;

            public State()
            {
                listeners = null;
            }
        }
        public static ScriptableEventManager _instance;

        public  static ScriptableEventManager instance
        {
            get
            {
                if (_instance == null)
                {
                    if (Application.isPlaying)
                    {
                        _instance = new GameObject("ScriptableEventManager").AddComponent<ScriptableEventManager>();
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }

                return _instance;
            }
            
        }

        [ShowNativeProperty]  private int count => _scriptableEvents.Count;
        
        private  Dictionary<ScriptableEvent, State> _scriptableEvents = new Dictionary<ScriptableEvent, State>();
        
        
        public static void ResetAll(ScriptableVariableScope scriptableVariableScope)
        {
            
            var keysToReset = new List<ScriptableEvent>();

            foreach (var variable in instance._scriptableEvents)
            {
                if (variable.Key.scriptableVariableScope == scriptableVariableScope || scriptableVariableScope == ScriptableVariableScope.Application)
                {
                    keysToReset.Add(variable.Key);
                }
            }

            foreach (var key in keysToReset)
            {
                Debug.Log("Reset Key " + key, key);

                instance._scriptableEvents[key] = new State();
            }
        }

        public static void GetState(ScriptableEvent scriptableEvent, out State b)
        {
            if (instance._scriptableEvents.ContainsKey(scriptableEvent) == false)
            {
                instance._scriptableEvents.Add(scriptableEvent, new State(  ));
            }
            instance._scriptableEvents.TryGetValue(scriptableEvent, out b);
        }

        [Button]
        private bool LogRegisteredBools()
        {
            foreach (var variable in _scriptableEvents)
            {
                Debug.Log($"Registered Bool {variable.Key} {variable.Value.listeners.CountRegisteredListeners()}", variable.Key);
            }

            return true;
            
        }
    }
}