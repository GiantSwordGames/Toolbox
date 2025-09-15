
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(ScriptableFloat))]
    public class ScriptableFloatEditor : CustomEditorBase<ScriptableFloat>
    {
        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();

            var scopeProperty = serializedObject.FindProperty("_scriptableVariableScope");
            var initialValueProperty = serializedObject.FindProperty("_initialValue");
            var constraintsProperty = serializedObject.FindProperty("_constraints");
            
            
            EditorGUILayout.PropertyField(scopeProperty);
            EditorGUILayout.PropertyField(initialValueProperty);
            
            GUI.enabled = Application.isPlaying;
            targetObject.value = EditorGUILayout.FloatField("Current Value", targetObject.value);
            GUI.enabled = true;
            
            EditorGUILayout.PropertyField(constraintsProperty);


            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("Increment by 0.1"))
            {
                targetObject.value += 0.1f;    
            }
            
            if (GUILayout.Button("Increment by 1"))
            {
                targetObject.value += 1f;    
            }
            
            if (GUILayout.Button("Increment by 100"))
            {
                targetObject.value += 100f;    
            }
        }
    }
}
