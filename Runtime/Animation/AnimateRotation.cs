using UnityEngine;

namespace JamKit
{
    public class AnimateRotation : AnimateTransformBase
    {
        [SerializeField] private Vector3 _euler = Vector3.up ;
         private Quaternion _offset = Quaternion.identity;

         public override void Reset()
         {
             if (additive)
             {
                 _target.localRotation *= Quaternion.Inverse(_offset);
             }
             
             base.Reset();
             if (additive==false)
             {
                 if(additive == false)
                 {
                     float value = EvaluateTween();
                     _target.localPosition = _euler.normalized * value;
                 }
             }
         }

         protected override void Evaluate()
        {
            if (additive)
            {
                _target.localRotation *= Quaternion.Inverse(_offset);
            }
            
            float rotation = EvaluateTween();
            _offset = Quaternion.Euler(_euler.normalized *rotation);

            if (additive)
            {
                _target.localRotation *= _offset;
            }
            else
            {
                _target.localRotation = _offset;
            }
        }

    }
}
