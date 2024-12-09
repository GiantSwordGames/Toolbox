using System.Collections.Generic;
using GiantSword;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Hardgore
{
    public class TriggerOverlap: MonoBehaviour
    {
         [SerializeField] private TagAsset[] _filterIncludeTags;
        [SerializeField] private Transform[] _ignoreNestedColliders;
        [SerializeField] private bool _dontClearUntilDisable =false;
        [SerializeField] private LayermaskAsset _occludedByLayers;

        [Space]
        private List<Collider> _overlappingColliders = new List<Collider>();
        private List<Rigidbody> _rigidbodies = new List<Rigidbody>();
        private List<Health> _healths = new List<Health>();

        [ShowNativeProperty] public int overlappingColliderCount => _overlappingColliders.Count;
        [ShowNativeProperty]  private int overlappingRigidBodies => _rigidbodies.Count;
        [ShowNativeProperty]  private int overlappingHealthCount => _healths.Count;
        
        [Foldout("On Trigger Enter")]
        public UnityEvent<Collider> onTriggerEnter;
     
        [Foldout("On Rigidbody Enter")]
        public UnityEvent<Rigidbody> onRigidbodyEnter;
       
        [Foldout("On Health Enter")]
        public UnityEvent<Health> onHealthEnter;
        
        [Foldout("On Player Enter")]
        public UnityEvent<Player> onPlayerEnter;

        private void OnDisable()
        {
            _overlappingColliders.Clear();
            _rigidbodies.Clear();
            _healths.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (FilterOut(other)) return;
            
            
            
            _overlappingColliders.Add(other);
            
            onTriggerEnter?.Invoke(other);

            Rigidbody rigidbody = other.GetComponentInParent<Rigidbody>();
            if (rigidbody && rigidbody.isKinematic == false)
            {
                if (_rigidbodies.Contains(rigidbody) == false)
                {
                    _rigidbodies.Add(rigidbody);
                    onRigidbodyEnter?.Invoke(rigidbody);
                }
            }
            
            Health health = other.GetComponentInParent<Health>();
            if (health)
            {
                if (_healths.Contains(health) == false)
                {
                    _healths.Add(health);
                    onHealthEnter?.Invoke(health);
                    
                    Player player = other.GetComponent<Player>();
                    if (player)
                    {
                        onPlayerEnter?.Invoke(player);
                    }
                }

            }
        }

        private bool FilterOut(Collider other)
        {
            
            if(_filterIncludeTags.Length > 0)
            {
                bool match = false;
                TagList tagList = other.GetComponentInParent<TagList>();
                if (tagList)
                {
                    foreach (TagAsset otherTag in tagList.tags)
                    {
                        foreach (TagAsset filterTag in _filterIncludeTags)
                        {
                            if (otherTag == filterTag)
                            {
                                match = true;
                                break;
                            }
                        }
                    } 
                }

                if (match == false)
                {
                    return true;
                }
            }
            
            foreach (Transform t in _ignoreNestedColliders)
            {
                if (other.transform.IsDescendentOfTransform(t))
                    return true;
            }

            if (_occludedByLayers)
            {
                // if(Physics.Linecast(transform.position, other.transform.position, out var hit,  _occludedByLayers))
                // {
                //     if (hit.collider != other)
                //     {
                //         return true;
                //     }
                // }
            }

            return false;
        }

        private void OnTriggerExit(Collider other)
        {
            if (_dontClearUntilDisable)
            {
                return;
            }
            if (FilterOut(other)) return;

            _overlappingColliders.Remove(other);
            
            Rigidbody rigidbody = other.GetComponentInParent<Rigidbody>();
            if (rigidbody)
            {
                if (_rigidbodies.Contains(rigidbody))
                {
                    _rigidbodies.Remove(rigidbody);
                }
            }
            
            Health health = other.GetComponentInParent<Health>();
            if (health)
            {
                if (_healths.Contains(health))
                {
                    _healths.Remove(health);
                }
            }
        }

        public List<Collider> GetColliders()
        {
            return _overlappingColliders;
        }

        private void Update()
        {
            for (int i = _overlappingColliders.Count - 1; i >= 0; i--)
            {
                // Remove null or inactive colliders. On Exit does not get called for when colliders get disabled
                if (_overlappingColliders[i] == null || _overlappingColliders[i].gameObject.activeInHierarchy == false)
                {
                    _overlappingColliders.RemoveAt(i);
                }
            }
        }
    }
}