using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class AnimationEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onTriggered;

        [Button]
        private void Trigger()
        {
            _onTriggered?.Invoke();
        }
    }
}