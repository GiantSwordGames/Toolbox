using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    public class CreatableAssetPropertyDrawer<T> : PropertyDrawer where T : ScriptableObject
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Check if the property is null
            if (property.objectReferenceValue == null)
            {
                // Draw the property field
                Rect propertyRect = new Rect(position.x, position.y, position.width - 60, position.height);
                EditorGUI.PropertyField(propertyRect, property, label);

                // Draw the "Create" button
                Rect buttonRect = new Rect(position.x + position.width - 55, position.y, 55, position.height);
                if (GUI.Button(buttonRect, "Create"))
                {
                    T asset = ScriptableObject.CreateInstance<T>();
                    property.objectReferenceValue = asset;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                // Draw the property field as normal
                EditorGUI.PropertyField(position, property, label);
            }

            EditorGUI.EndProperty();
        }
    }
}