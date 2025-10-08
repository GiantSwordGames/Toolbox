using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JamKit
{
    public class TileDrawer : MonoBehaviour
    {
        [SerializeField] private Vector3 _tileSize = Vector3.one;
        public Vector3 TileSize => _tileSize;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TileDrawer))]
    public class TileDrawerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TileDrawer drawer = (TileDrawer)target;

            if (GUILayout.Button(TileDrawingTool.disableTileDrawingTool.value 
                ? "Enable Tile Drawing Tool" 
                : "Disable Tile Drawing Tool"))
            {
                TileDrawingTool.disableTileDrawingTool.value = 
                    !TileDrawingTool.disableTileDrawingTool.value;
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Calculate Tile Size From Prefab"))
            {
                CalculateTileSize(drawer);
            }
        }

        private void CalculateTileSize(TileDrawer drawer)
        {
            GameObject prefab = drawer.gameObject;

            // Collect renderers
            var meshRenderers = prefab.GetComponentsInChildren<MeshRenderer>();
            var spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>();

            if (meshRenderers.Length == 0 && spriteRenderers.Length == 0)
            {
                Debug.LogWarning("No MeshRenderer or SpriteRenderer found in prefab.");
                return;
            }

            Bounds bounds = new Bounds(prefab.transform.position, Vector3.zero);

            foreach (var mr in meshRenderers)
                bounds.Encapsulate(mr.bounds);

            foreach (var sr in spriteRenderers)
                bounds.Encapsulate(sr.bounds);

            // Round to 2 decimal places
            Vector3 size = bounds.size;
            Vector3 rounded = new Vector3(
                Mathf.Round(size.x * 100f) / 100f,
                Mathf.Round(size.y * 100f) / 100f,
                Mathf.Round(size.z * 100f) / 100f
            );

            Undo.RecordObject(drawer, "Auto-calculate Tile Size");

            drawer.GetType()
                .GetField("_tileSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(drawer, rounded);

            EditorUtility.SetDirty(drawer);

            Debug.Log($"[TileDrawer] Tile size calculated and rounded to {rounded}");
        }
    }

    [InitializeOnLoad]
    public static class TileDrawingTool
    {
        static TileDrawingTool()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public static Preference<bool> disableTileDrawingTool = new Preference<bool>("DisableTileDrawingTool", false);

        private static bool isDragging = false;
        private static Vector3 startDragPosition;
        private static Vector3 currentDragPosition;
        private static bool eraseMode = false;

        static void OnSceneGUI(SceneView sceneView)
        {
            if (disableTileDrawingTool)
                return;

            Event e = Event.current;
            if (!(Selection.activeGameObject && Selection.activeGameObject.TryGetComponent<TileDrawer>(out var drawer)))
                return;

            // Require Control (⌃ on Mac, Ctrl on Windows) to activate tool
            if (!e.control)
                return;

// Erase if Shift is ALSO held
            eraseMode = e.shift;

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Plane plane = GetDrawingPlane(drawer.transform, sceneView);

            if (!plane.Raycast(ray, out float dist))
                return;

            Vector3 hit = ray.GetPoint(dist);
            Vector3 snapped = Snap(hit, drawer);

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                isDragging = true;
                startDragPosition = snapped;
                currentDragPosition = snapped;
                e.Use();
            }
            if (isDragging && e.type == EventType.MouseDrag && e.button == 0)
            {
                currentDragPosition = snapped;
                e.Use();
            }
            if (isDragging && e.type == EventType.MouseUp && e.button == 0)
            {
                isDragging = false;
                if (eraseMode)
                    EraseTiles(startDragPosition, currentDragPosition, drawer);
                else
                    PlaceTiles(startDragPosition, currentDragPosition, drawer);
                e.Use();
            }

            if (isDragging)
            {
                DrawPreview(startDragPosition, currentDragPosition, drawer, eraseMode);
                sceneView.Repaint();
            }
            else
            {
                Handles.color = eraseMode ? Color.red : Color.green;
                Handles.DrawWireCube(snapped, drawer.TileSize);
            }
        }

        private static Plane GetDrawingPlane(Transform drawer, SceneView sceneView)
        {
            Vector3 camForward = sceneView.camera.transform.forward;
            camForward.y = Mathf.Abs(camForward.y);

            if (Mathf.Abs(camForward.y) > Mathf.Abs(camForward.x) && Mathf.Abs(camForward.y) > Mathf.Abs(camForward.z))
                return new Plane(Vector3.up, drawer.position); // Top view → XZ
            else if (Mathf.Abs(camForward.x) > Mathf.Abs(camForward.z))
                return new Plane(Vector3.right, drawer.position); // Side view → YZ
            else
                return new Plane(Vector3.forward, drawer.position); // Front view → XY
        }

        private static Vector3 Snap(Vector3 pos, TileDrawer drawer)
        {
            Vector3 origin = drawer.transform.position;
            Vector3 size = drawer.TileSize;

            return new Vector3(
                Mathf.Round((pos.x - origin.x) / size.x) * size.x + origin.x,
                Mathf.Round((pos.y - origin.y) / size.y) * size.y + origin.y,
                Mathf.Round((pos.z - origin.z) / size.z) * size.z + origin.z
            );
        }

        private static void PlaceTiles(Vector3 start, Vector3 end, TileDrawer drawer)
        {
            Vector3 size = drawer.TileSize;
            Bounds bounds = GetBounds(start, end);

            List<GameObject> instances = new List<GameObject>();
            for (float x = bounds.min.x; x <= bounds.max.x; x += size.x)
            {
                for (float y = bounds.min.y; y <= bounds.max.y; y += size.y)
                {
                    for (float z = bounds.min.z; z <= bounds.max.z; z += size.z)
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        var instance = CreateTile(pos, drawer);
                        instances.Add(instance);
                    }
                }
            }

            if (instances.Count > 0)
                RuntimeEditorHelper.Select(instances);
        }

        private static void EraseTiles(Vector3 start, Vector3 end, TileDrawer drawer)
        {
            Bounds bounds = GetBounds(start, end);

            GameObject prefab = Selection.activeGameObject;
            var prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(prefab) ?? prefab;

            foreach (var go in Object.FindObjectsOfType<GameObject>())
            {
                if (go == drawer.gameObject) continue;

                var source = PrefabUtility.GetCorrespondingObjectFromSource(go) ?? go;

                // Only erase if prefab matches
                if (source != prefabSource) continue;

                if (bounds.Contains(go.transform.position))
                {
                    Undo.DestroyObjectImmediate(go);
                }
            }
        }

        private static void DrawPreview(Vector3 start, Vector3 end, TileDrawer drawer, bool erase)
        {
            Vector3 size = drawer.TileSize;
            Bounds bounds = GetBounds(start, end);

            Handles.color = erase ? Color.red : Color.green;

            for (float x = bounds.min.x; x <= bounds.max.x; x += size.x)
            {
                for (float y = bounds.min.y; y <= bounds.max.y; y += size.y)
                {
                    for (float z = bounds.min.z; z <= bounds.max.z; z += size.z)
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        Handles.DrawWireCube(pos, size);
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
            if (PrefabUtility.IsPartOfPrefabInstance(prefab))
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
    }
#endif
}
