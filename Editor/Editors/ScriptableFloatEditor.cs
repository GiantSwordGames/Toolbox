
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(ScriptableFloat))]
    public class ScriptableFloatEditor : CustomEditorBase<ScriptableFloat>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = false;
            EditorGUILayout.FloatField("Current Value", targetObject.value);
            
            GUI.enabled = true;
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
