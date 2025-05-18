using System;
using System.Collections.Generic;
using NaughtyAttributes;
using RichardPieterse;
using UnityEngine;
using UnityEngine.UI;

namespace GiantSword
{
    public class MenuDefinition : ScriptableObject
    {
        [SerializeField] private MenuStyleDefinition _styleDefinition;
        [Space]
        [SerializeField] private InputKeyAsset _upKey;
        [SerializeField] private InputKeyAsset _downKey;
        [SerializeField] private InputKeyAsset _acceptKey;
        [SerializeField] private InputKeyAsset _backButton;
        private Color _selectedColor => _styleDefinition.selectedColor;
        private Color _deselectedColor => _styleDefinition.deselectedColor;
        private Color _deactivatedColor => _styleDefinition.deactivatedColor;
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

        public InputKeyAsset BackButton => _backButton;

        public MenuStyleDefinition styleDefinition => _styleDefinition;


        [Button]
        public void Open(MenuDefinition openedBy)
        {
            _openedBy = openedBy;
            onOpen?.Invoke();
        }

        [Button]
        public void Close()
        {
            onClose?.Invoke();
        }
        
        public virtual void RegenerateDynamicOptions()
        {
            
        }

        [Button(enabledMode:EButtonEnableMode.Editor)]
        public void GenerateInScene()
        {
            GameObject canvas = new GameObject("Canvas" + name);
            canvas.AddComponent<Canvas>();
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<Canvas4K>();
            
            GameObject menu = new GameObject("" + name);
            menu.transform.SetParent(canvas.transform);
            menu.transform.localPosition = Vector3.zero;

            menu.AddComponent<VerticalLayoutGroup>();
            menu.AddComponent<MenuGenerator>().menuDefinition = this;
            RuntimeEditorHelper.RegisterCreatedObjectUndo(canvas);
        }
    }
}