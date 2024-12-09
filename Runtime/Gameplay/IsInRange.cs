using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class IsInRange : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        [SerializeField] private SmartFloat _range = 2f;

        [SerializeField] private UnityEvent _onEnterRange;
        [SerializeField] private UnityEvent _onExitRange;
        
        private float _targetDistance = float.PositiveInfinity;

        private void Start()
        {
            _targetDistance = transform.position.DistanceTo(_target.position);
            if ( _targetDistance < _range)
            {
                _onEnterRange?.Invoke();
            }
            else if (  _targetDistance >= _range)
            {
                _onExitRange?.Invoke();
            }
        }

        private void Update()
        {
            float previousDistance = this._targetDistance;
            _targetDistance = transform.position.DistanceTo(_target.position);
            if (previousDistance >= _range && _targetDistance < _range)
            {
                _onEnterRange?.Invoke();
            }
            else if (previousDistance < _range && _targetDistance >= _range)
            {
                _onExitRange?.Invoke();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if(transform== null)
                return;
            
            Gizmos.color = Color.yellow.WithAlpha(0.5f);
            
            Gizmos.DrawLine(transform.position, _target.position);
            Gizmos.DrawSphere(_target.position, 0.1f);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 0.001f, 1));
            Gizmos.DrawWireSphere(Vector3.zero, _range);
        }
    }
}
