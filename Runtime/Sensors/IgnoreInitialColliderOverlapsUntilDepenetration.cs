using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class IgnoreInitialColliderOverlapsUntilDepenetration : MonoBehaviour
    {
        [SerializeField] private OverlapCheck _overlapCheck;
        private List<Collider> _initialOverlaps = new List<Collider>();
        [ShowNativeProperty] private int remainingOverlaps => _initialOverlaps.Count;

        void Start()
        {
            if (_overlapCheck == null)
            {
                enabled = false;
                return;
            }
            _overlapCheck.Evaluate();
            _initialOverlaps = new List<Collider>( _overlapCheck.overlappingColliders);
            foreach (Collider otherCollider in _initialOverlaps)
            {
                foreach (Collider collider in _overlapCheck.colliders)
                {
                    Physics.IgnoreCollision(collider, otherCollider);
                }
            }
        }

        private void Update()
        {
            if (_initialOverlaps.Count > 0)
            {
                _overlapCheck.Evaluate();
                for (var index = _initialOverlaps.Count - 1; index >= 0; index--)
                {
                    var intialOverlap = _initialOverlaps[index];
                    if (_overlapCheck.overlappingColliders.Contains(intialOverlap) == false)
                    {
                        foreach (Collider collider in _overlapCheck.colliders)
                        {
                            Physics.IgnoreCollision(intialOverlap, collider, false);
                        }

                        _initialOverlaps.Remove(intialOverlap);
                    }
                }
            }
            else
            {
                // enabled = false;
            }
        }
    }
}
