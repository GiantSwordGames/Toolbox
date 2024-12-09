using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{

    public class ExplostionDamage : MonoBehaviour
    {
        [SerializeField] private Damage _damage;
        [SerializeField] private float _radius = 5;
        [SerializeField] private MonoBehaviour _sender;
         private  List<DamageIncident> _lastDamageIncidents = new List<DamageIncident>();

        [Button]
        public void Trigger()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
            List<Collider> _collidersToDamage = new List<Collider>();
            List<Rigidbody> _rigidbodiesToDamage = new List<Rigidbody>();
            
            MonoBehaviour sender = _sender;
            if (sender == null)
            {
                sender = this;
            }

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    Collider collider = colliders[i];
                    Rigidbody rigidbody = collider.GetComponentInParent<Rigidbody>();
                    if (rigidbody)
                    {
                        if (_rigidbodiesToDamage.Contains(rigidbody) == false)
                        {
                            _rigidbodiesToDamage.Add(rigidbody); 
                        }
                    }
                    else
                    {
                        _collidersToDamage.Add(collider);
                    }
                }

                foreach (var collider in _collidersToDamage)
                {
                    float squareRadius = _radius * _radius;
                    float squareDistance = Vector3.SqrMagnitude(transform.position - collider.transform.position);
                    float attenuation = (squareRadius - squareDistance) / squareRadius;
                    attenuation = Mathf.Max(0, 1f);
                    Vector3 direction = transform.position.To(collider.transform.position).normalized;
                    var lastDamageIncident = DamageIncident.Create(_damage, sender, collider, transform.position,  direction, -direction, attenuation);
                    Debug.DrawLine(transform.position, collider.transform.position, Color.red, 5);
                    _lastDamageIncidents.Add(lastDamageIncident);
                }
                
                foreach (var rigidbody in _rigidbodiesToDamage)
                {
                    float squareRadius = _radius * _radius;
                    float squareDistance = Vector3.SqrMagnitude(transform.position - rigidbody.transform.position);
                    float attenuation = (squareRadius - squareDistance) / squareRadius;
                    Vector3 direction = transform.position.To(rigidbody.transform.position).normalized;
                    var lastDamageIncident = DamageIncident.Create(_damage, sender, rigidbody, transform.position,  direction, -direction, attenuation);
                    Debug.DrawLine(transform.position, rigidbody.transform.position, Color.red, 5);
                    _lastDamageIncidents.Add(lastDamageIncident);
                }

            }
            
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
            
        }
    }
}