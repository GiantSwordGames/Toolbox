
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JamKit
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LocalSnap))]
    public class LocalSnapEditor : CustomEditorBase<LocalSnap>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            if(GUILayout.Button("Snap Selected"))
            {
                foreach (var target in targetObjects)
                {
                    target.Apply();
                }
            }
            if(GUILayout.Button("Snap Siblings"))
            {
                List<LocalSnap> siblings = targetObject.transform.parent.GetDirectChildren<LocalSnap>();
                foreach (var target in siblings)
                {
                    target.Apply();
                }
            }
            // Add custom inspector code here
        }
    }
}
