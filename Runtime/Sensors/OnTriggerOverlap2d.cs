using System;
using System.Collections.Generic;
using GiantSword;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class OnTriggerOverlap2d: MonoBehaviour
    {
         [SerializeField] private TagAsset[] _filterIncludeTags;
        [SerializeField] private Transform[] _ignoreNestedColliders;
        [SerializeField] private bool _dontClearUntilDisable =false;
        [FormerlySerializedAs("_filterLayer")] [SerializeField] private LayermaskAsset _layerMask;

        [Space]
        private List<Collider2D> _overlappingColliders = new List<Collider2D>();
        private List<Rigidbody2D> _rigidbodies = new List<Rigidbody2D>();
        private List<Health> _healths = new List<Health>();

        [ShowNativeProperty] public int overlappingColliderCount => _overlappingColliders.Count;
        [ShowNativeProperty]  private int overlappingRigidBodies => _rigidbodies.Count;
        [ShowNativeProperty]  private int overlappingHealthCount => _healths.Count;
        
        [FormerlySerializedAs("onTriggerEnter")] [Foldout("On Trigger Enter")]
        public UnityEvent<Collider2D> onColliderEnter;

        public UnityEvent<Collider2D> onColliderExit { get; set; } = new UnityEvent<Collider2D>();

        public List<Rigidbody2D> rigidbodies => _rigidbodies;

        [Foldout("On Rigidbody Enter")]
        public UnityEvent<Rigidbody2D> onRigidbodyEnter;
        public UnityEvent<Rigidbody2D> onRigidbodyExit;
       
        [Foldout("On Health Enter")]
        public UnityEvent<Health> onHealthEnter;
        
        [Foldout("On Player Enter")]
        public UnityEvent<Player> onPlayerEnter;
        
        [Foldout("On Player Enter")]
        public UnityEvent<Player> onPlayerExit;

        private void OnDisable()
        {
            _overlappingColliders.Clear();
            _rigidbodies.Clear();
            _healths.Clear();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (FilterOut(other)) return;
            
            _overlappingColliders.Add(other);
            
            onColliderEnter?.Invoke(other);

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
            if (_layerMask)
            {
                if (_layerMask.Contains(other) == false)
                    return true;
            }
            if(_filterIncludeTags.Length > 0)
            {
                bool match = false;
                List<TagAsset> tagList = other.GetTagsOnGameObject();
                    foreach (TagAsset otherTag in tagList)
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

           
            return false;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_dontClearUntilDisable)
            {
                return;
            }
            if (FilterOut(other)) return;

            _overlappingColliders.Remove(other);
            
            onColliderExit?.Invoke(other);
            
            Rigidbody2D rigidbody = other.GetComponentInParent<Rigidbody2D>();
            if (rigidbody)
            {
                if (_rigidbodies.Contains(rigidbody))
                {
                    _rigidbodies.Remove(rigidbody);
                    onRigidbodyExit?.Invoke(rigidbody);
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
            return _overlappingColliders;
        }

        public bool IsOverlapping()
        {
            return _overlappingColliders.Count > 0;
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