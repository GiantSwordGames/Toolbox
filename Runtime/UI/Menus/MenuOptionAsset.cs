using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GiantSword
{
    public class MenuOptionAsset : ScriptableObject
    {
        [SerializeField] protected string _text;
        [SerializeField] private bool _interactable = true; // TO DO: move this state out of the scriptable object
        [SerializeField] private SoundAsset _sound;
        [FormerlySerializedAs("_openSubMenu")] 
        [SerializeField] private MenuDefinition _subMenu;
        [SerializeField] private bool _allowReClick = false; 

        [SerializeField] private   UnityEvent _onClicked;
      
        [ShowNonSerializedField]
        private Action _onClickedAction; // TO DO: move this state out of the scriptable object

        [ShowNonSerializedField] private Action _onSelect;

        private Action _onDeselect;
        public Action onTextRefreshed;
        public virtual string text
        {
            get => _text;
            set { _text = value; }
        }

        public bool interactable => _interactable;

        public  Action onSelect
        {
            get => _onSelect;
            set => _onSelect = value;
        }

        public  Action onDeselect
        {
            get => _onDeselect;
            set => _onDeselect = value;
        }

        public SoundAsset sound => _sound;

        public Action onClicked
        {
            get => _onClickedAction;
            set => _onClickedAction = value;
        }

        public MenuDefinition subMenu => _subMenu;

        public bool allowReClick => _allowReClick;


        public void UnregisterListener(Action action)
        {
            _onClickedAction -= action;
        }

        [Button]
        public virtual void Click()
        {
            _onClickedAction?.Invoke();
            _onClicked?.Invoke();
        }

        public void Select()
        {
            _onSelect?.Invoke();
        }
    }
}