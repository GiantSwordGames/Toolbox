using UnityEngine;

namespace JamKit
{
    public class DiscGizmo : DrawGizmoBase
    {

        [SerializeField] private Vector3 _up = Vector3.up;
        [SerializeField] private float _radius = 0.5f;
        public float radius => _radius;

        protected override void CustomDrawWire()
        {
            Matrix4x4 matrix4X4 = Gizmos.matrix;
            Gizmos.matrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one -_up.Absolute().normalized*0.99f);
            Gizmos.DrawWireSphere(Vector3.zero, _radius);
            Gizmos.matrix = matrix4X4;
        }

        protected override void CustomDrawSolid()
        {
            Matrix4x4 matrix4X4 = Gizmos.matrix;
            Gizmos.matrix *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one -_up.Absolute().normalized*0.99f);
            Gizmos.DrawSphere(Vector3.zero, _radius);
            Gizmos.matrix = matrix4X4;
        }
    }
}