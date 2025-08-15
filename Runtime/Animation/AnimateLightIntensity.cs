using UnityEngine;

namespace JamKit
{
    public class AnimateLightIntensity : AnimateBase
    {
        [SerializeField] private Light _light;
        private float _offset;

        protected override void Evaluate()
        {
            if (additive)
            {
                _light.intensity -= _offset;
            }
            float intensity = tweenSettings.Evaluate(_time);
            _offset = intensity;
            if (additive)
            {
                _light.intensity += _offset;
            }
            else
            {
                _light.intensity = _offset;
            }
        }
    }
}