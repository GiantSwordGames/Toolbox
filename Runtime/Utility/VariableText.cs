    using System;
    using NaughtyAttributes;
    using UnityEngine;
    using UnityEngine.Events;

    namespace GiantSword
    {
        public class VariableText : MonoBehaviour
        {
            [SerializeField] private UnityEvent _onRefresh;
            [SerializeField] private UnityEvent _onValueChanged;
            [SerializeField] private SmartFloat _value;
            [SerializeField] private TMPro.TMP_Text _text;
            [SerializeField] private string _prefix;
            [SerializeField] private string _postFix;
            [SerializeField] private SmartFloat[] _additionalValues = {};

            public SmartFloat value => _value;


            private void OnValidate()
            {
                if (Application.isPlaying == false)
                {
                    Refresh();
                }
            }

            void Start()
            {
                _value.onValueChanged += OnValueValueChanged;
                Refresh();
            }

            private void OnDestroy()
            {

                _value.onValueChanged -= OnValueValueChanged;
            }

            private void OnValueValueChanged(float obj)
            {
               Refresh();
               _onValueChanged?.Invoke();
            }


            [Button]
            private void Refresh()
            {
                if (ValidationUtility.IsPrefabAsset(this))
                {
                    return;
                }
                
                if(_text == null)
                {
                    return;
                }

                _text.text = _prefix + Math.Round(_value.value, 1).ToString() + _postFix;
                for (int i = 0; i < _additionalValues.Length; i++)
                {
                    string oldValue = $"{{{i}}}";
                    _text.text = _text.text.Replace(oldValue, _additionalValues[i].value + "");
                }

                _onRefresh?.Invoke();
                name = "Stat_" + _prefix;
            }

            public void SetValue(float value)
            {
                _value.value = value;
                Refresh();
            }
        }
    }
