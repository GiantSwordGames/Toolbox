using UnityEditor;
using UnityEngine;

namespace GiantSword
{
     [CustomPropertyDrawer(typeof(ScriptableFloat))]
    public class ScriptableFloatDrawer : PropertyDrawer
    {
        // Define a ratio for the value field width
        private const float ValueFieldRatio = 0.2f; // 30% of the total width for the float value field

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin property field
            EditorGUI.BeginProperty(position, label, property);

            // Calculate width based on the ratio
            float valueFieldWidth = position.width * ValueFieldRatio;
            float objectFieldWidth = position.width - valueFieldWidth - 5f; // Subtract 5 for spacing

            // Define the rects for the object field and value field
            Rect objectFieldRect = new Rect(position.x, position.y, objectFieldWidth, position.height);
            Rect valueRect = new Rect(position.x + objectFieldWidth + 5f, position.y, valueFieldWidth, position.height);

            // Draw the object field for the ConfigurationFloat asset
            SerializedProperty valueProperty = property.FindPropertyRelative("_initialValue");
            EditorGUI.PropertyField(objectFieldRect, property, label);

            // Draw the editable float field if the object reference is valid
            var targetObject = property.objectReferenceValue as ConfigurationFloat;
            if (targetObject != null)
            {
                // Allow editing the float value
                EditorGUI.BeginChangeCheck();
                float newValue = EditorGUI.FloatField(valueRect, targetObject.value);
                if (EditorGUI.EndChangeCheck())
                {
                    // Record changes for undo/redo
                    Undo.RecordObject(targetObject, "Changed ConfigurationFloat Value");
                    targetObject.value = newValue;

                    // Mark the object dirty to save the changes
                    EditorUtility.SetDirty(targetObject);
                }
            }

            // End property field
            EditorGUI.EndProperty();
        }
    }
    
    [CustomPropertyDrawer(typeof(ConfigurationFloat))]
    public class ConfigurationFloatDrawer : PropertyDrawer
    {
        // Define a ratio for the value field width
        private const float ValueFieldRatio = 0.2f; // 30% of the total width for the float value field

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin property field
            EditorGUI.BeginProperty(position, label, property);

            // Calculate width based on the ratio
            float valueFieldWidth = position.width * ValueFieldRatio;
            float objectFieldWidth = position.width - valueFieldWidth - 5f; // Subtract 5 for spacing

            // Define the rects for the object field and value field
            Rect objectFieldRect = new Rect(position.x, position.y, objectFieldWidth, position.height);
            Rect valueRect = new Rect(position.x + objectFieldWidth + 5f, position.y, valueFieldWidth, position.height);

            // Draw the object field for the ConfigurationFloat asset
            SerializedProperty valueProperty = property.FindPropertyRelative("_value");
            EditorGUI.PropertyField(objectFieldRect, property, label);

            // Draw the editable float field if the object reference is valid
            var targetObject = property.objectReferenceValue as ConfigurationFloat;
            if (targetObject != null)
            {
                // Allow editing the float value
                EditorGUI.BeginChangeCheck();
                float newValue = EditorGUI.FloatField(valueRect, targetObject.value);
                if (EditorGUI.EndChangeCheck())
                {
                    // Record changes for undo/redo
                    Undo.RecordObject(targetObject, "Changed ConfigurationFloat Value");
                    targetObject.value = newValue;

                    // Mark the object dirty to save the changes
                    EditorUtility.SetDirty(targetObject);
                }
            }

            // End property field
            EditorGUI.EndProperty();
        }
    }
}