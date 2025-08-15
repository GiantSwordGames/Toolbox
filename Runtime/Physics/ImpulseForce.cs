using System.Collections;
using UnityEngine;

    namespace JamKit
    {
        [ExecuteInEditMode]
        public class ImpulseForce : AddForceBase
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
                _rigidbody.AddForceAtPosition(force * _multiplier * lerp, transform.position, ForceMode.Impulse);
            }

            private void OnDrawGizmosSelected()
            {
                Gizmos.DrawRay(transform.position, force);
            }
        }
    }
