using System;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public abstract class ScriptablePrimitive : ScriptableObject
    {
        [SerializeField] protected bool _savable;

        public abstract void Save();
        public abstract void Load();
    }
    
    [CreateAssetMenu(menuName = MenuPaths.CREATE_ASSET_MENU + "/Scriptable Bool")]

    public class ScriptableBool : ScriptablePrimitive
    {
        public ScriptableVariableScope _scriptableVariableScope;
        public ScriptableVariableScope scriptableVariableScope => _scriptableVariableScope;

        [SerializeField] private bool _initialValue;
        
        public bool initialValue
        {
            get => _initialValue;
            set => _initialValue = value;
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
