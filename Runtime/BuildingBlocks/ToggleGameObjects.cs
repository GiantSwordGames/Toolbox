using System;
using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class ToggleGameObjects : MonoBehaviour
    {
        [SerializeField]   private bool _state = false;
        [SerializeField] private GameObject _on;
        [SerializeField] private GameObject _off;
        void Start()
        {
            Refresh();
        }

        [Button]
        public void Toggle()
        {
            _state = !_state;
            Refresh();
        }

        void Refresh()
        {
            if (_on)
            {
                _on.SetActive(_state);
            }
            
            if (_off)
            {
                _off.SetActive(!_state);
            }
        }
    }
}
