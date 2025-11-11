using System;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] protected SmartFloat _value;

        private void OnValidate()
        {
            // Refresh();
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

        private void Start()
        {
            _value.onValueChanged += UpdateBar;
            Refresh();

        }

        private void OnDestroy()
        {
            _value.onValueChanged -= UpdateBar;
        }

        private void OnEnable()
        {
        }

        protected virtual void UpdateBar(float v)
        {
            // if (_primaryBar)
            // {
            //     _primaryBar.localScale = new Vector3( _value.normalizedValue, 1, 1);
            // }
            //
            // if (_secondaryBar)
            // {
            //     _secondaryBar.localScale = new Vector3( _value.normalizedValue, 1, 1);
            // }

            transform.SetLocalScaleX(_value.normalizedValue);
        }

        public void SetValue(float newValue)
        {
            newValue = Mathf.Clamp01(newValue);
            _value.value = newValue;
            // UpdateBar(newValue);
        }
    }
}
