
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ReplacePrefab))]
    public class ReplacePrefabEditor : CustomEditorBase<ReplacePrefab>
    {
        
       static  Preference<bool> _copyScale = new Preference<bool>("CopyScale", true, PreferenceMode.Global);
       static  Preference<bool> _deleteOriginal = new Preference<bool>("DeleteOriginal", true, PreferenceMode.Global);
        private Object _replaceWith;
        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();

            _replaceWith =  EditorGUILayout.ObjectField(_replaceWith, typeof(GameObject), false);
            _copyScale.DrawDefaultGUI();
            _deleteOriginal.DrawDefaultGUI();
            GUILayout.Space(5);
            GUI.enabled = _replaceWith != null && ValidationUtility.IsPrefabAsset((GameObject)_replaceWith);
            List<GameObject> newSelection = new List<GameObject>();

            if(GUILayout.Button("Replace Selected") )
            {
                foreach (GameObject selected in Selection.gameObjects)
                {
                    GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(_replaceWith);
                    go.transform.SetParent(selected.transform.parent);
                    go.transform.SetSiblingIndex(go.transform.GetSiblingIndex());
                    go.transform.localPosition = selected.transform.localPosition;
                    go.transform.localRotation = selected.transform.localRotation;

                    if (_copyScale.value)
                    {
                        go.transform.localScale = selected.transform.localScale;
                        
                    }
                    RuntimeEditorHelper.RegisterCreatedObjectUndo(go);
                    RuntimeEditorHelper.RecordObjectUndo(selected);

                    if (_deleteOriginal)
                    {
                        Undo.DestroyObjectImmediate(selected);
                        newSelection.Add(selected);
                    }
                    else
                    {
                        selected.gameObject.SetActive(false);
                    }
                }

                if (newSelection.Count > 0)
                {
                    RuntimeEditorHelper.Select(newSelection);
                }
            }
        }
    }
}
