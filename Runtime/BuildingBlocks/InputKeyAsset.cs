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
        [SerializeField] private InputKeyAsset[] _compositeKeys = { };
        [SerializeField] private bool _editorOnly =false;

        private void Awake()
        {
        }

        public bool IsDown()
        {
            if(_editorOnly && Application.isEditor == false)
            {
                return false;
            }
            
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

            foreach (InputKeyAsset keyAsset in _compositeKeys)
            {
                if (keyAsset != this && keyAsset.IsDown())
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public bool IsUp()
        {
            if(_editorOnly && Application.isEditor == false)
            {
                return false;
            }
            
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
            
            foreach (InputKeyAsset keyAsset in _compositeKeys)
            {
                if (keyAsset != this && keyAsset.IsUp())
                {
                    return true;
                }
            }
            
            return false;
        }

        public bool IsHeld()
        {
            if(_editorOnly && Application.isEditor == false)
            {
                return false;
            }
            
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
                        
            foreach (InputKeyAsset keyAsset in _compositeKeys)
            {
                if (keyAsset != this && keyAsset.IsHeld())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
