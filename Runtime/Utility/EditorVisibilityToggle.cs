using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace JamKit
{
    public class EditorVisibilityToggle : MonoBehaviour
    {
#if UNITY_EDITOR
        public void ToggleEditorVisibility()
        {
            GameObject[] targets = GetTargetsToSync();
            if (targets == null || targets.Length == 0)
            {
                targets = new[] { gameObject };
            }

            SceneVisibilityManager sceneVisibilityManager = SceneVisibilityManager.instance;

            bool anyVisible = false;

            for (int i = 0; i < targets.Length; i++)
            {
                GameObject target = targets[i];

                if (target == null)
                    continue;

                if (!sceneVisibilityManager.IsHidden(target, true))
                {
                    anyVisible = true;
                    break;
                }
            }

            bool shouldHide = anyVisible;

            Undo.RecordObjects(targets, "Toggle Editor Visibility");

            for (int i = 0; i < targets.Length; i++)
            {
                GameObject target = targets[i];

                if (target == null)
                    continue;

                if (shouldHide)
                {
                    sceneVisibilityManager.Hide(target, true);
                }
                else
                {
                    sceneVisibilityManager.Show(target, true);
                }

                EditorUtility.SetDirty(target);
            }

            SceneView.RepaintAll();
        }

        private GameObject[] GetTargetsToSync()
        {
            if (!PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                return new[] { gameObject };
            }

            GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            if (prefabSource == null)
            {
                return new[] { gameObject };
            }

            List<GameObject> matches = new List<GameObject>();

            GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            for (int i = 0; i < allGameObjects.Length; i++)
            {
                GameObject candidate = allGameObjects[i];

                if (candidate == null)
                    continue;

                if (!candidate.scene.IsValid())
                    continue;

                if (EditorUtility.IsPersistent(candidate))
                    continue;

                if (PrefabUtility.GetPrefabInstanceStatus(candidate) == PrefabInstanceStatus.NotAPrefab)
                    continue;

                GameObject candidateSource = PrefabUtility.GetCorrespondingObjectFromSource(candidate);
                if (candidateSource != prefabSource)
                    continue;

                matches.Add(candidate);
            }

            return matches.ToArray();
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EditorVisibilityToggle))]
    public class EditorVisibilityToggleEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            EditorVisibilityToggle toggle = (EditorVisibilityToggle)target;

            using (new EditorGUI.DisabledScope(Application.isPlaying))
            {
                if (GUILayout.Button("Toggle Editor Visibility"))
                {
                    toggle.ToggleEditorVisibility();
                }
            }
        }
    }
#endif
}