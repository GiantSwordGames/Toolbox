using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class OverlapCheck : MonoBehaviour
    {
        [SerializeField] private bool _debugDraw;
        [SerializeField] private bool _debugAlwayEvaluate;
        [SerializeField] List<Collider> _colliders = new List<Collider>();
        private List<Collider> _overlappingColliders = new List<Collider>();
       [ShowNativeProperty]  int overlapCount => _overlappingColliders.Count;
        
        public List<Collider> overlappingColliders => _overlappingColliders;

        public List<Collider> colliders => _colliders;

        public void Evaluate()
        {
            _overlappingColliders.Clear();
            foreach (Collider collider in _colliders)
            {
                var overlapping = collider.GetOverlappingColliders();
                foreach (Collider overlappingCollider in overlapping)
                {
                    if (colliders.Contains(overlappingCollider) == false)
                    {
                        _overlappingColliders.Add(overlappingCollider);
                    }
                }
            }
        }

        [Button]
        public void ManualEvaluate()
        {
            Evaluate();
            foreach (var collider in overlappingColliders)  
            {
                Debug.Log(collider.name, collider);
            }
        }

        [Button]
        public void Clear()
        {
            _overlappingColliders.Clear();
        }
        

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;

            foreach (Collider collider in _colliders)
            {
                if (collider is BoxCollider boxCollider)
                {
                    boxCollider.DrawWireGizmos();
                }
                if (collider is SphereCollider sphereCollider)
                {
                    sphereCollider.DrawWireGizmos();
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_debugAlwayEvaluate)
            {
                Evaluate();
            }
            
            if (_debugDraw)
            {
              
                Gizmos.color = Color.yellow;

                foreach (Collider collider in _colliders)
                {
                    collider.DrawWireGizmos();
                }

                Gizmos.color = Color.red;

                foreach (var overlapping in overlappingColliders)
                {
                    overlapping.DrawWireGizmos();
                }
            }
        }
    }
}