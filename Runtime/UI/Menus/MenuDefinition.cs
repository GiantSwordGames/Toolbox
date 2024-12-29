using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace GiantSword
{
    public class MenuDefinition : ScriptableObject
    {
        [SerializeField] private InputKeyAsset _upKey;
        [SerializeField] private InputKeyAsset _downKey;
        [SerializeField] private InputKeyAsset _acceptKey;
        [Space]
        [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Color _deselectedColor = Color.Lerp(Color.white, Color.gray, .2f );
        [SerializeField] private Color _deactivatedColor = Color.grey;
        [Space]
        [SerializeField] private MenuOption _optionPrefab;
        [SerializeField] private List<MenuOptionAsset> _options;
        private MenuDefinition _openedBy;
        
        [ShowNonSerializedField]
        private Action _onOpen;

        [ShowNonSerializedField]
        private Action _onClose;

        public Action onOpen
        {
            get => _onOpen;
            set => _onOpen = value;
        }

        public Action onClose
        {
            get => _onClose;
            set => _onClose = value;
        }

        public InputKeyAsset upKey => _upKey;

        public InputKeyAsset downKey => _downKey;

        public InputKeyAsset acceptKey => _acceptKey;

        public MenuOption optionPrefab => _optionPrefab;

        public virtual List<MenuOptionAsset> options => _options;


        public Color selectedColor => _selectedColor;

        public Color deselectedColor => _deselectedColor;

        public Color deactivatedColor => _deactivatedColor;

        public MenuDefinition openedBy => _openedBy;

        
        [Button]
        public void Open(MenuDefinition openedBy)
        {
            Debug.Log("Open");
            _openedBy = openedBy;
            onOpen?.Invoke();
        }

        [Button]
        public void Close()
        {
            onClose?.Invoke();
        }
    }
}