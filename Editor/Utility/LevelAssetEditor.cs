using UnityEditor;
using UnityEngine;

namespace GiantSword
{
    public class LevelAssetUtility
    {
        [UnityEditor.Callbacks.OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            // Check if the asset being opened is a Level asset
            Level level = EditorUtility.InstanceIDToObject(instanceID) as Level;
            if (level != null)
            {
                level.OpenLevel();
                return true; // Return true to indicate the asset open has been handled
            }
            return false; // Return false to let Unity handle other assets as normal
        }
    }
}
