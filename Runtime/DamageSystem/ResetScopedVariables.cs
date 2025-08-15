    using System;
    using System.Collections.Generic;
    using NaughtyAttributes;
    using UnityEngine;

namespace JamKit
{
   
    public class ResetScopedVariables : MonoBehaviour
    {
        [SerializeField] private ScriptableVariableScope _scope;
        [SerializeField] private bool _triggerOnEnable;

        private void OnEnable()
        {
            if (_triggerOnEnable)
            {
                Trigger();
            }
        }

        [Button]
        public void Trigger()
        {
            ScriptableFloatManager.ResetAll(_scope);
        }
    }
}