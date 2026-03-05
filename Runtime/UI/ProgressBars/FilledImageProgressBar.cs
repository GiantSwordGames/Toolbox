using Framework;
using UnityEngine;
using UnityEngine.UI;

namespace JamKit
{
    public class FilledImageProgressBar : ProgressBar, IPropertyEditListener
    {
        [SerializeField] private Image _primaryRenderer;

        public Color color
        {
            get => _primaryRenderer ? _primaryRenderer.color : Color.white;
            set
            {
                if (_primaryRenderer)
                {
                    _primaryRenderer.color = value;
                }
            }
            
        }


        protected override void UpdateBar(float value)
        {
            if (_primaryRenderer)
            {
                _primaryRenderer.fillAmount = value;
            }

        }

        public void OnPropertyEdited()
        {
            UpdateBar(_value.normalizedValue);
        }
    }
}