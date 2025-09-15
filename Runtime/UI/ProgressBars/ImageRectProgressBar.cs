using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace JamKit
{
    public class ImageRectProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _primaryBar;
        [SerializeField] private float _filledWidth = 100f;
        [SerializeField] private float _incrementOverTime = 0;
        [SerializeField] private SmartFloat _value;
        private float _previousValue;
        private Coroutine _tween;
        public virtual Color primaryColor { get; set; }
        public virtual Color secondary { get; set; }

        private void OnValidate()
        {
            if (Application.isPlaying == false)
            {
                Refresh();
            }
        }

        [Button]
        private void Refresh()
        {
            UpdateBar(_value.normalizedValue);
        }

        public SmartFloat value
        {
            get => _value;
            set => SetValue(value);
        }


        private void OnEnable()
        {
            _value.onValueChanged += OnValueChanged;
        }
        
        private void OnDisable()
        {
            _value.onValueChanged -= OnValueChanged;
            this.KilLCoroutineIfNeeded(_tween);
        }

        private void Start()
        {
            _previousValue = _value.normalizedValue;
            Refresh();

        }

        private void OnValueChanged(float obj)
        {

            if (_incrementOverTime > 0)
            {
                this.KilLCoroutineIfNeeded(_tween);
                _tween = StartCoroutine(IEIncrementTowardsNewValue(_previousValue));
            }
            else
            {
                UpdateBar( _value.value);
            }
            
            _previousValue = _value.normalizedValue;
        }
        
     

        private void UpdateBar(float lerp)
        {
            
            if (_primaryBar)
            {
                lerp = Mathf.Clamp01(lerp);
                _primaryBar.rectTransform.sizeDelta = _primaryBar.rectTransform.sizeDelta .WithX( _filledWidth*lerp);
            }
            
            // if (_secondaryBar)
            // {
                // _secondaryBar.localScale = new Vector3( _value.normalizedValue, 1, 1);
            // }
        }

        public void SetValue(float newValue)
        {
            newValue = Mathf.Clamp01(newValue);
            _value.value = newValue;
        }
        
          

        private IEnumerator IEIncrementTowardsNewValue(float oldValue)
        {

            float duration = _incrementOverTime;
            float startValue = oldValue;
            float endValue = value.normalizedValue;
            float elapsed = 0f;
                
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float newValue = Mathf.Lerp(startValue, endValue, t);
                UpdateBar(newValue);
                yield return null;
            }
            UpdateBar(value.normalizedValue);

        }
    }
}