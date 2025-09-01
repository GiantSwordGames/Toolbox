using HotWings;
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(DoPunchRotation))]
    public class DoPunchRotationEditor : CustomEditorBase<DoPunchRotation>
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