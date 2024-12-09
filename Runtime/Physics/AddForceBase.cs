using NaughtyAttributes;
using UnityEngine.Serialization;

namespace GiantSword
{
    using UnityEngine;
    
    public abstract class AddForceBase : MonoBehaviour
    {
        public enum  Space
        {
            World,
            LocalToRigidBody, 
            LocalToThisTransform
        }
        [SerializeField] protected Rigidbody _rigidbody;
        [SerializeField] protected Vector3 _normalizedForce = Vector3.forward;
        [SerializeField] protected float _multiplier = 1f;
        [SerializeField] protected bool _localSpace = false;
        [SerializeField] protected Space _space = Space.World;
        
        private float _lerp = 1;

        private void OnValidate()
        {
            // if (_localSpace)
            // {
            //     _space = Space.LocalToThisTransform;
            // }
        }

        public Vector3 force
        {
            get
            {
                switch (_space)
                {
                    case Space.LocalToThisTransform:
                        return transform.TransformDirection(_normalizedForce).normalized;
                    
                    case Space.LocalToRigidBody:
                        return _rigidbody.transform.TransformDirection(_normalizedForce).normalized;
                    
                    case Space.World:
                        return _normalizedForce.normalized;
                }

                return _normalizedForce.normalized;
            }
        }
       
        public float lerp
        {
            get => _lerp;
            set => _lerp = value;
        }
        
        [Button]

        public abstract void Trigger();

    }
}
