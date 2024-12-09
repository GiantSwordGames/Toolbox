using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class OnStartTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEnable;
        
        [Button]
        void Start()
        {
            if (Time.frameCount != 0)
            {
                onEnable?.Invoke();
            }
        }
    }
}