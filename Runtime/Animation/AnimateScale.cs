using UnityEngine;

namespace GiantSword
{
    public class AnimateScale : AnimateTransformBase
    {
        [SerializeField] private Vector3 _mask = Vector3.one;
        private Vector3 _scaleOffset = Vector3.zero;


        public override void Reset()
        {
            if (Application.isPlaying == false)
            {
                return;
            }
            if (additive)
            {
                _target.localScale -= _scaleOffset;
            }
            base.Reset();
            if(additive== false)
            {
                float value = EvaluateTween();
                _target.localScale = Vector3.one*value;
            }
        }

        protected override void Evaluate()
        {
            if (additive)
            {
                _target.localScale -= _scaleOffset;
            }
            float value = EvaluateTween();
            _scaleOffset = Vector3.one * value;
            _scaleOffset.Scale(_mask);

            if (additive)
            {
                _target.localScale += _scaleOffset;
            }
            else
            {
                _target.localScale = _scaleOffset;
            }
        }
    }
}