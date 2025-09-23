using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class OnPingScriptableFloat : MonoBehaviour
    {
        public ScriptableFloat scriptableFloat;
        public UnityEvent onPing;

        private void OnEnable()
        {
            if (scriptableFloat)
            {
                scriptableFloat.onPing += OnPing;
            }
        }

        private void OnPing()
        {
            onPing?.Invoke();
        }

        private void OnDisable()
        {
            if (scriptableFloat)
            {
                scriptableFloat.onPing -= OnPing;
            }
        }
    }
}