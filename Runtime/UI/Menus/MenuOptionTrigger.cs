using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class MenuOptionTrigger : MonoBehaviour
    {
        [SerializeField] private MenuOptionAsset _menuOptionAsset;
        [SerializeField] private UnityEvent _onSelected;

        [Button]
        private void Awake()
        {
            _menuOptionAsset.onClicked += Trigger;
        }
        
        private void OnDestroy()
        {
            _menuOptionAsset.onClicked -= Trigger;
        }
        

        [Button]
        public void Trigger()
        {
            _onSelected?.Invoke();
        }
    }
}