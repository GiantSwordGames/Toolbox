namespace GiantSword
{
    using UnityEngine;
    
    [ExecuteInEditMode]
    public class PullForce2D : AddForceBase2D
    {
        [SerializeField] private ForceMode2D _forceMode = ForceMode2D.Force;
        [SerializeField] private bool _applyForce = true;
    
        void FixedUpdate()
        {
            if (_applyForce && Application.isPlaying )
            {
                Pull();
                
            }
        }
    
        public void Pull()
        {
    
            if (_rigidbody2D)
            {
                _rigidbody2D.AddForceAtPosition(force * (_multiplier * lerp), transform.position, _forceMode);
            }
        }

        public override void Trigger()
        {
            _applyForce = true;
        }
    }
}
