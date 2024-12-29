using System;
using System.Collections.Generic;
using System.Linq;
using GiantSword;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace RichardPieterse
{
    public class MenuGeneratorBase : MonoBehaviour
    {
        [SerializeField] protected List<MenuOption> _instancedOptions = new List<MenuOption>();
        [SerializeField] protected MenuDefinition _menuDefinition;
        [SerializeField] protected UnityEvent _onClosed;
        [SerializeField] protected UnityEvent _onOpened;
        [SerializeField] protected bool _invokeOnOpenOnStart;
        
        [ShowNonSerializedField]
        private bool _isOpen = true;
        

        private MenuOptionAsset _generatedBackOption;

        private void Awake()
        {
            _menuDefinition.onOpen += OnOpen;
            _menuDefinition.onClose += OnClose;
        }

        private void OnDestroy()
        {
            _menuDefinition.onOpen -= OnOpen;
            _menuDefinition.onClose -= OnClose;
        }

        private void Start()
        {
            SelectInitial();
         ;

            foreach (var menuOptionAsset in _menuDefinition.options)
            {
                if (menuOptionAsset.subMenu)
                {
                    menuOptionAsset.subMenu.Close();
                }
            }

            if (_invokeOnOpenOnStart)
            {
                _menuDefinition.Open(null);
            }
        }

        private void Update()
        {
            if (Application.isPlaying && _isOpen)
            {
                if (_menuDefinition.upKey.IsDown())
                {
                    SelectPrevious();
                }

                if (_menuDefinition.downKey.IsDown())
                {
                    SelectNext();
                }

                if (_menuDefinition.acceptKey.IsDown())
                {
                    ClickOnSelected();
                }
            }
        }

        // [Button]
        protected void SelectInitial()
        {
            foreach (MenuOption option in _instancedOptions)
            {
                option.Deselect();
            }

            if (_instancedOptions.Count > 1)
            {
                _instancedOptions[0].Select();
            }
        }

        private void ClickOnSelected()
        {
            foreach (var option in _instancedOptions)
            {
                if (option.isSelected && option.isInteractable)
                {
                    option.Click();
                }
            }
        }


        private void SelectNext()
        {
            if(_instancedOptions.Count == 0)
            {
                return;
            }
            
            int selectedIndex = GetCurrentSelectedIndex();
            _instancedOptions[selectedIndex].Deselect();

            int index = _instancedOptions.GetNextWrappedIndex(selectedIndex);
            _instancedOptions[index].Select();
        }

        private void SelectPrevious()
        {
            if(_instancedOptions.Count == 0)
            {
                return;
            }
            
            int selectedIndex = GetCurrentSelectedIndex();
            _instancedOptions[selectedIndex].Deselect();

            int index = _instancedOptions.GetPreviousWrappedIndex(selectedIndex);
            _instancedOptions[index].Select();
        }


        private int GetCurrentSelectedIndex()
        {
            for (int i = 0; i < _instancedOptions.Count; i++)
            {
                if (_instancedOptions[i].isSelected)
                {
                    return i;
                }
            }

            return 0;
        }
        
        
        [Button]
        protected void Clear()
        {
            try
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    transform.GetChild(i).gameObject.SmartDestroy();
                }
            
                _instancedOptions.Clear();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
          
        }

        [Button]
        protected  void Generate()
        {
            Clear();

            List<MenuOptionAsset> options = _menuDefinition.options;

            
            foreach (var option in options)
            {
                MenuOption menuOption = _menuDefinition.optionPrefab.SmartInstantiate();
                menuOption.transform.SetParent(transform, false);
                menuOption.Setup(option, _menuDefinition);
                _instancedOptions.Add(menuOption);
            }
            
            if (_menuDefinition.openedBy != null && _generatedBackOption==null)
            {
                _generatedBackOption = ScriptableObject.CreateInstance<MenuOptionAsset>();
                _generatedBackOption.text = "Back ";
                _generatedBackOption.onClicked += CloseFromBackButton;
                _generatedBackOption.name = _generatedBackOption.text;
                
            }
            
            if (_generatedBackOption != null)
            {
                MenuOption menuOption = _menuDefinition.optionPrefab.SmartInstantiate();
                menuOption.transform.SetParent(transform, false);
                menuOption.Setup(_generatedBackOption, _menuDefinition);
                _instancedOptions.Add(menuOption);
            }
        }

        private void CloseFromBackButton()
        {
            _menuDefinition.Close();
            if (_menuDefinition.openedBy)
            {
                _menuDefinition.openedBy.Open(null);
            }
        }


        protected void OnClose()
        {
            _isOpen = false;
            _onClosed?.Invoke();
        }

        protected void OnOpen()
        {
            Generate();
            _onOpened?.Invoke();
            SelectInitial();
            AsyncHelper.WaitForFrame(() =>
                {
                    this._isOpen = true;
                }
            );
        }
    }
}