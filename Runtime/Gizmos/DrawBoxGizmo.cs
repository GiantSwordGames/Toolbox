using UnityEngine;

namespace GiantSword
{
    public class DrawBoxGizmo : DrawGizmoBase
    {
        [SerializeField] private Vector3 _localPosition;
        [SerializeField] private Vector3 _size = Vector3.one; 
        protected override void CustomDrawWire()
        {
            Gizmos.DrawWireCube(_localPosition, _size);
        }

        protected override void CustomDrawSolid()
        {
            Gizmos.DrawCube(_localPosition, _size);
        }
    }
}