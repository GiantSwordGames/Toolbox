using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(DoShake))]
    public class DoShakeEditor : CustomEditorBase<DoShake>
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
