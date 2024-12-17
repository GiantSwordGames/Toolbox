using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ClipboardTexturePaster2 : Editor
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.projectWindowItemOnGUI += OnEditorUpdate;
        }

        private static void OnEditorUpdate(string guid, Rect selectionRect)
        {
            Event e = Event.current;

            if (e != null)
            {
                if (e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.V)
                    {
                        Debug.Log("v2");

                        PasteClipboardImage();
                    }
                }
            }
        }
      
        private static void PasteClipboardImage()
        {
            
            Debug.Log("PasteClipboardImage");
            // Check if a folder is selected
            string folderPath = GetSelectedFolderPath();
            if (string.IsNullOrEmpty(folderPath))
            {
                Debug.LogError("Please select a folder in the Project view to paste the texture.");
                return;
            }

            // Get image data from the clipboard
            Texture2D clipboardTexture = GetImageFromClipboard();
            if (clipboardTexture == null)
            {
                Debug.LogError("No image data found in the clipboard.");
                return;
            }

            // Save the texture as an asset
            string texturePath = Path.Combine(folderPath, "PastedTexture.png");
            File.WriteAllBytes(texturePath, clipboardTexture.EncodeToPNG());
            AssetDatabase.Refresh();

            TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                AssetDatabase.ImportAsset(texturePath, ImportAssetOptions.ForceUpdate);
            }

            Debug.Log("Image pasted and saved as Texture2D at: " + texturePath);
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

