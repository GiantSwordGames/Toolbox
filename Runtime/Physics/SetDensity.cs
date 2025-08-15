using System;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class SetDensity : MonoBehaviour
    {
        [SerializeField ]private Rigidbody _rigidbody;

        [SerializeField] PhysicalMaterialDefinition _materialType;
        // [SerializeField] private BoxCollider _referenceCollider;
        [SerializeField] private Vector3 _size = Vector3.one;
        [ShowNonSerializedField] private float _volume;
        [SerializeField] [ShowNonSerializedField] private float _multiplier =1;
        [ShowNativeProperty] private float density => _materialType?_materialType.density:0;
        [ShowNativeProperty] public float mass => _rigidbody?_rigidbody.mass:0;

        private void Reset()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnValidate()
        {
            ApplyDensity();
        }

        public void ApplyDensity()
        {
            _rigidbody = GetComponent<Rigidbody>();
                if (_rigidbody == null)
                {
                    return;
                }

                // if (_referenceCollider)
                // {
                //     Quaternion rotation = _referenceCollider.transform.rotation;
                //     _referenceCollider.transform.rotation = Quaternion.identity;
                //     _size = _referenceCollider.bounds.size;
                //     // _referenceCollider.transform.rotation = rotation;
                // }

                _volume = _size.x * _size.y * _size.z;
            _rigidbody.mass = (float)Math.Round( _volume * density*_multiplier,1);
        }

        private void OnDrawGizmosSelected()
        {
            if(_rigidbody == null)
                return;
            
            // Debug.DrawLine(transform.position, _rigidbody.worldCenterOfMass);
            // Gizmos.DrawWireCube(transform.position, _size); 
            Gizmos.matrix = Matrix4x4.TRS(_rigidbody.worldCenterOfMass, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, _size);    
            
        }

        public void SetDensityAtRuntime(PhysicalMaterialDefinition materialDefinition)
        {
            _materialType = materialDefinition;
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)
            {
                return;
            }
            _rigidbody.SetDensity(materialDefinition.density);
            _rigidbody.mass *= 12;
        }
        
    }
}
