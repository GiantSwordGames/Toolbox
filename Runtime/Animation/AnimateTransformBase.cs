using UnityEngine;

namespace GiantSword
{
    public abstract class AnimateTransformBase : AnimateBase
    {
        protected override void OnEnable()
        {
            _target.Initialize(this);
            base.OnEnable();
        }

        [SerializeField] protected TargetTransform _target;
    }
}