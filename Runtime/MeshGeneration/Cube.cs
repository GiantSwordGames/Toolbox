namespace GiantSword
{
    using UnityEngine;
    
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider))]
    public class Cube : MonoBehaviour
    {
        public Vector3 size = Vector3.one;
        public Vector3 offset = Vector3.zero;
        public Material customMaterial;
        public Color color = Color.white;
    
        private void OnValidate()
        {
            GenerateCube();
            AssignMaterial();
            UpdateCollider();
        }
    
        private void GenerateCube()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mesh.name = "Cube";
    
            Vector3 halfSize = size * 0.5f;
    
            Vector3[] vertices = {
                // Front face
                new Vector3(-halfSize.x, -halfSize.y, halfSize.z) + offset,
                new Vector3( halfSize.x, -halfSize.y, halfSize.z) + offset,
                new Vector3( halfSize.x,  halfSize.y, halfSize.z) + offset,
                new Vector3(-halfSize.x,  halfSize.y, halfSize.z) + offset,
                // Back face
                new Vector3(-halfSize.x, -halfSize.y, -halfSize.z) + offset,
                new Vector3( halfSize.x, -halfSize.y, -halfSize.z) + offset,
                new Vector3( halfSize.x,  halfSize.y, -halfSize.z) + offset,
                new Vector3(-halfSize.x,  halfSize.y, -halfSize.z) + offset,
                // Left face
                new Vector3(-halfSize.x, -halfSize.y, -halfSize.z) + offset,
                new Vector3(-halfSize.x, -halfSize.y,  halfSize.z) + offset,
                new Vector3(-halfSize.x,  halfSize.y,  halfSize.z) + offset,
                new Vector3(-halfSize.x,  halfSize.y, -halfSize.z) + offset,
                // Right face
                new Vector3( halfSize.x, -halfSize.y, -halfSize.z) + offset,
                new Vector3( halfSize.x, -halfSize.y,  halfSize.z) + offset,
                new Vector3( halfSize.x,  halfSize.y,  halfSize.z) + offset,
                new Vector3( halfSize.x,  halfSize.y, -halfSize.z) + offset,
                // Top face
                new Vector3(-halfSize.x,  halfSize.y, -halfSize.z) + offset,
                new Vector3( halfSize.x,  halfSize.y, -halfSize.z) + offset,
                new Vector3( halfSize.x,  halfSize.y,  halfSize.z) + offset,
                new Vector3(-halfSize.x,  halfSize.y,  halfSize.z) + offset,
                // Bottom face
                new Vector3(-halfSize.x, -halfSize.y, -halfSize.z) + offset,
                new Vector3( halfSize.x, -halfSize.y, -halfSize.z) + offset,
                new Vector3( halfSize.x, -halfSize.y,  halfSize.z) + offset,
                new Vector3(-halfSize.x, -halfSize.y,  halfSize.z) + offset
            };
    
            int[] triangles = {
                // Front face
                0, 1, 2,
                0, 2, 3,
                // Back face
                4, 6, 5,
                4, 7, 6,
                // Left face
                8, 9, 10,
                8, 10, 11,
                // Right face
                12, 14, 13,
                12, 15, 14,
                // Top face
                16, 18, 17,
                16, 19, 18,
                // Bottom face
                20, 21, 22,
                20, 22, 23
            };
    
            Vector3[] normals = {
                // Front face
                Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
                // Back face
                Vector3.back, Vector3.back, Vector3.back, Vector3.back,
                // Left face
                Vector3.left, Vector3.left, Vector3.left, Vector3.left,
                // Right face
                Vector3.right, Vector3.right, Vector3.right, Vector3.right,
                // Top face
                Vector3.up, Vector3.up, Vector3.up, Vector3.up,
                // Bottom face
                Vector3.down, Vector3.down, Vector3.down, Vector3.down
            };
    
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
    
            meshFilter.mesh = mesh;
        }
    
        private void AssignMaterial()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer.sharedMaterial == null)
            {
                meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            }
            
            // meshRenderer.sharedMaterial.color = color;
        }
    
        private void UpdateCollider()
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            boxCollider.size = size;
            boxCollider.center = offset;
        }
    }
}
