using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JamKit
{
    public class HealthMonitor : MonoBehaviour
    {
        [FormerlySerializedAs("_health")] [SerializeField] private JamKitHealth _jamKitHealth;
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
            previousValue = _jamKitHealth.currentHealth;
            _jamKitHealth.onDamageTaken.AddListener(Reevaluate);
        }

        void Reevaluate()
        {
            if (_state == State.LessThan)
            {
                if (_jamKitHealth.currentHealth < _value && previousValue >= _value)
                {
                    _onTrigger?.Invoke();
                }
            }
            else
            {
                if (_jamKitHealth.currentHealth > _value && previousValue <= _value)
                {
                    _onTrigger?.Invoke();
                }
            }

            previousValue = _jamKitHealth.currentHealth;
        }
    }
}
