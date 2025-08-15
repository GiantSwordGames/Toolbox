
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(SingleTag))]
    public class SingleTagEditor : CustomEditorBase<SingleTag>
    {
        public override void OnInspectorGUI()
        {
            SerializedProperty serializedProperty = this.serializedObject.FindProperty("_tag");
            EditorGUILayout.PropertyField(serializedProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
