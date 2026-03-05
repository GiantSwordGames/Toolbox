using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    public class AlignRigidBodyToDirection : AlignRigidBodyToDirectionBase
    {
        [SerializeField]
        private Vector3 _direction  = Vector3.forward;

        public override Vector3 direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
    }
}