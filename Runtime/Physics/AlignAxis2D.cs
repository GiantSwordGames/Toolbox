using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AlignAxis2D : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private bool _addHingeJoint;

        [SerializeField] private float _uprightJointSpringStrength = 1;
        [Range(0, 1)]
        [SerializeField] private float _uprightJointSpringDamper = 0.1f;

        [Tooltip("Local axis (should typically be Vector2.up or Vector2.right)")]
        [SerializeField] private Vector2 _localAxis = Vector2.up;
        [SerializeField] private Vector2 _worldAxisTarget = Vector2.up;
        [SerializeField] private float _relativeTargetAngle = 0;
        [SerializeField] private bool _invert = false;

        [Min(0.001f)]
        [SerializeField] private float _gizmoScale = 1;
        [SerializeField] private float _previewForceRange = 10;

        private float _torque;
        private HingeJoint2D _hingeJoint;

        public Vector2 worldAxisTarget => _invert ? -_worldAxisTarget : _worldAxisTarget;

        private void OnEnable()
        {
            if (_addHingeJoint)
            {
                _hingeJoint = gameObject.AddComponent<HingeJoint2D>();
            }
        }

        private void OnDisable()
        {
            if (_hingeJoint)
            {
                Destroy(_hingeJoint);
            }
        }

        private void FixedUpdate()
        {
            RotateTowardsTarget();
        }

        void RotateTowardsTarget()
        {
            // Convert the local axis to world space
            Vector2 worldAxis = _rigidbody2D.transform.TransformDirection(_localAxis);

            // Determine the angle difference between current and desired world axis
            Vector2 axisTarget = worldAxisTarget;
            axisTarget = axisTarget.Rotate(_relativeTargetAngle);
            float angleDiff = Vector2.SignedAngle(worldAxis, axisTarget);

            // Spring torque (based on angle difference)
            float springTorque = angleDiff * _uprightJointSpringStrength;

            // Damping torque (opposes angular velocity)
            float dampingTorque = -_rigidbody2D.angularVelocity * _uprightJointSpringDamper;

            _torque = springTorque + dampingTorque;

            // Apply torque around Z-axis
            _rigidbody2D.AddTorque(_torque);
        }

        private void OnDrawGizmosSelected()
        {
            if (!enabled || _rigidbody2D == null)
                return;

            Vector2 position = _rigidbody2D.position;
            Vector2 worldAxis = _rigidbody2D.transform.TransformDirection(_localAxis);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(position, position + worldAxis * 0.5f * _gizmoScale);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, position + worldAxisTarget * 0.5f * _gizmoScale);
            Gizmos.color = Color.green;
            Vector2 torqueVec = Vector2.Perpendicular(worldAxisTarget).normalized * (_torque * _gizmoScale / _previewForceRange);
            Gizmos.DrawLine(position, position + torqueVec);
            Gizmos.DrawSphere(position + torqueVec, 0.02f);
        }
    }
}
