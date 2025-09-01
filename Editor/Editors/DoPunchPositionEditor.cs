using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(DoPunchPosition))]
    public class DoPunchPositionEditor : CustomEditorBase<DoPunchPosition>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Trigger"))
            {
                targetObject.Trigger();
            }

        }
    }
}