using UnityEngine;
using UnityEditor;

namespace GiantSword
{
    public class CreateParticleMaterialContext
    {
        private const string MaterialFolderPath = "Assets/Project/Materials";

        [MenuItem("CONTEXT/ParticleSystem/Duplicate Material")]
        private static void CreateURPMaterial(MenuCommand command)
        {
            // Ensure the folder exists
            EnsureFolderExists("Assets/Project");
            EnsureFolderExists(MaterialFolderPath);

            // Get the target ParticleSystem from the context menu
            ParticleSystem particleSystem = command.context as ParticleSystem;
            if (particleSystem == null)
            {
                Debug.LogError("Target is not a ParticleSystem.");
                return;
            }

            // Get the renderer for the ParticleSystem
            var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            if (renderer == null)
            {
                Debug.LogError("ParticleSystemRenderer not found.");
                return;
            }

            // Create a new URP-compatible material

            Material material = new Material(renderer.sharedMaterial);
            string path = AssetDatabase.GenerateUniqueAssetPath($"{MaterialFolderPath}/Material_{renderer.name.ToUpperCamelCase()}.mat");

            AssetDatabase.CreateAsset(material, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Assign the material to the particle system's renderer
            renderer.sharedMaterial = material;

            // Select the new material in the Project window
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = material;
        }

        private static void EnsureFolderExists(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
                string folderName = System.IO.Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }
    }
}
