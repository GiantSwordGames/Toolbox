using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JamKit
{
    public class OnStartTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEnable;
        [FormerlySerializedAs("ignoreFirstFrame")] [SerializeField] private bool _ignoreFirstFrame = true;
        
        void Start()
        {
            if (Time.frameCount != 0|| _ignoreFirstFrame == false)
            {
                onEnable?.Invoke();
            }
        }
    }
}