using UnityEngine;

namespace GiantSword
{
    
    public class AnimatePosition : AnimateTransformBase
    {
         private Vector3 _positionOffset = Vector3.zero;
         [SerializeField] private Vector3 _direction = Vector3.up;

         public override void Reset()
         {
             if (additive)
             {
                 _target.localPosition -= _positionOffset;
             }
            
             base.Reset();
             _positionOffset = Vector3.zero;
             
             if(additive == false)
             {
                 float value = EvaluateTween();
                 _target.localPosition = _direction * value;
             }
           
         }

         protected override void Evaluate()
        {
            if (additive)
            {
                _target.localPosition -= _positionOffset;
            }

            float value = EvaluateTween();
            _positionOffset = _direction * value;

            if (additive)
            {
                _target.localPosition += _positionOffset;
            }
            else
            {
                _target.localPosition = _positionOffset;
            }
        }
    }
}
