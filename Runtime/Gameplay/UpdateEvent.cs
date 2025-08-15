using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class UpdateEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent onUpdateEvent;

        private void Update()
        {
            onUpdateEvent?.Invoke();
        }
    }
}