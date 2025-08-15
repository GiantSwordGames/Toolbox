using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class IncrementFloat : MonoBehaviour
    {
        [SerializeField] private SmartFloat _float;
        [SerializeField] private SmartFloat _incrementAmount;
        [Button]
        public void Trigger()
        {
            _float.value += _incrementAmount.value;
        }
    }
}