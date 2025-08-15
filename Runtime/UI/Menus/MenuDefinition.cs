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

        public InputKeyAsset upKey => _styleDefinition.upKey;

        public InputKeyAsset downKey => _styleDefinition.downKey;

        public InputKeyAsset acceptKey => _styleDefinition.acceptKey;

        public MenuOption optionPrefab => _styleDefinition.optionPrefab;

        public virtual List<MenuOptionAsset> options => _options;

        public Color selectedColor => _styleDefinition.selectedColor;
        public Color deselectedColor => _styleDefinition.deselectedColor;
        public Color deactivatedColor => _styleDefinition.deactivatedColor;

        public MenuDefinition openedBy => _openedBy;

        public InputKeyAsset backButton => _styleDefinition.backButton;

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
            MenuGenerator[] menuGenerators = FindObjectsOfType<MenuGenerator>();
            MenuGenerator menuGenerator =null;
            foreach (MenuGenerator generator in menuGenerators)
            {
                if (generator.menuDefinition == this)
                {
                    menuGenerator = generator;
                    break;
                }
            }

            if (menuGenerator == null)
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
                menuGenerator =  menu.AddComponent<MenuGenerator>(); 
                menuGenerator.menuDefinition = this;
                RuntimeEditorHelper.RegisterCreatedObjectUndo(canvas);
            }
            
            menuGenerator.Generate();
        }
    }
}