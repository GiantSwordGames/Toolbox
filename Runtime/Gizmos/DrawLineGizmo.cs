using UnityEngine;

namespace GiantSword
{
    public class DrawLineGizmo : DrawGizmoBase
    {
        [SerializeField] private Vector3 _from;
        [SerializeField] private Vector3 _to;
        
        protected override void CustomDrawWire()
        {
            Gizmos.DrawLine(_from, _to);
        }

        protected override void CustomDrawSolid()
        {
            Gizmos.DrawLine(_from, _to);
        }
    }
}