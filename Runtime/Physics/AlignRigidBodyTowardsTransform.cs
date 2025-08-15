using UnityEngine;

namespace JamKit
{
    public class AlignRigidBodyTowardsTransform : AlignRigidBodyToDirectionBase
    {
        [SerializeField] private Transform _locationTransform;
        protected override Vector3 direction => _locationTransform? transform.position.To(_locationTransform.position).normalized: Vector3.forward;
    }
}