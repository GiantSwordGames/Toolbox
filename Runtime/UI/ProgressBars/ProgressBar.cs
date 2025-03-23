using System;
using UnityEngine;

namespace GiantSword
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Transform _primaryBar;
        [SerializeField] private Transform _secondaryBar;
        [SerializeField] private SmartFloat _value;
        public virtual Color primaryColor { get; set; }
        public virtual Color secondary { get; set; }

        public SmartFloat value
        {
            get => _value;
            set => SetValue(value);
        }

        private void Start()
        {
            _value.onValueChanged += UpdateBar;
        }

        private void UpdateBar(float value)
        {
            _primaryBar.localScale = new Vector3( _value.value, 1, 1);
            _secondaryBar.localScale = new Vector3( _value.value, 1, 1);
        }

        public void SetValue(float newValue)
        {
            _value.value = newValue;
            // UpdateBar(newValue);
        }
    }
}
