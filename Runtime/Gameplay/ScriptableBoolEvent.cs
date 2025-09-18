using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class ScriptableBoolEvent : MonoBehaviour
    {
        [SerializeField] private ScriptableBool _scriptableBool;
        public UnityEvent onTrue;
        public UnityEvent onFalse;
        
        void Awake()
        {
            _scriptableBool.onValueChanged += Trigger;
            Trigger();
        }

        private void Trigger(bool obj)
        {
            Trigger();
        }

        void OnDestroy()
        {
            _scriptableBool.onValueChanged -=Trigger;
        }

        [Button]
        public void Trigger()
        {
            if (_scriptableBool.value)
            {
                onTrue.Invoke();
            }
            else
            {
                onFalse.Invoke();
            }
        }
    }
}