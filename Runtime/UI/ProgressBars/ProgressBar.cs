using System;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Transform _primaryBar;
        [SerializeField] private Transform _secondaryBar;
        [SerializeField] private SmartFloat _value;
        public virtual Color primaryColor { get; set; }
        public virtual Color secondary { get; set; }


        private void OnValidate()
        {
            Refresh();
        }

        [Button]
        private void Refresh()
        {
            UpdateBar(_value.value);
        }

        public SmartFloat value
        {
            get => _value;
            set => SetValue(value);
        }

        private void Start()
        {
            _value.onValueChanged += UpdateBar;
            Refresh();

        }

        private void OnEnable()
        {
        }

        private void UpdateBar(float value)
        {
            if (_primaryBar)
            {
                _primaryBar.localScale = new Vector3( _value.normalizedValue, 1, 1);
            }
            
            if (_secondaryBar)
            {
                _secondaryBar.localScale = new Vector3( _value.normalizedValue, 1, 1);
            }
        }

        public void SetValue(float newValue)
        {
            newValue = Mathf.Clamp01(newValue);
            _value.value = newValue;
            // UpdateBar(newValue);
        }
    }
}
