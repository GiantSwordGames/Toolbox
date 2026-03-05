using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    public class NewProgressBarBase : MonoBehaviour
    {
        [SerializeField] private  TimeScale _timeScale = TimeScale.Scaled;
        [SerializeField] protected SmartFloat _value;
        [SerializeField] private float _tweenWhenIncreasing = 0;
        [SerializeField] private float _tweenWhenDecreasing = 0;

        private float _previousValue;
        private Coroutine _tween;
        protected TimeScale timeScale => _timeScale;

        private void OnValidate()
        {
            if (Application.isPlaying == false)
            {
                Refresh();
            }
        }

        private void OnDestroy()
        {
            _value.onValueChanged -= OnValueChanged;
        }
      
        [Button]
        private void Refresh()
        {
            UpdateBar(_value.normalizedValue);
        }

        public float value
        {
            get => _value.value;
            set => SetValue(value);
        }


        private void OnDisable()
        {
            this.KilLCoroutineIfNeeded(_tween);
        }

        private void Start()
        {
            _previousValue = _value.normalizedValue;
            _value.onValueChanged += OnValueChanged;
            Refresh();

        }

        private void OnValueChanged(float obj)
        {
            this.KilLCoroutineIfNeeded(_tween);

            float newValue =_value.normalizedValue;
            if (newValue > _previousValue && _tweenWhenIncreasing > 0)
            {
                _tween = StartCoroutine(IETweenToNewValue(_previousValue, newValue, _tweenWhenIncreasing));
            }
            else    if (newValue < _previousValue && _tweenWhenDecreasing > 0)
            {
                _tween = StartCoroutine(IETweenToNewValue(_previousValue, newValue, _tweenWhenDecreasing));
            }
            else
            {
                UpdateBar( _value.normalizedValue);
            }
            
            _previousValue = _value.normalizedValue;
        }
        
     

        protected  virtual void UpdateBar(float lerp)
        {
         
        }

        public void SetValue(float newValue)
        {
            newValue = Mathf.Clamp01(newValue);
            _value.value = newValue;
        }
        
          

        private IEnumerator IETweenToNewValue(float startValue, float endValue, float duration)
        {

            float elapsed = 0f;
                
            while (elapsed < duration)
            {
                elapsed +=_timeScale.GetDeltaTime();
                float t = Mathf.Clamp01(elapsed / duration);
                float newValue = Mathf.Lerp(startValue, endValue, t);
                _previousValue = newValue;
                UpdateBar(newValue);
                yield return null;
            }
            UpdateBar(_value.normalizedValue);

        }
    }
}