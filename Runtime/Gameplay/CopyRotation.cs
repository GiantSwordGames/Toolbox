using UnityEngine;

namespace GiantSword
{
    public class CopyRotation : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        private void OnEnable()
        {
            transform.rotation = _target.rotation;
        }

        private void Update()
        {
            transform.rotation = _target.rotation;
        }
    }
}