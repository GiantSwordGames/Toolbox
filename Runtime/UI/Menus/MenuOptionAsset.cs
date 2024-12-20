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
        [SerializeField] private string _text;
        private Action _action; // TO DO: move this state out of the scriptable object
        [SerializeField] private bool _interactable = true; // TO DO: move this state out of the scriptable object
        [SerializeField] private SoundAsset _sound;

        [SerializeField] private   UnityEvent _onClicked;

        // private ScopedState<bool> b;
        private Action _onSelect = () => { };

        private Action _onDeselect = () => { };
        public string text => _text;

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

        public void RegisterListener(Action action)
        {
            _action += action;
        }
        
        public void UnregisterListener(Action action)
        {
            _action -= action;
        }

        [Button]
        public void Trigger()
        {
            _action?.Invoke();
            _onClicked?.Invoke();
        }

        public void Select()
        {
            _onSelect?.Invoke();
        }
    }
}