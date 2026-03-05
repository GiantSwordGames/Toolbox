#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace JamKitEditor
{
    public static class EditorWindowTemplateCreator
    {
        [MenuItem("Assets/Create/Editor Window Script", false, 3010)]
        private static void CreateEditorWindowScript()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "New Editor Window Script",
                "NewEditorWindow",
                "cs",
                "Choose a location for the new editor window script."
            );

            if (string.IsNullOrEmpty(path))
                return;

            string className = Path.GetFileNameWithoutExtension(path);

            var code = new StringBuilder();
            code.AppendLine("using UnityEngine;");
            code.AppendLine("using UnityEditor;");
            code.AppendLine();
            code.AppendLine("namespace JamKitEditor");
            code.AppendLine("{");
            code.AppendLine($"\tpublic class {className} : EditorWindow");
            code.AppendLine("\t{");
            code.AppendLine($"\t\t[MenuItem(\"Window/JamKit/{className}\")]");
            code.AppendLine("\t\tpublic static void ShowWindow()");
            code.AppendLine("\t\t{");
            code.AppendLine($"\t\t\tvar window = GetWindow<{className}>();");
            code.AppendLine($"\t\t\twindow.titleContent = new GUIContent(\"{className}\");");
            code.AppendLine("\t\t\twindow.Show();");
            code.AppendLine("\t\t}");
            code.AppendLine();
            code.AppendLine("\t\tprivate void OnEnable()");
            code.AppendLine("\t\t{");
            code.AppendLine("\t\t\t// Initialize window state here");
            code.AppendLine("\t\t}");
            code.AppendLine();
            code.AppendLine("\t\tprivate void OnGUI()");
            code.AppendLine("\t\t{");
            code.AppendLine("\t\t\tEditorGUILayout.LabelField(\"Generic Editor Window\", EditorStyles.boldLabel);");
            code.AppendLine("\t\t\tEditorGUILayout.Space();");
            code.AppendLine("\t\t\tEditorGUILayout.HelpBox(\"Add your custom GUI here.\", MessageType.Info);");
            code.AppendLine();
            code.AppendLine("\t\t\t// Example controls:");
            code.AppendLine("\t\t\tEditorGUILayout.TextField(\"Example Text\", \"\");");
            code.AppendLine("\t\t\tif (GUILayout.Button(\"Do Thing\"))");
            code.AppendLine("\t\t\t{");
            code.AppendLine("\t\t\t\tDebug.Log(\"Button pressed in " + className + "\");");
            code.AppendLine("\t\t\t}");
            code.AppendLine("\t\t}");
            code.AppendLine("\t}");
            code.AppendLine("}");

            File.WriteAllText(path, code.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();
        }
    }
}
#endif
