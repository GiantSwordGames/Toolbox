using UnityEditor;
using UnityEngine;
using System.IO;

namespace GiantSword
{
    public class CustomEditorGenerator : ScriptableObject
    {
        [MenuItem("Assets/Create/Generate Custom Editor Script", false, 50)]
        private static void GenerateCustomEditorScript()
        {
            Object selectedObject = Selection.activeObject;

            if (selectedObject is MonoScript monoScript)
            {
                string className = monoScript.name;
                string scriptPath = AssetDatabase.GetAssetPath(monoScript);
                string directory = Path.GetDirectoryName(scriptPath);
                string editorDirectory = Path.Combine(directory, "Editor");

                // Create "Editor" folder if it doesn't exist
                if (!Directory.Exists(editorDirectory))
                {
                    Directory.CreateDirectory(editorDirectory);
                    AssetDatabase.Refresh();
                }

                string editorScriptPath = Path.Combine(editorDirectory, $"{className}Editor.cs");

                if (File.Exists(editorScriptPath))
                {
                    Debug.LogWarning($"Editor script for {className} already exists at {editorScriptPath}");
                    return;
                }

                string editorTemplate = $@"
using UnityEditor;
using UnityEngine;

namespace GiantSword
{{
    [CustomEditor(typeof({className}))]
    public class {className}Editor : CustomEditorBase<{className}>
    {{
        public override void OnInspectorGUI()
        {{
            base.OnInspectorGUI();

            // Add custom inspector code here
        }}
    }}
}}
";
                File.WriteAllText(editorScriptPath, editorTemplate);
                AssetDatabase.Refresh();

                // Select the generated script in the Project window
                Object generatedScript = AssetDatabase.LoadAssetAtPath<Object>(editorScriptPath);
                Selection.activeObject = generatedScript;

                Debug.Log($"Generated custom editor script for {className} at {editorScriptPath}");
            }
            else
            {
                Debug.LogError("Selected asset is not a MonoBehaviour script.");
            }
        }
    }
}
