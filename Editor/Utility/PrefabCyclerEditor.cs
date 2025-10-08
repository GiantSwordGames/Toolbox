#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(PrefabCycler))]
    public class PrefabCyclerEditor : Editor
    {
        private static double _lastScrollTime;

        [InitializeOnLoadMethod]
        private static void InitScrollHandler()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            Event e = Event.current;
            if (e.type != EventType.ScrollWheel || !e.shift) return;

            float axis = Mathf.Abs(e.delta.y) > 0.0001f ? e.delta.y : e.delta.x;
            if (Mathf.Abs(axis) < 0.0001f) return;

            // Throttle
            if (EditorApplication.timeSinceStartup - _lastScrollTime < 0.1) { e.Use(); return; }
            _lastScrollTime = EditorApplication.timeSinceStartup;

            var selected = Selection.gameObjects;
            if (selected == null || selected.Length == 0) return;

            // Use first selection to determine candidates
            var firstComp = selected[0].GetComponentInParent<PrefabCycler>();
            if (!firstComp) return;

            var instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(firstComp.gameObject);
            if (!instanceRoot) return;

            var currentAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instanceRoot);
            if (string.IsNullOrEmpty(currentAssetPath)) return;

            var folder = NormalizeToUnityPath(Path.GetDirectoryName(currentAssetPath));
            var candidates = GetCycleCandidatesInFolder(folder);
            if (candidates.Count == 0) return;

            int currentIndex = candidates.FindIndex(c => c.assetPath == currentAssetPath);
            int delta = axis > 0 ? -1 : 1;

            List<GameObject> newSelections = new List<GameObject>();

            // Cycle each selected object
            foreach (var go in selected)
            {
                var comp = go.GetComponentInParent<PrefabCycler>();
                if (!comp) continue;

                var root = PrefabUtility.GetNearestPrefabInstanceRoot(comp.gameObject);
                if (!root) continue;

                var newRoot = Cycle(root, candidates, currentIndex, delta);
                if (newRoot) newSelections.Add(newRoot);
            }

            if (newSelections.Count > 0)
                Selection.objects = newSelections.ToArray();

            e.Use();
        }

        public override void OnInspectorGUI()
        {
            var comp = (PrefabCycler)target;
            var instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(comp.gameObject);

            if (instanceRoot == null)
            {
                EditorGUILayout.HelpBox("Select prefab instances in the Scene to cycle.", MessageType.Info);
                return;
            }

            var currentAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instanceRoot);
            var folder = NormalizeToUnityPath(Path.GetDirectoryName(currentAssetPath));
            var candidates = GetCycleCandidatesInFolder(folder);
            int currentIndex = candidates.FindIndex(c => c.assetPath == currentAssetPath);

            EditorGUILayout.LabelField("Folder", folder);
            EditorGUILayout.LabelField("Index", $"{(currentIndex >= 0 ? currentIndex + 1 : 0)} / {candidates.Count}");

            EditorGUILayout.HelpBox("Tip: Hold Shift and use the mouse wheel in the Scene View to cycle all selected objects.", MessageType.None);
        }

        private struct PrefabEntry
        {
            public string assetPath;
            public GameObject asset;
        }

        private static string NormalizeToUnityPath(string p)
        {
            if (string.IsNullOrEmpty(p)) return p;
            return p.Replace('\\', '/');
        }

        private static List<PrefabEntry> GetCycleCandidatesInFolder(string folderPath)
        {
            var list = new List<PrefabEntry>();
            if (string.IsNullOrEmpty(folderPath)) return list;

            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (go == null) continue;

                if (go.GetComponent<PrefabCycler>() != null)
                {
                    list.Add(new PrefabEntry { assetPath = path, asset = go });
                }
            }

            list.Sort((a, b) =>
            {
#if UNITY_2021_2_OR_NEWER
                return EditorUtility.NaturalCompare(Path.GetFileNameWithoutExtension(a.assetPath),
                                                    Path.GetFileNameWithoutExtension(b.assetPath));
#else
                return string.Compare(Path.GetFileNameWithoutExtension(a.assetPath),
                                      Path.GetFileNameWithoutExtension(b.assetPath), System.StringComparison.OrdinalIgnoreCase);
#endif
            });
            return list;
        }

        private static GameObject Cycle(GameObject instanceRoot, List<PrefabEntry> candidates, int currentIndex, int delta)
        {
            if (candidates.Count == 0) return null;
            int nextIndex = candidates.WrapIndex(currentIndex + delta);
            var nextPrefab = candidates[nextIndex].asset;
            return ReplaceInstanceWith(instanceRoot, nextPrefab);
        }

        private static GameObject ReplaceInstanceWith(GameObject currentInstanceRoot, GameObject nextPrefabAsset)
        {
            if (currentInstanceRoot == null || nextPrefabAsset == null || !currentInstanceRoot.activeSelf) return null;
            if (!PrefabUtility.IsPartOfPrefabInstance(currentInstanceRoot)) return null;

            var parent = currentInstanceRoot.transform.parent;
            int siblingIndex = currentInstanceRoot.transform.GetSiblingIndex();

            var localPos = currentInstanceRoot.transform.localPosition;
            var localRot = currentInstanceRoot.transform.localRotation;

            var staticFlags = GameObjectUtility.GetStaticEditorFlags(currentInstanceRoot);
            var scene = currentInstanceRoot.scene;

            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Cycle Prefab");

            var newRootObj = PrefabUtility.InstantiatePrefab(nextPrefabAsset, scene) as GameObject;
            Undo.RegisterCreatedObjectUndo(newRootObj, "Create replacement prefab");

            Undo.SetTransformParent(newRootObj.transform, parent, "Set parent");
            Undo.RecordObject(newRootObj.transform, "Set transform");
            newRootObj.transform.SetSiblingIndex(siblingIndex);
            newRootObj.transform.localPosition = localPos;
            newRootObj.transform.localRotation = localRot;

            Undo.RecordObject(newRootObj, "Set properties");
            GameObjectUtility.SetStaticEditorFlags(newRootObj, staticFlags);

            Undo.DestroyObjectImmediate(currentInstanceRoot);

            if (scene.IsValid())
                EditorSceneManager.MarkSceneDirty(scene);

            Undo.CollapseUndoOperations(group);
            return newRootObj;
        }
    }
}
#endif
