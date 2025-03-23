using System.IO;
using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    public abstract class FloatAssetDrawerBase<T> : PropertyDrawer where T: ScriptableObject
    {
        protected virtual string customPrefix => "";

        private string fallbackPath => "Assets/Project/Configurations";

        protected abstract float GetValue(SerializedProperty property);

        protected abstract void SetValue(SerializedProperty property, float newValue);
        
        
        // Define a ratio for the value field width
        private const float VALUE_FIELD_RATIO = 0.33f; // 30% of the total width for the float value field

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Begin property field
            EditorGUI.BeginProperty(position, label, property);

            // Calculate width based on the ratio
            float valueFieldWidth = position.width * VALUE_FIELD_RATIO;
            float objectFieldWidth = position.width - valueFieldWidth - 5f; // Subtract 5 for spacing

            // Define the rects for the object field and value field
            Rect objectFieldRect = new Rect(position.x, position.y, objectFieldWidth, position.height);
            Rect valueRect = new Rect(position.x + objectFieldWidth + 5f, position.y, valueFieldWidth, position.height);

            // Draw the object field for the ConfigurationFloat asset
            // SerializedProperty valueProperty = property.FindPropertyRelative("_value");
            EditorGUI.PropertyField(objectFieldRect, property, label);

            // Draw the editable float field if the object reference is valid
            var targetObject = property.objectReferenceValue as T;
            if (targetObject != null)
            {
                // Allow editing the float value
                EditorGUI.BeginChangeCheck();
                float currentValue = GetValue(property);
                float newValue = EditorGUI.FloatField(valueRect, currentValue);
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
                    T newAsset = ScriptableObject.CreateInstance<T>();
                    string folderPath = RuntimeEditorHelper.GetMostCommonDirectoryForAssetType<T>();
                    if (folderPath == "")
                    {
                        folderPath = fallbackPath;
                    }
                    
                    //create folder if it doesn't exist
                    if (!AssetDatabase.IsValidFolder(folderPath))
                    {
                        Debug.Log(folderPath);
                        // get parent directory
                        string parentDirectory = Path.GetDirectoryName(folderPath);
                        // get the last directory name
                        string newFolderName = Path.GetFileName(folderPath);
                        // create the directory
                        AssetDatabase.CreateFolder(parentDirectory, newFolderName);
                    }

                    string prefix = typeof(T).Name;
                    if(customPrefix != "")
                    {
                        prefix = customPrefix;
                    }
                    string assetName = prefix + "_" + label.text.ToUpperCamelCase();
                    string newPath = folderPath + "/" + assetName + ".asset";
                    AssetDatabase.CreateAsset(newAsset, newPath);
                    var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<T>(newPath);
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