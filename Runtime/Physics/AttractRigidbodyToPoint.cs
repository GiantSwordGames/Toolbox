using NaughtyAttributes;

namespace JamKit
{
    using UnityEngine;

    public class AttractRigidbodyToPoint : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Transform _target;

        [Header("Spring Settings")]
        [SerializeField] private float _springStrength = 100f;
        [SerializeField] private float _springDamper = 10f;
        [Tooltip("The distance from the target point the body will try to maintain (0 = snap to point).")]
        [SerializeField] private float _targetDistance = 0f;

        [Header("Behaviour")]
        [Tooltip("If enabled, the force will only ever pull towards the target (no pushing away).")]
        [SerializeField] private bool _onlyAttract = true;

        [Tooltip("If enabled, gravity is only applied when outside the target distance.")]
        [SerializeField] private bool _applyGravityOnlyWhenOutsideTargetDistance = false;

        [Header("Debug")]
        [ShowNonSerializedField] private float _currentDistance;
        [ShowNonSerializedField] private float _lastSpringForce;

        public Transform target
        {
            get => _target;
            set => _target = value;
        }

        private void Reset()
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (_rigidbody == null || _target == null)
                return;

            Vector3 origin = _rigidbody.worldCenterOfMass;
            Vector3 targetPosition = _target.position;

            // Direction from target to body
            Vector3 toBody = origin - targetPosition;
            float distance = toBody.magnitude;

            _currentDistance = distance;

            if (distance <= Mathf.Epsilon)
            {
                if (_applyGravityOnlyWhenOutsideTargetDistance)
                    _rigidbody.useGravity = false;

                return;
            }

            Vector3 directionFromTargetToBody = toBody / distance; // normalized

            // Velocity of the rigidbody along this direction
            Vector3 velocity = _rigidbody.GetLinearVelocity();
            float directionVelocity = Vector3.Dot(directionFromTargetToBody, velocity);

            // Spring displacement: how far from our desired distance we are
            float x = distance - _targetDistance;

            // Same spring formula as original:
            // springForce = (x * k) - (relativeVelocity * d)
            float springForce = (x * _springStrength) - (directionVelocity * _springDamper);

            // We want to pull towards the target, i.e. from body back to target:
            // forceDirection = -directionFromTargetToBody
            if (_onlyAttract && springForce < 0f)
            {
                // This would push away from the target; clamp if we only want attraction
                springForce = 0f;
            }

            Vector3 force = -directionFromTargetToBody * springForce;
            _lastSpringForce = springForce;

            _rigidbody.AddForce(force, ForceMode.Force);

            if (_applyGravityOnlyWhenOutsideTargetDistance)
            {
                // Apply gravity only when outside the "rest" distance
                _rigidbody.useGravity = distance > _targetDistance;
            }
        }

        private void OnDrawGizmos()
        {
            if (!enabled)
                return;

            if (_rigidbody == null || _target == null)
                return;

            Vector3 origin = _rigidbody.worldCenterOfMass;
            Vector3 targetPosition = _target.position;

            Vector3 toBody = origin - targetPosition;
            float distance = toBody.magnitude;

            // Line from target to body
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(targetPosition, origin);

            // Target point
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(targetPosition, 0.05f);

            // Body COM
            Gizmos.color = Color.white;
            Gizmos.DrawCube(origin, Vector3.one * 0.05f);

            // Visualize target distance as a sphere around the target
            if (_targetDistance > 0f)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(targetPosition, _targetDistance);
            }

            // If we’re outside target distance, show direction arrow
            if (distance > 0.001f)
            {
                Vector3 dir = (origin - targetPosition).normalized;
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(origin, -dir * 0.25f); // arrow pointing towards target
            }
        }
    }
}
