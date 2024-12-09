using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class ActionList : MonoBehaviour
    {
        [SerializeField] private UnityEvent _actions;

        
        [Button]
        public void Trigger()
        {
            if (this.isActiveAndEnabled == false)
            {
                return;
            }
            _actions.SafeInvoke();
        }
    }
}
