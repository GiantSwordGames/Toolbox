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
            [FormerlySerializedAs("_onRefresh")] [SerializeField] private UnityEvent _onDisplayValueChanged;
            [SerializeField] private UnityEvent _onValueChanged;
            [SerializeField] private SmartFloat _value;
            [SerializeField] private TMPro.TMP_Text _text;
            [SerializeField] private string _prefix;
            [SerializeField] private string _postFix;
            [SerializeField] private string _format = "F1";
            [SerializeField] private SmartFloat[] _additionalValues = {};
            [SerializeField] private float _incrementOverDuration = 0;
            private float _previousValue ;

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
                _previousValue = _value.value;
                _value.onValueChanged += OnValueValueChanged;
                Refresh();
            }

            private void OnDestroy()
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

                string newText = _prefix + value.ToString(_format) + _postFix;
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

                name = name = "STAT_" + _prefix.StripNonAlphabetCharacters();

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
            
        }
    }
