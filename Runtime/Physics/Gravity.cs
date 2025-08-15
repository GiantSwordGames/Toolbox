namespace JamKit
{
    using UnityEngine;
    
    public class Gravity : MonoBehaviour
    {
        [Range(0,1)]
        [SerializeField] private float _lerp = 1;
        [SerializeField] private float _gravity = 30;
        private Rigidbody _rigidbody;
    
    
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            if (_rigidbody)
            {
                _rigidbody.AddForce(Vector3.down*_gravity*_lerp, ForceMode.Force);
            }
        }
    }
}
