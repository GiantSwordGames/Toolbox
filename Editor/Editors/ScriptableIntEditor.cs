using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(ScriptableInt))]
    public class ScriptableIntEditor : CustomEditorBase<ScriptableInt>
    {
        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();

            var scopeProperty = serializedObject.FindProperty("_scriptableVariableScope");
            var initialValueProperty = serializedObject.FindProperty("_initialValue");
            var constraintsProperty = serializedObject.FindProperty("_constraints");
            
            
            EditorGUILayout.PropertyField(scopeProperty);
            targetObject.initialValue = EditorGUILayout.IntField("Initial Value", targetObject.initialValue);

            GUI.enabled = Application.isPlaying;
            targetObject.value = EditorGUILayout.IntField("Current Value", targetObject.value);
            GUI.enabled = true;
            
            EditorGUILayout.PropertyField(constraintsProperty);


            serializedObject.ApplyModifiedProperties();
    
            if (GUILayout.Button("Increment by 1"))
            {
                targetObject.value += 1;    
            }
            
            if (GUILayout.Button("Increment by 100"))
            {
                targetObject.value += 100;    
            }
               
            if (GUILayout.Button("Ping"))
            {
                targetObject.Ping();
            }
        }
    }
}