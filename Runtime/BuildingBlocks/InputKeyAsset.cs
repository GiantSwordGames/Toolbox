using UnityEngine;
using UnityEngine.Serialization;

namespace GiantSword
{
    
    public class InputKeyAsset : ScriptableObject
    {
        [SerializeField] private KeyCode _keyCode;
        [SerializeField] private KeyCode _secondaryKeyCode;
        [SerializeField] private KeyCode[] _otherKeyCodes = { };

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

            return false;
        }
    }
}
