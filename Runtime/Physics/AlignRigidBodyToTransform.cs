using UnityEngine;

namespace GiantSword
{
    public class AlignRigidBodyToTransform : MonoBehaviour
    {

         [SerializeField] private Transform _alignToForward;

        [SerializeField]  private Rigidbody _rigidbody;

        [SerializeField] private float _springStrength = 1;
       [Range(0, 1)] [SerializeField] private float _springDamper = 0.1f;
        [SerializeField] private Vector3 _localAxis = Vector3.forward;
        private Vector3 _torque;
        

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
            Vector3 rotationAxis = Vector3.Cross(worldAxis, _alignToForward.forward);
            float angle = Vector3.Angle(worldAxis, _alignToForward.forward);

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

            if (_alignToForward == false)
            {
                return;
            }

            Gizmos.color = Color.blue;
            Vector3 worldAxis = transform.TransformDirection(_localAxis);
            Gizmos.DrawRay(transform.position, worldAxis * 0.05f);
            Gizmos.DrawRay(transform.position, _alignToForward.forward * 0.05f);
            Gizmos.DrawRay(transform.position, _torque / _springStrength);
            Gizmos.DrawSphere(transform.position + _torque / _springStrength, 0.02f);
            GizmoUtility.DrawArrowFromPosition(transform.position, _alignToForward.forward * 0.1f, 0.01f);
        }
    }
}