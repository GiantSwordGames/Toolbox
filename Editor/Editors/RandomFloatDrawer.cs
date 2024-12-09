using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(FloatVariance))]
    public class RandomFloatDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            int lastIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            Rect[] split = position.SplitWidthPercent(0.6f, 10f);

            SerializedProperty a = property.FindPropertyRelative("_value");
            SerializedProperty b = property.FindPropertyRelative("_variance");
            
            Rect[] a_rects = split[0].SplitFromLeft(40f);
            Rect[] b_rects = split[1].SplitFromLeft(23f);
            
            EditorGUI.LabelField(a_rects[0], "Value");
            
            EditorGUI.PropertyField(a_rects[1], a, GUIContent.none);
            
            EditorGUI.LabelField(b_rects[0], new GUIContent("+/-", "Deviation"));
            
            EditorGUI.PropertyField(b_rects[1], b, GUIContent.none);
            
            EditorGUI.indentLevel = lastIndent;
            
            EditorGUI.EndProperty();
        }
    }
}