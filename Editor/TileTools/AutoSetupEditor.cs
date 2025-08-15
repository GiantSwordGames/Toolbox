using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AutoSetUpTile))]
    public class AutoSetupEditor : CustomEditorBase<AutoSetUpTile>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Refresh Self"))
            {
                ApplyToAllSelected(target => target.RefreshSelf());
            }
            
            if (GUILayout.Button("Refresh Adjacent"))
            {
                ApplyToAllSelected(target => target.RefreshAdjacent());
            }
            
            if (GUILayout.Button("Refresh All"))
            {
                ApplyToAllSelected(target => target.RefreshAll());
            }
        }

        private void ApplyToAllSelected(System.Action<AutoSetUpTile> action)
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is GameObject gameObject)
                {
                    var target = gameObject.GetComponent<AutoSetUpTile>();
                    if (target != null)
                    {
                        action(target);
                    }
                }
            }
        }
    }
}