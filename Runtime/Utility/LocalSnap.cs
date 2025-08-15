using GiantSword;
using NaughtyAttributes;
using UnityEngine;


[ExecuteInEditMode]
public class LocalSnap : MonoBehaviour
{
    [SerializeField] private Vector3 _interval = Vector3.one;
    
    void Update()
    {
        if (Application.isPlaying == false)
        {
            Apply();
        }
    }

    [Button]
    private void Apply()
    {
        Vector3 snap = transform.localPosition.Snap(_interval);
        if (transform.localPosition != snap)
        {
            RuntimeEditorHelper.RecordObjectUndo(transform);
            transform.localPosition = snap;
        }
    }
}
