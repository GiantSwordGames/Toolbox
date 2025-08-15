using UnityEngine;

namespace GiantSword
{
    public abstract class AlignRigidBodyToDirectionBase : MonoBehaviour
    {
        protected virtual Vector3 direction  => Vector3.forward;

        [SerializeField]  private Rigidbody _rigidbody;

        [SerializeField] private float _springStrength = 1;
        [SerializeField] private float _springDamper = 0.1f;
        [SerializeField] private Vector3 _localAxis = Vector3.forward;
        private Vector3 _torque;

        private void Start()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
        }

        void FixedUpdate()
        {
            RotateTowardsTarget();
        }

        /// <summary>
        /// Rotates the object towards a target world axis by applying a torque calculated using spring physics.
        /// </summary>
        void RotateTowardsTarget()
        {
            // Convert the local axis to a world axis
            Vector3 worldAxis = _rigidbody.transform.TransformDirection(_localAxis);

            // Calculate the axis of rotation needed to align the current world axis with the target world axis
            Vector3 direction = this.direction;
            Vector3 rotationAxis = Vector3.Cross(worldAxis, direction);
            float angle = Vector3.Angle(worldAxis, direction);

            // Calculate the spring force (torque) using Hooke's law
            // The torque is proportional to the angle difference and the spring strength
            _torque = rotationAxis * (angle * _springStrength);

            // Calculate the damping torque to reduce oscillations
            // This torque is proportional to the negative of the current angular velocity and the damping coefficient
            Vector3 dampingTorque = -_rigidbody.angularVelocity * _springDamper;

            // Apply the calculated torques (spring torque + damping torque) to the rigidbody
            _rigidbody.AddTorque(_torque + dampingTorque);
        }

        private void OnDrawGizmosSelected()
        {
            if (enabled == false)
            {
                return;
            }
            
            if(_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            Vector3 direction = this.direction;
            Gizmos.color = Color.blue;
            Vector3 worldAxis = transform.TransformDirection(_localAxis);
            Gizmos.DrawRay(transform.position, worldAxis);
            Gizmos.DrawRay(transform.position, direction );
            Gizmos.DrawRay(transform.position, _torque / _springStrength);
            Gizmos.DrawSphere(transform.position + _torque / _springStrength, 0.02f);
        }
    }
}