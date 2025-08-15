using System.IO;
using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    public abstract class CreateAssetDrawer<T> : PropertyDrawer where T : ScriptableObject
    {
         protected virtual string customPrefix => "";
        
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
                    T newAsset = ScriptableObject.CreateInstance<T>();
                    string folderPath = RuntimeEditorHelper.GetMostCommonDirectoryForAssetType<T>();
                    if (folderPath == "")
                    {
                      
                        folderPath = MenuPaths.CONFIGURATIONS_PATH;
                    }


                    string prefix = typeof(T).Name;
                    if(customPrefix != "")
                    {
                        prefix = customPrefix;
                    }
                    
                    RuntimeEditorHelper.CreateFoldersIfNeeded(folderPath);

                    string assetName = prefix + "_" + label.text.ToUpperCamelCase();
                    string newPath = folderPath + "/" + assetName + ".asset";
                    AssetDatabase.CreateAsset(newAsset, newPath);
                    var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<T>(newPath);
                    Debug.Log(newPath, loadAssetAtPath);
                    
                    property.objectReferenceValue = newAsset;
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