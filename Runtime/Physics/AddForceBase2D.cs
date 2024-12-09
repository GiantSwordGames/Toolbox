using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public abstract class AddForceBase2D : MonoBehaviour
    {
        public enum  Space
        {
            World,
            LocalToRigidBody, 
            LocalToThisTransform
        }
        [SerializeField] protected Rigidbody2D _rigidbody2D;
        [SerializeField] protected Vector2 _normalizedForce = Vector2.right;
        [SerializeField] protected SmartFloat _multiplier = 1f;
        [SerializeField] protected Space _space = Space.World;
        
        private float _lerp = 1;

        public Vector3 force
        {
            get
            {
                switch (_space)
                {
                    case Space.LocalToThisTransform:
                        return transform.TransformDirection(_normalizedForce).normalized;
                    
                    case Space.LocalToRigidBody:
                        return _rigidbody2D.transform.TransformDirection(_normalizedForce).normalized;
                    
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
        

        public abstract void Trigger();

    }
}