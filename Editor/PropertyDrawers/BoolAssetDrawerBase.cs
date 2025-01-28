using System;
using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(ScriptableBool))]
    public  class BoolAssetDrawerBase : PropertyDrawer
    {
        private string fallbackPath => "Assets/Project/Configurations";

        protected  bool GetValue( SerializedProperty property)
        {
            var targetObject = property.objectReferenceValue as ScriptableBool;
            if (targetObject != null)
            {
                return targetObject.value;
            }
            throw new Exception();   
        }

        protected  void SetValue(SerializedProperty property, bool newValue)
        {
            var targetObject = property.objectReferenceValue as ScriptableBool;
            if (targetObject != null)
            {
                targetObject.value = newValue;
            }
        }
        
        // Define a ratio for the value field width

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetObject = property.objectReferenceValue as ScriptableBool;

            // Begin property field
            EditorGUI.BeginProperty(position, label, property);
          float valueFieldRatio = 0.33f; // 30% of the total width for the float value field
          if (targetObject != null)
          {
              valueFieldRatio = .1f;
          }
            // Calculate width based on the ratio
            float valueFieldWidth = position.width * valueFieldRatio;
            float objectFieldWidth = position.width - valueFieldWidth - 5f; // Subtract 5 for spacing

            // Define the rects for the object field and value field
            Rect objectFieldRect = new Rect(position.x, position.y, objectFieldWidth, position.height);
            Rect valueRect = new Rect(position.x + objectFieldWidth + 5f, position.y, valueFieldWidth, position.height);

            // Draw the object field for the ConfigurationFloat asset
            // SerializedProperty valueProperty = property.FindPropertyRelative("_value");
            EditorGUI.PropertyField(objectFieldRect, property, label);

            // Draw the editable float field if the object reference is valid
            if (targetObject != null)
            {
                // Allow editing the float value
                EditorGUI.BeginChangeCheck();
                bool currentValue = GetValue(property);
                bool newValue = EditorGUI.Toggle(valueRect, currentValue);
                if (EditorGUI.EndChangeCheck())
                {
                    // Record changes for undo/redo
                    Undo.RecordObject(targetObject, "Changed Value");
                    // targetObject.value = newValue;
                    SetValue(property, newValue);
                    // Mark the object dirty to save the changes
                    EditorUtility.SetDirty(targetObject);
                }
            }
            else
            {
                if (GUI.Button(valueRect, "Create"))
                {
                    ScriptableBool newAsset = ScriptableObject.CreateInstance<ScriptableBool>();
                    string folderPath = RuntimeEditorHelper.GetMostCommonDirectoryForAssetType<ScriptableBool>();
                    if (folderPath == "")
                    {
                        folderPath = fallbackPath;
                    }

                    Debug.Log(label.text);
                    string assetName = typeof(ScriptableBool).Name + "_" + label.text;
                    string newPath = folderPath + "/" + assetName + ".asset";
                    AssetDatabase.CreateAsset(newAsset, newPath);
                    var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<ScriptableBool>(newPath);
                    Debug.Log(newPath, loadAssetAtPath);
                    
                    property.objectReferenceValue = newAsset;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            // End property field
            EditorGUI.EndProperty();
        }
    }
}