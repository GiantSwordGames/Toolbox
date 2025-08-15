using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class HealthMonitor : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private float _value;
        [SerializeField] private UnityEvent _onTrigger;
        private float previousValue;
        [SerializeField] private State _state;

        enum State
        {
            LessThan,
            GreaterThan,
        }


        private void Start()
        {
            previousValue = _health.currentHealth;
            _health.onDamageTaken.AddListener(Reevaluate);
        }

        void Reevaluate()
        {
            if (_state == State.LessThan)
            {
                if (_health.currentHealth < _value && previousValue >= _value)
                {
                    _onTrigger?.Invoke();
                }
            }
            else
            {
                if (_health.currentHealth > _value && previousValue <= _value)
                {
                    _onTrigger?.Invoke();
                }
            }

            previousValue = _health.currentHealth;
        }
    }
}
