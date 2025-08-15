using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class ScriptableEventListener : MonoBehaviour
    {
        [SerializeField] private ScriptableEvent _scriptableEvent;
        [SerializeField] UnityEvent _onEvent;
        
        void Awake()
        {
            _scriptableEvent.onFired += Trigger;
        }
        
        [Button]
        public void Trigger()
        {
            _onEvent?.Invoke();
        }
    }
}