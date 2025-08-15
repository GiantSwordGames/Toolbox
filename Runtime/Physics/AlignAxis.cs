using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;

namespace JamKit
{
    public class AlignAxis : MonoBehaviour
    {
        [SerializeField]   private Rigidbody _rigidbody;
            
        [SerializeField] private float _uprightJointSpringStrength = 1;
        [Range(0,1)]
        [SerializeField] private float _uprightJointSpringDamper = 0.1f;
        [SerializeField] private Vector3 _localAxis = Vector3.up;
        [SerializeField] private Vector3 _worldAxisTarget = Vector3.up;
        [SerializeField] private bool _invert = false;
        [FormerlySerializedAs("_intendedForceRange")]
        [Space]
        [Min(0.001f)] 
        [SerializeField] private float _gizmoScale = 1;
        [SerializeField] private float _previewForceRange = 10;
        private Vector3 _torque;
    
        public Vector3 worldAxisTarget
        {
            get
            {
                if (_invert)
                {
                    return -_worldAxisTarget;
                }
                return _worldAxisTarget;
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
            Vector3 rotationAxis = Vector3.Cross(worldAxis, worldAxisTarget);
            float angle = Vector3.Angle(worldAxis, worldAxisTarget);
    
            // Calculate the spring force (torque) using Hooke's law
            // The torque is proportional to the angle difference and the spring strength
            _torque = rotationAxis * (angle * _uprightJointSpringStrength);
    
            // Calculate the damping torque to reduce oscillations
            // This torque is proportional to the negative of the current angular velocity and the damping coefficient
            Vector3 dampingTorque = -_rigidbody.angularVelocity * _uprightJointSpringDamper;
    
            // Apply the calculated torques (spring torque + damping torque) to the rigidbody
            _rigidbody.AddTorque(_torque + dampingTorque);
        }
    
    
        private void OnDrawGizmosSelected()
        {
            if (enabled == false)
            {
                return;
            }
            
            Gizmos.color = Color.cyan;
            Vector3 worldAxis = _rigidbody.transform.TransformDirection(_localAxis);
            Gizmos.DrawRay(_rigidbody.position, worldAxis * 0.5f*_gizmoScale);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_rigidbody.position, worldAxisTarget * 0.5f*_gizmoScale);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(_rigidbody.position, _torque*_gizmoScale / _previewForceRange);
            Gizmos.DrawSphere(_rigidbody.position + _torque / _previewForceRange, 0.02f);
            // Gizmos.DrawRay(transform.position, worldAxisTarget * 0.1f, 0.01f);
        }
    
    }
}
