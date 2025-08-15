using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(TargetTransform))]
    public class TargetTransformDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Get properties
            var modeProperty = property.FindPropertyRelative("_mode");
            var otherTransformProperty = property.FindPropertyRelative("_otherTransform");
            var selfProperty = property.FindPropertyRelative("_self");

            // Calculate widths for each field on a single line
            float labelWidth = EditorGUIUtility.labelWidth;
            float dropdownWidth = (position.width - labelWidth) * 0.3f; // Smaller dropdown (30%)
            float transformWidth = (position.width - labelWidth) * 0.7f - 4; // Larger transform field (70%)

            Rect labelRect = new Rect(position.x, position.y, labelWidth, EditorGUIUtility.singleLineHeight);
            Rect modeRect = new Rect(position.x + labelWidth, position.y, dropdownWidth, EditorGUIUtility.singleLineHeight);
            Rect transformRect = new Rect(position.x + labelWidth + dropdownWidth + 4, position.y, transformWidth, EditorGUIUtility.singleLineHeight);

            // Draw label
            EditorGUI.PrefixLabel(labelRect, GUIUtility.GetControlID(FocusType.Passive), label);

            // Draw Mode dropdown (smaller)
            EditorGUI.PropertyField(modeRect, modeProperty, GUIContent.none);

            // Draw the relevant field based on selected mode
            TargetTransform.Mode mode = (TargetTransform.Mode)modeProperty.enumValueIndex;
            switch (mode)
            {
                case TargetTransform.Mode.Self:
                    // Display self transform as read-only
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.PropertyField(transformRect, selfProperty, GUIContent.none);
                    EditorGUI.EndDisabledGroup();
                    break;
                case TargetTransform.Mode.Other:
                    // Display Other Transform field
                    EditorGUI.PropertyField(transformRect, otherTransformProperty, GUIContent.none);
                    break;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
