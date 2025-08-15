using UnityEngine;

namespace JamKit
{
    public class DrawArrowGizmo : DrawGizmoBase
    {
        [SerializeField]private float _length = 1;
        // [SerializeField]private float _width = 0.1f;

        protected override void CustomDrawWire()
        {
            // GizmosUtils.Arrow(Vector3.zero,  Vector3.forward * _length, _width, Gizmos.color);
            Gizmos.DrawRay(Vector3.zero, Vector3.forward*_length);
        }

        protected override void CustomDrawSolid()
        {
            Gizmos.DrawRay(Vector3.zero, Vector3.forward*_length);
        }
    }
}
