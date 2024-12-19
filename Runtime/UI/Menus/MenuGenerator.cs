using System.Collections.Generic;
using GiantSword;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace RichardPieterse
{
    public class MenuGenerator : MonoBehaviour
    {
        [SerializeField] private MenuDefinition _menuDefinition;
        [SerializeField] List<MenuOption> _instancedOptions = new List<MenuOption>();
        
        private void Start()
        {
            foreach (MenuOption option in _instancedOptions)
            {
                option.Deselect();
            }

            SelectInitial();

            // AsyncHelper.Delay(5, () =>
            // {
            //     if (_instancedOptions.Count > 1)
            //     {
            //         _instancedOptions[0].Select();
            //     }
            // });
        }

        // [Button]
        private void SelectInitial()
        {
            if (_instancedOptions.Count > 1)
            {
                _instancedOptions[0].Select();
            }
            //
            // // _instancedOptions[0].Select();
            // if (_menuDefinition.options.Length > 1)
            // {
            //     Debug.LogError("select " + _menuDefinition.options[0]);
            //     _menuDefinition.options[0].Select();
            // }
        }

        private void Update()
        {
            if (Application.isPlaying)
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

        private void ClickOnSelected()
        {
            foreach (var option in _instancedOptions)
            {
                if (option.isSelected)
                {
                    option.OnClicked();
                }
            }
        }

        private void SelectNext()
        {
            int selectedIndex = GetCurrentSelectedIndex();
            _instancedOptions[selectedIndex].Deselect();

            int index = _instancedOptions.GetNextWrappedIndex(selectedIndex);
            _instancedOptions[index].Select();
        }

        private void SelectPrevious()
        {
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

            return -1;
        }

        [Button]
        private void Generate()
        {
            Clear();
            _instancedOptions.Clear();
        
            foreach (var option in _menuDefinition.options)
            {
                MenuOption menuOption = _menuDefinition.optionPrefab.SmartInstantiate();
                menuOption.transform.SetParent(transform, false);
                // menuOption.gameObject.hideFlags |= HideFlags.DontSave;
                menuOption.Setup(option, _menuDefinition);
                _instancedOptions.Add(menuOption);
            }
        }

        [Button]
        private void Clear()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                transform.GetChild(i).gameObject.SmartDestroy();
            }
            
            _instancedOptions.Clear();
        }
    }
}
