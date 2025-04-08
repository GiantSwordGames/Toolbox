using UnityEngine;

namespace GiantSword
{
    public class AnimateFloat : AnimateBase
    {
        [SerializeField] private SmartFloat _value;
        private float _offset;

        public SmartFloat value => _value;

        protected override void Evaluate()
        {
            if (additive)
            {
                _value.value -= _offset;
            }
            float intensity = tweenSettings.Evaluate(_time);
            _offset = intensity;
            if (additive)
            {
                _value.value += _offset;
            }
            else
            {
                _value.value = _offset;
            }
        }
    }
}