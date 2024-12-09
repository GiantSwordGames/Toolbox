using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class DoPunchScale : MonoBehaviour
    {
        [SerializeField] private TargetTransform _target;
        [SerializeField] private PunchScaleAsset _punchScaleAsset;
        private PunchInstance _instance;

        void OnEnable()
        {
            _target.Initialize(this);
            _punchScaleAsset.ListenForButtonTest += Trigger;
        }
        
        void OnDisable()
        {
            _punchScaleAsset.ListenForButtonTest -= Trigger;
        }
        
        [Button]
        public void Trigger()
        {
            Stop();
            _instance = _punchScaleAsset.Apply(_target.target);
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