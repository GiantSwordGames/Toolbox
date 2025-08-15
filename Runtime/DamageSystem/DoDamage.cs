using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class DoDamage : MonoBehaviour
    {
        [SerializeField] private Damage _damage;
        [SerializeField] private MonoBehaviour _sender;
        [SerializeField] private SmartFloat _cooldown = new SmartFloat(0f);
        [ShowNonSerializedField] private DamageIncident _lastDamageIncident;
        private float lastTimeStamp = 0;

        public void ApplyDamageToCollider(Collider collider)
        {
            if(Time.time - lastTimeStamp < _cooldown)
            {
                return;
            }
            lastTimeStamp = Time.time;
            Health health = collider.GetComponentInParent<Health>();
            if (health)
            {
                ApplyDamageTo(health);
            }
        }
        public void ApplyDamageTo(Health health)
        {
            MonoBehaviour sender = _sender;
            if (sender == null)
            {
                sender = this;
            }
            
            Vector3 direction = transform.position.To(health.transform.position);
            _lastDamageIncident = DamageIncident.Create(_damage, sender, health, transform.position,  transform.forward, direction);
        }
    }
}
