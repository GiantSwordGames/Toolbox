using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class SetFloat : MonoBehaviour
    {
        [SerializeField] private SmartFloat _float;
        [SerializeField] private SmartFloat _value ;
        
        [Button]
        public void Trigger()
        {
            _float.value = _value.value;
        }
    }
}