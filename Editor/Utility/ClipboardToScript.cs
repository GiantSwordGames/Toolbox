using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

namespace GiantSword
{
    public class ClipboardToScript
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

                            if (ClipboardToScript.CreateScriptFromClipboard())
                            {
                                e.Use();
                            }
                        }
                    }
            }
        }
        
        // [MenuItem("Assets/Create C# Script from Clipboard", priority = 0)]
        public static bool CreateScriptFromClipboard()
        {
            // Get the text from the clipboard
            string clipboardText = GUIUtility.systemCopyBuffer;

            // Validate the clipboard text
            if (string.IsNullOrWhiteSpace(clipboardText) || !clipboardText.Contains("class "))
            {
                // Debug.LogWarning("Clipboard does not contain valid C# class text.");
                return false;
            }

            // Extract the class name from the clipboard text
            string className = ExtractClassName(clipboardText);
            if (string.IsNullOrEmpty(className))
            {
                // Debug.LogWarning("Could not determine the class name from the clipboard content.");
                return false;
            }

            // Get the selected folder path in the Project view
            string folderPath = GetSelectedFolderPath();
            if (string.IsNullOrEmpty(folderPath))
            {
                // Debug.LogWarning("Please right-click on a folder to use this action.");
                return false;
            }

            // Construct the full path for the new script
            string scriptPath = Path.Combine(folderPath, $"{className}.cs");

            // Write the clipboard text to the new file
            File.WriteAllText(scriptPath, clipboardText);

            // Refresh the asset database to make the new script appear in Unity
            AssetDatabase.Refresh();

            Debug.Log($"Created new script '{className}.cs' from clipboard in folder: {folderPath}");
            return true;

        }

        // Extracts the class name from the clipboard content
        private static string ExtractClassName(string clipboardText)
        {
            // Use regex to find the class name (assuming a typical "class ClassName" structure)
            Match match = Regex.Match(clipboardText, @"\bclass\s+(\w+)");
            return match.Success ? match.Groups[1].Value : null;
        }

        // Gets the path of the selected folder in the Project view
        private static string GetSelectedFolderPath()
        {
            // Get the selected object in the Project view
            Object selected = Selection.activeObject;
            if (selected == null) return null;

            // Get the path to the selected object
            string path = AssetDatabase.GetAssetPath(selected);

            // Check if the selected object is a folder
            return Directory.Exists(path) ? path : Path.GetDirectoryName(path);
        }
    }
}
