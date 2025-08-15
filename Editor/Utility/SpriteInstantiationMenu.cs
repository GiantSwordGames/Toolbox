using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    public static class SpriteInstantiationMenu
    {
        [MenuItem("Assets/Instantiate Sprites in Scene", true)]
        private static bool ValidateInstantiateSpritesInScene()
        {
            // Validate that the selection contains sprites or textures in sprite mode
            return Selection.objects.Length > 0 && Selection.objects.All(obj => obj is Sprite || IsSpriteTexture(obj));
        }

        [MenuItem("Assets/Instantiate Sprites in Scene")]
        private static void InstantiateSpritesInScene()
        {
            // Get the Scene View camera position
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView == null)
            {
                Debug.LogError("No active Scene View to determine instantiation position.");
                return;
            }

            Vector3 basePosition = sceneView.pivot + sceneView.camera.transform.forward * 5f;

            float offset = 1.0f; // Distance between each sprite
            Vector3 spawnPosition = basePosition;

            // List to store created objects
            var createdObjects = new System.Collections.Generic.List<GameObject>();

            foreach (Object obj in Selection.objects)
            {
                Sprite sprite = null;

                if (obj is Sprite selectedSprite)
                {
                    // If the selected object is a sprite, use it directly
                    sprite = selectedSprite;
                }
                else if (IsSpriteTexture(obj))
                {
                    // Get the sprite associated with the texture
                    sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(obj));
                }

                if (sprite != null)
                {
                    // Create a new GameObject with a SpriteRenderer
                    GameObject newObject = new GameObject(sprite.name);
                    SpriteRenderer renderer = newObject.AddComponent<SpriteRenderer>();
                    renderer.sprite = sprite;

                    // Set the spawn position
                    newObject.transform.position = spawnPosition;

                    // Offset the spawn position for the next sprite
                    spawnPosition.x += offset;

                    // Register the new GameObject for undo
                    Undo.RegisterCreatedObjectUndo(newObject, "Instantiate Sprite");

                    // Add the new object to the list
                    createdObjects.Add(newObject);
                }
            }

            // Select the newly created objects in the scene
            if (createdObjects.Count > 0)
            {
                Selection.objects = createdObjects.ToArray();
            }

            // Focus the scene view on the newly instantiated objects
            SceneView.lastActiveSceneView.FrameSelected();
        }

        private static bool IsSpriteTexture(Object obj)
        {
            if (obj is Texture2D texture)
            {
                // Check if the texture is set to Sprite mode
                string assetPath = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                return importer != null && importer.textureType == TextureImporterType.Sprite;
            }

            return false;
        }
    }
}
