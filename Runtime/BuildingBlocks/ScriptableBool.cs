using System;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    [CreateAssetMenu]
    public class ScriptableBool : ScriptableObject
    {
        public ScriptableVariableScope _scriptableVariableScope;
        public ScriptableVariableScope scriptableVariableScope => _scriptableVariableScope;

        [SerializeField] private bool _initialValue;
        
        public bool initialValue => _initialValue;

       [ShowNativeProperty]  public bool value
        {
            get
            {
                if (Application.isPlaying)
                {
                    ScriptableBoolManager.GetState(this, out ScriptableBoolManager.State state);
                    return state.value;
                }
                else
                {
                    return initialValue;
                }
            }
            set
            {
                ScriptableBoolManager.GetState(this, out ScriptableBoolManager.State state);
                if (value.Equals( state.value) == false)
                {
                    state.value = value;
                    state.onValueChanged?.Invoke(state.value);
                }
            }
        }
       
        [ShowNativeProperty]  public Action<bool> onValueChanged
        {
            get
            {
                if (Application.isPlaying)
                {
                    ScriptableBoolManager.GetState(this, out ScriptableBoolManager.State state);
                    return state.onValueChanged ;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                ScriptableBoolManager.GetState(this, out ScriptableBoolManager.State state);
                 state.onValueChanged = value ;
            }
        }
        
        // implicit operator
        public static implicit operator bool(ScriptableBool scriptableBool)
        {
            return scriptableBool.value;
        }

        [Button]
        public void Toggle()
        {
            value = !value;
        }

        public void RegisterListener(Action<bool> onPlayerIsAliveChanged)
        {
            ScriptableBoolManager.GetState(this, out ScriptableBoolManager.State state);
            state.onValueChanged += onPlayerIsAliveChanged;
        }
        
        public void DeregisterListener(Action<bool> onPlayerIsAliveChanged)
        {
            ScriptableBoolManager.GetState(this, out ScriptableBoolManager.State state);
            state.onValueChanged -= onPlayerIsAliveChanged;
        }
    }
}