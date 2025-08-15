using System;
using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(CreateAssetButtonAttribute))]
    public class CreateAssetAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CreateAssetButtonAttribute buttonAttribute = this.attribute as CreateAssetButtonAttribute;
            EditorGUI.BeginProperty(position, label, property);

            if (property.objectReferenceValue == null)
            {
                // Draw the property field with room for the button
                Rect propertyRect = new Rect(position.x, position.y, position.width - 60, position.height);
                EditorGUI.PropertyField(propertyRect, property, label);

                // Draw the "Create" button
                Rect buttonRect = new Rect(position.x + position.width - 55, position.y, 55, position.height);
                if (GUI.Button(buttonRect, "Create"))
                {
                    Type type = null;
                    
                    // Check if the property is a managed reference
                    if (property.propertyType == SerializedPropertyType.ManagedReference)
                    {
                        string fieldTypeName = property.managedReferenceFieldTypename;
                        if (!string.IsNullOrEmpty(fieldTypeName))
                        {
                            string[] parts = fieldTypeName.Split(' ');
                            if (parts.Length == 2)
                            {
                                string assemblyName = parts[0];
                                string typeFullName = parts[1];
                                string assemblyQualifiedName = $"{typeFullName}, {assemblyName}";
                                type = Type.GetType(assemblyQualifiedName);
                            }
                        }
                    }
                    
                    // Fallback if not a managed reference, e.g. for object references (PPtr)
                    if (type == null)
                    {
                        type = fieldInfo.FieldType;
                        Debug.LogWarning("Managed reference type not available. Falling back to fieldInfo.FieldType.");
                    }

                    // Create an instance of the ScriptableObject
                    var newAsset = ScriptableObject.CreateInstance(type);
                    
                    // Define folder path for asset creation

                   
                    string folderPath =  RuntimeEditorHelper.GetMostCommonDirectoryForAssetType(type);
                    if (folderPath.IsEmpty())
                    {
                        folderPath = "Assets";
                    }
                    RuntimeEditorHelper.CreateFoldersIfNeeded(folderPath);

                    // Use custom prefix if provided
                    string prefix = type.Name;
                    if (!string.IsNullOrEmpty(buttonAttribute.customPrefix))
                    {
                        prefix = buttonAttribute.customPrefix;
                    }
                    string assetName = prefix + "_" + label.text.ToUpperCamelCase();
                    string newPath = $"{folderPath}/{assetName}.asset";
                    
                    AssetDatabase.CreateAsset(newAsset, newPath);
                    var loadedAsset = AssetDatabase.LoadAssetAtPath(newPath, type);
                    Debug.Log(newPath, loadedAsset);

                    // Assign the new asset to the property and apply modified properties
                    property.objectReferenceValue = newAsset;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                // Draw the property field normally if already assigned
                EditorGUI.PropertyField(position, property, label);
            }

            EditorGUI.EndProperty();
        }
    }
}
