using JamKit;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace JamKit
{
    public class DoPunchScale : DoPunch
    {
        [SerializeField] private bool _triggerOnEnable = false;
        [SerializeField] private Transform _target;
        [SerializeField] private PunchAsset _punchAsset;
        private PunchInstance _instance;
        
        void OnEnable()
        {
            if (_punchAsset)
            {
                _punchAsset.ListenForButtonTest += Trigger;
            }
            if (_triggerOnEnable)
            {
                Trigger();
            }
        }
        
        void OnDisable()
        {
            // if (gameObject)
            // {
                // _punchAsset.ListenForButtonTest -= Trigger;
            // }            
        }
        
        public override void Trigger()
        {
            if (_target == null)
            {
                _target = this.transform;
            }
            Stop();
            _instance = _punchAsset.ApplyToScale(_target);
        }
        public Coroutine GetCoroutine()
        {
            if (_instance != null)
            {
                return _instance.routine;
            }
            
            return null;
        }

        private void Stop()
        {
            if(_instance != null)
            {
                _instance.Kill();
                _instance = null;
            }
        }
    }
}