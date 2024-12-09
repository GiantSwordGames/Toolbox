using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class TimerTrigger : MonoBehaviour
    {
        [SerializeField] private SmartFloat _duration = 1f;
        [SerializeField] private SmartFloat _durationRandom = 0f;
        [SerializeField] private UnityEvent _onElapse;
        [SerializeField] private bool _repeating = false;
        [ShowNonSerializedField] private float _randomizedDuration = 0;
        [ShowNonSerializedField] private float _currentTime = 0;
        
        private void OnEnable()
        {
            Reset();
        }
        
        [Button("Reset")]
        public void Reset()
        {
            _randomizedDuration = _duration.value + _durationRandom.value*Random.value;
            _currentTime = 0;
        }
        
        private void Update()
        {
            if (_currentTime < _randomizedDuration)
            {
                _currentTime += Time.deltaTime;

                _currentTime = Mathf.Min(_currentTime, _randomizedDuration);
                if (_currentTime >= _randomizedDuration)
                {
                    _onElapse?.Invoke();
                    if (_repeating)
                    {
                        Reset();
                    }
                }
            }
        }
    }
}
