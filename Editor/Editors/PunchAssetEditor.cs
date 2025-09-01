
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(PunchAsset))]
    public class PunchAssetEditor : CustomEditorBase<PunchAsset>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Trigger"))
            {
                targetObject.Test();
            }
        }
    }
}
