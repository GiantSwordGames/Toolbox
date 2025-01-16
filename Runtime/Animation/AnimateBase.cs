using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public abstract class AnimateBase : MonoBehaviour
    {
        enum TriggerBehavior
        {
            WaitForTrigger,
            RunOnStart,
        }
        
        enum ResetBehaviour
        {
            ResetOnEnable,
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
        
        [FormerlySerializedAs("_behavior")] [SerializeField] private TriggerBehavior _startBehavior = TriggerBehavior.RunOnStart;
        [FormerlySerializedAs("_stopBehavior")] [SerializeField] private  ResetBehaviour _resetBehaviour = ResetBehaviour.ResetOnEnable;
        [SerializeField] private TimeMode _timeMode = TimeMode.Scaled;
        [SerializeField] private Mode _mode = Mode.Additive;

        [Range(0,1)]
        [SerializeField] public float lerp = 1f;
        [HideInInspector] [SerializeField] protected bool _additive = true;
        [SerializeField] protected SmartTween _tweenSettings;
        [SerializeField] float _duration = Mathf.Infinity;
        [ShowNonSerializedField] bool _running = false;
        [ShowNonSerializedField] protected float _time = 0f;
        [ShowNonSerializedField] private float _lastEvaluate;

        protected bool additive => _mode == Mode.Additive;

        protected virtual void OnEnable()
        {
            if (_resetBehaviour == ResetBehaviour.ResetOnEnable)
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

            _lastEvaluate = _tweenSettings.Evaluate(inputTime)*lerp;
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
        }

        public void Stop()
        {
            _running = false;
        }
    }
}