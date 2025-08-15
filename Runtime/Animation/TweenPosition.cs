using System;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class TweenPosition : MonoBehaviour
    {
        [SerializeField] private TargetTransform _target;
        [SerializeField] private Transform _from;
        [SerializeField] private Transform _to;
        [Min(0.0001f)]
        [SerializeField] private float _duration =1;

        [CurveRange(0,0,1,1)]
        [SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0,0,1,1 );

        [ShowNonSerializedField] private float _lerp;
        [ShowNonSerializedField] private bool _runningInReverse;
        [ShowNonSerializedField] private bool _running;

     

        [Button]
        public void Trigger()
        {
            _runningInReverse = false;
            Reset();
            _running = true;
        }
        
        [Button]
        public void TriggerInReverse()
        {
            _runningInReverse = true;
            Reset();
            _running = true;
        }
        
        [Button]
        public void Reset()
        {
            if (Application.isPlaying)
            {
                _target.Initialize(this);
                _lerp = 0;
                _running = false;
                Evaluate();
            }
        }
        
        [Button]
        public void CreateTargets()
        {
            if (Application.isPlaying == false)
            {
                _from = new GameObject("From").transform;
                _from.transform.parent = transform;
                _from.localPosition = Vector3.zero;
                
                _to = new GameObject("To").transform;
                _to.transform.parent = transform;
                _to.localPosition = Vector3.up;
                
                RuntimeEditorHelper.RegisterCreatedObjectUndo(_from.gameObject);
                RuntimeEditorHelper.RegisterCreatedObjectUndo(_to.gameObject);

            }
        }


        private void OnDrawGizmosSelected()
        {
            if (_from && _to)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_from.position, _to.position);
            }
            
        }


        protected void Evaluate()
        {
            float input = _lerp / _duration;
            if (_runningInReverse)
            {
                input = 1- input;
            }
            float value = _curve.Evaluate(input);
            if (_from && _to)
            {
                _target.position = Vector3.Lerp(_from.position, _to.position, value);
            }
        }

        protected void Update()
        {
            if (_running)
            {
                _lerp += Time.deltaTime/_duration;
                if (_lerp >= 1)
                {
                    _running = false;
                    _lerp = 1;
                }
                Evaluate();
            }
        }
    }
}
