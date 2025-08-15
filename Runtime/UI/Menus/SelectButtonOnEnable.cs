using UnityEngine;
using UnityEngine.EventSystems;

namespace JamKit
{
    public class SelectButtonOnEnable : MonoBehaviour
    {
        [SerializeField] private GameObject _button;
        
        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(_button);
        }
    }
}