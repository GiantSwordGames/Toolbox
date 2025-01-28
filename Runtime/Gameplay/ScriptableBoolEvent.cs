using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class ScriptableBoolEvent : MonoBehaviour
    {
        [SerializeField] private ScriptableBool _scriptableBool;
        public UnityEvent onTrue;
        public UnityEvent onFalse;
        
        void Awake()
        {
            _scriptableBool.onValueChanged += (f) => Trigger();
            Trigger();
        }
        
        [Button]
        public void Trigger()
        {
            if (_scriptableBool.value)
            {
                Debug.Log("OnTrue");
                onTrue.Invoke();
            }
            else
            {
                Debug.Log("False");
                onFalse.Invoke();
            }
        }
    }
}