using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class OnEnableTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEnable;
        
        [Button]
        void OnEnable()
        {
            // if (Time.frameCount != 0)
            {
                onEnable?.Invoke();
            }
        }
    }
}
