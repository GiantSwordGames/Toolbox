using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class HierarchyCutPaste
{
    // Store the list of GameObjects that have been cut.
    private static List<GameObject> cutGameObjects = new List<GameObject>();

    [MenuItem("GameObject/Cut GameObject", false, -10)]
    private static void CutGameObject()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("No GameObject selected to cut.");
            return;
        }
        
        // Clear any previously cut objects and store the current selection.
        cutGameObjects.Clear();
        cutGameObjects.AddRange(Selection.gameObjects);
        Debug.Log("Cut " + cutGameObjects.Count + " GameObject(s).");
    }

    [MenuItem("GameObject/Paste GameObject", false, -10)]
    private static void PasteGameObject()
    {
        if (cutGameObjects.Count == 0)
        {
            Debug.LogWarning("No GameObject has been cut.");
            return;
        }
        
        // Determine the target parent:
        // If a GameObject is selected, use it as the new parent; otherwise, place at the root of the scene.
        Transform targetParent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;

        // Reparent each cut GameObject.
        foreach (GameObject go in cutGameObjects)
        {
            // Record the current state for undo functionality.
            Undo.SetTransformParent(go.transform, targetParent, "Paste GameObject");
        }
        Debug.Log("Pasted " + cutGameObjects.Count + " GameObject(s).");

        // Clear the cut list once pasted.
        cutGameObjects.Clear();
    }
}
