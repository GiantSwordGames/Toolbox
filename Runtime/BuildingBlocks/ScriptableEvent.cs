using System;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    
    [CreateAssetMenu]

    public class ScriptableEvent : ScriptableObject
    {
        public ScriptableVariableScope _scriptableVariableScope;
        public ScriptableVariableScope scriptableVariableScope => _scriptableVariableScope;

       
        [ShowNativeProperty]  public Action onFired
        {
            get
            {
                if (Application.isPlaying)
                {
                    ScriptableEventManager.GetState(this, out ScriptableEventManager.State state);
                    return state.listeners ;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                ScriptableEventManager.GetState(this, out ScriptableEventManager.State state);
                state.listeners = value ;
            }
        }
        
     

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Fire()
        {
            onFired?.Invoke();
        }

        public void RegisterListener(Action onPlayerIsAliveChanged)
        {
            ScriptableEventManager.GetState(this, out ScriptableEventManager.State state);
            state.listeners += onPlayerIsAliveChanged;
        }
        
        public void DeregisterListener(Action onPlayerIsAliveChanged)
        {
            ScriptableEventManager.GetState(this, out ScriptableEventManager.State state);
            state.listeners -= onPlayerIsAliveChanged;
        }
    }
}