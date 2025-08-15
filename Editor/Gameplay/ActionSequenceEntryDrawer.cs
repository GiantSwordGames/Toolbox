using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace JamKit
{
    [CustomPropertyDrawer(typeof(ActionSequence.Entry))]
    public class EntryDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight; // Foldout height

            if (property.isExpanded)
            {
                // Add height for each property with spacing
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_delayBefore")) + EditorGUIUtility.standardVerticalSpacing;
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_actions")) + EditorGUIUtility.standardVerticalSpacing;
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_delayAfter")) + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // string customLabel = GetCustomLabel(property);

            // Foldout to show/hide contents
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++; // Indent within the foldout
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Draw each field with dynamic height
                SerializedProperty delayBeforeProp = property.FindPropertyRelative("_delayBefore");
                SerializedProperty actionsProp = property.FindPropertyRelative("_actions");
                SerializedProperty delayAfterProp = property.FindPropertyRelative("_delayAfter");

                // Draw _delayBefore with its dynamic height
                float delayBeforeHeight = EditorGUI.GetPropertyHeight(delayBeforeProp);
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, delayBeforeHeight), delayBeforeProp, new GUIContent("Delay Before"));
                position.y += delayBeforeHeight + EditorGUIUtility.standardVerticalSpacing;

                // Draw _actions (UnityEvent) with its dynamic height
                float actionsHeight = EditorGUI.GetPropertyHeight(actionsProp);
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, actionsHeight), actionsProp, new GUIContent("Actions"));
                position.y += actionsHeight + EditorGUIUtility.standardVerticalSpacing;

                // Draw _delayAfter with its dynamic height
                float delayAfterHeight = EditorGUI.GetPropertyHeight(delayAfterProp);
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, delayAfterHeight), delayAfterProp, new GUIContent("Delay After"));
                position.y += delayAfterHeight + EditorGUIUtility.standardVerticalSpacing;

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
        // private string GetCustomLabel(SerializedProperty property)
        // {
        //     SerializedProperty nameProp = property.FindPropertyRelative("name"); // Replace "name" with the actual field name in Entry
        //     return nameProp != null && !string.IsNullOrEmpty(nameProp.stringValue) ? nameProp.stringValue : "Entry";
        // }
    }

}
