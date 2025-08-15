using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    public class OverlapCheck2D : MonoBehaviour
    {
        [SerializeField] private bool _debugDraw;
        [SerializeField] private bool _debugAlwayEvaluate;
        [SerializeField] private bool _triggerOnEnable = true;
        [FormerlySerializedAs("_layermaskAsset")] [SerializeField] private LayermaskAsset _layermask;
        [SerializeField] List<Collider2D> _colliders = new List<Collider2D>();
        private List<Collider2D> _results = new List<Collider2D>();
       [ShowNativeProperty]  int overlapCount => _results.Count;
        
        public List<Collider2D> results => _results;

        public List<Collider2D> colliders => _colliders;

        public void Evaluate()
        {
            _results.Clear();
            foreach (Collider2D collider in _colliders)
            {
                var overlapping = collider.GetOverlappingColliders();
                foreach (Collider2D overlappingCollider in overlapping)
                {
                    if (overlappingCollider.isTrigger)
                    {
                        continue;
                    }
                    
                    if (_layermask)
                    {
                        if (_layermask.Contains(overlappingCollider) == false)
                        {
                            continue;
                        }
                    }
                    
                    if (colliders.Contains(overlappingCollider) == false)
                    {
                        _results.Add(overlappingCollider);
                    }
                }
            }
        }

        [Button]
        public void Trigger()
        {
            Evaluate();
            foreach (var collider in results)  
            {
                Debug.Log(collider.name, collider);
            }
        }

        [Button]
        public void Clear()
        {
            _results.Clear();
        }
        

        private void OnDrawGizmos()
        {
            if (_debugAlwayEvaluate)
            {
                Evaluate();
            }
        }
    }
}