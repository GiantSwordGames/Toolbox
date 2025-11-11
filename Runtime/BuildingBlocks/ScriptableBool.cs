using System;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public abstract class ScriptablePrimitive : ScriptableObject
    {
        [DisableSerializedField] [SerializeField] protected bool _savable;

        public abstract void Save();
        public abstract void Load();
    }
    
    
    public class ScriptableBool : ScriptablePrimitive
    {
        public ScriptableVariableScope _scriptableVariableScope = ScriptableVariableScope.Application;
        public ScriptableVariableScope scriptableVariableScope => _scriptableVariableScope;

        [SerializeField] private bool _initialValue;
        
        public bool initialValue
        {
            get => _initialValue;
            set
            {
                _initialValue = value;
                RuntimeEditorHelper.SetDirty(this);
            }
        }

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
                if(!Application.isPlaying)
                {
                    return;
                }
                ScriptableBoolManager.GetState(this, out ScriptableBoolManager.State state);
                if (value.Equals( state.value) == false)
                {
                        
                    state.value = value;

                    try
                    {
                        state.onValueChanged?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, this);
                    }
                }
            }
        }
       
        [ShowNativeProperty]  public Action onValueChanged
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
        
        [Button]
        public void ToggleTrue()
        {
            value = true;
        }
        
        [Button]
        public void ToggleFalse()
        {
            value = false;
        }
        
        [Button]
        public void SeeAllBools()
        {
            // ScriptableBoolViewer.ShowWindow();
        }


        public void ResetToDefaultValue()
        {
            value = initialValue;
        }

        public void RegisterListener(Action onPlayerIsAliveChanged)
        {
            ScriptableBoolManager.GetState(this, out ScriptableBoolManager.State state);
            state.onValueChanged += onPlayerIsAliveChanged;
        }
        
        public void DeregisterListener(Action onPlayerIsAliveChanged)
        {
            ScriptableBoolManager.GetState(this, out ScriptableBoolManager.State state);
            state.onValueChanged -= onPlayerIsAliveChanged;
        }
        
        [Button]
        public override void Save()
        {
            SaveService.SaveBool(name, value);
        }
        
        [Button]
        public override void Load()
        {

            // PlayerPrefs.s
            if (SaveService.HasKey(name))
            {
                value = SaveService.GetBool(name, value);
            }
        }
    }
}
