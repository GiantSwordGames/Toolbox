using System.Collections;
using System.Collections.Generic;
using GiantSword;
using NaughtyAttributes;
using UnityEngine;


[RequireComponent(typeof(TileDrawer))]
[RequireComponent(typeof(PrefabCycler))]
public class LevelChunk : MonoBehaviour
{
    [Button]
    private void ShiftPivotYToZero()
    {
        Transform parent = transform;
        Transform[] children = parent.GetDirectChildren<Transform>(true).ToArray();
        children = System.Array.FindAll(children, t => t != parent);

        if (children.Length == 0)
        {
            Debug.LogWarning("No children found to center on.");
            return;
        }


        RuntimeEditorHelper.RecordObjectUndo(parent);

        Vector3 originalPosition = parent.position;

        parent.position = parent.position.WithY(0);

        Vector3 delta = parent.position - originalPosition;
        foreach (Transform child in children)
        {
            RuntimeEditorHelper.RecordObjectUndo(child);
            child.position -= delta;
        }
    }

    [Button]
    private void SnapYToZero()
    {
        transform.position = transform.position.WithY(0);
    }

}
