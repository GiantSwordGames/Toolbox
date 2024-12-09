using UnityEngine;

namespace GiantSword
{
    public class EnableIfAlive : MonoBehaviour
    {
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private Health _health;
        
        // Update is called once per frame
        void Update()
        {
        _gameObject.SetActive(_health.isAlive);
        }
    }
}
