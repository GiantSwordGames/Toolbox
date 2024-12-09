using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GiantSwordEditor
{
    public static class CreateScriptableObject
    {
        [MenuItem("Assets/Create/ScriptableObject from Script", false, 1)]
        public static void CreateAsset()
        {
            // Get the selected script file
            var selected = Selection.activeObject;
            var scriptPath = AssetDatabase.GetAssetPath(selected);

            // Ensure the selected file is a script
            if (Path.GetExtension(scriptPath) != ".cs")
            {
                Debug.LogError("Selected file is not a C# script.");
                return;
            }

            // Extract the class name from the script file
            var className = Path.GetFileNameWithoutExtension(scriptPath);

            // Find the type in the current assembly
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.Name == className && typeof(ScriptableObject).IsAssignableFrom(t));

            if (type == null)
            {
                Debug.LogError($"No ScriptableObject class found with the name {className}.");
                return;
            }

            // Create an instance of the ScriptableObject
            var asset = ScriptableObject.CreateInstance(type);

            // Determine the path to save the asset
            var path = Path.GetDirectoryName(scriptPath);
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{className}.asset");

            // Save the asset
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/ScriptableObject from Script", true)]
        public static bool ValidateCreateAsset()
        {
            var selected = Selection.activeObject;
            var scriptPath = AssetDatabase.GetAssetPath(selected);
            return Path.GetExtension(scriptPath) == ".cs";
        }
    }
}