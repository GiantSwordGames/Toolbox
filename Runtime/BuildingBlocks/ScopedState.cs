using System;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class ScopedEventBase
    {
        
    }
    public class ScopedState<T> :ScopedEventBase
    {
        private T _initialValue;

        [ShowNativeProperty]  public T value
        {
            get
            {
                if (Application.isPlaying)
                {
                    ScopedEventManager.GetState(this, out ScopedEventManager.State<T> state);
                    return state.value;
                }
                else
                {
                    return default;
                }
            }
            set
            {
                ScopedEventManager.GetState(this, out ScopedEventManager.State<T> state);
                if (value.Equals( state.value) == false)
                {
                    state.value = value;
                }
            }
        }
        
        // implicit operator
        public static implicit operator T(ScopedState<T> scopedState)
        {
            return scopedState.value;
        }
    }
}