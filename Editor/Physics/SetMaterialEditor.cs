using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SetDensity))]
    public class SetPhysicalMaterialEditor : CustomEditorBase<SetDensity>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Label($"Mass:     {targetObject.mass}kg");
            if (GUILayout.Button("Apply Density"))
            {
                foreach (var target in targets)
                {
                    Undo.RecordObject(target, "Apply Density");
                    ((SetDensity) target).ApplyDensity();
                }
            }
        }
    }
}
