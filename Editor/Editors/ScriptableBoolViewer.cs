using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    public class ScriptableBoolViewer : EditorWindow
    {
        [System.Serializable]
        private class Wrapper : ScriptableObject
        {
            public ScriptableBool value;
        }

        private Vector2 scroll;
        private List<ScriptableBool> foundAssets = new List<ScriptableBool>();
        private readonly List<SerializedObject> wrappers = new List<SerializedObject>();

        // [MenuItem("Tools/Find All ScriptableBools")]
        public static void ShowWindow()
        {
            GetWindow<ScriptableBoolViewer>("ScriptableBool Viewer");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Find All ScriptableBools", EditorStyles.boldLabel);

            if (GUILayout.Button("Find All"))
                RefreshList();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Found {foundAssets.Count} assets", EditorStyles.miniBoldLabel);

            scroll = EditorGUILayout.BeginScrollView(scroll);

            foreach (var so in wrappers)
            {
                so.Update();

                EditorGUILayout.BeginVertical("box");
                SerializedProperty prop = so.FindProperty("value");
                EditorGUILayout.PropertyField(prop, new GUIContent(prop.objectReferenceValue != null ? prop.objectReferenceValue.name : "null"), true);
                so.ApplyModifiedProperties();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }

        private void RefreshList()
        {
            foundAssets = RuntimeEditorHelper.FindAssetsOfType<ScriptableBool>();
            wrappers.Clear();

            foreach (var asset in foundAssets)
            {
                var wrapper = ScriptableObject.CreateInstance<Wrapper>();
                wrapper.value = asset;
                wrappers.Add(new SerializedObject(wrapper));
            }

            Repaint();
        }

        private void OnDisable()
        {
            foreach (var so in wrappers)
                if (so.targetObject != null)
                    DestroyImmediate(so.targetObject);
            wrappers.Clear();
        }
    }
}

