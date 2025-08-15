using System.Collections.Generic;
using GiantSword;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class OnTriggerEnterListener: MonoBehaviour
    {
         [SerializeField] private TagAsset[] _filterIncludeTags;
        [SerializeField] private Transform[] _ignoreNestedColliders;
        [SerializeField] private bool _dontClearUntilDisable =false;
        [SerializeField] private LayermaskAsset _occludedByLayers;

        [Space]
        private List<Collider> _overlappingColliders = new List<Collider>();
        private List<Rigidbody> _rigidbodies = new List<Rigidbody>();

        [ShowNativeProperty] public int overlappingColliderCount => _overlappingColliders.Count;
        [ShowNativeProperty]  private int overlappingRigidBodies => _rigidbodies.Count;
        
        public UnityEvent<Collider> onColliderEnter;
     
        public UnityEvent<Rigidbody> onRigidbodyEnter;
        public UnityEvent onTriggerEnter;
       
        private void OnDisable()
        {
            _overlappingColliders.Clear();
            _rigidbodies.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(enabled ==false)
                return;
            
            if (FilterOut(other)) return;
            
            _overlappingColliders.Add(other);
            
            onColliderEnter?.Invoke(other);
            onTriggerEnter?.Invoke();

            Rigidbody rigidbody = other.GetComponentInParent<Rigidbody>();
            if (rigidbody && rigidbody.isKinematic == false)
            {
                if (_rigidbodies.Contains(rigidbody) == false)
                {
                    _rigidbodies.Add(rigidbody);
                    onRigidbodyEnter?.Invoke(rigidbody);
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