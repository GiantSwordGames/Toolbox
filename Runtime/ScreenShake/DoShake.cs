using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class DoShake : MonoBehaviour
    {
        [SerializeField] private TargetTransform _targetTransform;
        [SerializeField] private bool _targetCamera = true;
        [SerializeField] private bool _triggerOnEnable = false;
        [SerializeField] private ScreenShakeAsset _screenShakeAsset;
        private float _time = 0f;
        private bool _running = false;
        private Vector3 _positionOffset = Vector3.zero;
        private Quaternion _rotationOffset = Quaternion.identity;

        private void OnEnable()
        {
            if (ValidationUtility.IsPrefabAsset(this))
            {
                return;
            }
            
            _targetTransform.Initialize(transform);
            if (_targetCamera)
            {
                _targetTransform.target =  FindObjectOfType<ScreenShakeCameraReference>().transform;
            }
            ResetState();
            _screenShakeAsset.listenForButtonTest += Trigger;

            if (_triggerOnEnable)
            {
                Trigger();
            }
        }

        private void OnDisable()
        {
            _screenShakeAsset.listenForButtonTest -= Trigger;
            ResetState();
        }

        private void ResetState()
        {
            if (_targetTransform)
            {
                _targetTransform.localPosition -= _positionOffset;
                _targetTransform.localRotation = Quaternion.Inverse(_rotationOffset);
            }
            _positionOffset = Vector3.zero;
            _rotationOffset = Quaternion.identity;
            _time = 0f;
        }

        private void Update()
        {
            if (_running)
            {
                float deltaTime = TimeHelper.GetDeltaTime(_screenShakeAsset.timeScale);
                _time += deltaTime;
                _targetTransform.localPosition -= _positionOffset;
                _targetTransform.localRotation = Quaternion.Inverse(_rotationOffset);

                _positionOffset = _screenShakeAsset.EvaluatePosition(_time);
                _rotationOffset = Quaternion.Euler(_screenShakeAsset.EvaluateRotation(_time));
                
                _targetTransform.localPosition += _positionOffset;
                _targetTransform.localRotation *= _rotationOffset;
                
                if(_time >= _screenShakeAsset.duration)
                {
                    ResetState();
                    _running = false;
                }
            }
        }


        [Button]
        public void Trigger()
        {
            ResetState();
            _running = true;
        }
    }
}