using System;
using UnityEditor;

namespace JamKit.Plugins.Framework.Generic.Editor.JamKit
{
    [CustomPropertyDrawer(typeof(ScriptableInt))]
    public class ScriptableIntDrawer : FloatAssetDrawerBase<ScriptableInt>
    {
        protected override string customPrefix => "Int";

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