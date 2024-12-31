using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace GiantSword
{
    
    public class InputKeyAsset : ScriptableObject
    {
        [SerializeField] private KeyCode _keyCode;
        [SerializeField] private KeyCode _secondaryKeyCode;
        [SerializeField] private KeyCode[] _otherKeyCodes = { };
        [SerializeField] private InputActionReference _actionAsset;

        private void Awake()
        {
        }

        public bool IsDown()
        {
            if (Input.GetKeyDown(_keyCode))
            {
                return true;
            }

            if (Input.GetKeyDown(_secondaryKeyCode))
            {
                return true;
            }

            foreach (var otherKeyCode in _otherKeyCodes)
            {
                if (Input.GetKeyDown(otherKeyCode))
                {
                    return true;
                }
            }
            
            
            // read the action value
            if (_actionAsset != null)
            {
                if (_actionAsset.action.actionMap.enabled == false)
                {
                    _actionAsset.action.actionMap.Enable();
                    _actionAsset.action.Enable();
                }
                
                if (_actionAsset.action.WasPressedThisFrame())
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public bool IsUp()
        {
            if (Input.GetKeyUp(_keyCode))
            {
                return true;
            }

            if (Input.GetKeyUp(_secondaryKeyCode))
            {
                return true;
            }

            foreach (var otherKeyCode in _otherKeyCodes)
            {
                if (Input.GetKeyUp(otherKeyCode))
                {
                    return true;
                }
            }
            
            // read the action value
            if (_actionAsset != null)
            {
                if (_actionAsset.action.actionMap.enabled == false)
                {
                    _actionAsset.action.actionMap.Enable();
                    _actionAsset.action.Enable();
                }
               
                if (_actionAsset.action.WasReleasedThisFrame())
                {
                    return true;
                }
            }
            
            return false;
        }

        public bool IsHeld()
        {
            if (Input.GetKey(_keyCode))
            {
                return true;
            }

            if (Input.GetKey(_secondaryKeyCode))
            {
                return true;
            }

            foreach (var otherKeyCode in _otherKeyCodes)
            {
                if (Input.GetKey(otherKeyCode))
                {
                    return true;
                }
            }
            
            // read the action value
            if (_actionAsset != null)
            {
                if (_actionAsset.action.actionMap.enabled == false)
                {
                    _actionAsset.action.actionMap.Enable();
                    _actionAsset.action.Enable();
                }
               
                if (_actionAsset.action.IsPressed())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
