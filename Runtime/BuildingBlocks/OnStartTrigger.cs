using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace JamKit
{
    public class OnStartTrigger : MonoBehaviour
    {
        [SerializeField] private float _delay;
        [FormerlySerializedAs("onEnable")] [SerializeField] private UnityEvent onTriggered;
        [FormerlySerializedAs("ignoreFirstFrame")] [SerializeField] private bool _ignoreFirstFrame = true;
        
        void Start()
        {
            if (Time.frameCount != 0|| _ignoreFirstFrame == false)
            {
                if (_delay > 0)
                {
                    AsyncHelper.Delay(_delay, () => Trigger());
                }
                else
                {
                    Trigger();
                }
            }
        }

        [Button]
        private void Trigger()
        {
            onTriggered?.Invoke();
        }
    }
}