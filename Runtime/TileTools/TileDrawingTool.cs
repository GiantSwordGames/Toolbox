#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JamKit
{
    /// <summary>
    /// Scene view tile drawing tool.
    ///
    /// Key behaviors:
    /// - Hold Control and drag with left mouse to draw tiles.
    /// - Hold Control + Shift and drag to erase tiles.
    /// - Press , or . to rotate the TileDrawer by the configured step.
    /// - Hold Shift and scroll to cycle selected prefab instances.
    ///
    /// IMPORTANT TRANSFORM RULE:
    /// This tool intentionally respects the TileDrawer's POSITION and ROTATION,
    /// but ignores its SCALE for all draw/grid calculations.
    ///
    /// This is critical. Future edits should preserve that rule unless the
    /// tool is being deliberately redesigned.
    /// </summary>
    [InitializeOnLoad]
    public static class TileDrawingTool
    {
        #region Preferences

        public static Preference<bool> enableTileDrawing =
            new Preference<bool>("enableTileDrawing", true);

        public static Preference<float> rotationStepDegrees =
            new Preference<float>("rotationStepDegrees", 90f);

        public static Preference<bool> enablePrefabCycling =
            new Preference<bool>("enablePrefabCycling", true);

        #endregion

        #region Drag State

        /// <summary>
        /// True while the user is actively dragging in the scene view.
        /// </summary>
        private static bool isDragging = false;

        /// <summary>
        /// Drag start in drawer-local space, ignoring drawer scale.
        /// </summary>
        private static Vector3 startDragPosition;

        /// <summary>
        /// Current drag position in drawer-local space, ignoring drawer scale.
        /// </summary>
        private static Vector3 currentDragPosition;

        /// <summary>
        /// True when the current interaction is in erase mode.
        /// </summary>
        private static bool eraseMode = false;

        #endregion

        #region Hover Preview State

        /// <summary>
        /// Last snapped world position used for preview drawing.
        /// </summary>
        private static Vector3 lastSnappedWorldPosition;

        /// <summary>
        /// Last snapped local position used for preview drawing.
        /// This is local relative to the TileDrawer, but scale is ignored.
        /// </summary>
        private static Vector3 lastSnappedLocalPosition;

        #endregion

        #region Misc State

        /// <summary>
        /// Small debounce for prefab cycling scroll input.
        /// </summary>
        private static double _lastScrollTime;

        #endregion

        #region Initialization

        static TileDrawingTool()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        #endregion

        #region Scene GUI Entry Point

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (enableTileDrawing.value == false)
                return;

            Event e = Event.current;
            if (e == null)
                return;

            // Prefab cycling works independently of TileDrawer selection.
            HandlePrefabCycling(e);

            if (TryGetSelectedDrawer(out TileDrawer drawer) == false)
                return;

            HandleRotationHotkeys(e, drawer);

            // Tile drawing only activates while Control is held.
            // Mac note: this intentionally uses Event.control.
            if (e.control == false)
                return;

            // Claim scene view control during layout so Unity does not consume our clicks.
            if (e.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                return;
            }

            if (TryGetSnappedMousePosition(sceneView, drawer, e.mousePosition, out Vector3 snappedLocal, out Vector3 snappedWorld) == false)
                return;

            lastSnappedLocalPosition = snappedLocal;
            lastSnappedWorldPosition = snappedWorld;

            eraseMode = e.shift;

            HandleMouseInteraction(e, sceneView, drawer, snappedLocal);

            if (e.type == EventType.Repaint)
                DrawScenePreview(drawer);
        }

        #endregion

        #region Selection / Context

        /// <summary>
        /// Returns the currently selected TileDrawer, if any.
        /// </summary>
        private static bool TryGetSelectedDrawer(out TileDrawer drawer)
        {
            drawer = null;

            if (Selection.activeGameObject == null)
                return false;

            return Selection.activeGameObject.TryGetComponent(out drawer);
        }

        #endregion

        #region Mouse Position / Snapping

        /// <summary>
        /// Converts the current mouse position into snapped local/world positions for the drawer.
        ///
        /// IMPORTANT:
        /// - The drawer's rotation is respected.
        /// - The drawer's scale is ignored.
        ///
        /// Future AI edits should not replace this with TransformPoint / InverseTransformPoint
        /// unless scale support is intentionally desired.
        /// </summary>
        private static bool TryGetSnappedMousePosition(
            SceneView sceneView,
            TileDrawer drawer,
            Vector2 mousePosition,
            out Vector3 snappedLocal,
            out Vector3 snappedWorld)
        {
            snappedLocal = default;
            snappedWorld = default;

            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            Plane plane = GetDrawingPlane(drawer.transform, sceneView);

            if (plane.Raycast(ray, out float distance) == false)
                return false;

            Vector3 hitWorld = ray.GetPoint(distance);

            Vector3 hitLocal = WorldToDrawerLocalNoScale(drawer.transform, hitWorld);
            snappedLocal = SnapLocal(hitLocal, drawer);
            snappedWorld = DrawerLocalToWorldNoScale(drawer.transform, snappedLocal);

            return true;
        }

        /// <summary>
        /// Snaps a local position to the drawer's grid.
        ///
        /// Grid space is defined by:
        /// - tileSize
        /// - offset
        ///
        /// This method assumes the input local position is already in a space
        /// where drawer scale has been removed.
        /// </summary>
        private static Vector3 SnapLocal(Vector3 localPos, TileDrawer drawer)
        {
            Vector3 size = drawer.tileSize;
            Vector3 local = localPos - drawer.offset;

            local = new Vector3(
                size.x != 0f ? Mathf.Round(local.x / size.x) * size.x : local.x,
                size.y != 0f ? Mathf.Round(local.y / size.y) * size.y : local.y,
                size.z != 0f ? Mathf.Round(local.z / size.z) * size.z : local.z
            );

            return local + drawer.offset;
        }

        #endregion

        #region Mouse Interaction

        private static void HandleMouseInteraction(Event e, SceneView sceneView, TileDrawer drawer, Vector3 snappedLocal)
        {
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                BeginDrag(snappedLocal, e, sceneView);
                return;
            }

            if (e.type == EventType.MouseDrag && e.button == 0 && isDragging)
            {
                UpdateDrag(snappedLocal, e, sceneView);
                return;
            }

            if (e.type == EventType.MouseUp && e.button == 0 && isDragging)
            {
                EndDrag(drawer, e, sceneView);
                return;
            }

            if (e.type == EventType.MouseMove)
            {
                sceneView.Repaint();
            }
        }

        private static void BeginDrag(Vector3 snappedLocal, Event e, SceneView sceneView)
        {
            isDragging = true;
            startDragPosition = snappedLocal;
            currentDragPosition = snappedLocal;

            e.Use();
            sceneView.Repaint();
        }

        private static void UpdateDrag(Vector3 snappedLocal, Event e, SceneView sceneView)
        {
            currentDragPosition = snappedLocal;

            e.Use();
            sceneView.Repaint();
        }

        private static void EndDrag(TileDrawer drawer, Event e, SceneView sceneView)
        {
            isDragging = false;

            if (eraseMode)
                EraseTiles(startDragPosition, currentDragPosition, drawer);
            else
                PlaceTiles(startDragPosition, currentDragPosition, drawer);

            e.Use();
            sceneView.Repaint();
        }

        #endregion

        #region Scene Preview Drawing

        private static void DrawScenePreview(TileDrawer drawer)
        {
            if (isDragging)
            {
                DrawPreviewVolume(startDragPosition, currentDragPosition, drawer, eraseMode);
                return;
            }

            DrawHoverPreview(drawer, eraseMode);
        }

        /// <summary>
        /// Draws the single-tile hover preview at the current mouse position.
        /// </summary>
        private static void DrawHoverPreview(TileDrawer drawer, bool erase)
        {
            Handles.color = erase ? Color.red : Color.green;

            if (drawer.autoCalculateBounds && TryGetPrefabBounds(drawer, out Bounds prefabBounds))
            {
                Vector3 centerOffsetWorld = prefabBounds.center - drawer.transform.position;
                Handles.DrawWireCube(lastSnappedWorldPosition + centerOffsetWorld, prefabBounds.size);
            }
            else
            {
                // Explicitly ignore scale in the preview transform matrix.
                using (new Handles.DrawingScope(
                           Matrix4x4.TRS(drawer.transform.position, drawer.transform.rotation, Vector3.one)))
                {
                    Handles.DrawWireCube(lastSnappedLocalPosition, drawer.tileSize);
                }
            }
        }

        /// <summary>
        /// Draws the drag selection preview.
        /// </summary>
        private static void DrawPreviewVolume(Vector3 startLocal, Vector3 endLocal, TileDrawer drawer, bool erase)
        {
            Bounds localBounds = GetBounds(startLocal, endLocal);
            Vector3 tileSize = drawer.tileSize;

            Handles.color = erase ? Color.red : Color.green;

            if (drawer.autoCalculateBounds && TryGetPrefabBounds(drawer, out Bounds prefabBounds))
            {
                Vector3 centerOffsetWorld = prefabBounds.center - drawer.transform.position;
                Vector3 size = prefabBounds.size;

                for (float x = localBounds.min.x; x <= localBounds.max.x; x += tileSize.x)
                {
                    for (float y = localBounds.min.y; y <= localBounds.max.y; y += tileSize.y)
                    {
                        for (float z = localBounds.min.z; z <= localBounds.max.z; z += tileSize.z)
                        {
                            Vector3 localCenter = new Vector3(x, y, z);
                            Vector3 worldCenter = DrawerLocalToWorldNoScale(drawer.transform, localCenter);
                            Handles.DrawWireCube(worldCenter + centerOffsetWorld, size);
                        }
                    }
                }
            }
            else
            {
                // Explicitly ignore scale in the preview transform matrix.
                using (new Handles.DrawingScope(
                           Matrix4x4.TRS(drawer.transform.position, drawer.transform.rotation, Vector3.one)))
                {
                    for (float x = localBounds.min.x; x <= localBounds.max.x; x += tileSize.x)
                    {
                        for (float y = localBounds.min.y; y <= localBounds.max.y; y += tileSize.y)
                        {
                            for (float z = localBounds.min.z; z <= localBounds.max.z; z += tileSize.z)
                            {
                                Vector3 localCenter = new Vector3(x, y, z);
                                Handles.DrawWireCube(localCenter, tileSize);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Tile Placement / Erasing

        /// <summary>
        /// Places tiles across the snapped local bounds.
        ///
        /// IMPORTANT:
        /// startLocal and endLocal are already snapped positions in drawer-local
        /// space and already include offset handling.
        ///
        /// Do NOT subtract drawer.offset again here.
        /// That was a previous bug source.
        /// </summary>
        private static void PlaceTiles(Vector3 startLocal, Vector3 endLocal, TileDrawer drawer)
        {
            Bounds localBounds = GetBounds(startLocal, endLocal);
            Vector3 tileSize = drawer.tileSize;

            List<GameObject> instances = new List<GameObject>();

            for (float x = localBounds.min.x; x <= localBounds.max.x; x += tileSize.x)
            {
                for (float y = localBounds.min.y; y <= localBounds.max.y; y += tileSize.y)
                {
                    for (float z = localBounds.min.z; z <= localBounds.max.z; z += tileSize.z)
                    {
                        Vector3 localCenter = new Vector3(x, y, z);
                        Vector3 worldPosition = DrawerLocalToWorldNoScale(drawer.transform, localCenter);

                        GameObject instance = CreateTile(worldPosition, drawer);
                        instances.Add(instance);
                    }
                }
            }

            if (instances.Count > 0)
                RuntimeEditorHelper.Select(instances);
        }

        /// <summary>
        /// Erases matching tiles within the local bounds.
        /// </summary>
        private static void EraseTiles(Vector3 startLocal, Vector3 endLocal, TileDrawer drawer)
        {
            Bounds localBounds = GetBounds(startLocal, endLocal);

            GameObject prefab = Selection.activeGameObject;
            GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(prefab) ?? prefab;

            foreach (GameObject go in CompatibilityHelper.FindObjectsByType<GameObject>())
            {
                if (go == drawer.gameObject)
                    continue;

                GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(go) ?? go;

                if (source != prefabSource)
                    continue;

                Vector3 localPosition = WorldToDrawerLocalNoScale(drawer.transform, go.transform.position);

                if (localBounds.Contains(localPosition))
                    Undo.DestroyObjectImmediate(go);
            }
        }

        #endregion

        #region Tile Creation

        /// <summary>
        /// Creates a new tile instance based on the current selection.
        ///
        /// Supports:
        /// - prefab assets
        /// - prefab instances (preserving overrides)
        /// - plain scene objects
        /// </summary>
        private static GameObject CreateTile(Vector3 worldPosition, TileDrawer drawer)
        {
            GameObject prefab = Selection.activeGameObject;
            Transform parent = prefab.transform.parent;

            GameObject instance;

            if (PrefabUtility.IsPartOfPrefabAsset(prefab))
            {
                instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            }
            else if (PrefabUtility.IsPartOfPrefabInstance(prefab))
            {
                GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(prefab);
                instance = (GameObject)PrefabUtility.InstantiatePrefab(source, parent);
                PrefabUtility.SetPropertyModifications(instance, PrefabUtility.GetPropertyModifications(prefab));
            }
            else
            {
                instance = Object.Instantiate(prefab, parent);
            }

            instance.transform.position = worldPosition;

            Undo.RegisterCreatedObjectUndo(instance, "Draw Tile");
            EditorUtility.SetDirty(instance);

            return instance;
        }

        #endregion

        #region Rotation Hotkeys

        private static void HandleRotationHotkeys(Event e, TileDrawer drawer)
        {
            if (e.type != EventType.KeyDown)
                return;

            int direction = 0;

            if (e.keyCode == KeyCode.Comma)
                direction = -1;
            else if (e.keyCode == KeyCode.Period)
                direction = 1;

            if (direction == 0)
                return;

            ApplyRotation(drawer.transform, direction, rotationStepDegrees.value);

            e.Use();
            SceneView.RepaintAll();
        }

        private static void ApplyRotation(Transform target, int direction, float stepDegrees)
        {
            if (target == null || direction == 0)
                return;

            Undo.SetCurrentGroupName("Rotate Tile Drawer Y By Step");
            int group = Undo.GetCurrentGroup();

            Undo.RecordObject(target, "Rotate Tile Drawer Y By Step");

            Vector3 euler = target.eulerAngles;
            euler.y = NormalizeAngle(euler.y + direction * stepDegrees);
            target.eulerAngles = euler;

            EditorUtility.SetDirty(target);
            Undo.CollapseUndoOperations(group);
        }

        private static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle < 0f)
                angle += 360f;
            return angle;
        }

        #endregion

        #region Prefab Cycling

        private static void HandlePrefabCycling(Event e)
        {
            if (enablePrefabCycling.value == false)
                return;

            if (e.type != EventType.ScrollWheel || e.shift == false)
                return;

            float axis = Mathf.Abs(e.delta.y) > 0.0001f ? e.delta.y : e.delta.x;
            if (Mathf.Abs(axis) < 0.0001f)
                return;

            if (EditorApplication.timeSinceStartup - _lastScrollTime < 0.1)
            {
                e.Use();
                return;
            }

            _lastScrollTime = EditorApplication.timeSinceStartup;

            GameObject[] selected = Selection.gameObjects;
            if (selected == null || selected.Length == 0)
                return;

            GameObject firstValidRoot = GetFirstSelectedPrefabRoot(selected);
            if (firstValidRoot == null)
                return;

            string currentAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(firstValidRoot);
            if (string.IsNullOrEmpty(currentAssetPath))
                return;

            string folder = NormalizeToUnityPath(Path.GetDirectoryName(currentAssetPath));
            List<PrefabEntry> candidates = GetCycleCandidatesInFolder(folder);
            if (candidates.Count == 0)
                return;

            int delta = axis > 0 ? -1 : 1;

            Undo.SetCurrentGroupName("Cycle Prefabs");
            int group = Undo.GetCurrentGroup();

            List<GameObject> newSelections = new List<GameObject>();

            foreach (GameObject go in selected)
            {
                if (go == null)
                    continue;

                GameObject root = PrefabUtility.GetNearestPrefabInstanceRoot(go);
                if (root == null)
                    continue;

                string rootAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(root);
                int currentIndex = candidates.FindIndex(c => c.assetPath == rootAssetPath);
                if (currentIndex < 0)
                    continue;

                GameObject newRoot = CyclePrefab(root, candidates, currentIndex, delta);
                if (newRoot != null)
                    newSelections.Add(newRoot);
            }

            if (newSelections.Count > 0)
                Selection.objects = newSelections.ToArray();

            Undo.CollapseUndoOperations(group);

            e.Use();
            SceneView.RepaintAll();
        }

        private static GameObject GetFirstSelectedPrefabRoot(GameObject[] selected)
        {
            foreach (GameObject go in selected)
            {
                if (go == null)
                    continue;

                GameObject root = PrefabUtility.GetNearestPrefabInstanceRoot(go);
                if (root != null)
                    return root;
            }

            return null;
        }

        private static GameObject CyclePrefab(GameObject instanceRoot, List<PrefabEntry> candidates, int currentIndex, int delta)
        {
            if (candidates.Count == 0)
                return null;

            int nextIndex = WrapIndex(currentIndex + delta, candidates.Count);
            GameObject nextPrefab = candidates[nextIndex].asset;

            return ReplaceInstance(instanceRoot, nextPrefab);
        }

        private static GameObject ReplaceInstance(GameObject currentInstanceRoot, GameObject nextPrefabAsset)
        {
            if (currentInstanceRoot == null || nextPrefabAsset == null || currentInstanceRoot.activeSelf == false)
                return null;

            if (PrefabUtility.IsPartOfPrefabInstance(currentInstanceRoot) == false)
                return null;

            Transform parent = currentInstanceRoot.transform.parent;
            int siblingIndex = currentInstanceRoot.transform.GetSiblingIndex();

            Vector3 localPosition = currentInstanceRoot.transform.localPosition;
            Quaternion localRotation = currentInstanceRoot.transform.localRotation;
            Vector3 localScale = currentInstanceRoot.transform.localScale;

            var staticFlags = GameObjectUtility.GetStaticEditorFlags(currentInstanceRoot);
            var scene = currentInstanceRoot.scene;

            GameObject newRootObject = PrefabUtility.InstantiatePrefab(nextPrefabAsset, scene) as GameObject;
            if (newRootObject == null)
                return null;

            Undo.RegisterCreatedObjectUndo(newRootObject, "Create replacement prefab");

            Undo.SetTransformParent(newRootObject.transform, parent, "Set parent");
            Undo.RecordObject(newRootObject.transform, "Set transform");

            newRootObject.transform.SetSiblingIndex(siblingIndex);
            newRootObject.transform.localPosition = localPosition;
            newRootObject.transform.localRotation = localRotation;
            newRootObject.transform.localScale = localScale;

            Undo.RecordObject(newRootObject, "Set properties");
            GameObjectUtility.SetStaticEditorFlags(newRootObject, staticFlags);

            Undo.DestroyObjectImmediate(currentInstanceRoot);

            if (scene.IsValid())
                EditorSceneManager.MarkSceneDirty(scene);

            return newRootObject;
        }

        private struct PrefabEntry
        {
            public string assetPath;
            public GameObject asset;
        }

        private static List<PrefabEntry> GetCycleCandidatesInFolder(string folderPath)
        {
            List<PrefabEntry> list = new List<PrefabEntry>();

            if (string.IsNullOrEmpty(folderPath))
                return list;

            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (go == null)
                    continue;

                // Preserve original behavior:
                // only include prefabs that also contain a TileDrawer.
                if (go.GetComponent<TileDrawer>() != null)
                {
                    list.Add(new PrefabEntry
                    {
                        assetPath = path,
                        asset = go
                    });
                }
            }

            list.Sort((a, b) =>
            {
#if UNITY_2021_2_OR_NEWER
                return EditorUtility.NaturalCompare(
                    Path.GetFileNameWithoutExtension(a.assetPath),
                    Path.GetFileNameWithoutExtension(b.assetPath));
#else
                return string.Compare(
                    Path.GetFileNameWithoutExtension(a.assetPath),
                    Path.GetFileNameWithoutExtension(b.assetPath),
                    System.StringComparison.OrdinalIgnoreCase);
#endif
            });

            return list;
        }

        private static int WrapIndex(int index, int count)
        {
            if (count <= 0)
                return 0;

            index %= count;
            if (index < 0)
                index += count;

            return index;
        }

        #endregion

        #region Bounds / Plane Helpers

        private static Bounds GetBounds(Vector3 a, Vector3 b)
        {
            Vector3 min = Vector3.Min(a, b);
            Vector3 max = Vector3.Max(a, b);
            return new Bounds((min + max) * 0.5f, max - min);
        }

        /// <summary>
        /// Chooses a drawing plane based on the scene camera direction.
        ///
        /// Current behavior:
        /// - top-ish camera => XZ plane
        /// - side-ish camera => YZ plane
        /// - front-ish camera => XY plane
        ///
        /// This currently uses world axes, not drawer-local rotated plane normals.
        /// That preserves the existing behavior.
        /// </summary>
        private static Plane GetDrawingPlane(Transform drawer, SceneView sceneView)
        {
            Vector3 camForward = sceneView.camera.transform.forward;

            if (Mathf.Abs(camForward.y) > Mathf.Abs(camForward.x) &&
                Mathf.Abs(camForward.y) > Mathf.Abs(camForward.z))
            {
                return new Plane(Vector3.up, drawer.position);
            }

            if (Mathf.Abs(camForward.x) > Mathf.Abs(camForward.z))
            {
                return new Plane(Vector3.right, drawer.position);
            }

            return new Plane(Vector3.forward, drawer.position);
        }

        #endregion

        #region Bounds Auto Calculation

        /// <summary>
        /// Attempts to compute world-space bounds from renderers under the TileDrawer object.
        ///
        /// Note:
        /// These are renderer bounds in world space.
        /// </summary>
        public static bool TryGetPrefabBounds(TileDrawer drawer, out Bounds bounds)
        {
            GameObject root = drawer.gameObject;

            MeshRenderer[] meshRenderers = root.GetComponentsInChildren<MeshRenderer>();
            SpriteRenderer[] spriteRenderers = root.GetComponentsInChildren<SpriteRenderer>();

            if (meshRenderers.Length == 0 && spriteRenderers.Length == 0)
            {
                bounds = default;
                return false;
            }

            bool initialized = false;
            bounds = new Bounds(Vector3.zero, Vector3.zero);

            foreach (MeshRenderer mr in meshRenderers)
            {
                if (initialized == false)
                {
                    bounds = mr.bounds;
                    initialized = true;
                }
                else
                {
                    bounds.Encapsulate(mr.bounds);
                }
            }

            foreach (SpriteRenderer sr in spriteRenderers)
            {
                if (initialized == false)
                {
                    bounds = sr.bounds;
                    initialized = true;
                }
                else
                {
                    bounds.Encapsulate(sr.bounds);
                }
            }

            return initialized;
        }

        #endregion

        #region No-Scale Transform Helpers

        /// <summary>
        /// Converts world position to drawer-local position while ignoring drawer scale.
        ///
        /// This is one of the most important methods in the file.
        /// Do not replace with InverseTransformPoint unless scale should affect grid logic.
        /// </summary>
        private static Vector3 WorldToDrawerLocalNoScale(Transform drawerTransform, Vector3 worldPosition)
        {
            return Quaternion.Inverse(drawerTransform.rotation) * (worldPosition - drawerTransform.position);
        }

        /// <summary>
        /// Converts drawer-local position to world position while ignoring drawer scale.
        ///
        /// This is one of the most important methods in the file.
        /// Do not replace with TransformPoint unless scale should affect grid logic.
        /// </summary>
        private static Vector3 DrawerLocalToWorldNoScale(Transform drawerTransform, Vector3 localPosition)
        {
            return drawerTransform.position + (drawerTransform.rotation * localPosition);
        }

        #endregion

        #region Path Helpers

        private static string NormalizeToUnityPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

            return path.Replace('\\', '/');
        }

        #endregion
    }
}
#endif