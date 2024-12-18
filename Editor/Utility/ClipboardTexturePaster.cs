using UnityEditor;
using UnityEngine;
using System.IO;

namespace GiantSword
{
    [InitializeOnLoad]
    public class ClipboardTexturePaster : Editor
    {
        static ClipboardTexturePaster()
        {
            EditorApplication.update += CheckForPasteShortcut;
        }

        private static void CheckForPasteShortcut()
        {
            // Check if Command (or Control on Windows) and V are pressed
            Event currentEvent = Event.current;
            if (currentEvent == null || !currentEvent.isKey || !currentEvent.control || !currentEvent.command ||
                currentEvent.keyCode != KeyCode.V)
                return;

            // Ensure we're in the Project view
            if (!EditorWindow.focusedWindow || EditorWindow.focusedWindow.titleContent.text != "Project")
                return;

            // Trigger paste functionality
            if (PasteClipboardImage())
            {

                // Consume the event
                currentEvent.Use();
            }
        }

        [MenuItem("Assets/Paste Clipboard Image as Texture", false, 150)]
        private static bool PasteClipboardImage()
        {
            // Check if a folder is selected
            string folderPath = GetSelectedFolderPath();
            if (string.IsNullOrEmpty(folderPath))
            {
                Debug.LogError("Please select a folder in the Project view to paste the texture.");
                return false;
            }

            // Get image data from the clipboard
            Texture2D clipboardTexture = GetImageFromClipboard();
            if (clipboardTexture == null)
            {
                return false;
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
            return true;
        }

        private static string GetSelectedFolderPath()
        {
            foreach (Object obj in Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (AssetDatabase.IsValidFolder(path))
                    return path;
            }
            return null;
        }

        private static Texture2D GetImageFromClipboard()
        {
            // This requires a native plugin for clipboard image access on MacOS.
            Debug.LogError("Accessing image data from the clipboard is not natively supported by Unity. Use a plugin for clipboard access.");
            return null;
        }
    }
}
