namespace GiantSword
{
    using UnityEngine;
    
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Cylinder : MonoBehaviour
    {
        public float height = 2.0f;
        public float radius = 1.0f;
        public int segments = 20;
        public int capSegments = 10;
    
        private MeshFilter meshFilter;
    
        void OnValidate()
        {
            // Ensure the segments and cap segments are at least 3
            segments = Mathf.Max(segments, 3);
            capSegments = Mathf.Max(capSegments, 3);
    
            // Generate the cylinder mesh
            GenerateCylinderMesh();
        }
    
        void GenerateCylinderMesh()
        {
            if (meshFilter == null)
            {
                meshFilter = GetComponent<MeshFilter>();
            }
    
            Mesh mesh = new Mesh();
            mesh.name = "Cylinder";
    
            int vertexCount = (segments + 1) * 2 + segments * capSegments * 2;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(segments * 4 * (capSegments - 1)) * 3];
    
            float angleStep = 2 * Mathf.PI / segments;
            float capAngleStep = 2 * Mathf.PI / capSegments;
    
            int vert = 0;
            int tri = 0;
    
            // Generate vertices for top and bottom circles
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * angleStep;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
    
                vertices[vert++] = new Vector3(x, height / 2, z);
                vertices[vert++] = new Vector3(x, -height / 2, z);
            }
    
            // Generate vertices for cap segments
            for (int i = 0; i < segments; i++)
            {
                for (int j = 1; j < capSegments; j++)
                {
                    float angle = i * angleStep;
                    float capAngle = j * capAngleStep;
    
                    float x = Mathf.Cos(angle) * Mathf.Cos(capAngle) * radius;
                    float z = Mathf.Sin(angle) * Mathf.Cos(capAngle) * radius;
                    float y = Mathf.Sin(capAngle) * (height / 2);
    
                    vertices[vert++] = new Vector3(x, y, z);
                    vertices[vert++] = new Vector3(x, -y, z);
                }
            }
    
            // Generate triangles for top and bottom circles
            for (int i = 0; i < segments; i++)
            {
                int nextI = (i + 1) % segments;
    
                // Top circle
                triangles[tri++] = i * 2;
                triangles[tri++] = nextI * 2;
                triangles[tri++] = segments * 2;
    
                // Bottom circle
                triangles[tri++] = i * 2 + 1;
                triangles[tri++] = segments * 2 + 1;
                triangles[tri++] = nextI * 2 + 1;
            }
    
            // Generate triangles for the sides
            for (int i = 0; i < segments; i++)
            {
                int nextI = (i + 1) % segments;
    
                for (int j = 0; j < capSegments - 1; j++)
                {
                    int baseVert = segments * 2 + 2 + (i * (capSegments - 1) + j) * 2;
    
                    triangles[tri++] = baseVert;
                    triangles[tri++] = baseVert + 1;
                    triangles[tri++] = baseVert + 2;
    
                    triangles[tri++] = baseVert + 1;
                    triangles[tri++] = baseVert + 3;
                    triangles[tri++] = baseVert + 2;
    
                    if (j == capSegments - 2)
                    {
                        // Cap the segments
                        triangles[tri++] = baseVert + 2;
                        triangles[tri++] = baseVert + 1;
                        triangles[tri++] = nextI * 2;
    
                        triangles[tri++] = baseVert + 1;
                        triangles[tri++] = nextI * 2 + 1;
                        triangles[tri++] = nextI * 2;
                    }
                }
            }
    
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
    
            meshFilter.mesh = mesh;
        }
    }
}
