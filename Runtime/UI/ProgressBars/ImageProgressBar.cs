using UnityEngine;
using UnityEngine.UI;

namespace JamKit
{
    public class ImageProgressBar : ProgressBar
    {
        [SerializeField] private Image _primaryRenderer;
        [SerializeField] private Image _secondaryRenderer;

        public override Color primaryColor
        {
            get => _primaryRenderer.color;
            set => _primaryRenderer.color = value;
        }

        public override Color secondary
        {
            get => _secondaryRenderer.color;
            set => _secondaryRenderer.color = value;
        }
    }
}