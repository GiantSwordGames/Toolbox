using System;
using UnityEditor;

namespace GiantSword
{
    [CustomPropertyDrawer(typeof(ConfigurationFloat))]
    public class ConfigurationFloatDrawer : FloatAssetDrawerBase<ConfigurationFloat>
    {
        protected override float GetValue( SerializedProperty property)
        {
            var targetObject = property.objectReferenceValue as ConfigurationFloat;
            if (targetObject != null)
            {
                return targetObject.value;
            }
            throw new Exception();   
        }

        protected override void SetValue(SerializedProperty property, float newValue)
        {
            var targetObject = property.objectReferenceValue as ConfigurationFloat;
            if (targetObject != null)
            {
                targetObject.value = newValue;
            }
        }
    }
}