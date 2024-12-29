using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GiantSword
{
    public class MenuOption : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _textMeshPro;
        [SerializeField] private MenuOptionAsset _optionAsset;
        [SerializeField] private MenuDefinition _menuDefinition;
        [FormerlySerializedAs("_onClicked")]
        [FormerlySerializedAs("_onClick")] 
        [SerializeField] private UnityEvent _onPlayClickedAnimation;
        [SerializeField] private float _clickAnimationDuration = 0f;
        [FormerlySerializedAs("_onSelect")] [SerializeField] private UnityEvent _onSelected;
        [FormerlySerializedAs("_onDeselect")] [SerializeField] private UnityEvent _onDeselected;
        [SerializeField] private GameObject _selector;
        [SerializeField] private TextContentFitter _textContentFitter;
        
        
        private bool _clicked;
        private bool _isSelected;

        public bool isSelected => _isSelected;
        public bool isInteractable => _optionAsset.interactable;


        private void OnEnable()
        {
            _clicked = false;
        }

        
        [Button]
        public void Select()
        {
            if (_selector != null)
            {
                _selector.gameObject.SetActive(true);
            }

            _textMeshPro.color = _menuDefinition.selectedColor;

            _isSelected = true;
            
            _onSelected?.Invoke();
        }

        public void Deselect()
        {
            if (_selector != null)
            {
                _selector.gameObject.SetActive(false);
            }
            
            _textMeshPro.color = _menuDefinition.deselectedColor;

            _isSelected = false;

            _onDeselected?.Invoke();
        }

        private void SetUp()
        {
            if (_optionAsset)
            {
                _textMeshPro.text = _optionAsset.text;
                name = "Option_" + _optionAsset.text;

                if (_canvasGroup)
                {
                    _canvasGroup.interactable = _optionAsset.interactable;
                }

                if (_textContentFitter)
                {
                    _textContentFitter.Apply();
                    
                }
            }
        }

        private void Start()
        {
            SetUp();
            _optionAsset.onSelect += (Select);
            _optionAsset.onDeselect += (Deselect);
        }
        
        void OnDestroy()
        {
            if (_optionAsset)
            {
                _optionAsset.onSelect -= Select;
                _optionAsset.onDeselect -= (Deselect);
            }
        }

        [Button]
        public void Click()
        {
            if (_clicked)
            {
                return;
            }
            _clicked = true;
            _onPlayClickedAnimation.Invoke();

            AsyncHelper.Delay(_clickAnimationDuration, () =>
            {
                _optionAsset.sound?.Play();
                _optionAsset.Click();
                if (_optionAsset.subMenu)
                {
                    Debug.Log("Sub Menu " + _optionAsset.subMenu);
                    _menuDefinition.Close();
                    _optionAsset.subMenu.Open(_menuDefinition);
                }
                
            });
            
        }
        
        public MenuDefinition GetSubMenu()
        {
            return _optionAsset.subMenu;
        }

        public void Setup(MenuOptionAsset option, MenuDefinition menuDefinition)
        {
            _menuDefinition = menuDefinition;
            _optionAsset = option;
            SetUp();
        }
    }
}
