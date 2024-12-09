using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GiantSword
{
    public class ShowAction : MonoBehaviour
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private bool _disableOnAwake;

        private void Awake()
        {
            if (_disableOnAwake)
            {
                _gameObject.SetActive(false);
            }
        }

        public void Trigger()
        {
            _gameObject.SetActive(true);
        }
    }
}
