using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    public abstract class AnimateBase : MonoBehaviour
    {
        enum TriggerBehavior
        {
            WaitForTrigger,
            RunOnStart,
            RunOnEnable,
        }
        
        enum ResetBehaviour
        {
            ResetOnEnable,
            ResetOnDisable,
            None,
        }

        enum TimeMode
        {
            Scaled,
            Unscaled,
        }
        
        enum Mode
        {
            Additive,
            Absolute,
        }
        
        [SerializeField] private TriggerBehavior _startBehavior = TriggerBehavior.RunOnEnable;
        [SerializeField] private  ResetBehaviour _resetBehaviour = ResetBehaviour.ResetOnDisable;
        [SerializeField] private TimeMode _timeMode = TimeMode.Scaled;
        [SerializeField] private Mode _mode = Mode.Additive;

        [FormerlySerializedAs("lerp")]
        [Range(0,1)]
        [SerializeField] public float _lerp = 1f;
        [SerializeField] protected SmartTween _tweenSettings;
        [SerializeField] float _duration = Mathf.Infinity;
        [SerializeField] float _resultOffset =0;
        [ShowNonSerializedField] bool _running = false;
        [ShowNonSerializedField] protected float _time = 0f;
        [ShowNonSerializedField] private float _lastEvaluate;

        protected bool additive => _mode == Mode.Additive;

        public bool running => _running;

        public float lerp => _lerp;

        public SmartTween tweenSettings => _tweenSettings;

        protected virtual void OnEnable()
        {
            if (_resetBehaviour == ResetBehaviour.ResetOnEnable)
            {
                Reset();
            }
            
            if (_startBehavior == TriggerBehavior.RunOnEnable)
            {
                Trigger();
            }
            
        }
        
        protected virtual void OnDisable()
        {
            if (_resetBehaviour == ResetBehaviour.ResetOnDisable)
            {
                Reset();
            }
        }

        private void OnValidate()
        {
          
        }

        protected void Start()
        {
            if (_startBehavior == TriggerBehavior.RunOnStart)
            {
                Trigger();
            }
        }

        [Button("Reset")]
        private void ResetButton()
        {
            Reset();
        }
        
        public virtual void Reset()
        {
            _time = 0f;
            _running = false;
        }
        
        [Button]
        public void Restart()
        {
            Reset();
            Trigger();
        }
        
        protected float EvaluateTween()
        {
            float inputTime = _time;
            if (float.IsPositiveInfinity(_duration) == false)
            {
                if (_time >= _duration) 
                {
                    _running = false;
                }
                inputTime = Mathf.Clamp(_time, 0, _duration);
            }

            // inputTime += _timeOffset;

            _lastEvaluate = (_tweenSettings.Evaluate(inputTime) + _resultOffset) *_lerp;
            return _lastEvaluate;
        }

        void Update()
        {
            if (_running)
            {
                if (_timeMode == TimeMode.Scaled)
                {
                    _time += Time.deltaTime;
                }
                else
                {
                    _time += Time.unscaledDeltaTime;
                }
                Evaluate();
            }
        }

        protected abstract void Evaluate();
        
        [Button]
        public void Trigger()
        {
            Reset();
            _running = true;
            Evaluate();
        }

        public void Stop()
        {
            _running = false;
        }
    }
}