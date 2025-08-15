#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GiantSword
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

            // macOS: Shift converts vertical scroll to horizontal → delta.y = 0, delta.x ≠ 0
            float axis = Mathf.Abs(e.delta.y) > 0.0001f ? e.delta.y : e.delta.x;
            if (Mathf.Abs(axis) < 0.0001f) return;

            // Throttle
            if (EditorApplication.timeSinceStartup - _lastScrollTime < 0.1) { e.Use(); return; }
            _lastScrollTime = EditorApplication.timeSinceStartup;

            var go = Selection.activeGameObject;
            if (!go) return;

            var comp = go.GetComponentInParent<PrefabCycler>();
            if (!comp) return;

            var instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(comp.gameObject);
            if (!instanceRoot) return;

            var currentAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instanceRoot);
            if (string.IsNullOrEmpty(currentAssetPath)) return;

            var folder = NormalizeToUnityPath(Path.GetDirectoryName(currentAssetPath));
            var candidates = GetCycleCandidatesInFolder(folder);
            if (candidates.Count == 0) return;

            int currentIndex = candidates.FindIndex(c => c.assetPath == currentAssetPath);

            // Positive axis (scroll down/right) → previous, negative (up/left) → next
            int delta = axis > 0 ? -1 : 1;
            Cycle(instanceRoot, candidates, currentIndex, delta);

            e.Use();
        }

        public override void OnInspectorGUI()
        {

            var comp = (PrefabCycler)target;
            var instanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(comp.gameObject);

            if (instanceRoot == null)
            {
                EditorGUILayout.HelpBox("Select a prefab INSTANCE in the Scene to cycle.", MessageType.Info);
                return;
            }

            var currentAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instanceRoot);
            var folder = NormalizeToUnityPath(Path.GetDirectoryName(currentAssetPath));
            var candidates = GetCycleCandidatesInFolder(folder);
            int currentIndex = candidates.FindIndex(c => c.assetPath == currentAssetPath);

            EditorGUILayout.LabelField("Folder", folder);
            EditorGUILayout.LabelField("Index", $"{(currentIndex >= 0 ? currentIndex + 1 : 0)} / {candidates.Count}");

            EditorGUILayout.HelpBox("Tip: Hold Shift and use the mouse wheel in the Scene View to cycle.", MessageType.None);
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

        private static void Cycle(GameObject instanceRoot, List<PrefabEntry> candidates, int currentIndex, int delta)
        {
            if (candidates.Count == 0) return;

            Debug.Log(currentIndex + " "+ delta);
            int nextIndex = candidates.WrapIndex(currentIndex + delta);

            var nextPrefab = candidates[nextIndex].asset;
            ReplaceInstanceWith(instanceRoot, nextPrefab);
        }

        private static void ReplaceInstanceWith(GameObject currentInstanceRoot, GameObject nextPrefabAsset)
        {
            if (currentInstanceRoot == null || nextPrefabAsset == null || currentInstanceRoot.activeSelf == false) return;
            if (!PrefabUtility.IsPartOfPrefabInstance(currentInstanceRoot)) return;

            var parent = currentInstanceRoot.transform.parent;
            int siblingIndex = currentInstanceRoot.transform.GetSiblingIndex();

            var localPos = currentInstanceRoot.transform.localPosition;
            var localRot = currentInstanceRoot.transform.localRotation;
            // var localScale = currentInstanceRoot.transform.localScale;

            bool wasActive = currentInstanceRoot.activeSelf;
            string oldName = currentInstanceRoot.name;
            int oldLayer = currentInstanceRoot.layer;
            string oldTag = currentInstanceRoot.tag;
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
            // newRootObj.transform.localScale = localScale;

            Undo.RecordObject(newRootObj, "Set properties");
            // newRootObj.name = oldName;
            // newRootObj.tag = oldTag;
            // newRootObj.layer = oldLayer;
            GameObjectUtility.SetStaticEditorFlags(newRootObj, staticFlags);
            // newRootObj.SetActive(wasActive);

            Undo.DestroyObjectImmediate(currentInstanceRoot);

            Selection.activeObject = newRootObj;
            if (scene.IsValid())
                EditorSceneManager.MarkSceneDirty(scene);

            Undo.CollapseUndoOperations(group);
        }
    }
}
#endif
