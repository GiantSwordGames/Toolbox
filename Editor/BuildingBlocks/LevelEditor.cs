#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace JamKit.EditorTools
{
    [CustomEditor(typeof(JamKit.Level))]
    public class LevelEditor_MissingScenes : Editor
    {
        private void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            // Draw the normal inspector first
            serializedObject.Update();
            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Build Settings", EditorStyles.boldLabel);

            // Compute referenced scene paths and which are missing
            var referenced = GetAllReferencedScenePaths().ToList();
            var missing = referenced.Where(p => !string.IsNullOrEmpty(p) && !BuildSettingsContains(p)).Distinct().ToList();

            using (new EditorGUILayout.VerticalScope("box"))
            {
                if (missing.Count > 0)
                {
                    EditorGUILayout.HelpBox($"{missing.Count} referenced scene(s) are NOT in Build Settings.", MessageType.Warning);

                    foreach (var path in missing)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            // Show path (readonly)
                            // GUI.enabled = false; // Make the text field read-only
                            EditorGUILayout.LabelField(path);
                            // GUI.enabled = true; // Re-enable GUI for the button

                            // Add button per scene
                            // if (GUILayout.Button("Add", GUILayout.Width(64)))
                            // {
                            //     AddSceneToBuildSettings(path, true);
                            // }
                        }
                    }

                    EditorGUILayout.Space(4);
                    if (GUILayout.Button("Add to Build Settings"))
                    {
                        foreach (var path in missing)
                        {
                            AddSceneToBuildSettings(path, true);
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("All referenced scenes are already present in Build Settings.", MessageType.Info);
                }

                EditorGUILayout.Space(2);
              
            }
        }

        // ---- Helpers ------------------------------------------------------

        private static bool BuildSettingsContains(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            foreach (var s in EditorBuildSettings.scenes)
            {
                // Use ordinal ignore-case for Windows/macOS case-insensitive file systems
                if (string.Equals(s.path, path, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static void AddSceneToBuildSettings(string path, bool enabled)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("[LevelEditor] Cannot add empty scene path to Build Settings.");
                return;
            }

            // Verify the asset exists and is a scene
            var mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
            if (mainAsset == null)
            {
                Debug.LogWarning($"[LevelEditor] Scene asset not found at path: {path}");
                return;
            }

            if (BuildSettingsContains(path))
                return; // already present

            var list = EditorBuildSettings.scenes.ToList();
            list.Add(new EditorBuildSettingsScene(path, enabled));
            EditorBuildSettings.scenes = list.ToArray();

            Debug.Log($"[LevelEditor] Added scene to Build Settings: {path}");
        }

        private List<string> GetAllReferencedScenePaths()
        {
            Level level = target as JamKit.Level;

            List<string> scenePaths = new List<string>(); 
            List<SceneReference> allSceneReferences = level.GetAllSceneReferences();
            foreach (SceneReference sceneReference in allSceneReferences)
            {
                string path = sceneReference.EditorAssetPath;
                if (!string.IsNullOrEmpty(path) && path.EndsWith(".unity"))
                {
                    scenePaths.Add(path);
                }
            }

            return scenePaths;
        }


    }
}
#endif
