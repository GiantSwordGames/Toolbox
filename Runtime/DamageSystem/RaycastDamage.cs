using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class RaycastDamage : Raycaster
    {
        [SerializeField] private Damage _damage;
        [SerializeField] private MonoBehaviour _sender;
        [ShowNonSerializedField] private DamageIncident _lastDamageIncident;

        [Button]
        public new DamageIncident Trigger()
        {
            RaycastHit raycastHit = Raycast();
            if(raycastHit.collider)
            {
                MonoBehaviour sender = _sender;
                if (sender == null)
                {
                    sender = this;
                }
                
                _lastDamageIncident = DamageIncident.Create(_damage, sender, raycastHit.collider, raycastHit.point,  transform.forward, raycastHit.normal);

                return _lastDamageIncident;
            }
            
            return null;
        }

        public void Clear()
        {
            _lastDamageIncident = null;
            this.colliders.Clear();
        }

        public static void Create()
        {
            GameObject go = new GameObject("DamageInstance");
            RaycastDamage raycastDamage = go.AddComponent<RaycastDamage>();
        }
        
    }
}
