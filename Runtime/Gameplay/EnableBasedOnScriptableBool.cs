using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JamKit
{
    public class EnableBasedOnScriptableBool : MonoBehaviour
    {
        enum Comparison
        {
            IfTrue,
            IfFalse
        }
        [SerializeField] private ScriptableBool _scriptableBool;
        [SerializeField] private Comparison _comparison = Comparison.IfTrue;
        [SerializeField] private Object _target;

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
            if (_comparison == Comparison.IfFalse)
            {
                _target.SetEnabled(_scriptableBool.value == false);
            }
            else
            {
                _target.SetEnabled(_scriptableBool.value);
            }
        }

        private void OnValueChanged()
        {
            Evaluate();
        }
    }
}