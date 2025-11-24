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

            SerializedProperty modeProp = property.FindPropertyRelative("_mode");
            SerializedProperty constantValueProp = property.FindPropertyRelative("_constantValue");
            SerializedProperty variableProp = property.FindPropertyRelative("_variable");
            SerializedProperty monoFloatProp = property.FindPropertyRelative("_monoFloat");
            SerializedProperty configurationFloatProp = property.FindPropertyRelative("_configurationFloat");
            SerializedProperty floatRange = property.FindPropertyRelative("_floatRange");
            SerializedProperty floatVariance = property.FindPropertyRelative("_floatVariance");

            float dropdownButtonWidth = 20f;

            // Reserve space for the dropdown on the right
            Rect fieldRect = new Rect(position.x, position.y, position.width - dropdownButtonWidth, position.height);
            Rect buttonRect = new Rect(fieldRect.xMax, position.y, dropdownButtonWidth, position.height);

            switch ((SmartFloat.Mode)modeProp.enumValueIndex)
            {
                case SmartFloat.Mode.Constant:
                    // IMPORTANT: give the label to the constantValueProp,
                    // not to the parent, so label-drag scrubbing works.
                    EditorGUI.PropertyField(fieldRect, constantValueProp, label);
                    break;

                case SmartFloat.Mode.Variable:
                    DrawWithOuterLabel(fieldRect, label, variableProp);
                    break;

                case SmartFloat.Mode.MonoFloat:
                    DrawWithOuterLabel(fieldRect, label, monoFloatProp);
                    break;

                case SmartFloat.Mode.ConfigurationFloat:
                    DrawWithOuterLabel(fieldRect, label, configurationFloatProp);
                    break;

                case SmartFloat.Mode.FloatRange:
                    DrawWithOuterLabel(fieldRect, label, floatRange);
                    break;

                case SmartFloat.Mode.FloatVariance:
                    DrawWithOuterLabel(fieldRect, label, floatVariance);
                    break;
            }

            if (GUI.Button(buttonRect, "..."))
            {
                GenericMenu menu = new GenericMenu();

                void Add(string text, SmartFloat.Mode mode)
                {
                    menu.AddItem(
                        new GUIContent(text),
                        modeProp.enumValueIndex == (int)mode,
                        () =>
                        {
                            modeProp.enumValueIndex = (int)mode;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                }

                Add("Use Constant", SmartFloat.Mode.Constant);
                Add("Use ScriptableFloat", SmartFloat.Mode.Variable);
                Add("Use MonoFloat", SmartFloat.Mode.MonoFloat);
                Add("Use ConfigurationFloat", SmartFloat.Mode.ConfigurationFloat);
                Add("Use FloatRange", SmartFloat.Mode.FloatRange);
                Add("Use FloatVariance", SmartFloat.Mode.FloatVariance);

                menu.ShowAsContext();
            }

            EditorGUI.EndProperty();
        }

        private static void DrawWithOuterLabel(Rect position, GUIContent label, SerializedProperty innerProp)
        {
            // Manually split label + field so the label still shows,
            // but the inner property doesn't get its own label.
            float labelWidth = EditorGUIUtility.labelWidth;

            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            Rect valueRect = new Rect(labelRect.xMax, position.y, position.width - labelWidth, position.height);

            EditorGUI.LabelField(labelRect, label);
            EditorGUI.PropertyField(valueRect, innerProp, GUIContent.none, true);
        }
    }
}