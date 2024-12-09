using UnityEngine;
using UnityEngine.EventSystems;

namespace GiantSword
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
