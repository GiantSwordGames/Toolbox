using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(ScriptableBool))]

    public class ScriptableBoolEditor : CustomEditorBase<ScriptableBool>
    {
        public override void OnInspectorGUI()
        {
            var scopeProperty = serializedObject.FindProperty("_scriptableVariableScope");
            var initialValueProperty = serializedObject.FindProperty("_initialValue");
            
            EditorGUILayout.PropertyField(scopeProperty);
            targetObject.initialValue = EditorGUILayout.Toggle("Initial Value", targetObject.initialValue);

            GUI.enabled = Application.isPlaying;
            targetObject.value = EditorGUILayout.Toggle("Current Value", targetObject.value);
            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();
    
            if (GUILayout.Button("Toggle"))
            {
                targetObject.value = !targetObject.value;    
            }
            if (GUILayout.Button("Toggle True"))
            {
                targetObject.value = true;    
            }
            if (GUILayout.Button("Toggle False"))
            {
                targetObject.value = false;    
            }

            if (GUILayout.Button("Show All Bools"))
            {
                ScriptableBoolViewer.ShowWindow();
            }
            

            // if (GUILayout.Button("Ping"))
            // {
            //     // targetObject.Ping();
            // }
        }
        
    }
}