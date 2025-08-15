namespace JamKit
{
    using UnityEngine;
    
    
    [ExecuteInEditMode]
    public class JointGizmo : MonoBehaviour
    {
         private Joint _joint;
        [SerializeField] private float _gizmoScale = 1f;
        [SerializeField] private Vector3 _offset;
    
        private void Awake()
        {
        }
    
        private void Update()
        {
            if (_joint == null)
            {
                _joint = GetComponent<Joint>();
            }
    
            if (_joint == null)
            {
                enabled = false;
                return;
            }
    
            Vector3 connectedAnchorPosition;
                if (_joint.connectedBody != null)
                {
                    // Get the connected anchor position in world space
                    connectedAnchorPosition = _joint.connectedBody.transform.TransformPoint(_joint.connectedAnchor);
                }
                else
                {
                    // If there's no connected body, use the connected anchor position in local space
                    connectedAnchorPosition = _joint.connectedAnchor;
                }
    
                // Get the anchor position in world space
                Vector3 anchorPosition = transform.TransformPoint(_joint.anchor);
    
    
    
        }
    
        private void OnDrawGizmos()
        {
            if (_joint == null)
            {
                _joint = GetComponent<Joint>();
            }
            if (_joint != null)
            {
                Vector3 connectedAnchorPosition;
                if (_joint.connectedBody != null)
                {
                    // Get the connected anchor position in world space
                    connectedAnchorPosition = _joint.connectedBody.transform.TransformPoint(_joint.connectedAnchor);
                }
                else
                {
                    // If there's no connected body, use the connected anchor position in local space
                    connectedAnchorPosition = _joint.connectedAnchor;
                }
    
                // Get the anchor position in world space
                Vector3 anchorPosition = transform.TransformPoint(_joint.anchor);
    
                // Set the gizmo color
                Gizmos.color = Color.yellow;
    
                // Draw the line between the connected anchor and the anchor
                Gizmos.DrawLine(anchorPosition, connectedAnchorPosition);
            }
        }
    }
}
