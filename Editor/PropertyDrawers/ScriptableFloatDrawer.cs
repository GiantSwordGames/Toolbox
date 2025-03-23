using System;
using UnityEditor;
using UnityEngine;

namespace GiantSword.Plugins.Framework.Generic.Editor.GiantSword
{
    
    [CustomPropertyDrawer(typeof(ScriptableFloat))]
    public class ScriptableFloatDrawer : FloatAssetDrawerBase<ScriptableFloat>
    {
        protected override string customPrefix => "Float";

        protected override float GetValue( SerializedProperty property)
        {
            var targetObject = property.objectReferenceValue as ScriptableFloat;
            if (targetObject != null)
            {
                return targetObject.value;
            }
            throw new Exception();   
        }

        protected override void SetValue(SerializedProperty property, float newValue)
        {
            var targetObject = property.objectReferenceValue as ScriptableFloat;
            if (targetObject != null)
            {
                targetObject.value = newValue;
            }
        }
    }
}