using System;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
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
        
        [ShowNativeProperty]  public int lastFrameFired 
        {
            get
            {
                if (Application.isPlaying)
                {
                    ScriptableEventManager.GetState(this, out ScriptableEventManager.State state);
                    return state.lastFrameFired ;
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                ScriptableEventManager.GetState(this, out ScriptableEventManager.State state);
                state.lastFrameFired = value ;
            }
        }


        public void Fire()
        {
            lastFrameFired = Time.frameCount;
            onFired?.Invoke();
        }
        
        public bool WasFiredThisFrame()
        {
            return lastFrameFired == Time.frameCount;
        }
        
        public bool WasFiredLastFrame()
        {
            return lastFrameFired == Time.frameCount -1;
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