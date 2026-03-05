#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [InitializeOnLoad]
    public static class TileDrawingTool
    {
        static TileDrawingTool()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public static Preference<bool> disableTileDrawingTool =
            new Preference<bool>("DisableTileDrawingTool", false);

        private static bool isDragging = false;
        // These are now in LOCAL space (relative to TileDrawer)
        private static Vector3 startDragPosition;
        private static Vector3 currentDragPosition;
        private static bool eraseMode = false;

        // For hover preview, we keep both world & local
        private static Vector3 lastSnappedWorldPosition;
        private static Vector3 lastSnappedLocalPosition;

        static void OnSceneGUI(SceneView sceneView)
        {
            if (disableTileDrawingTool.value)
                return;

            Event e = Event.current;

            // Only run tool if a TileDrawer is selected
            if (!(Selection.activeGameObject &&
                  Selection.activeGameObject.TryGetComponent<TileDrawer>(out var drawer)))
                return;

            // Require Control (⌃ on Mac, Ctrl on Windows) to activate tool
            bool controlDown = e.control; // change to (e.control || e.command) if you want Cmd too
            if (!controlDown)
                return;

            // During layout, grab default control so SceneView doesn't eat our clicks
            if (e.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                return;
            }

            // Raycast from mouse onto our drawing plane
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Plane plane = GetDrawingPlane(drawer.transform, sceneView);
            if (!plane.Raycast(ray, out float dist))
                return;

            Vector3 hitWorld = ray.GetPoint(dist);

            // --- SNAPPING IN LOCAL SPACE ---

            // Convert hit point to TileDrawer local space
            Vector3 hitLocal = drawer.transform.InverseTransformPoint(hitWorld);
            // Snap in local grid (respects rotation + offset)
            Vector3 snappedLocal = SnapLocal(hitLocal, drawer);
            // Convert snapped back to world for preview / placement
            Vector3 snappedWorld = drawer.transform.TransformPoint(snappedLocal);

            lastSnappedLocalPosition = snappedLocal;
            lastSnappedWorldPosition = snappedWorld;

            // Update erase mode based on Shift
            eraseMode = e.shift;

            // --- INPUT HANDLING ---

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                isDragging = true;
                startDragPosition = snappedLocal;   // store LOCAL
                currentDragPosition = snappedLocal; // store LOCAL
                e.Use();
                sceneView.Repaint();
            }
            else if (e.type == EventType.MouseDrag && e.button == 0 && isDragging)
            {
                currentDragPosition = snappedLocal; // LOCAL
                e.Use();
                sceneView.Repaint();
            }
            else if (e.type == EventType.MouseUp && e.button == 0 && isDragging)
            {
                isDragging = false;

                if (eraseMode)
                    EraseTiles(startDragPosition, currentDragPosition, drawer);
                else
                    PlaceTiles(startDragPosition, currentDragPosition, drawer);

                e.Use();
                sceneView.Repaint();
            }
            else if (e.type == EventType.MouseMove)
            {
                // Just moving the mouse with Ctrl held → update preview
                sceneView.Repaint();
            }

            // --- DRAWING (only on repaint) ---

            if (e.type == EventType.Repaint)
            {
                if (isDragging)
                {
                    DrawPreview(startDragPosition, currentDragPosition, drawer, eraseMode);
                }
                else
                {
                    // Hover preview
                    Handles.color = eraseMode ? Color.red : Color.green;

                    // Only use prefab bounds when auto-calc is enabled
                    if (drawer.autoCalculateBounds && TryGetPrefabBounds(drawer, out var prefabBounds))
                    {
                        Vector3 centerOffset = prefabBounds.center - drawer.transform.position;
                        Handles.DrawWireCube(lastSnappedWorldPosition + centerOffset, prefabBounds.size);
                    }
                    else
                    {
                        // Fallback to simple cube, respecting rotation + offset via local grid
                        using (new Handles.DrawingScope(
                                   Matrix4x4.TRS(drawer.transform.position, drawer.transform.rotation, Vector3.one)))
                        {
                            // lastSnappedLocalPosition already includes offset (via SnapLocal)
                            Vector3 localCenter = lastSnappedLocalPosition;
                            Handles.DrawWireCube(localCenter, drawer.tileSize);
                        }
                    }
                }
            }
        }

        private static Plane GetDrawingPlane(Transform drawer, SceneView sceneView)
        {
            Vector3 camForward = sceneView.camera.transform.forward;

            // Decide dominant axis of camera to choose plane (still axis-aligned in world)
            if (Mathf.Abs(camForward.y) > Mathf.Abs(camForward.x) &&
                Mathf.Abs(camForward.y) > Mathf.Abs(camForward.z))
                return new Plane(Vector3.up, drawer.position);      // Top view → XZ
            else if (Mathf.Abs(camForward.x) > Mathf.Abs(camForward.z))
                return new Plane(Vector3.right, drawer.position);   // Side view → YZ
            else
                return new Plane(Vector3.forward, drawer.position); // Front view → XY
        }

        /// <summary>
        /// Snap a LOCAL position to the TileDrawer grid, taking tileSize and local offset into account.
        /// </summary>
        private static Vector3 SnapLocal(Vector3 localPos, TileDrawer drawer)
        {
            Vector3 size = drawer.tileSize;

            // Interpret drawer.offset as LOCAL-space offset for the grid origin.
            Vector3 local = localPos - drawer.offset;

            local = new Vector3(
                Mathf.Round(local.x / size.x) * size.x,
                Mathf.Round(local.y / size.y) * size.y,
                Mathf.Round(local.z / size.z) * size.z
            );

            return local + drawer.offset;
        }

        /// <summary>
        /// Place tiles in the LOCAL-space selection box, then convert to world to instantiate.
        /// </summary>
        private static void PlaceTiles(Vector3 startLocal, Vector3 endLocal, TileDrawer drawer)
        {
            Vector3 size = drawer.tileSize;
            Bounds localBounds = GetBounds(startLocal, endLocal);

            List<GameObject> instances = new List<GameObject>();
            for (float x = localBounds.min.x; x <= localBounds.max.x; x += size.x)
            {
                for (float y = localBounds.min.y; y <= localBounds.max.y; y += size.y)
                {
                    for (float z = localBounds.min.z; z <= localBounds.max.z; z += size.z)
                    {
                        Vector3 localCenter = new Vector3(x, y, z) - drawer.offset; // already includes offset via snapping
                        Vector3 worldPos = drawer.transform.TransformPoint(localCenter);
                        var instance = CreateTile(worldPos, drawer);
                        instances.Add(instance);
                    }
                }
            }

            if (instances.Count > 0)
                RuntimeEditorHelper.Select(instances);
        }

        /// <summary>
        /// Erase tiles whose LOCAL positions fall within the LOCAL selection bounds.
        /// </summary>
        private static void EraseTiles(Vector3 startLocal, Vector3 endLocal, TileDrawer drawer)
        {
            Bounds localBounds = GetBounds(startLocal, endLocal);

            GameObject prefab = Selection.activeGameObject;
            var prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(prefab) ?? prefab;

            foreach (var go in CompaitibilityHelper.FindObjectsByType<GameObject>())
            {
                if (go == drawer.gameObject) continue;

                var source = PrefabUtility.GetCorrespondingObjectFromSource(go) ?? go;

                // Only erase if prefab matches
                if (source != prefabSource) continue;

                // Convert object's world position to LOCAL drawer space
                Vector3 localPos = drawer.transform.InverseTransformPoint(go.transform.position);

                if (localBounds.Contains(localPos))
                {
                    Undo.DestroyObjectImmediate(go);
                }
            }
        }

        /// <summary>
        /// Draw preview in LOCAL space, then convert to world / use a matrix for rotation.
        /// </summary>
        private static void DrawPreview(Vector3 startLocal, Vector3 endLocal, TileDrawer drawer, bool erase)
        {
            Bounds localBounds = GetBounds(startLocal, endLocal);
            Handles.color = erase ? Color.red : Color.green;

            // Only use prefab bounds when auto-calc is enabled
            if (drawer.autoCalculateBounds && TryGetPrefabBounds(drawer, out var prefabBounds))
            {
                Vector3 tileSize = drawer.tileSize;
                Vector3 centerOffset = prefabBounds.center - drawer.transform.position; // world offset
                Vector3 size = prefabBounds.size;

                for (float x = localBounds.min.x; x <= localBounds.max.x; x += tileSize.x)
                {
                    for (float y = localBounds.min.y; y <= localBounds.max.y; y += tileSize.y)
                    {
                        for (float z = localBounds.min.z; z <= localBounds.max.z; z += tileSize.z)
                        {
                            Vector3 localCenter = new Vector3(x, y, z); // LOCAL snapped center
                            Vector3 worldCenter = drawer.transform.TransformPoint(localCenter);
                            Handles.DrawWireCube(worldCenter + centerOffset, size);
                        }
                    }
                }
            }
            else
            {
                // Fallback to simple cubes using tileSize, fully respecting rotation via local grid + matrix
                Vector3 tileSize = drawer.tileSize;

                using (new Handles.DrawingScope(
                           Matrix4x4.TRS(drawer.transform.position, drawer.transform.rotation, Vector3.one)))
                {
                    for (float x = localBounds.min.x; x <= localBounds.max.x; x += tileSize.x)
                    {
                        for (float y = localBounds.min.y; y <= localBounds.max.y; y += tileSize.y)
                        {
                            for (float z = localBounds.min.z; z <= localBounds.max.z; z += tileSize.z)
                            {
                                Vector3 localCenter = new Vector3(x, y, z); // LOCAL snapped center (+offset baked in)
                                Handles.DrawWireCube(localCenter, tileSize);
                            }
                        }
                    }
                }
            }
        }

        private static Bounds GetBounds(Vector3 a, Vector3 b)
        {
            Vector3 min = Vector3.Min(a, b);
            Vector3 max = Vector3.Max(a, b);
            return new Bounds((min + max) * 0.5f, max - min);
        }

        private static GameObject CreateTile(Vector3 position, TileDrawer drawer)
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
                var source = PrefabUtility.GetCorrespondingObjectFromSource(prefab);
                instance = (GameObject)PrefabUtility.InstantiatePrefab(source, parent);
                PrefabUtility.SetPropertyModifications(instance, PrefabUtility.GetPropertyModifications(prefab));
            }
            else
            {
                instance = Object.Instantiate(prefab, parent);
            }

            instance.transform.position = position;
            Undo.RegisterCreatedObjectUndo(instance, "Draw Tile");
            EditorUtility.SetDirty(instance);
            return instance;
        }

        /// <summary>
        /// Get combined world-space bounds of all child MeshRenderers / SpriteRenderers
        /// for the given TileDrawer's GameObject.
        /// </summary>
        private static bool TryGetPrefabBounds(TileDrawer drawer, out Bounds bounds)
        {
            var root = drawer.gameObject;

            var meshRenderers = root.GetComponentsInChildren<MeshRenderer>();
            var spriteRenderers = root.GetComponentsInChildren<SpriteRenderer>();

            if (meshRenderers.Length == 0 && spriteRenderers.Length == 0)
            {
                bounds = default;
                return false;
            }

            bool initialized = false;
            bounds = new Bounds(Vector3.zero, Vector3.zero);

            foreach (var mr in meshRenderers)
            {
                if (!initialized)
                {
                    bounds = mr.bounds;
                    initialized = true;
                }
                else
                {
                    bounds.Encapsulate(mr.bounds);
                }
            }

            foreach (var sr in spriteRenderers)
            {
                if (!initialized)
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
    }
}
#endif
