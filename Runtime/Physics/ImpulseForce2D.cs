using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
    {
        [ExecuteInEditMode]
        public class ImpulseForce2D : AddForceBase2D
        {
            [Space]
            [SerializeField] private float _delay;
            [SerializeField] private UnityEvent _onTrigger;
            
            [Button]
            public override void Trigger()
            {
                if (enabled == false)
                {
                    return;
                }
                
                if (_rigidbody2D)
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
                _rigidbody2D.AddForceAtPosition(force * _multiplier * lerp, transform.position, ForceMode2D.Impulse);
                
                _onTrigger?.Invoke();
            }

            private void OnDrawGizmosSelected()
            {
                Gizmos.DrawRay(transform.position, force);
            }
        }
    }
