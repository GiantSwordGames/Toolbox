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
        [SerializeField] private bool _isFalse;
        [FormerlySerializedAs("_comaparison")] [SerializeField] private Comparison _comparison = Comparison.IfTrue;

        private void OnValidate()
        {
            if (_isFalse)
            {
                _comparison = Comparison.IfFalse;
            }
            else
            {
                _comparison = Comparison.IfTrue;
            }
            
        }

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
            if (_isFalse)
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
    }
}
