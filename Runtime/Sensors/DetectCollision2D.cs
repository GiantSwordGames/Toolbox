using System;
using System.Collections.Generic;
using GiantSword;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Hardgore
{
    public class DetectCollision2D: MonoBehaviour
    {
         [SerializeField] private TagAsset[] _filterIncludeTags;
        [SerializeField] private Transform[] _ignoreNestedColliders;
        [SerializeField] private bool _dontClearUntilDisable =false;
        [FormerlySerializedAs("_occludedByLayers")] 
        [SerializeField] private LayermaskAsset _layerMask;

        [Space]
        private List<Collider2D> _contactingColliders = new List<Collider2D>();
        private List<Rigidbody2D> _rigidbodies = new List<Rigidbody2D>();
        private List<Health> _healths = new List<Health>();

        [ShowNativeProperty] public int contactingColliderCount => _contactingColliders.Count;
        [ShowNativeProperty]  private int contactingRigidBodies => _rigidbodies.Count;
        [ShowNativeProperty]  private int contactingHealthCount => _healths.Count;
        
        
        [Foldout("On Collision Enter")]
        public UnityEvent<Collision2D> onCollisionEnter;
        
        [FormerlySerializedAs("onCollisionEnter")] [Foldout("On Collider Enter")]
        public UnityEvent<Collider2D> onColliderEnter;
     
        [Foldout("On Rigidbody Enter")]
        public UnityEvent<Rigidbody2D> onRigidbodyEnter;
       
        [Foldout("On Health Enter")]
        public UnityEvent<Health> onHealthEnter;
        
        [Foldout("On Player Enter")]
        public UnityEvent<Player> onPlayerEnter;

        private void OnDisable()
        {
            _contactingColliders.Clear();
            _rigidbodies.Clear();
            _healths.Clear();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Collider2D other = collision.collider;
            if (FilterOut(other)) return;
            
            _contactingColliders.Add(other);
            
            onColliderEnter?.Invoke(other);
            onCollisionEnter?.Invoke(collision);

            Rigidbody2D rigidbody = other.GetComponentInParent<Rigidbody2D>();
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
                }
            }
            
            Player player = other.GetComponentInParent<Player>();
            if (player)
            {
                onPlayerEnter?.Invoke(player);
            }

        }

        private bool FilterOut(Collider2D other)
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

            if (_layerMask)
            {
                if (_layerMask.Contains(other) == false)
                {
                    return true;
                }
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

        private void OnCollisionExit2D(Collision2D collision)
        {
            Collider2D other = collision.collider;
            if (_dontClearUntilDisable)
            {
                return;
            }
            if (FilterOut(other)) return;

            _contactingColliders.Remove(other);
            
            Rigidbody2D rigidbody = other.GetComponentInParent<Rigidbody2D>();
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

        public List<Collider2D> GetColliders()
        {
            return _contactingColliders;
        }

        private void Update()
        {
            for (int i = _contactingColliders.Count - 1; i >= 0; i--)
            {
                // Remove null or inactive colliders. On Exit does not get called for when colliders get disabled
                if (_contactingColliders[i] == null || _contactingColliders[i].gameObject.activeInHierarchy == false)
                {
                    _contactingColliders.RemoveAt(i);
                }
            }
        }
    }
}