using UnityEngine;

namespace GiantSword
{
    public class SetVelocity : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] float _speed = 5f;

        // Update is called once per frame
        void FixedUpdate()
        {
            _rigidbody.linearVelocity = transform.forward * _speed;
        }
    }
}
