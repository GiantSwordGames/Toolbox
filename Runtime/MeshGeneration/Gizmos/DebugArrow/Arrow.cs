namespace GiantSword
{
    using UnityEngine;

    public class Arrow : MonoGizmo
    {
        [SerializeField] private float _length = 2f;     // Height of the cone
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private Material _material;
        [SerializeField]  float _radius = 1f;
    
        const  float SCALE = 1f;   
        const  float SPHERE_RADIUS = 0.02f*SCALE;   
        const  float TUBE_RADIUS = 0.01f*SCALE;
        const  float CONE_RADIUS = 0.035f*SCALE;
        const int NUMBER_OF_SIDES = 20;  // Number of segments in the cone's base
    
        public float length
        {
            get => _length;
            set
            {
                _length = value;
                Regenerate();
            }
        }
    
        public Vector3 direction
        {
            get => transform.forward;
            set => transform.LookAt(transform.position + value);
        }
    
        public Vector3 position
        {
            get => transform.position;
            set => transform.position = value;
        }
    
        public float radius
        {
            get => _radius;
            set => _radius = value;
        }
    
        private void OnValidate()
        {
            Regenerate();
        }
    
        public void SetMaterial(Material material)
        {
            if (material)
            {
                _material = material;
                Regenerate();
            }
        }
    
        private void Regenerate()
        {
            return;
            // MeshFilter meshFilter = gameObject.GetOrAddComponent<MeshFilter>();
            // MeshRenderer meshRenderer = gameObject.GetOrAddComponent<MeshRenderer>();
            // Debug.Log("Regenerate");
            //
            // if (_material)
            // {
            //     meshRenderer.sharedMaterial = _material;
            // }
            // else if (meshRenderer.sharedMaterial == null)
            // {
            //     meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            //     meshRenderer.material.name = "ArrowMaterial";
            // }
            //
            // Matrix4x4 rotateToFaceZ = Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0));
            //
            // CombineInstance[] combine = new CombineInstance[3];
            //
            // combine[0].mesh = GenerateConeMesh();
            // combine[1].mesh = GenerateTubeMesh();
            // combine[2].mesh = GenerateSphereMesh();
            //
            // combine[0].transform = rotateToFaceZ * Matrix4x4.Translate(Vector3.up * length);
            // combine[1].transform = rotateToFaceZ * Matrix4x4.identity;
            // combine[2].transform = rotateToFaceZ * Matrix4x4.identity;
            //
            // Mesh combinedMesh = new Mesh();
            // combinedMesh.name = "ArrowMesh";
            // combinedMesh.CombineMeshes(combine);
            // meshFilter.mesh = combinedMesh;
            //
            // if (_material == null)
            // {
            //     MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            //     meshRenderer.GetPropertyBlock(propertyBlock);
            //     propertyBlock.SetColor("_BaseColor", _color);
            //     meshRenderer.SetPropertyBlock(propertyBlock);
            // }
        }
    
        Mesh GenerateConeMesh()
        {
            float coneRadius = CONE_RADIUS;
            float coneHeight = coneRadius * 2;
            Mesh mesh = new Mesh();
    
            Vector3[] vertices = new Vector3[NUMBER_OF_SIDES + 2];
            int[] triangles = new int[NUMBER_OF_SIDES * 3 + NUMBER_OF_SIDES * 3];
    
            // Tip of the cone
            vertices[0] = Vector3.up * coneHeight;
    
            // Base vertices
            float angleStep = 360.0f / NUMBER_OF_SIDES;
            for (int i = 0; i < NUMBER_OF_SIDES; i++)
            {
                float angle = Mathf.Deg2Rad * i * angleStep;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle) * coneRadius, 0, Mathf.Sin(angle) * coneRadius);
            }
    
            // Center of the base
            vertices[NUMBER_OF_SIDES + 1] = Vector3.zero;
    
            // Side triangles
            for (int i = 0; i < NUMBER_OF_SIDES; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i == NUMBER_OF_SIDES - 1 ? 1 : i + 2;
            }
    
            // Base triangles
            for (int i = 0; i < NUMBER_OF_SIDES; i++)
            {
                int baseIndex = NUMBER_OF_SIDES * 3 + i * 3;
                triangles[baseIndex] = NUMBER_OF_SIDES + 1;
                triangles[baseIndex + 1] = i == NUMBER_OF_SIDES - 1 ? 1 : i + 2;
                triangles[baseIndex + 2] = i + 1;
            }
    
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            
            FlipMeshNormals(mesh);
            return mesh;
        }
        
        
        Mesh GenerateTubeMesh()
        {
            float tubeRadius = TUBE_RADIUS*radius;
            float tubeHeight = length;
            Mesh mesh = new Mesh();
    
            Vector3[] vertices = new Vector3[NUMBER_OF_SIDES * 2];
            int[] triangles = new int[NUMBER_OF_SIDES * 6];
    
            // Create vertices for top and bottom rings
            float angleStep = 360.0f / NUMBER_OF_SIDES;
            for (int i = 0; i < NUMBER_OF_SIDES; i++)
            {
                float angle = Mathf.Deg2Rad * i * angleStep;
                float x = Mathf.Cos(angle) * tubeRadius;
                float z = Mathf.Sin(angle) * tubeRadius;
    
                vertices[i] = new Vector3(x, tubeHeight, z);       // Top ring
                vertices[i + NUMBER_OF_SIDES] = new Vector3(x, 0, z); // Bottom ring
            }
    
            // Create triangles
            for (int i = 0; i < NUMBER_OF_SIDES; i++)
            {
                int topIndex = i;
                int bottomIndex = i + NUMBER_OF_SIDES;
                int nextTopIndex = (i + 1) % NUMBER_OF_SIDES;
                int nextBottomIndex = ((i + 1) % NUMBER_OF_SIDES) + NUMBER_OF_SIDES;
    
                // Side triangles
                int triangleIndex = i * 6;
                triangles[triangleIndex] = topIndex;
                triangles[triangleIndex + 1] = nextBottomIndex;
                triangles[triangleIndex + 2] = bottomIndex;
    
                triangles[triangleIndex + 3] = topIndex;
                triangles[triangleIndex + 4] = nextTopIndex;
                triangles[triangleIndex + 5] = nextBottomIndex;
            }
    
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
    
            return mesh;
        }
        
      
        Mesh GenerateSphereMesh()
        {
            float sphereRadius = SPHERE_RADIUS;
            int longitudeSegments = 24; // Number of longitudinal segments
            int latitudeSegments = 16;  // Number of latitudinal segments
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[(longitudeSegments + 1) * (latitudeSegments + 1)];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[longitudeSegments * latitudeSegments * 6];
    
            for (int lat = 0; lat <= latitudeSegments; lat++)
            {
                float theta = lat * Mathf.PI / latitudeSegments;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);
    
                for (int lon = 0; lon <= longitudeSegments; lon++)
                {
                    float phi = lon * 2 * Mathf.PI / longitudeSegments;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);
    
                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;
                    vertices[lat * (longitudeSegments + 1) + lon] = new Vector3(x, y, z) * sphereRadius;
                    uv[lat * (longitudeSegments + 1) + lon] = new Vector2((float)lon / longitudeSegments, (float)lat / latitudeSegments);
                }
            }
    
            int index = 0;
            for (int lat = 0; lat < latitudeSegments; lat++)
            {
                for (int lon = 0; lon < longitudeSegments; lon++)
                {
                    int current = lat * (longitudeSegments + 1) + lon;
                    int next = current + longitudeSegments + 1;
    
                    triangles[index++] = current;
                    triangles[index++] = next + 1;
                    triangles[index++] = next;
    
                    triangles[index++] = current;
                    triangles[index++] = current + 1;
                    triangles[index++] = next + 1;
                }
            }
    
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
    
            return mesh;
        }
        
        void FlipMeshNormals(Mesh mesh)
        {
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;
    
            // Invert the normals
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -normals[i];
            }
            mesh.normals = normals;
    
            // Reverse the winding order of the triangles
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int temp = triangles[i];
                triangles[i] = triangles[i + 1];
                triangles[i + 1] = temp;
            }
            mesh.triangles = triangles;
        }
    }
}
