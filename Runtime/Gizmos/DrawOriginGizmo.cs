using UnityEngine;

namespace GiantSword
{
    public class DrawOriginGizmo : DrawGizmoBase
    {   
        [SerializeField] private float _size = .1f;
        [SerializeField] private float _thickness = .01f;

        protected override void CustomDrawWire()
        {
            // Draw a wire box at the origin with the given size and thickness
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(_size, _thickness, _thickness)); // X axis
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(_thickness, _size, _thickness)); // Y axis
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(_thickness, _thickness, _size)); // Z axis
        }

        protected override void CustomDrawSolid()
        {
            // Draw a solid box at the origin with the given size and thickness
            Gizmos.DrawCube(Vector3.zero, new Vector3(_size, _thickness, _thickness)); // X axis
            Gizmos.DrawCube(Vector3.zero, new Vector3(_thickness, _size, _thickness)); // Y axis
            Gizmos.DrawCube(Vector3.zero, new Vector3(_thickness, _thickness, _size)); // Z axis
        }   
    }
}