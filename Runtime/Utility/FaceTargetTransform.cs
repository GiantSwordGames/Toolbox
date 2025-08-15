using UnityEngine;

namespace JamKit
{
    public class FaceTargetTransform : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private bool _ignoreY;

        private void Update()
        {
            Vector3 direction = _target.position - transform.position;
            if (_ignoreY)
            {
                direction.y = 0;
            }
            transform.rotation = Quaternion.LookRotation(direction);
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, _target.position);
            Gizmos.DrawSphere(transform.position, 0.01f);
                
            
        }
    }
}
