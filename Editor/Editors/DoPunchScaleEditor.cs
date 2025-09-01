
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(DoPunchScale))]
    public class DoPunchScaleEditor : CustomEditorBase<DoPunchScale>
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
