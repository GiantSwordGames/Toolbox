using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    public class ScriptableBoolViewer : EditorWindow
    {
        private Vector2 scroll;
        private List<ScriptableBool> foundAssets = new List<ScriptableBool>();
        private readonly Dictionary<ScriptableBool, Editor> editors = new Dictionary<ScriptableBool, Editor>();

        [MenuItem("Tools/Find All Scriptable Bools")]
        public static void ShowWindow()
        {
            GetWindow<ScriptableBoolViewer>("ScriptableBool Viewer");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Find All ScriptableBools", EditorStyles.boldLabel);

            if (GUILayout.Button("Find All"))
            {
                RefreshList();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Found {foundAssets.Count} assets", EditorStyles.miniBoldLabel);

            scroll = EditorGUILayout.BeginScrollView(scroll);

            foreach (var asset in foundAssets)
            {
                if (asset == null)
                    continue;

                // Draw foldout with default inspector
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                asset.name = EditorGUILayout.TextField("Name", asset.name);
                if (GUILayout.Button("Ping", GUILayout.Width(50)))
                    EditorGUIUtility.PingObject(asset);
                EditorGUILayout.EndHorizontal();

                if (!editors.TryGetValue(asset, out var editor) || editor == null)
                {
                    editor = Editor.CreateEditor(asset);
                    editors[asset] = editor;
                }

                EditorGUI.BeginDisabledGroup(true); // make it read-only if desired
                editor.OnInspectorGUI();
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }

        private void RefreshList()
        {
            foreach (var e in editors.Values)
                DestroyImmediate(e);
            editors.Clear();

            foundAssets = RuntimeEditorHelper.FindAssetsOfType<ScriptableBool>();
            Repaint();
        }

        private void OnDisable()
        {
            foreach (var e in editors.Values)
                DestroyImmediate(e);
            editors.Clear();
        }
    }
}
