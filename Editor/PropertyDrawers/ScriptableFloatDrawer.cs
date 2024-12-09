using UnityEditor;
using UnityEngine;

namespace GiantSword.Plugins.Framework.Generic.Editor.GiantSword
{
    [CustomPropertyDrawer(typeof(ScriptableFloat))]
    public class ScriptableFloatDrawer : PropertyDrawer
    {
        // Define a ratio for the value field width
        private const float VALUE_FIELD_RATIO = 0.3f;  // 30% of the total width for the float value field

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin property field
            EditorGUI.BeginProperty(position, label, property);

            // Calculate width based on the ratio
            float valueFieldWidth = position.width * VALUE_FIELD_RATIO;
            float objectFieldWidth = position.width - valueFieldWidth - 5f;  // Subtract 5 for spacing

            // Define the rects for the object field and value field
            Rect objectFieldRect = new Rect(position.x, position.y, objectFieldWidth, position.height);
            Rect valueRect = new Rect(position.x + objectFieldWidth + 5f, position.y, valueFieldWidth, position.height);

            // Draw the object field for the ScriptableFloat asset
            EditorGUI.PropertyField(objectFieldRect, property, label);

            // Draw the editable float field if the object reference is valid
            var targetObject = property.objectReferenceValue as ScriptableFloat;
            if (targetObject != null)
            {
                // Allow editing the current float value
                EditorGUI.BeginChangeCheck();
                float newValue = EditorGUI.FloatField(valueRect, targetObject.value);
                if (EditorGUI.EndChangeCheck())
                {
                    // Record changes for undo/redo
                    Undo.RecordObject(targetObject, "Changed ScriptableFloat Value");
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