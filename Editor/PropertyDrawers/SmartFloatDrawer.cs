using UnityEditor;
using UnityEngine;

namespace JamKitEditor
{   
    [CustomPropertyDrawer(typeof(SmartFloat))]
    public class SmartFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Find the properties
            SerializedProperty modeProp = property.FindPropertyRelative("_mode");
            SerializedProperty constantValueProp = property.FindPropertyRelative("_constantValue");
            SerializedProperty variableProp = property.FindPropertyRelative("_variable");
            SerializedProperty monoFloatProp = property.FindPropertyRelative("_monoFloat"); // Renamed from floatRangeProp
            SerializedProperty configurationFloatProp = property.FindPropertyRelative("_configurationFloat"); // New property for ConfigurationFloat
            SerializedProperty floatRange = property.FindPropertyRelative("_floatRange"); // New property for ConfigurationFloat
            SerializedProperty floatVariance = property.FindPropertyRelative("_floatVariance"); // New property for ConfigurationFloat

            // Draw the label
            position = EditorGUI.PrefixLabel(position, label);

            // Calculate the width of the dropdown button
            float dropdownButtonWidth = 24f;
            Rect buttonRect = new Rect(position.xMax - dropdownButtonWidth, position.y, dropdownButtonWidth, position.height);
            Rect objectRect = new Rect(position.x, position.y, position.width - dropdownButtonWidth, position.height);

            // Draw the main property field based on the selected mode
            switch ((SmartFloat.Mode)modeProp.enumValueIndex)
            {
                case SmartFloat.Mode.Constant:
                    EditorGUI.PropertyField(objectRect, constantValueProp, GUIContent.none);
                    break;
                case SmartFloat.Mode.Variable:
                    EditorGUI.PropertyField(objectRect, variableProp, GUIContent.none);
                    break;
                case SmartFloat.Mode.MonoFloat:
                    EditorGUI.PropertyField(objectRect, monoFloatProp, GUIContent.none); // Using renamed monoFloatProp
                    break;
                case SmartFloat.Mode.ConfigurationFloat: // New case for ConfigurationFloat
                    EditorGUI.PropertyField(objectRect, configurationFloatProp, GUIContent.none); // Drawing _configurationFloat
                    break;
                case SmartFloat.Mode.FloatRange:
                    EditorGUI.PropertyField(objectRect, floatRange, GUIContent.none);
                    break;
                case SmartFloat.Mode.FloatVariance:
                    EditorGUI.PropertyField(objectRect, floatVariance, GUIContent.none);
                    break;
            }

            // Draw the dropdown button
            if (GUI.Button(buttonRect, "▼"))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Use Constant"), modeProp.enumValueIndex == (int)SmartFloat.Mode.Constant, () =>
                {
                    modeProp.enumValueIndex = (int)SmartFloat.Mode.Constant;
                    property.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Use ScriptableFloat"), modeProp.enumValueIndex == (int)SmartFloat.Mode.Variable, () =>
                {
                    modeProp.enumValueIndex = (int)SmartFloat.Mode.Variable;
                    property.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Use MonoFloat"), modeProp.enumValueIndex == (int)SmartFloat.Mode.MonoFloat, () =>
                {
                    modeProp.enumValueIndex = (int)SmartFloat.Mode.MonoFloat;
                    property.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Use ConfigurationFloat"), modeProp.enumValueIndex == (int)SmartFloat.Mode.ConfigurationFloat, () =>
                {
                    modeProp.enumValueIndex = (int)SmartFloat.Mode.ConfigurationFloat;
                    property.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Use FloatRange"), modeProp.enumValueIndex == (int)SmartFloat.Mode.FloatRange, () =>
                {
                    modeProp.enumValueIndex = (int)SmartFloat.Mode.FloatRange;
                    property.serializedObject.ApplyModifiedProperties();
                });
                menu.AddItem(new GUIContent("Use FloatVariance"), modeProp.enumValueIndex == (int)SmartFloat.Mode.FloatVariance, () =>
                {
                    modeProp.enumValueIndex = (int)SmartFloat.Mode.FloatVariance;
                    property.serializedObject.ApplyModifiedProperties();
                });
                menu.ShowAsContext();
            }

            EditorGUI.EndProperty();
        }
    }
}