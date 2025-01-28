using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class SetScriptableBool:MonoBehaviour
    {
        [SerializeField] private ScriptableBool _scriptableBool;
        [SerializeField] private bool _value;
        [Button]
        public void Trigger()
        {
            _scriptableBool.value = _value;
        }
        
        [Button]
        public void Toggle()
        {
            _scriptableBool.Toggle();
        }
    }
}