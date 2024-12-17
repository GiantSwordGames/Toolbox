using System;
using System.Collections.Generic;
using UnityEngine;

namespace GiantSword
{
    public  class ScopedEventManager: MonoBehaviour
    {
        public class StateBase
        {
            
        }
        public class State<T> :StateBase
        {
            public T value;

            public State( )
            {
                value = default;
            }

            public State(T action)
            {
                value = action;
            }
        }
        public static ScopedEventManager _instance;

        public  static ScopedEventManager instance
        {
            get
            {
                if (_instance == null)
                {
                    if (Application.isPlaying)
                    {
                        _instance = new GameObject("ScopedEventManager").AddComponent<ScopedEventManager>();
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }

                return _instance;
            }
            
        }
        
        private  Dictionary<ScopedEventBase, StateBase> _states = new Dictionary<ScopedEventBase, StateBase>();
        

        public static void GetState<T>(ScopedState<T> scopedState, out State<T> b)
        {
            if (instance._states.ContainsKey(scopedState) == false)
            {
                instance._states.Add(scopedState, new State<T>( ));
            }

            StateBase result;
            instance._states.TryGetValue(scopedState, out result);

            b = result as State<T>;
        }

        public static void SetValue<T>(ScopedState<T> scopedState, T value)
        {
            if (instance._states.ContainsKey(scopedState) == false)
            {
                instance._states.Add(scopedState, new State<T>(value));
            }

            ScopedState<T> state = instance._states[scopedState] as ScopedState<T> ;
            state.value = value;
        }
    }
}