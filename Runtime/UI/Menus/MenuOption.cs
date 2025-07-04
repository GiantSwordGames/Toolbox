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
        [FormerlySerializedAs("_clickAnimationDuration")] [SerializeField] private float _delayEventInvocation = 0f;
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

            ApplySelectedColor();

            _isSelected = true;
            
            _onSelected?.Invoke();
        }

        public void ApplySelectedColor()
        {
            _textMeshPro.color = _menuDefinition.selectedColor;
        }

        public void Deselect()
        {
            if (_selector != null)
            {
                _selector.gameObject.SetActive(false);
            }
            
            ApplyDeselectedColor();

            _isSelected = false;

            _onDeselected?.Invoke();
        }

        public void ApplyDeselectedColor()
        {
            _textMeshPro.color = _menuDefinition.deselectedColor;
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

                if (Application.isPlaying)
                {
                    _optionAsset.onTextRefreshed += () =>
                    {
                        _textMeshPro.text = _optionAsset.text;
                        if (_textContentFitter)
                        {
                            _textContentFitter.Apply();
                        }
                    };
                }
            }
            else
            {
                Debug.LogError("Option asset is null", this);
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
            if (_clicked && _optionAsset.allowReClick == false)
            {
                return;
            }
            _clicked = true;
            _onPlayClickedAnimation.Invoke();


            AsyncHelper.DelayUnscaled(_delayEventInvocation, () =>
            {
                _optionAsset.Click();

                _optionAsset.sound?.Play();
                if (_optionAsset.subMenu)
                {
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
