using UnityEngine;

namespace GiantSword
{
    public class EnableGameObjectBasedOnThisOne : MonoBehaviour
    {
        [SerializeField] private GameObject _gameObject;
        void OnEnable()
        {
            _gameObject.SetActive(true);
        }

        void OnDisable()
        {
            _gameObject.SetActive(false);
        }
    }
}
