using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class ScriptableEventTrigger : MonoBehaviour
    {
        [SerializeField] private ScriptableEvent _scriptableEvent;
        [SerializeField] private UnityEvent _onTriggered;
        private void OnEnable()
        {
            _scriptableEvent.onFired += Trigger;
        }

        private void OnDisable()
        {
            _scriptableEvent.onFired -= Trigger;
        }

        [Button]
        public void Trigger()
        {
            _onTriggered?.Invoke();
        }
    }
}