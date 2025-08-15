using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    
    public class MonoFloat : MonoBehaviour
    {

        public enum WrapBehaviour
        {
            Clamp,
            Wrap,
            Unlimited,
        }


        [SerializeField] private WrapBehaviour _wrapBehaviour;
        [SerializeField] private SmartFloat _value;
        [SerializeField] private SmartFloat _max = new SmartFloat(100); 
        [SerializeField] private Action<float> _onValueChangedAction;
        [SerializeField] private UnityEvent _onValueChanged;
        [SerializeField] private UnityEvent _onValueDecreasedUnclamped;
        [SerializeField] private UnityEvent _onValueDecreased;
        [SerializeField] private UnityEvent _onValueIncreased;
        [SerializeField] private UnityEvent _onEmpty;
        [SerializeField] private UnityEvent _onFull;
        [SerializeField] private UnityEvent _onOverKill;
        [SerializeField] private bool _fireEventsOnNoChange;

        private void OnValidate()
        {
        }

        public void Start()
        {
            _value.onValueChanged += OnValueChanged;
            _max.onValueChanged += OnValueChanged;
        }

        private void OnValueChanged(float obj)
        {
            _onValueChangedAction?.Invoke(obj);
            onValueChanged.Invoke();

        }

        public float value
        {
            get => _value;
            set
            {
                float previous = _value;
                _value.value = value;

                if (value < previous)
                {
                    _onValueDecreasedUnclamped?.Invoke();
                }
                
                if(_wrapBehaviour == WrapBehaviour.Clamp)
                {
                    _value.value = Mathf.Clamp(_value, 0, _max.value);
                }

                if (previous != _value || _fireEventsOnNoChange)
                {
                    onValueChanged.Invoke();
                    _onValueChangedAction?.Invoke(_value);
                    
                    if (_value < previous)
                    {
                        onValueDecreased.Invoke();
                    }
                    else if (_value > previous)
                    {
                        onValueIncreased.Invoke();
                    }
                    
                    if (_value <= 0 && previous > 0)
                    {
                        _onEmpty?.Invoke();
                    }
                    
                    
                    if ( _value >= _max)
                    {
                        _onFull?.Invoke();
                    }
                    if (_wrapBehaviour == WrapBehaviour.Wrap)
                    {
                        _value.value = Mathf.Repeat(_value, _max.value);
                    }

                  
                }
                if (value < 0 && previous <= 0)
                {
                    _onOverKill?.Invoke();
                }
                
            }
        }
        public float max
        {
            get => _max;
            set
            {
                _max.value = value;
            }
        }
        

        public bool isFull
        {
            get =>  _value >= _max;
        }


        public event Action<float> onValueChangedAction
        {
            add => _onValueChangedAction += value;
            remove => _onValueChangedAction -= value;
        }

        public UnityEvent onValueChanged => _onValueChanged;

        public UnityEvent onValueDecreased => _onValueDecreased;

        public UnityEvent onValueIncreased => _onValueIncreased;

        public UnityEvent onFull
        {
            get => _onFull;
            set => _onFull = value;
        }

        public static implicit operator float(MonoFloat variable)
        {
            return variable.value;
        } 
        
        public float GetNormalizedValue( bool clampMinToZero = false)
        {
            float min = 0;
            if (clampMinToZero)
            {
                min = Mathf.Max(0, min);
            }
            return Mathf.InverseLerp(  min, _max, _value);
        }
        
        public Coroutine IncrementValueTo( float newValue, float duration)
        {
            float difference = newValue - value;
            return AsyncHelper.StartCoroutine(IEIncrementValueOverTime(difference, duration));
        }
        
        public Coroutine IncrementValueOverTime( float increment, float duration)
        {
            return AsyncHelper.StartCoroutine(IEIncrementValueOverTime(increment, duration));
        }
        private IEnumerator IEIncrementValueOverTime(float increment, float duration)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            float lerpPrev = 0;

            while (Time.time < endTime)
            {
                float currentTime = Time.time;
                float lerp = Mathf.Clamp01((currentTime - startTime) / duration);

                float diff = lerp - lerpPrev;
                lerpPrev = lerp;
                value += diff * increment;
Debug.Log(value);
                yield return null;
            }

            // Ensure final increment in case of precision issues
            value += (1 - lerpPrev) * increment;
        }
    }
}
