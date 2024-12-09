using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    
    public class Raycaster : MonoBehaviour
    {
        [SerializeField] private float _distance = 1;
        [Min(0)]
        [SerializeField] private float _thickness = 0;
        [SerializeField] private LayermaskAsset _layermask;
        [SerializeField] private bool _raycastTriggers = false;

        [SerializeField] private UnityEvent<Collider> _onEnter;
        [SerializeField] private UnityEvent<Collider> _onExit;
        [SerializeField] private UnityEvent _onMovementDetected;
        
        private List<Collider> _colliders = new List<Collider>();
        private  List<Collider> _previousColliders = new List<Collider>( );

        [ShowNonSerializedField] private float _currentDistance;
        
        protected RaycastHit[] _raycastHits = {};
        private RaycastHit _closestHit;
        
        public bool result => _raycastHits.Length > 0;

        [ShowNativeProperty] private int hitCount => _raycastHits.Length;

        public List<Collider> colliders => _colliders;

        private void Start()
        {
            Raycast(true);
        }

        [Button]
        public void Trigger()
        {
            Raycast(true);
        }

        public RaycastHit Raycast(bool suppressEvents = false)
        {
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore;

            if (_raycastTriggers)
            {
                queryTriggerInteraction = QueryTriggerInteraction.Collide;
            }

            if (_thickness == 0)
            {
                _raycastHits = Physics.RaycastAll(transform.position, transform.forward, _distance, _layermask, queryTriggerInteraction);
            }
            else
            {
                _raycastHits = Physics.SphereCastAll(transform.position, _thickness, transform.forward, _distance, _layermask, queryTriggerInteraction);
            }
               
            // _raycastHits = Physics.RaycastAll(transform.position, transform.forward, _distance, _layermask, QueryTriggerInteraction.Ignore);
            (_previousColliders, _colliders) = (_colliders, _previousColliders); // swap
            _colliders.Clear();
            _currentDistance = Mathf.Infinity;
            
            float closestDistance = Mathf.Infinity;
            _closestHit = default;
            
            foreach (RaycastHit raycastHit in _raycastHits)
            {
                if (raycastHit.collider != null)
                {
                    _currentDistance = Mathf.Min(_currentDistance, raycastHit.distance);
                    _colliders.Add(raycastHit.collider);
                    
                    if (raycastHit.distance < closestDistance)
                    {
                        closestDistance = raycastHit.distance;
                        _closestHit = raycastHit;
                    }
                }
            }

            if (suppressEvents == false)
            {
                foreach (Collider previousCollider in _previousColliders)
                {
                    if (!_colliders.Contains(previousCollider))
                    {
                        _onExit?.Invoke(previousCollider);
                    }
                }

                foreach (Collider collider in _colliders)
                {
                    if (!_previousColliders.Contains(collider))
                    {
                        _onEnter?.Invoke(collider);
                    }
                }
            }

            return _closestHit;

        }


        private void OnDrawGizmosSelected()
        {
            
            Gizmos.color = Color.white;
            Gizmos.DrawRay(transform.position, transform.forward * _distance);

            if (_raycastHits.Length > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, transform.forward * _currentDistance);
            }
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position, 0.025f);
            Gizmos.DrawRay(transform.position, transform.forward * 0.05f);
        }
    }
}
