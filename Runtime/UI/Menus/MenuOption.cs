using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GiantSword
{
    public class MenuOption : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _textMeshPro;
        [SerializeField] private MenuOptionAsset _optionAsset;
        [SerializeField] private MenuDefinition _menuDefinition;
        [SerializeField] private UnityEvent _onClick;
        [SerializeField] private UnityEvent _onSelect;
        [SerializeField] private UnityEvent _onDeselect;
        [SerializeField] private GameObject _selector;
        [SerializeField] private TextContentFitter _textContentFitter;
        
        
        private bool _clicked;
        private bool _isSelected;

        public bool isSelected => _isSelected;


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
            
            _onSelect?.Invoke();
        }

        public void Deselect()
        {
            if (_selector != null)
            {
                _selector.gameObject.SetActive(false);
            }
            
            _textMeshPro.color = _menuDefinition.deselectedColor;

            _isSelected = false;

            _onDeselect?.Invoke();
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
        public void OnClicked()
        {
            if (_clicked)
            {
                return;
            }
            _optionAsset.sound?.Play();
            _clicked = true;
            _optionAsset.Trigger();
            _onClick.Invoke();
        }

        public void Setup(MenuOptionAsset option, MenuDefinition menuDefinition)
        {
            _menuDefinition = menuDefinition;
            _optionAsset = option;
            SetUp();
        }

        // private void OnDestroy()
        // {
        //     // if (Application.isPlaying )
        //     // {
        //     //     if (_optionAsset)
        //     //     {
        //     //         Debug.Log("RemoveListener Listener " + this, this);
        //     //         if (RuntimeEditorHelper.IsQuitting == false)
        //     //         {
        //     //             Action action = _optionAsset.onSelect;
        //     //             Debug.Log(action == null);
        //     //             _optionAsset.onSelect -= Select;
        //     //             _optionAsset.onDeselect -= (Deselect);
        //     //         }
        //     //     }
        //     // }
        // }

    }
}
