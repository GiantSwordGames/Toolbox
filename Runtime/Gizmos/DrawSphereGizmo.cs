using UnityEngine;

namespace JamKit
{
    public class DrawSphereGizmo : DrawGizmoBase
    {
        [SerializeField] private float _radius = 0.5f;
        public float radius => _radius;

        protected override void CustomDrawWire()
        {
            Gizmos.DrawWireSphere(Vector3.zero, _radius);
        }

        protected override void CustomDrawSolid()
        {
            Gizmos.DrawSphere(Vector3.zero, _radius);
        }
    }
}