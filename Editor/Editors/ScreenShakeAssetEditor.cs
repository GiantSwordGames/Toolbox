
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CustomEditor(typeof(ScreenShakeAsset))]
    public class ScreenShakeAssetEditor : CustomEditorBase<ScreenShakeAsset>
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
