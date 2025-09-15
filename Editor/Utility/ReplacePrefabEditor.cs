
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ReplacePrefab))]
    public class ReplacePrefabEditor : CustomEditorBase<ReplacePrefab>
    {
        
       static  Preference<bool> _copyScale = new Preference<bool>("ReplacePrefabEditor.CopyScale", true, PreferenceMode.Global);
        private Object _replaceWith;
        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();

            _replaceWith =  EditorGUILayout.ObjectField(_replaceWith, typeof(GameObject), false);
            _copyScale.DrawDefaultGUI();
            
            GUILayout.Space(5);
            GUI.enabled = _replaceWith != null && ValidationUtility.IsPrefabAsset((GameObject)_replaceWith);

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
                    selected.gameObject.SetActive(false);
                }
            }
        }
    }
}
