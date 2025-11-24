using NaughtyAttributes;
using UnityEngine;

namespace JamKit
{
    public class TwoStateObject : MonoBehaviour
    {
        [SerializeField] private GameObject _onState;
        [SerializeField] private GameObject _offState;
        
        [Button]
        public void Toggle()
        {
            if (_onState.gameObject.activeSelf)
            {
                ToggleOff();
            }
            else
            {
                ToggleOn();
            }
        }
        
        [Button]
        public void ToggleOn()
        {
            _onState.SetActive(true);
            _offState.SetActive(false);
        }
        
        [Button]
        public void ToggleOff()
        {
            _onState.SetActive(false);
            _offState.SetActive(true);
        }


    }
}
