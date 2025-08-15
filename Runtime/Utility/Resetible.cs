using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace JamKit
{
    public class Resettable : MonoBehaviour
    {
        [SerializeField] private bool _position;
        [SerializeField] private bool _rotation;
        [SerializeField] private bool _scale;
        [SerializeField] private bool _isActiveState;
        [SerializeField] private UnityEvent _resset;

        [Button]
        public void DoReset()
        {
            
        }
    }
}
