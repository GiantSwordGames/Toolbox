    using System;
    using System.Collections;
    using NaughtyAttributes;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Serialization;
    using Framework;

    namespace JamKit
    {
        public class VariableText : MonoBehaviour, IPropertyEditListener
        {
            [SerializeField] private TMPro.TMP_Text _text;
            
            [SerializeField] private UnityEvent _onDisplayValueChanged;
            [SerializeField] private UnityEvent _onValueChanged;

            [SerializeField] private SmartFloat _value;
            [SerializeField] private float _increment = 0;
            [SerializeField] private float _multiplier = 1;
            [FormerlySerializedAs("_abbreviatedMoney")]
            [FormerlySerializedAs("_abbreviate")]
            [Space]
            [SerializeField] private bool _formatAsAbbreviatedMoney = false;
            [SerializeField] private bool _formatAsTime = false;
            [SerializeField] private string _format = "F1";
            [SerializeField] [TextArea(1, 5)] private string _prefix;
            [SerializeField] [TextArea(1, 5)] private string _postFix;
            [Space]
            [SerializeField] private SmartFloat[] _additionalValues = { };
            [SerializeField] private float _incrementOverDuration = 0;
            private float _previousValue;

            public float value
            {
                get => _value;
                set => _value.value = value;
            }


            
            public void OnPropertyEdited()
            {
                Refresh();

            }

            void OnEnable()
            {
                _previousValue = _value.value;
                _value.onValueChanged += OnValueValueChanged;
                foreach (SmartFloat smartFloat in _additionalValues)
                {
                    smartFloat.onValueChanged += OnValueValueChanged;
                }
                Refresh();
            }

            private void OnDisable()
            {
                _value.onValueChanged -= OnValueValueChanged;
                
                foreach (SmartFloat smartFloat in _additionalValues)
                {
                    smartFloat.onValueChanged -= OnValueValueChanged;
                }
            }

            private void OnValueValueChanged(float obj)
            {
                if (_incrementOverDuration > 0)
                {
                    StartCoroutine(IEIncrementTowardsNewValue(_previousValue));
                }
                else
                {
                    Refresh();
                }

                _onValueChanged?.Invoke();
                _previousValue = _value.value;
            }


            [Button]
            private void Refresh()
            {
                if (ValidationUtility.IsPrefabAsset(this))
                {
                    return;
                }

                if (_text == null)
                {
                    return;
                }

                float value = _value.value;

                SetText(value);
            }

            private void SetText(float value)
            {
                string previousText = _text.text;
                string formattedNumber = default;

                var modifiedValue = (value + _increment)* _multiplier;
                if (_formatAsAbbreviatedMoney)
                {
                    formattedNumber = modifiedValue.ToAbbreviatedMoneyString();
                }
                else if (_formatAsTime)
                {
                    formattedNumber = TimeSpan.FromSeconds(modifiedValue).ToString(@"mm\:ss");
                }
                else
                {
                    formattedNumber = (modifiedValue).ToString(_format);
                }

                string newText = _prefix + formattedNumber + _postFix;
                for (int i = 0; i < _additionalValues.Length; i++)
                {
                    string oldValue = $"{{{i}}}";
                    newText = newText.Replace(oldValue, _additionalValues[i].value + "");
                }

                if (newText != previousText)
                {
                    _text.text = newText;
                    _onDisplayValueChanged?.Invoke();
                }

                string valueName = _value.name;
                if(valueName == "")
                    valueName = _text.text;
                name = name = "" + valueName;

            }

            public void SetValue(float value)
            {
                _value.value = value;
                Refresh();
            }

            private IEnumerator IEIncrementTowardsNewValue(float oldValue)
            {

                float duration = _incrementOverDuration;
                float startValue = oldValue;
                float endValue = value;
                float elapsed = 0f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / duration);
                    float newValue = Mathf.Lerp(startValue, endValue, t);
                    SetText(newValue);
                    yield return null;
                }

                Refresh();
            }

        
        }
    }
