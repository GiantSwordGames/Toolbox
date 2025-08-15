using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace JamKit
{
    public class ImageRectProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _primaryBar;
        [SerializeField] private float _filledWidth = 100f;
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
            UpdateBar(_value.normalizedValue);
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

        private void UpdateBar(float v)
        {
            if (_primaryBar)
            {
                _primaryBar.rectTransform.sizeDelta = _primaryBar.rectTransform.sizeDelta .WithX( _filledWidth*value.normalizedValue);
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
            // UpdateBar(newValue);
        }
    }
}