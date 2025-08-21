using UnityEngine;
using UnityEditor;

namespace JamKit
{
    public class DisableSerializedFieldAttribute : PropertyAttribute {}

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DisableSerializedFieldAttribute))]
    public class DisabledDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
}