using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class DisableTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEnable;
        void OnDisable()
        {
            if (Time.frameCount != 0)
            {
                onEnable?.Invoke();
            }
        }
    }
}