using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class TriggerBasedOnFloat : MonoBehaviour
    {
        enum Comparison
        {
            GreaterThan,
            LessThan,
            EqualTo,
            GreaterThanOrEqualTo,
            LessThanOrEqualTo,
        }

        [SerializeField] private SmartFloat _value;
        [SerializeField] private SmartFloat _compareTo;
        [SerializeField] private Comparison _comparison = Comparison.GreaterThan;
        [SerializeField] private UnityEvent _onTrigger;



        void Awake()
        {
            _value.onValueChanged += OnValueChanged;
            Rrefresh();
        }

        private void OnDestroy()
        {
            _value.onValueChanged -= OnValueChanged;
        }

        public bool EvaluateComparison()
        {
            switch (_comparison)
            {
                case Comparison.GreaterThan:
                    return _value.value > _compareTo.value;
                case Comparison.LessThan:
                    return _value.value < _compareTo.value;
                case Comparison.EqualTo:
                    return _value.value.Equals(_compareTo.value);
                case Comparison.GreaterThanOrEqualTo:
                    return _value.value >= _compareTo.value;
                case Comparison.LessThanOrEqualTo:
                    return _value.value <= _compareTo.value;
                default:
                    return true;
            }

        }

        private void Rrefresh()
        {
            if (enabled == false) return;

            if (EvaluateComparison())
            {
                Trigger();
            }
        }

        [Button]
        private void Trigger()
        {
            _onTrigger?.Invoke();
        }

        private void OnValueChanged(float f)
        {
            Rrefresh();
        }

        private void Start()
        {
            
        }
    }
}