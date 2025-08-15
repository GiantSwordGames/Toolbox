using UnityEngine;
using UnityEngine.EventSystems;

namespace JamKit
{
    public class FocusButtonOnEnable : MonoBehaviour
    {
        [SerializeField] private GameObject _gameObject;
        
        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(_gameObject);
        }

    }
}
