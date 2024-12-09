
    using UnityEngine;
    
    namespace GiantSword
    {
    
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Torus : MonoBehaviour
    {
        public float radius1 = 0.5f; // Radius from the center of the torus to the center of the tube
        public float radius2 = 0.25f; // Radius of the tube
        public int segments = 56;    // Number of segments around the torus
        public int tubeSegments = 10;// Number of segments around the tube
        public float arcAngle = 360; // Angle of the arc in degrees
    
        void OnValidate()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = CreateTorusArc(radius1, radius2, segments, tubeSegments, arcAngle);
        }
    
        Mesh CreateTorusArc(float radius1, float radius2, int segments, int tubeSegments, float arcAngle)
        {
            Mesh mesh = new Mesh();
    
            int vertexCount = (segments + 1) * (tubeSegments + 1);
            Vector3[] vertices = new Vector3[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            int[] triangles = new int[segments * tubeSegments * 6];
    
            float segmentAngleStep = arcAngle / segments;
            float tubeSegmentAngleStep = 360.0f / tubeSegments;
    
            for (int i = 0; i <= segments; i++)
            {
                float segmentAngle = Mathf.Deg2Rad * segmentAngleStep * i;
                Vector3 segmentCenter = new Vector3(Mathf.Cos(segmentAngle) * radius1, Mathf.Sin(segmentAngle) * radius1, 0);
    
                for (int j = 0; j <= tubeSegments; j++)
                {
                    float tubeSegmentAngle = Mathf.Deg2Rad * tubeSegmentAngleStep * j;
                    Vector3 vertexOffset = new Vector3(Mathf.Cos(tubeSegmentAngle) * radius2, 0, Mathf.Sin(tubeSegmentAngle) * radius2);
                    int vertexIndex = i * (tubeSegments + 1) + j;
                    vertices[vertexIndex] = segmentCenter + Quaternion.Euler(0, 0, segmentAngle * Mathf.Rad2Deg) * vertexOffset;
                    normals[vertexIndex] = (vertices[vertexIndex] - segmentCenter).normalized;
                    uvs[vertexIndex] = new Vector2((float)i / segments, (float)j / tubeSegments);
                }
            }
    
            int triangleIndex = 0;
            for (int i = 0; i < segments; i++)
            {
                for (int j = 0; j < tubeSegments; j++)
                {
                    int current = i * (tubeSegments + 1) + j;
                    int next = (i + 1) * (tubeSegments + 1) + j;
    
                    triangles[triangleIndex++] = current;
                    triangles[triangleIndex++] = next;
                    triangles[triangleIndex++] = current + 1;
    
                    triangles[triangleIndex++] = current + 1;
                    triangles[triangleIndex++] = next;
                    triangles[triangleIndex++] = next + 1;
                }
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
