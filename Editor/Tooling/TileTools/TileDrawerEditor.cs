using System.Collections.Generic;
using GiantSword;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[CanEditMultipleObjects]
[CustomEditor(typeof(TileDrawer))]
public class TileDrawerEditor : Editor
{
    private GameObject selectedObject => Selection.activeGameObject;
    private Vector3 startDragPosition;   // Start position of the drag
    private Vector3 currentDragPosition; // Current mouse position during drag
    private bool isDragging = false;     // Flag to check if the user is dragging
    private TileDrawer _target;

    private enum DrawingPlane
    {
        XZ,
        XY,
        YZ
    }

    private DrawingPlane currentDrawingPlane;

    private void Awake()
    {
        _target = (TileDrawer)target;
    }

    private void OnSceneGUI()
    {
        Event e = Event.current;
        List<Object> created = null;

        if (selectedObject != null)
        {
            bool isErasing = e.shift && (e.command || e.control);

            if (e.shift && !isDragging)
            {
                startDragPosition = GetSnappedPosition();
                DrawSingleTilePreview(startDragPosition);
                SceneView.RepaintAll();
            }

            if (isDragging)
            {
                DrawTilePreview(startDragPosition, currentDragPosition);
                SceneView.RepaintAll();
            }

            if (e.shift && e.type == EventType.MouseDown && e.button == 0)
            {
                isDragging = true;
                startDragPosition = GetSnappedPosition();
                currentDragPosition = GetSnappedPosition();

                e.Use();
            }

            if (isDragging && e.type == EventType.MouseDrag && e.button == 0)
            {
                currentDragPosition = GetSnappedPosition();
                e.Use();
            }

            if (isDragging && e.type == EventType.MouseUp && e.button == 0)
            {
                isDragging = false;

                if (isErasing)
                {
                    EraseTilesInDraggedArea(startDragPosition, currentDragPosition);
                }
                else
                {
                    created = PlaceTilesInDraggedArea(startDragPosition, currentDragPosition);
                }

                e.Use();
            }
        }

        if (created != null)
        {
            Selection.objects = created.ToArray();
        }
    }

    // Determines which plane to snap to and draw tiles based on the camera's angle
    Vector3 GetSnappedPosition()
    {
        Vector3 activeTransformPosition = Selection.activeTransform.position;
        Camera sceneCamera = SceneView.lastActiveSceneView.camera;

        // Plane normals
        Vector3 normalXZ = Vector3.up;         // XZ plane
        Vector3 normalXY = Vector3.forward;    // XY plane
        Vector3 normalYZ = Vector3.right;      // YZ plane

        // Get the forward direction of the scene camera
        Vector3 cameraForward = sceneCamera.transform.forward;

        // Calculate dot products to determine which plane is most aligned with the camera
        float dotXZ = Mathf.Abs(Vector3.Dot(cameraForward, normalXZ));
        float dotXY = Mathf.Abs(Vector3.Dot(cameraForward, normalXY));
        float dotYZ = Mathf.Abs(Vector3.Dot(cameraForward, normalYZ));

        Plane plane;

        // Select the plane with the smallest dot product (most aligned)
        if (dotXZ >= dotXY && dotXZ >= dotYZ)
        {
            // XZ plane
            plane = new Plane(normalXZ, new Vector3(0, selectedObject.transform.position.y, 0));
            currentDrawingPlane = DrawingPlane.XZ;
        }
        else if (dotXY >= dotXZ && dotXY >= dotYZ)
        {
            // XY plane
            plane = new Plane(normalXY, new Vector3(0, 0, selectedObject.transform.position.z));
            currentDrawingPlane = DrawingPlane.XY;
        }
        else
        {
            // YZ plane
            plane = new Plane(normalYZ, new Vector3(selectedObject.transform.position.x, 0, 0));
            currentDrawingPlane = DrawingPlane.YZ;
        }

        // Raycast from the camera to the plane
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 snapped = hitPoint - activeTransformPosition;

            // Snap to grid
            snapped.x = Mathf.Round(snapped.x / _target.tileSize) * _target.tileSize;
            snapped.y = Mathf.Round(snapped.y / _target.tileSize) * _target.tileSize;
            snapped.z = Mathf.Round(snapped.z / _target.tileSize) * _target.tileSize;

            snapped += activeTransformPosition;

            return snapped;
        }

        return Vector3.zero;
    }

    // Draw a single wireframe preview at the starting position, considering the current drawing plane
    private void DrawSingleTilePreview(Vector3 position)
    {
        Handles.color = Color.green;
        Vector3 previewSize;

        // Adjust the size and orientation of the preview based on the selected plane
        switch (currentDrawingPlane)
        {
            case DrawingPlane.XZ:
                previewSize = new Vector3(_target.tileSize, 0.1f, _target.tileSize);
                break;
            case DrawingPlane.XY:
                previewSize = new Vector3(_target.tileSize, _target.tileSize, 0.1f);
                break;
            case DrawingPlane.YZ:
                previewSize = new Vector3(0.1f, _target.tileSize, _target.tileSize);
                break;
            default:
                previewSize = Vector3.one;
                break;
        }

        Handles.DrawWireCube(position, previewSize);
    }

    // Draw a wireframe preview for all the tiles in the dragged area, considering the current drawing plane
    private void DrawTilePreview(Vector3 start, Vector3 end)
    {
        Handles.color = Color.green;

        switch (currentDrawingPlane)
        {
            case DrawingPlane.XZ:
                DrawTilePreviewXZ(start, end);
                break;
            case DrawingPlane.XY:
                DrawTilePreviewXY(start, end);
                break;
            case DrawingPlane.YZ:
                DrawTilePreviewYZ(start, end);
                break;
        }
    }

    // Draw preview for XZ plane
    private void DrawTilePreviewXZ(Vector3 start, Vector3 end)
    {
        for (float x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x += _target.tileSize)
        {
            for (float z = Mathf.Min(start.z, end.z); z <= Mathf.Max(start.z, end.z); z += _target.tileSize)
            {
                Vector3 previewPosition = new Vector3(x, start.y, z);
                Vector3 previewSize = new Vector3(_target.tileSize, 0.1f, _target.tileSize);
                Handles.DrawWireCube(previewPosition, previewSize);
            }
        }
    }

    // Draw preview for XY plane
    private void DrawTilePreviewXY(Vector3 start, Vector3 end)
    {
        for (float x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x += _target.tileSize)
        {
            for (float y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y += _target.tileSize)
            {
                Vector3 previewPosition = new Vector3(x, y, start.z);
                Vector3 previewSize = new Vector3(_target.tileSize, _target.tileSize, 0.1f);
                Handles.DrawWireCube(previewPosition, previewSize);
            }
        }
    }

    // Draw preview for YZ plane
    private void DrawTilePreviewYZ(Vector3 start, Vector3 end)
    {
        for (float y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y += _target.tileSize)
        {
            for (float z = Mathf.Min(start.z, end.z); z <= Mathf.Max(start.z, end.z); z += _target.tileSize)
            {
                Vector3 previewPosition = new Vector3(start.x, y, z);
                Vector3 previewSize = new Vector3(0.1f, _target.tileSize, _target.tileSize);
                Handles.DrawWireCube(previewPosition, previewSize);
            }
        }
    }

    // Place tiles based on the active plane, similar to the previews
    private List<Object> PlaceTilesInDraggedArea(Vector3 start, Vector3 end)
    {
        List<Object> created = new List<Object>();

        switch (currentDrawingPlane)
        {
            case DrawingPlane.XZ:
                created = PlaceTilesInAreaXZ(start, end);
                break;
            case DrawingPlane.XY:
                created = PlaceTilesInAreaXY(start, end);
                break;
            case DrawingPlane.YZ:
                created = PlaceTilesInAreaYZ(start, end);
                break;
        }

        return created;
    }

    // Place tiles in XZ area
    private List<Object> PlaceTilesInAreaXZ(Vector3 start, Vector3 end)
    {
        List<Object> created = new List<Object>();
        for (float x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x += _target.tileSize)
        {
            for (float z = Mathf.Min(start.z, end.z); z <= Mathf.Max(start.z, end.z); z += _target.tileSize)
            {
                Vector3 tilePosition = new Vector3(x, start.y, z);
                created.Add(CreateTileAtPosition(tilePosition));
            }
        }
        return created;
    }

    // Place tiles in XY area
    private List<Object> PlaceTilesInAreaXY(Vector3 start, Vector3 end)
    {
        List<Object> created = new List<Object>();
        for (float x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x += _target.tileSize)
        {
            for (float y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y += _target.tileSize)
            {
                Vector3 tilePosition = new Vector3(x, y, start.z);
                created.Add(CreateTileAtPosition(tilePosition));
            }
        }
        return created;
    }

    // Place tiles in YZ area
    private List<Object> PlaceTilesInAreaYZ(Vector3 start, Vector3 end)
    {
        List<Object> created = new List<Object>();
        for (float y = Mathf.Min(start.y, end.y); y <= Mathf.Max(start.y, end.y); y += _target.tileSize)
        {
            for (float z = Mathf.Min(start.z, end.z); z <= Mathf.Max(start.z, end.z); z += _target.tileSize)
            {
                Vector3 tilePosition = new Vector3(start.x, y, z);
                created.Add(CreateTileAtPosition(tilePosition));
            }
        }
        return created;
    }

    // Helper function to create a tile at a given position
    private GameObject CreateTileAtPosition(Vector3 tilePosition)
    {
        GameObject duplicate = null;

        // Check if the selected object is a prefab and instantiate accordingly
        if (PrefabUtility.IsPartOfPrefabInstance(selectedObject))
        {
            // Retain prefab link by instantiating via PrefabUtility
            duplicate = (GameObject)PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(selectedObject), Selection.activeTransform.parent);
            PrefabUtility.SetPropertyModifications(duplicate, PrefabUtility.GetPropertyModifications(selectedObject));
        }
        else
        {
            // Regular duplication for non-prefab objects
            duplicate = (GameObject)Instantiate(selectedObject);
        }

        duplicate.transform.position = tilePosition;

        // Register the undo and mark the scene as dirty
        Undo.RegisterCreatedObjectUndo(duplicate, "Duplicate Object");
        EditorUtility.SetDirty(duplicate);

        return duplicate;
    }

    // Erase tiles based on the active plane
    private void EraseTilesInDraggedArea(Vector3 start, Vector3 end)
    {
        switch (currentDrawingPlane)
        {
            case DrawingPlane.XZ:
                EraseTilesInAreaXZ(start, end);
                break;
            case DrawingPlane.XY:
                EraseTilesInAreaXY(start, end);
                break;
            case DrawingPlane.YZ:
                EraseTilesInAreaYZ(start, end);
                break;
        }
    }

    // Example methods for erasing tiles on different planes (similar to placement)
    private void EraseTilesInAreaXZ(Vector3 start, Vector3 end) { /* Implement erasing logic for XZ plane */ }
    private void EraseTilesInAreaXY(Vector3 start, Vector3 end) { /* Implement erasing logic for XY plane */ }
    private void EraseTilesInAreaYZ(Vector3 start, Vector3 end) { /* Implement erasing logic for YZ plane */ }

    // The Inspector UI to assign the snap interval
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUI.changed)
        {
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Snap"))
        {
            SnapToGrid();
            
        }
    }

    private void SnapToGrid()
    {
        foreach (Object obj in targets)
        {
            if (obj is TileDrawer tileDrawer)
            {
                Undo.RecordObject(obj,"snap");
                Vector3 snapPos = tileDrawer.transform.localPosition- Vector3.one*tileDrawer.SnapOffset;
                snapPos =  snapPos.Snap(tileDrawer.tileSize)+ Vector3.one*tileDrawer.SnapOffset;;
                tileDrawer.transform.localPosition = snapPos;
            }
        }
    }
}
