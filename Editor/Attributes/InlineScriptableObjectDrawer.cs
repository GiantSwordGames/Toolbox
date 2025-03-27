using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(InlineScriptableObjectAttribute))]
public class InlineScriptableObjectDrawer : PropertyDrawer
{
    private static readonly Dictionary<string, bool> foldouts = new();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (property.objectReferenceValue == null)
            return height;

        if (!foldouts.TryGetValue(property.propertyPath, out bool expanded) || !expanded)
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

        // Add a bit of vertical padding for the background
        height += 6f;
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string key = property.propertyPath;
        if (!foldouts.ContainsKey(key))
            foldouts[key] = false;

        float indentWidth = EditorGUI.indentLevel * 15f;
        Rect foldoutRect = new Rect(position.x + indentWidth, position.y, 14, EditorGUIUtility.singleLineHeight);
        Rect objectFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        foldouts[key] = EditorGUI.Foldout(foldoutRect, foldouts[key], GUIContent.none, true);
        EditorGUI.PropertyField(objectFieldRect, property, label);

        if (property.objectReferenceValue == null || !foldouts[key])
            return;

        EditorGUI.indentLevel++;
        SerializedObject serializedObject = new SerializedObject(property.objectReferenceValue);
        SerializedProperty iterator = serializedObject.GetIterator();

        // Determine height of inner inspector for background
        float y = objectFieldRect.yMax + EditorGUIUtility.standardVerticalSpacing;
        float backgroundY = y;
        float backgroundHeight = 0f;

        List<(SerializedProperty, float)> propsToDraw = new();

        if (iterator.NextVisible(true))
        {
            do
            {
                if (iterator.name == "m_Script") continue;

                float propHeight = EditorGUI.GetPropertyHeight(iterator, true);
                propsToDraw.Add((iterator.Copy(), propHeight));
                backgroundHeight += propHeight + EditorGUIUtility.standardVerticalSpacing;
            } while (iterator.NextVisible(false));
        }

        // Background box
        Rect backgroundRect = new Rect(position.x, backgroundY - 3f, position.width, backgroundHeight + 6f);
        // EditorGUI.DrawRect(backgroundRect, new Color(0.1f, 0.4f, 0.6f, 0.05f)); // Light blue, subtle
        // EditorGUI.DrawRect(backgroundRect, new Color(0.1f, 0.4f, 0.6f, 0.05f));
        EditorGUI.DrawRect(backgroundRect, new Color(0.2f, 0.45f, 0.75f, 0.15f));

        // Draw the nested properties
        foreach (var (prop, height) in propsToDraw)
        {
            Rect propRect = new Rect(position.x, y, position.width, height);
            EditorGUI.PropertyField(propRect, prop, true);
            y += height + EditorGUIUtility.standardVerticalSpacing;
        }

        serializedObject.ApplyModifiedProperties();
        EditorGUI.indentLevel--;
    }
}
