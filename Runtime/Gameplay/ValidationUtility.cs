using UnityEngine;

namespace JamKit
{
    public static class ValidationUtility
    {
        public static bool IsPrefabAsset(this GameObject gameObject)
        {
            return string.IsNullOrEmpty(gameObject.scene.path) ;
        }

        public static bool IsPrefabAsset(Component component)
        {
            return IsPrefabAsset(component.gameObject);
        }
    }   
}