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
            if ( _rigidbody.velocity.magnitude > _maxVelocity)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized * _maxVelocity;
            }
        }
    }
}
