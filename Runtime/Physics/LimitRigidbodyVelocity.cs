using UnityEngine;

namespace GiantSword
{
    
    [DefaultExecutionOrder(100000)]
    public class LimitRigidbodyVelocity : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private float _maxVelocity = 10f;
        [SerializeField] private Vector3 direction = Vector3.forward;

        void Update()
        {
            if ( _rigidbody.linearVelocity.magnitude > _maxVelocity)
            {
                _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _maxVelocity;
            }
        }
    }
}
