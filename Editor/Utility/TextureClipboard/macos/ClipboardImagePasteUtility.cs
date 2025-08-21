using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace JamKit.ClipboardImagePaste
{
    public class ClipboardImagePasteUtility : Editor
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectViewUpdate;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyUpdate;
            SceneView.duringSceneGui += OnSceneViewUpdate;
        }

        private static void OnSceneViewUpdate(SceneView obj)
        {
            TryPasteIntoScene();
        }

        private static void OnHierarchyUpdate(int instanceid, Rect selectionrect)
        {
            TryPasteIntoScene();
        }
        private static void TryPasteIntoScene()
        {
            Event e = Event.current;
            if (e != null && e.type == EventType.KeyDown && e.keyCode == KeyCode.V)
            {
                Sprite clipboardSprite = PasteClipboardImage();
                if (clipboardSprite)
                {
                    e.Use();
                    
                    // PRIORITY 1: Assign to Selected UI Image (UnityEngine.UI.Image)
                    foreach (var obj in Selection.gameObjects)
                    {
                        Debug.Log(obj);
                        Image uiImage = obj.GetComponent<Image>();
                        if (uiImage != null)
                        {
                            Undo.RecordObject(uiImage, "Paste Sprite to UI Image");
                            uiImage.sprite = clipboardSprite;
                            Debug.Log("Pasted image assigned to UI Image: " + obj.name, obj);
                            return; // Exit early, we're done
                        }
                    }

                    // PRIORITY 2: Spawn a SpriteRenderer in Scene View
                    GameObject go = new GameObject(clipboardSprite.name);
                    SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = clipboardSprite;
                    spriteRenderer.transform.position = RuntimeEditorHelper.GetSceneCenterPosition().WithZ(0);

                    if (Selection.activeTransform)
                    {
                        spriteRenderer.transform.SetParent(Selection.activeTransform);
                    }
                    else
                    {
                        Collider2D collider2D = Physics2D.OverlapCircle(spriteRenderer.transform.position, 10);
                        if (collider2D)
                        {
                            EditorSceneManager.MoveGameObjectToScene(spriteRenderer.gameObject, collider2D.transform.root.gameObject.scene);
                        }
                    }

                    Selection.activeObject = spriteRenderer;
                    Debug.Log("Pasted image spawned in Scene View as SpriteRenderer", spriteRenderer);
                }
            }
        }

        private static void OnProjectViewUpdate(string guid, Rect selectionRect)
        {
            Event e = Event.current;

            if (e != null)
            {
                if (e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.V)
                    {
                        Sprite sprite = PasteClipboardImage();
                        if (sprite)
                        {
                            e.Use();
                            RuntimeEditorHelper.SelectAndFocus(sprite);
                        }
                    }
                }
            }
        }

        private static Sprite PasteClipboardImage()
        {
            Sprite result = null;
            // Check if a folder is selected
            string folderPath = GetSelectedFolderPath();
            if (string.IsNullOrEmpty(folderPath))
            {
                
                folderPath = MenuPaths.DEFAULT_PROJECT_PATH+"Art/PastedTextures";
            }

            // Ensure the folder exists
            CreateDirectoryFromAssetPath(folderPath);

            // Get image data from the clipboard
            Texture2D clipboardTexture = GetImageFromClipboard();
            if (clipboardTexture == null)
            {
                return null;
            }

            // Save the texture as an asset
            string texturePath = Path.Combine(folderPath, $"PastedTexture{Random.Range(111, 999)}.png");
            File.WriteAllBytes(texturePath, clipboardTexture.EncodeToPNG());
            AssetDatabase.Refresh();

            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
                result = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
                Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            }
            Debug.Log("Image pasted and saved as Texture2D at: " + texturePath, result);
            return result;
        }

        private static string GetSelectedFolderPath()
        {
            foreach (Object obj in Selection.GetFiltered<Object>(mode: SelectionMode.Assets))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (AssetDatabase.IsValidFolder(path))
                {
                    return path;
                }
                else if (File.Exists(path))
                {
                    return Path.GetDirectoryName(path);
                }
            }

            return null;
        }
        
        public static void CreateDirectoryFromAssetPath(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string[] folders = folderPath.Split('/');
                string currentPath = "";
                for (int i = 0; i < folders.Length; i++)
                {
                    if (i == 0)
                    {
                        currentPath = folders[i];
                    }
                    else
                    {
                        string newPath = currentPath + "/" + folders[i];
                        if (!AssetDatabase.IsValidFolder(newPath))
                        {
                            AssetDatabase.CreateFolder(currentPath, folders[i]);
                        }
                        currentPath = newPath;
                    }
                }
            }
        }


#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        [DllImport("ClipboardPlugin")]
        private static extern IntPtr GetClipboardImagePath();
#endif

        public static Texture2D GetImageFromClipboard()
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            IntPtr pathPtr = GetClipboardImagePath();
            if (pathPtr == IntPtr.Zero)
            {
                Debug.LogWarning("No image found in clipboard.");
                return null;
            }

            string imagePath = Marshal.PtrToStringAnsi(pathPtr);
            if (!string.IsNullOrEmpty(imagePath))
            {
                byte[] fileData = System.IO.File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(fileData))
                {
                    return texture;
                }
            }

            Debug.LogError("Failed to load image from clipboard.");
#endif
            return null;
        }
    }

}