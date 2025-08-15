using GiantSword;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(TileDrawer))]
public class TileDrawerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TileDrawer drawer = (TileDrawer)target;

        if (TileDrawingTool.disableTileDrawingTool == false)
        {
            if (GUILayout.Button("Disable Tile Drawing Tool"))
            {
                TileDrawingTool.disableTileDrawingTool.value = true;
            }

        }
        else
        {
            if (GUILayout.Button("Enable Tile Drawing Tool"))
            {
                TileDrawingTool.disableTileDrawingTool.value = false;
            }
        }
    }
}

[InitializeOnLoad]
public class TileDrawingTool
{
    static TileDrawingTool()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }
    public static Preference<bool> disableTileDrawingTool = new Preference<bool>("DisableTileDrawingTool", false);

    private static bool isDragging = false;
    private static Vector3 startDragPosition;
    private static Vector3 currentDragPosition;
    

    static void OnSceneGUI(SceneView sceneView)
    {
        if (disableTileDrawingTool)
        {
            return;
        }
        Event e = Event.current;

        if (!(Selection.activeGameObject && Selection.activeGameObject.TryGetComponent<TileDrawer>(out var drawer)))
            return;

        if (!e.command)
            return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive)); // Prevent scene selection

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane plane = new Plane(Vector3.up, drawer.transform.position);

        if (!plane.Raycast(ray, out float dist))
            return;

        Vector3 hit = ray.GetPoint(dist);
        Vector3 snapped = Snap(hit, drawer.tileSize);

        // Start dragging
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            isDragging = true;
            startDragPosition = snapped;
            currentDragPosition = snapped;
            e.Use();
        }

        // Update drag
        if (isDragging && e.type == EventType.MouseDrag && e.button == 0)
        {
            currentDragPosition = snapped;
            e.Use();
        }

        // Place tiles on drag end
        if (isDragging && e.type == EventType.MouseUp && e.button == 0)
        {
            isDragging = false;
            PlaceTiles(startDragPosition, currentDragPosition, drawer);
            e.Use();
        }

        // Draw preview
        if (isDragging)
        {
            DrawPreview(startDragPosition, currentDragPosition, drawer);
            sceneView.Repaint();
        }
        else
        {
            Handles.color = Color.green;
            Handles.DrawWireCube(snapped, Vector3.one * drawer.tileSize);
        }
    }

    private static Vector3 Snap(Vector3 pos, float size)
    {
        return new Vector3(
            Mathf.Round(pos.x / size) * size,
            Mathf.Round(pos.y / size) * size,
            Mathf.Round(pos.z / size) * size
        );
    }

    private static void PlaceTiles(Vector3 start, Vector3 end, TileDrawer drawer)
    {
        float size = drawer.tileSize;
        float minX = Mathf.Min(start.x, end.x);
        float maxX = Mathf.Max(start.x, end.x);
        float minZ = Mathf.Min(start.z, end.z);
        float maxZ = Mathf.Max(start.z, end.z);

        GameObject lastIntance = null;
        for (float x = minX; x <= maxX; x += size)
        {
            for (float z = minZ; z <= maxZ; z += size)
            {
                Vector3 pos = new Vector3(x, start.y, z);
                lastIntance = CreateTile(pos, drawer);
            }
        }

        if (lastIntance)
        {
         RuntimeEditorHelper.Select(lastIntance);   
        }
    }

    private static void DrawPreview(Vector3 start, Vector3 end, TileDrawer drawer)
    {
        float size = drawer.tileSize;
        float minX = Mathf.Min(start.x, end.x);
        float maxX = Mathf.Max(start.x, end.x);
        float minZ = Mathf.Min(start.z, end.z);
        float maxZ = Mathf.Max(start.z, end.z);

        Handles.color = Color.green;

        for (float x = minX; x <= maxX; x += size)
        {
            for (float z = minZ; z <= maxZ; z += size)
            {
                Vector3 pos = new Vector3(x, start.y, z);
                Handles.DrawWireCube(pos, Vector3.one * size);
            }
        }
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
