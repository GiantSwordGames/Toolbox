using UnityEngine;

namespace GiantSword
{
    public class CopyPosition : MonoBehaviour
    {
        [SerializeField] private TargetTransform _target;

        private void OnEnable()
        {
            Refresh();
        }
        
        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            if (_target.isAvaliable)
            {
                transform.position = _target.position;
            }
        }
    }
}
