using UnityEngine;
using UnityEngine.UI;

namespace JamKit
{
    public class FilledImageProgressBar : ProgressBar
    {
        [SerializeField] private Image _primaryRenderer;
        [SerializeField] private Image _secondaryRenderer;
   
        protected override void UpdateBar(float value)
        {
            if (_primaryRenderer)
            {
                _primaryRenderer.fillAmount = value;
            }

            if (_secondaryRenderer)
            {
                _secondaryRenderer.fillAmount = value;
            }
        }
    }
}