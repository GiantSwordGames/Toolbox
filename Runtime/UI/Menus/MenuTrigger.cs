using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class MenuTrigger : MonoBehaviour
    {
        [SerializeField] private MenuDefinition _menuOptionAsset;
        [SerializeField] private UnityEvent _onOpen;
        [SerializeField] private UnityEvent _onClose;

        private void Awake()
        {
            _menuOptionAsset.onOpen += TriggerOnOpen;
            _menuOptionAsset.onClose += TriggerOnClose;
        }
        
        private void OnDestroy()
        {
            _menuOptionAsset.onOpen -= TriggerOnOpen;
            _menuOptionAsset.onClose -= TriggerOnClose;
        }

        [Button]
        public void TriggerOnOpen()
        {
            _onOpen?.Invoke();
        }
        
        [Button]
        public void TriggerOnClose()
        {
            _onClose?.Invoke();
        }
    }
}