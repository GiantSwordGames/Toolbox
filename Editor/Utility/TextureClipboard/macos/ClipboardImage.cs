using System;
using System.IO;
using System.Runtime.InteropServices;
using GiantSword;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class ClipboardTexturePaster2 : Editor
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectViewUpdate;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyUpdate;
        }

        private static void OnHierarchyUpdate(int instanceid, Rect selectionrect)
        {
            // create a sprite renderer and paste the clipboard image into it
            
            Event e = Event.current;

            if (e != null)
            {
                if (e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.V)
                    {
                        Sprite clipboardTexture = PasteClipboardImage();
                        if ( clipboardTexture)
                        {
                            e.Use();
                            
                            GameObject go = new GameObject(clipboardTexture.name);
                            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                            if (clipboardTexture != null)
                            {
                                sr.sprite = clipboardTexture;
                            }
                            RuntimeEditorHelper.Select(sr);
                        }
                    }
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
                        if (PasteClipboardImage())
                        {
                            e.Use();
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
                folderPath = "Assets/Project/Art/PastedTextures";
            }

            // Ensure the folder exists
            RuntimeEditorHelper.CreateDirectoryFromAssetPath(folderPath);

            // Get image data from the clipboard
            Texture2D clipboardTexture = GetImageFromClipboard();
            if (clipboardTexture == null)
            {
                Debug.LogError("No image data found in the clipboard.");
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
               RuntimeEditorHelper.Select(texture2D);

           }

           Debug.Log("Image pasted and saved as Texture2D at: " + texturePath,result);
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

