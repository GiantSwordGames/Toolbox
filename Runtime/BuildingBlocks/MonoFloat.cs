using System;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
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
        [SerializeField] private SmartFloat _max = 100f; 
        [SerializeField] private Action<float> _onValueChangedAction;
        [SerializeField] private UnityEvent _onValueChanged;
        [SerializeField] private UnityEvent _onValueDecreasedUnclamped;
        [SerializeField] private UnityEvent _onValueDecreased;
        [SerializeField] private UnityEvent _onValueIncreased;
        [SerializeField] private UnityEvent _onEmpty;
        [SerializeField] private UnityEvent _onFull;
        [SerializeField] private UnityEvent _onOverKill;

        private void OnValidate()
        {
        }

        public void Start()
        {
            _max.onValueChanged += onValueChangedAction;
        }

        public float value
        {
            get => _value;
            set
            {
                float previous = _value;
                _value = value;

                if (value < previous)
                {
                    _onValueDecreasedUnclamped?.Invoke();
                }
                
                if(_wrapBehaviour == WrapBehaviour.Clamp)
                {
                    _value = Mathf.Clamp(_value, 0, _max.value);
                }

                if (previous != _value)
                {
                    onValueChanged.Invoke();
                    onValueChangedAction?.Invoke(_value);
                    
                    if (_value < previous)
                    {
                        onValueDecreased.Invoke();
                    }
                    else
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
                        _value = Mathf.Repeat(_value, _max.value);
                    }

                  
                }
                if (value < 0 && previous <= 0)
                {
                    _onOverKill?.Invoke();
                }
                
            }
        }
        

        public bool isFull
        {
            get =>  _value >= _max;
        }


        public Action<float> onValueChangedAction
        {
            get => _onValueChangedAction;
            set => _onValueChangedAction = value;
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
    }
}