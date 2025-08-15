using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Cooldown))]
public class CooldownDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw Prefix Label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Calculate field widths
        float halfWidth = position.width / 2f;
        Rect durationRect = new Rect(position.x, position.y, halfWidth - 2, position.height);
        Rect remainingRect = new Rect(position.x + halfWidth + 2, position.y, halfWidth - 2, position.height);

        // Draw Duration Field
        SerializedProperty durationProp = property.FindPropertyRelative("_duration");
        EditorGUI.PropertyField(durationRect, durationProp, GUIContent.none);

        // Get the actual Cooldown instance
        object targetObject = property.serializedObject.targetObject;
        var cooldownInstance = fieldInfo.GetValue(targetObject) as Cooldown;

        // Fallback for array/list elements
        if (cooldownInstance == null && property.propertyPath.Contains("["))
        {
            string arrayPath = property.propertyPath.Substring(0, property.propertyPath.IndexOf('['));
            var arrayField = fieldInfo.GetValue(targetObject) as System.Collections.IList;
            int index = int.Parse(property.propertyPath.Substring(property.propertyPath.IndexOf('[') + 1).TrimEnd(']'));
            cooldownInstance = arrayField[index] as Cooldown;
        }

        // Draw Time Remaining Field
        GUI.enabled = false;
        float timeRemaining = cooldownInstance != null ? cooldownInstance.GetRemainingTime() : 0f;
        EditorGUI.FloatField(remainingRect, timeRemaining);
        GUI.enabled = true;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
