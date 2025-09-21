using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using JamKit;

[CustomPropertyDrawer(typeof(InlineScriptableObjectAttribute))]
public class InlineScriptableObjectDrawer : PropertyDrawer
{
    
    static Preference<bool> foldoutPref = new Preference<bool>("InlineScriptableObjectDrawer_Foldout", false);  
    // private static readonly Dictionary<string, bool> foldouts = new Dictionary<string, bool>();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.objectReferenceValue == null)
            return EditorGUIUtility.singleLineHeight;

        float height = EditorGUIUtility.singleLineHeight;

        if (!foldoutPref.value)
            return height;

        SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
        SerializedProperty iterator = serializedObject.GetIterator();

        if (iterator.NextVisible(true))
        {
            do
            {
                if (iterator.name == "m_Script") continue;
                height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
            } while (iterator.NextVisible(false));
        }

        height += 6f; // padding
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // string key = property.propertyPath;
        // if (!foldouts.ContainsKey(key))
        //     foldouts[key] = false;

        // CASE 1: Draw "Create" when null
        if (property.objectReferenceValue == null)
        {
            Rect fieldRect = new Rect(position.x, position.y, position.width - 60, EditorGUIUtility.singleLineHeight);
            Rect buttonRect = new Rect(position.x + position.width - 55, position.y, 55, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(fieldRect, property, label);

            if (GUI.Button(buttonRect, "Create"))
            {
                // Figure out target type
                System.Type type = fieldInfo.FieldType;
                if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    type = type.GetElementType() ?? type.GetGenericArguments()[0];
                }

                if (typeof(ScriptableObject).IsAssignableFrom(type))
                {
                    ScriptableObject newAsset = ScriptableObject.CreateInstance(type);

                    // Use RuntimeEditorHelper logic
                    string folderPath = RuntimeEditorHelper.GetMostCommonDirectoryForAssetType(type);
                    if (string.IsNullOrEmpty(folderPath))
                        folderPath = MenuPaths.CONFIGURATIONS_PATH;

                    RuntimeEditorHelper.CreateFoldersIfNeeded(folderPath);

                    string prefix = type.Name;
                    string assetName = prefix + "_" + label.text.ToUpperCamelCase();
                    string newPath = Path.Combine(folderPath, assetName + ".asset");
                    newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);

                    AssetDatabase.CreateAsset(newAsset, newPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    property.objectReferenceValue = newAsset;
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError($"InlineScriptableObjectDrawer: {type} is not a ScriptableObject.");
                }
            }

            EditorGUI.EndProperty();
            return;
        }

        // CASE 2: Draw foldout + inline inspector when not null
        Rect foldoutRect = new Rect(position.x, position.y, 14, EditorGUIUtility.singleLineHeight);
        Rect objectFieldRect = new Rect(position.x + 14, position.y, position.width - 14, EditorGUIUtility.singleLineHeight);

        foldoutPref.value = EditorGUI.Foldout(foldoutRect, foldoutPref.value, GUIContent.none, true);
        EditorGUI.PropertyField(objectFieldRect, property, label);

        if (!foldoutPref.value)
        {
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.indentLevel++;
        SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
        SerializedProperty iterator = serializedObject.GetIterator();

        float y = objectFieldRect.yMax + EditorGUIUtility.standardVerticalSpacing;
        float backgroundHeight = 0f;
        List<KeyValuePair<SerializedProperty, float>> propsToDraw = new List<KeyValuePair<SerializedProperty, float>>();

        if (iterator.NextVisible(true))
        {
            do
            {
                if (iterator.name == "m_Script") continue;
                float propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                propsToDraw.Add(new KeyValuePair<SerializedProperty, float>(iterator.Copy(), propHeight));
                backgroundHeight += propHeight + EditorGUIUtility.standardVerticalSpacing;
            } while (iterator.NextVisible(false));
        }

        Rect backgroundRect = new Rect(position.x, y - 3f, position.width, backgroundHeight + 6f);
#if UNITY_2019_1_OR_NEWER
        EditorGUI.DrawRect(backgroundRect, new Color(0.2f, 0.45f, 0.75f, 0.15f));
#endif

        foreach (var kvp in propsToDraw)
        {
            SerializedProperty prop = kvp.Key;
            float height = kvp.Value;
            Rect propRect = new Rect(position.x, y, position.width, height);
            EditorGUI.PropertyField(propRect, prop, true);
            y += height + EditorGUIUtility.standardVerticalSpacing;
        }

        serializedObject.ApplyModifiedProperties();
        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}
