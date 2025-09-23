    using System;
    using System.Collections;
    using NaughtyAttributes;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Serialization;

    namespace JamKit
    {
        public class VariableText : MonoBehaviour
        {
            [SerializeField] private TMPro.TMP_Text _text;
            [FormerlySerializedAs("_onRefresh")] [SerializeField] private UnityEvent _onDisplayValueChanged;
            [SerializeField] private UnityEvent _onValueChanged;
            [SerializeField] private SmartFloat _value;
            [SerializeField] private float _multiplier = 1;
            [SerializeField] private bool _abbreviate = false;
            [SerializeField] private string _format = "F1";
            [SerializeField] [TextArea(1,5)] private string _prefix;
             [SerializeField] [TextArea(1,5)] private string _postFix;
          
            [SerializeField] private SmartFloat[] _additionalValues = {};
            [SerializeField] private float _incrementOverDuration = 0;
            private float _previousValue;

            public SmartFloat value => _value;


            private void OnValidate()
            {
                if (Application.isPlaying == false)
                {
                    Refresh();
                }
            }

            void OnEnable()
            {
                _previousValue = _value.value;
                _value.onValueChanged += OnValueValueChanged;
                Refresh();
            }


            private void OnDisable()
            {

                _value.onValueChanged -= OnValueValueChanged;
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
                
                if(_text == null)
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
                
                if (_abbreviate )
                {
                    formattedNumber = Abbreviate(value * _multiplier);
                }
                else
                {
                    formattedNumber = (value * _multiplier).ToString(_format);
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

                name = name = "Stat_" + _prefix.StripNonAlphabetCharacters();

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
                float endValue = value.value;
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
            
            /// <summary>
            /// Abbreviates a number using K (thousand), M (million), B (billion).
            /// Examples:
            /// 1000 -> 1K
            /// 1500 -> 1.5K
            /// 2500000 -> 2.5M
            /// </summary>
            public static string Abbreviate(float number)
            {
                if (number < 1000)
                    return number.ToString("0"); // no abbreviation under 1K

                if (number < 10000)
                    return (number / 1000d).ToString("0.#") + "K"; // 1 decimal place up to 9.9K

                if (number < 1000000)
                    return (number / 1000d).ToString("0") + "K"; // no decimals above 10K

                if (number < 10000000)
                    return (number / 1000000d).ToString("0.#") + "M"; // 1 decimal place up to 9.9M

                if (number < 1000000000)
                    return (number / 1000000d).ToString("0") + "M"; // no decimals above 10M

                if (number < 10000000000)
                    return (number / 1000000000d).ToString("0.#") + "B"; // 1 decimal place up to 9.9B

                return (number / 1000000000d).ToString("0") + "B"; // no decimals above 10B
            }
            
        }
    }
