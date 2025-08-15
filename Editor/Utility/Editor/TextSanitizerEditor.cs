
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(TextSanitizer))]
    public class TextSanitizerEditor : CustomEditorBase<TextSanitizer>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(15);
            GUI.enabled = false;
          EditorGUILayout.TextArea(targetObject.Apply( targetObject.test));
          GUI.enabled = true;

        }
    }
}
