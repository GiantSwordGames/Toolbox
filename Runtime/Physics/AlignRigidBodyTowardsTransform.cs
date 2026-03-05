using UnityEngine;

namespace JamKit
{
    public class AlignRigidBodyTowardsTransform : AlignRigidBodyToDirectionBase
    {
        [SerializeField] private Transform _locationTransform;
        public override Vector3 direction => _locationTransform? transform.position.To(_locationTransform.position).normalized: Vector3.forward;

        public Transform locationTransform
        {
            get => _locationTransform;
            set => _locationTransform = value;
        }
    }
}