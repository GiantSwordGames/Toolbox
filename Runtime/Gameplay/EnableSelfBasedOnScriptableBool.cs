using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    public class EnableSelfBasedOnScriptableBool : MonoBehaviour
    {
        enum Comparison
        {
            IfTrue,
            IfFalse
        }
        [SerializeField] private ScriptableBool _scriptableBool;
        [SerializeField] private Comparison _comparison = Comparison.IfTrue;


        void Awake()
        {
            if (_scriptableBool != null)
            {
                _scriptableBool.onValueChanged += OnValueChanged;
            }
            Evaluate();
        }

        private void OnDestroy()
        {
            if (_scriptableBool != null)
            {
                _scriptableBool.onValueChanged -= OnValueChanged;
            }
        }

        private void Evaluate()
        {
            if(enabled == false) return;
            if (_comparison == Comparison.IfFalse)
            {
                gameObject.SetActive(_scriptableBool.value == false);
            }
            else
            {
                gameObject.SetActive(_scriptableBool.value );
            }
        }

        private void OnValueChanged(bool state)
        {
            Evaluate();
        }

        private void Start()
        {
            
        }
    }
}
