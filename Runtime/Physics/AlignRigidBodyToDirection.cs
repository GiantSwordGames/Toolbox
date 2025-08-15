using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class AlignRigidBodyToDirection : AlignRigidBodyToDirectionBase
    {
        [SerializeField]
        private Vector3 _direction  = Vector3.forward;

        protected override Vector3 direction => _direction;
    }
}