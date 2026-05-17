using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

namespace JamKit
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

        public static bool CreateScriptFromClipboard()
        {
            string clipboardText = GUIUtility.systemCopyBuffer;

            if (string.IsNullOrWhiteSpace(clipboardText))
                return false;

            // Try shader first, then C# class
            if (IsShader(clipboardText))
                return CreateShaderFromClipboard(clipboardText);

            if (clipboardText.Contains("class "))
                return CreateCSharpFromClipboard(clipboardText);

            return false;
        }

        // ── Shader ────────────────────────────────────────────────────────────

        private static bool IsShader(string text)
        {
            // A Unity shader file always opens with  Shader "Some/Name" {
            return Regex.IsMatch(text, @"^\s*Shader\s+""", RegexOptions.Multiline);
        }

        private static bool CreateShaderFromClipboard(string clipboardText)
        {
            string shaderName = ExtractShaderName(clipboardText);
            if (string.IsNullOrEmpty(shaderName))
            {
                Debug.LogWarning("ClipboardToScript: Could not determine shader name.");
                return false;
            }

            string folderPath = GetSelectedFolderPath();
            if (string.IsNullOrEmpty(folderPath))
                return false;

            // Use only the last segment of "Category/ShaderName" as the filename
            string fileName = shaderName.Contains("/")
                ? shaderName.Substring(shaderName.LastIndexOf('/') + 1)
                : shaderName;

            // Sanitise – strip characters that are illegal in filenames
            fileName = Regex.Replace(fileName, @"[\\/:*?""<>|]", "_");

            string scriptPath = Path.Combine(folderPath, $"{fileName}.shader");
            File.WriteAllText(scriptPath, clipboardText);
            AssetDatabase.Refresh();

            Debug.Log($"Created new shader '{fileName}.shader' from clipboard in folder: {folderPath}");
            return true;
        }

        /// <summary>
        /// Pulls the name from:  Shader "Category/Name"  or  Shader 'Name'
        /// </summary>
        private static string ExtractShaderName(string text)
        {
            Match m = Regex.Match(text, @"Shader\s+[""']([^""']+)[""']");
            return m.Success ? m.Groups[1].Value : null;
        }

        // ── C# class ──────────────────────────────────────────────────────────

        private static bool CreateCSharpFromClipboard(string clipboardText)
        {
            string className = ExtractClassName(clipboardText);
            if (string.IsNullOrEmpty(className))
            {
                Debug.LogWarning("ClipboardToScript: Could not determine class name.");
                return false;
            }

            string folderPath = GetSelectedFolderPath();
            if (string.IsNullOrEmpty(folderPath))
                return false;

            string scriptPath = Path.Combine(folderPath, $"{className}.cs");
            File.WriteAllText(scriptPath, clipboardText);
            AssetDatabase.Refresh();

            Debug.Log($"Created new script '{className}.cs' from clipboard in folder: {folderPath}");
            return true;
        }

        private static string ExtractClassName(string clipboardText)
        {
            Match match = Regex.Match(clipboardText, @"\bclass\s+(\w+)");
            return match.Success ? match.Groups[1].Value : null;
        }

        // ── Shared helpers ────────────────────────────────────────────────────

        private static string GetSelectedFolderPath()
        {
            Object selected = Selection.activeObject;
            if (selected == null) return null;

            string path = AssetDatabase.GetAssetPath(selected);
            return Directory.Exists(path) ? path : Path.GetDirectoryName(path);
        }
    }
}