using System;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    
    public enum  TriggerBehavior
    {
        TriggerEveryFrame,
        OnlyOnceThisLoad,
        OnlyOnceSinceStartUp,
    }

    public class DisableTrigger : MonoBehaviour
    {

        [SerializeField]  private TriggerBehavior _triggerBehavior;

        [SerializeField] private UnityEvent onEnable;
        private bool _triggered;
        private  StaticInstanceBool _hasTriggeredStaticInstance;
        

        private void Start()
        {
            _hasTriggeredStaticInstance = new StaticInstanceBool(this, nameof(_hasTriggeredStaticInstance), false);
        }

        void OnDisable()
        {
                if (_triggerBehavior == TriggerBehavior.OnlyOnceSinceStartUp)
                {
                    if (_hasTriggeredStaticInstance.value)
                    {
                        return;
                    }
                }
                else if (_triggerBehavior == TriggerBehavior.OnlyOnceThisLoad)
                {
                    if (_triggered)
                    {
                        return;
                    }
                }
     
            
            if (Time.frameCount != 0)
            {
                _triggered = true;
                _hasTriggeredStaticInstance.value = true;
                onEnable?.Invoke();
            }
        }
    }
}