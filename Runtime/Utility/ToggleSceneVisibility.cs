using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JamKit
{
    public class ToggleSceneVisibility : MonoBehaviour
    {

        public void Toggle()
        {
#if UNITY_EDITOR

            SceneVisibilityManager.instance.ToggleVisibility(gameObject, false);
#endif
        }

    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ToggleSceneVisibility))]
    public class SceneVisibilityGroupEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            if (GUILayout.Button("Toggle Visibility"))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    ToggleSceneVisibility g = (ToggleSceneVisibility)targets[i];
                    g.Toggle();
                }
            }

        }
    }
#endif
}
