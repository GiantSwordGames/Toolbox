using NaughtyAttributes;
using UnityEngine.Serialization;

namespace GiantSword
{
    using UnityEngine;


    public class SuspendRigidbody : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField]  private float _springStrength = 100; 
        [SerializeField]  private float _springDamper= 10;
        [SerializeField]  private float _targetHeight = 1;
        [SerializeField] private float _raycastOverShoot = 1.2f;
        [SerializeField] private LayermaskAsset _raycastLayerMask;
        [FormerlySerializedAs("_applyGravityOnlyWhenOutOfRange")]
        [Space]
        [SerializeField] private bool _applyGravityOnlyWhenAboveTargetHeight = true;
        [SerializeField] private bool _applyDownwards = true;
        [Space]
        [ShowNonSerializedField] private Collider _collider;
    
        private void FixedUpdate()
        {
            float raycastDistance = _targetHeight*_raycastOverShoot;;
             Vector3 _downDirection = Vector3.down;
    
            Vector3 origin = _rigidbody.worldCenterOfMass;
            
            Physics.Raycast(origin, _downDirection, out RaycastHit _rayHit, raycastDistance,  _raycastLayerMask.NullSafe());
    
            _collider = _rayHit.collider;
    
            var hitGround = _rayHit.collider;
    
            if (hitGround)
            {
                Vector3 velocity = _rigidbody.velocity;
                Vector3 rayDirection = _downDirection;
                Vector3 otherVelocity = Vector3.zero;
                Rigidbody hitBody = _rayHit.rigidbody;
                
                if (hitBody != null)
                    otherVelocity = hitBody.velocity;
                
                float rayDirectionVelocity = Vector3.Dot(rayDirection, velocity);
                float otherVelocityDirection = Vector3.Dot(rayDirection, otherVelocity);
                float relativeVelocity = rayDirectionVelocity - otherVelocityDirection;
                float x = _rayHit.distance - _targetHeight;
                float springForce = (x * _springStrength) - (relativeVelocity * _springDamper);
    
                if (_applyDownwards ==false)
                {
                    if (springForce > 0)
                    {
                        springForce = 0;
                    }
                }
                _rigidbody.AddForce(rayDirection * springForce);
            }
    
            if (_applyGravityOnlyWhenAboveTargetHeight)
            {
                _rigidbody.useGravity = hitGround == false;
            }
        }
        
        
        
        private void Start()
        {
            // _rigidbody = GetComponent<Rigidbody>();
        }
    
    
        private void OnDrawGizmos()
        {
    
            if (enabled == false)
                return;
            if (_rigidbody == null)
            {
                return;
            }
    
            Vector3 origin = _rigidbody.worldCenterOfMass ;
            Vector3 _downDirection = Vector3.down;
    
            float raycastDistance = _targetHeight;
            raycastDistance *= _raycastOverShoot;
            RaycastHit _rayHit;
            Physics.Raycast(origin, _downDirection, out _rayHit, raycastDistance, _raycastLayerMask);
    
            if (_rayHit.collider == null)
            {
                Gizmos.color = Color.red;
            }
    
            Gizmos.DrawRay(_rayHit.point, _rayHit.normal * _targetHeight);
            Gizmos.DrawCube(origin, new Vector3(1, 1, 1) * 0.005f);
            // Gizmos.DrawSphere(_rayHit.point, 0.01f);
            Gizmos.DrawCube(_rayHit.point, new Vector3(3, 1, 3) * 0.005f);
    
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(_rayHit.point - _downDirection * _targetHeight, new Vector3(5, 1, 5) * 0.002f);
    
            _collider = _rayHit.collider;
    
        }
    }
}
