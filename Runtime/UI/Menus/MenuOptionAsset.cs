using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace GiantSword
{
    public class MenuOptionAsset : ScriptableObject
    {
        [SerializeField] private string _text;
        private Action _action; // TO DO: move this state out of the scriptable object
        [SerializeField] private bool _interactable = true; // TO DO: move this state out of the scriptable object

        [SerializeField] private UnityEvent _onClicked;
        public string text => _text;

        public bool interactable => _interactable;


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
    }
}