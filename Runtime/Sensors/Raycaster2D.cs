using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    
    public class Raycaster2D : MonoBehaviour
    {
        enum UpdateMode
        {
            Manual,
            OnUpdate,
            OnFixedUpdate,
            LateUpdate
        }
        
        [SerializeField] private UpdateMode _updateMode = UpdateMode.Manual;
        [SerializeField] private float _distance = 1;
        [Min(0)]
        [SerializeField] private float _thickness = 0;
        [SerializeField] private LayermaskAsset _layermask;
        [SerializeField] private bool _raycastTriggers = false;

        [SerializeField] private UnityEvent<Collider2D> _onEnter;
        [SerializeField] private UnityEvent<Collider2D> _onExit;
        [SerializeField] private UnityEvent _onMovementDetected;
        
        [SerializeField] private Vector3 _localDirection = Vector3.right;
        Vector3 worldDirection => transform.TransformVector(_localDirection);

        private List<Collider2D> _colliders = new List<Collider2D>();
        private  List<Collider2D> _previousColliders = new List<Collider2D>( );

        [ShowNonSerializedField] private float _currentDistance;
        
        protected RaycastHit2D[] _raycastHits = {};
        private RaycastHit2D _closestHit;
        
        public bool result => _raycastHits.Length > 0;

        [ShowNativeProperty] private int hitCount => _raycastHits.Length;

        public List<Collider2D> colliders => _colliders;

        public UnityEvent<Collider2D> onEnter => _onEnter;

        public UnityEvent<Collider2D> onExit => _onExit;

        public RaycastHit2D closestHit
        {
            get => _closestHit;
        }

        private void Start()
        {
            Raycast(false);
        }

        [Button]
        public void Trigger()
        {
            Raycast(false);
        }
        
        void Update()
        {
            if (_updateMode == UpdateMode.OnUpdate)
            {
                Raycast(false);
            }
        }
        
        void FixedUpdate()
        {
            if (_updateMode == UpdateMode.OnFixedUpdate)
            {
                Raycast(false);
            }
        }
        
        void LateUpdate()
        {
            if (_updateMode == UpdateMode.LateUpdate)
            {
                Raycast(false);
            }
        }

        public RaycastHit2D Raycast(bool suppressEvents = false)
        {
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore;

            if (_raycastTriggers)
            {
                queryTriggerInteraction = QueryTriggerInteraction.Collide;
            }

       
            if (_thickness == 0)
            {
                _raycastHits = Physics2D.RaycastAll(transform.position, worldDirection, _distance, _layermask);
            }
            else
            {
                _raycastHits = Physics2D.CircleCastAll(transform.position, _thickness, worldDirection, _distance, _layermask);
            }
               
            
            (_previousColliders, _colliders) = (_colliders, _previousColliders); // swap
            _colliders.Clear();
            _currentDistance = Mathf.Infinity;
            
            float closestDistance = Mathf.Infinity;
            _closestHit = default;
            
            foreach (RaycastHit2D raycastHit in _raycastHits)
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
                foreach (Collider2D previousCollider in _previousColliders)
                {
                    if (!_colliders.Contains(previousCollider))
                    {
                        _onExit?.Invoke(previousCollider);
                    }
                }

                foreach (Collider2D collider in _colliders)
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
            Gizmos.DrawRay(transform.position, worldDirection * _distance);
            Gizmos.DrawWireSphere(transform.position, _thickness);
            Gizmos.DrawWireSphere(transform.position + worldDirection * _distance, _thickness);

            if (_raycastHits.Length > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, worldDirection * _currentDistance);
            }
            
        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.DrawSphere(transform.position, 0.025f);
        //     Gizmos.DrawRay(transform.position, transform.right * 0.05f);
        // }
    }
}
