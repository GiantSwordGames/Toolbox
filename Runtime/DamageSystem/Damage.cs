using System.Collections.Generic;
using UnityEngine;

namespace JamKit
{
    public class Damage : ScriptableObject
    {
        [SerializeField] private float _damage;
        [SerializeField] private float _knockBack;
        [SerializeField] private List<TagAsset> _tagList;

        public float damage
        {
            get => _damage;
            set => _damage = value;
        }

        public float knockBack => _knockBack;


        public override string ToString()
        {
            return "Damage: " + damage;
        }

        public bool TryApplyDamage(MonoBehaviour from, GameObject to)
        {
            // // Health fromHealth = from.GetComponentInParent<Health>();
            // Health toHealth = to.GetComponentInParent<Health>();
            //
            // if (toHealth == null)
            // {
            //     return false;
            // }
            //
            // if (toHealth == fromHealth)
            // {
            //     return false;
            // }
            //
            // toHealth.ApplyDamage(from, this);
            //
            // TryDoKnock(from, to );

            return true;
        }
        
      

        private void TryDoKnock(MonoBehaviour from, GameObject to)
        {
            Rigidbody fromRigidbody = null;
            if (from)
            {
                fromRigidbody = from.GetComponentInChildren<Rigidbody>();
            }
            Rigidbody otherRigidbody = to.GetComponentInChildren<Rigidbody>();

            if (otherRigidbody == null || to == null)
            {
                return;
            }
            
            Vector3 direction = fromRigidbody.transform.position.To(otherRigidbody.position);
            direction.y = 0;
            direction.Normalize();
            
            Debug.DrawRay(otherRigidbody.transform.position, direction, Color.red, 5f);

            otherRigidbody.AddForce (direction*this.knockBack, ForceMode.Impulse);
        }
    }
}