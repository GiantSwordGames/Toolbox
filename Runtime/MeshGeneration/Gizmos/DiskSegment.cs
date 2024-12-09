namespace GiantSword
{
    using UnityEngine;
    
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class DiskSegment : MonoBehaviour
    {
        [SerializeField]
        private float radius = 1.0f; // Radius of the disk
        [SerializeField]
        private int segments = 20; // Number of segments around the disk
        [SerializeField]
        private float startAngle = 0.0f; // Start angle of the arc in degrees
        [SerializeField]
        private float endAngle = 90.0f; // End angle of the arc in degrees
        [SerializeField]
        private Vector3 axis = Vector3.up; // Axis of the disk
    
        void OnValidate()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = CreateDiskSegment(radius, segments, startAngle, endAngle, axis);
        }
    
        Mesh CreateDiskSegment(float radius, int segments, float startAngle, float endAngle, Vector3 axis)
        {
            Mesh mesh = new Mesh();
    
            int vertexCount = segments + 2;
            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            int[] triangles = new int[segments * 3];
    
            vertices[0] = Vector3.zero;
            normals[0] = axis.normalized;
            uvs[0] = new Vector2(0.5f, 0.5f);
    
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, axis.normalized);
            float angleStep = (endAngle - startAngle) / segments;
    
            for (int i = 0; i <= segments; i++)
            {
                float angle = Mathf.Deg2Rad * (startAngle + angleStep * i);
                Vector3 vertexPosition = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                vertices[i + 1] = rotation * vertexPosition;
                normals[i + 1] = axis.normalized;
                uvs[i + 1] = new Vector2((vertices[i + 1].x / radius + 1) * 0.5f, (vertices[i + 1].z / radius + 1) * 0.5f);
            }
    
            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
    
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
    
            return mesh;
        }
    }
}
