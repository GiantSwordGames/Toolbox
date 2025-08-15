using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class ImpactTrigger : MonoBehaviour
    {
        [SerializeField] private TagAsset[] exclude;
        [SerializeField] private bool _onlyTriggerOnce;
        private bool _triggered;
        public UnityEvent<Collider> _onCollisionEnter;
        
        private void OnCollisionEnter(Collision other)
        {
            if (_triggered && _onlyTriggerOnce)
            {
                return;
            }
            List<TagAsset> tagAssets = other.collider.GetTagsInParents();

            if (tagAssets.ContainsAny(exclude))
            {
                return;
            }
            
            _onCollisionEnter.Invoke(other.collider);
            _triggered = true;

            
        }
    }
}
