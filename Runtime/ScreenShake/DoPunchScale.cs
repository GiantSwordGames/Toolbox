using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class DoPunchScale : MonoBehaviour
    {
        [SerializeField] private TargetTransform _target;
        [FormerlySerializedAs("_punchScaleAsset")] [SerializeField] private PunchAsset _punchAsset;
        private PunchInstance _instance;

        void OnEnable()
        {
            _target.Initialize(this);
            _punchAsset.ListenForButtonTest += Trigger;
        }
        
        void OnDisable()
        {
            _punchAsset.ListenForButtonTest -= Trigger;
        }
        
        [Button]
        public void Trigger()
        {
            Stop();
            _instance = _punchAsset.ApplyToScale(_target.target);
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