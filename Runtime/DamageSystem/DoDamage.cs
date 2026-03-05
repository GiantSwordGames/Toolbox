using NaughtyAttributes;
using UnityEngine;

namespace JamKit
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
            JamKitHealth jamKitHealth = collider.GetComponentInParent<JamKitHealth>();
            if (jamKitHealth)
            {
                ApplyDamageTo(jamKitHealth);
            }
        }
        public void ApplyDamageTo(JamKitHealth jamKitHealth)
        {
            MonoBehaviour sender = _sender;
            if (sender == null)
            {
                sender = this;
            }
            
            Vector3 direction = transform.position.To(jamKitHealth.transform.position);
            _lastDamageIncident = DamageIncident.Create(_damage, sender, jamKitHealth, transform.position,  transform.forward, direction);
        }
    }
}
