using System.Collections;
using UnityEngine;

namespace GiantSword
{
    [ExecuteInEditMode]
    public class TorqueImpulseForce : AddForceBase
    {

        [Space]
        [SerializeField] private float _delay;
            
        public override void Trigger()
        {
            if (enabled == false)
            {
                return;
            }
                
            if (_rigidbody)
            {
                StartCoroutine(DoApplyForce());
            }
        }
            
        private IEnumerator DoApplyForce()
        {
            if(_delay > 0)
            {
                yield return new WaitForSeconds(_delay);
            }
            _rigidbody.AddTorque(force * _multiplier * lerp, ForceMode.Impulse);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawRay(transform.position, force);
        }
    }
}